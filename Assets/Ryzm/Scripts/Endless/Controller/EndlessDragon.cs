using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;
using Ryzm.Dragon;
using TrailsFX;

namespace Ryzm.EndlessRunner
{
    public class EndlessDragon : EndlessController
    {
        #region Public Variables
        public GameObject placeGOPrefab;
        public LayerMask monsterLayer;
        public BaseDragon baseDragon;
        public Transform monkeyPos;
        public DragonFire fire;
        public float flyUpSpeed = 10;
        public float flyDownSpeed = 5;
        public bool forceJump;
        public bool forceHeadbutt;
        public bool forceTailSlap;
        public bool forceShift;
        public Transform fakeShiftTransform;
        public float headbuttTime = 1;

        [Header("Trail Effects")]
        public TrailEffect bodyTrailEffect;
        public TrailEffect wingTrailEffect;

        [Header("Attacks")]
        public DragonTailSlap dragonTailSlap;
        public DragonHeadbutt dragonHeadbutt;
        public Collider attackCollider;
        public ShiftParticles shiftParticles;
        public BurstParticles burstParticles;

        [HideInInspector]
        public Vector3 monkeyOffset;
        #endregion

        #region Private Variables
        IEnumerator flyToPosition;
        IEnumerator fireBreath;
        bool isBreathingFire;
        bool _isFlying;
        bool flyingInitialized;
        float elevation = 1.5f;
        IEnumerator flyUp;
        float baselineY;
        bool isTailSlapping;
        IEnumerator headbutt;
        bool canFly;
        IEnumerator tailSlap;
        IEnumerator simulateTailSlap;
        IEnumerator finishShiftThenTailSlap;
        float _speedMultiplier;
        Vector3 prevPosition;
        Vector3 currentPosition;
        IEnumerator shiftAttack;
        bool noShifting;
        AttackState _shiftAttackState;
        AttackState _headbuttAttackState;
        int numHeadbutts;
        int maxHeadbutts = 2;
        AttackState _tailSlapAttackState;
        #endregion

        #region Properties
        bool IsFlying
        {
            get
            {
                return _isFlying;
            }
            set
            {
                if(value != _isFlying || !flyingInitialized)
                {
                    _isFlying = value;
                    animator.SetBool("fly", value);
                    flyingInitialized = true;
                }
            }
        }

        float SpeedMultiplier 
        {
            get
            {
                return _speedMultiplier;
            }
            set
            {
                _speedMultiplier = value;
                animator.SetFloat("speedMultiplier", _speedMultiplier);
            }
        }

        AttackState ShiftAttackState
        {
            get
            {
                return _shiftAttackState;
            }
            set
            {
                _shiftAttackState = value;
                Message.Send(new ShiftAttackStateResponse(_shiftAttackState));
            }
        }

        AttackState HeadbuttAttackState
        {
            get
            {
                return _headbuttAttackState;
            }
            set
            {
                if(value == AttackState.Off)
                {
                    numHeadbutts = 0;
                }
                else if(value == AttackState.On)
                {
                    numHeadbutts++;
                }
                _headbuttAttackState = value;
                Message.Send(new HeadbuttAttackStateResponse(_headbuttAttackState));
            }
        }

        AttackState TailSlapAttackState
        {
            get
            {
                return _tailSlapAttackState;
            }
            set
            {
                _tailSlapAttackState = value;
                Message.Send(new TailSlapAttackStateResponse(_tailSlapAttackState));
            }
        }
        #endregion

        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            monkeyOffset = monkeyPos.position - trans.position;
            maxShiftCooldown = 1f;
            if(fire != null)
            {
                fire.type = FireType.User;
            }
            SetBodyTrailEffect(false);
            SetWingTrailEffect(false);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Message.AddListener<ControllerTransformRequest>(OnControllerTransformRequest);
            Message.AddListener<ShiftAttackStateRequest>(OnShiftAttackStateRequest);
            Message.AddListener<HeadbuttAttackStateRequest>(OnHeadbuttAttackStateRequest);
        }

        void Start()
        {
            SpeedMultiplier = 1;
            SetHeadbutt(false);
            SetShiftAttack(false);
            prevPosition = transform.position;
            Message.Send(new ControllerTransformResponse(prevPosition));
        }

        // void OnDrawGizmos()
        // {
        //     trans = transform;
        //     currentPosition = trans.position;
        //     Vector3 spherePos = currentPosition;
        //     spherePos.z += trans.forward.z;
        //     spherePos.x += trans.right.x * 1.5f;
        //     Gizmos.DrawSphere(spherePos, 1);
        // }

        void Update()
        {
            currentPosition = trans.position;
            Vector3 distance = (currentPosition - prevPosition);
            float sqrDistance = distance.sqrMagnitude;
            if(sqrDistance > 1)
            {
                Message.Send(new ControllerTransformResponse(currentPosition));
                prevPosition = currentPosition;
            }

            // animator.SetInteger("state", state);
            if((mode == ControllerMode.MonkeyDragon || mode == ControllerMode.Dragon) && gameStatus == GameStatus.Active)
            {
                EndlessRun();
            }

            if(!InJump && forceJump)
            {
                IsFlying = true;
                Jump();
            }
            else
            {
                forceJump = false;
            }

            if(HeadbuttAttackState == AttackState.Off && forceHeadbutt)
            {
                IsFlying = true;
                if(headbutt != null)
                {
                    StopCoroutine(headbutt);
                    headbutt = null;
                }
                headbutt = _Headbutt(true);
                StartCoroutine(headbutt);
            }
            else
            {
                forceHeadbutt = false;
            }

            if(TailSlapAttackState == AttackState.Off && forceTailSlap)
            {
                simulateTailSlap = SimulateTailSlap();
                StartCoroutine(simulateTailSlap);
                forceTailSlap = false;
            }
            else
            {
                forceTailSlap = false;
            }
        }

        void OnTriggerEnter(Collider col)
        {
            if(HeadbuttAttackState != AttackState.Off || ShiftAttackState != AttackState.Off)
            {
                MonsterBase monster = col.gameObject.GetComponent<MonsterBase>();
                if(monster != null)
                {
                    monster.TakeDamage();
                }
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Message.RemoveListener<ControllerTransformRequest>(OnControllerTransformRequest);
            Message.RemoveListener<ShiftAttackStateRequest>(OnShiftAttackStateRequest);
            Message.RemoveListener<HeadbuttAttackStateRequest>(OnHeadbuttAttackStateRequest);
        }
        #endregion

        #region Listener Functions
        protected override void OnRunnerDistanceRequest(RunnerDistanceRequest request)
        {
            if(mode == ControllerMode.Dragon || mode == ControllerMode.MonkeyDragon)
            {
                Message.Send(new RunnerDistanceResponse(distanceTraveled));
            }
        }

        protected override void OnGameStatusResponse(GameStatusResponse response)
        {
            base.OnGameStatusResponse(response);
            if(response.status == GameStatus.Starting)
            {
                baselineY = trans.position.y;
            }
            else if(response.status == GameStatus.Restart || response.status == GameStatus.Exit)
            {
                Reset();
                if(response.status == GameStatus.Exit)
                {
                    IsFlying = false;
                }
            }
        }

        void OnControllerTransformRequest(ControllerTransformRequest request)
        {
            currentPosition = trans.position;
            Message.Send(new ControllerTransformResponse(currentPosition));
        }

        void OnShiftAttackStateRequest(ShiftAttackStateRequest request)
        {
            Message.Send(new ShiftAttackStateResponse(ShiftAttackState));
        }

        void OnHeadbuttAttackStateRequest(HeadbuttAttackStateRequest request)
        {
            Message.Send(new HeadbuttAttackStateResponse(HeadbuttAttackState));
        }
        #endregion

        #region Public Functions

        public void GetTextures()
        {
            baseDragon.GetTextures();
            // Debug.Log("dragon is initialized " + data.id);
            // Message.Send(new DragonInitialized(data.id));
        }

        public void DisableMaterials()
        {
            baseDragon.DisableMaterials();
        }

        public void EnableMaterials()
        {
            baseDragon.EnableMaterials();
        }

        public override void Shift(Direction direction)
        {
            if(!inShift && !noShifting)
            {
                if(_endlessTurnSection != null)
                {
                    _endlessTurnSection.Shift(direction, this, ref turned);
                }
                else if(_endlessSection != null)
                {
                    _endlessSection.Shift(direction, this);
                }
            }
        }

        public override void ShiftToPosition(Transform pos, ShiftDistanceType type)
        {
            base.ShiftToPosition(pos, type);
            // StopHeadbutt();
            // if(ShiftAttackState != AttackState.Off)
            // {
            //     ShiftAttackState = AttackState.Off;
            //     StopCoroutine(shiftAttack);
            // }
            // float _shiftDistance = GetShiftDistance(pos, type);
            // float _distance = Mathf.Lerp(0, _shiftDistance, 0.1f);
            // float direction = Mathf.Sign(_distance);
            // bool hitIt = CheckIfHit(direction);
            
            // shiftAttack = ShiftAttack();
            // StartCoroutine(shiftAttack);
        }

        bool CheckIfHit(float direction)
        {
            Vector3 spherePos = currentPosition;
            spherePos.x += trans.right.x * 1.5f * direction;
            bool hitIt = Physics.SphereCast(spherePos, 1, trans.forward, out hit, 3, (int)monsterLayer);
            return hitIt;
        }

        public override void Attack()
        {
            // Debug.Log("numHeadbutts " + numHeadbutts + "HeadbuttAttackState " + HeadbuttAttackState.ToString());
            if(!isBreathingFire && HeadbuttAttackState == AttackState.Off && TailSlapAttackState != AttackState.On)
            {
                float absDiff = Mathf.Abs(trans.position.y - baselineY);
                if(absDiff > 0.1f && InJump) // high enough above ground to do ground pound move
                {
                    finishShiftThenTailSlap = FinishShiftAndTailSlap();
                    StartCoroutine(finishShiftThenTailSlap);
                }
                else
                {
                    if(TailSlapAttackState == AttackState.Cooldown)
                    {
                        StopCoroutine(tailSlap);
                        tailSlap = null;
                    }
                    SetShiftAttack(false, true);
                    headbutt = _Headbutt();
                    StartCoroutine(headbutt);
                }
                // else if(!InJump && ShiftAttackState != AttackState.On)
                // {
                //     // ShiftAttackState = AttackState.Off;
                //     SetShiftAttack(false, true);
                //     headbutt = _Headbutt();
                //     StartCoroutine(headbutt);
                // }
                // else if(fire != null)
                // {
                //     fireBreath = FireBreath();
                //     StartCoroutine(fireBreath);
                // }
            }
            else if(!InJump && HeadbuttAttackState != AttackState.Off && numHeadbutts < maxHeadbutts)
            {
                Debug.Log("doing tha other one");
                StopCoroutine(headbutt);
                headbutt = null;
                // StopAllCoroutines();
                headbutt = _Headbutt();
                StartCoroutine(headbutt);
            }
        }

        public override void Jump()
        {
            if(!inShift)
            {
                if(IsFlying)
                {
                    if(!InJump)
                    {
                        StopHeadbutt();
                        flyUp = FlyUp(trans.position.y);
                        StartCoroutine(flyUp);
                    }
                    else if(canFly)
                    {
                        StopHeadbutt();
                        StopCoroutine(flyUp);
                        flyUp = null;
                        flyUp = FlyUp(baselineY);
                        StartCoroutine(flyUp);
                    }
                }
                else if(IsGrounded())
                {
                    // todo: handle jumping up from ground
                }
            }
        }

        public override void Die()
        {
            StopAllCoroutines();
            State = 2;
            if(fire != null)
            {
                fire.Stop();
                animator.SetBool("fireBreath", false);
            }
            rb.constraints = RigidbodyConstraints.FreezeAll;
            SetHeadbutt(false);
            SetBodyTrailEffect(false);
            SetWingTrailEffect(false);
            SetShiftAttack(false);
            animator.SetBool("tailSlapUp", false);
            animator.SetBool("tailSlapDown", false);
        }

        public void Fly(bool shouldFly = true)
        {
            animator.SetBool("fly", shouldFly);
        }

        public void MoveWithMultiplier(float multi)
        {
            SpeedMultiplier = multi;
            Move(0, multi);
        }

        public void FlyToPosition(Transform t)
        {
            animator.SetBool("fly", true);
            flyToPosition = _FlyToPosition(t, forwardSpeed * 2.5f);
            StartCoroutine(flyToPosition);
        }

        public void FlyToPosition(Transform t, float speed)
        {
            animator.SetBool("fly", true);
            flyToPosition = _FlyToPosition(t, speed);
            StartCoroutine(flyToPosition);
        }

        public void ForceEnableTailSlapExplosion()
        {
            if(TailSlapAttackState == AttackState.On)
            {
                dragonTailSlap.EnableExplosion();
            }
        }
        #endregion

        #region Protected Functions
        protected override void Reset()
        {
            animator.SetBool("flyDown", false);
            animator.SetBool("flyUp", false);
            animator.SetBool("tailSlapUp", false);
            animator.SetBool("tailSlapDown", false);
            SetHeadbutt(false);
            HeadbuttAttackState = AttackState.Off;
            TailSlapAttackState = AttackState.Off;
            isBreathingFire = false;
            noShifting = false;
            ShiftAttackState = AttackState.Off;
            if(InJump)
            {
                trans.position = new Vector3(trans.position.x, baselineY, trans.position.z);
            }
            base.Reset();
            canFly = true;
            SetBodyColliders(true);
        }
        #endregion

        #region Private Functions
        void EndlessRun()
        {
            IsFlying = true;
            if(!InJump && HeadbuttAttackState == AttackState.Off && TailSlapAttackState == AttackState.Off)
            {
                Move(0);
            }

            // if(IsShifting(Direction.Left))
            // {
            //     Shift(Direction.Left);
            // }
            // else if(IsShifting(Direction.Right))
            // {
            //     Shift(Direction.Right);
            // }
            // if(IsAttacking())
            // {
            //     Attack();
            // }
            // if(IsJumping())
            // {
            //     Jump();
            // }
        }

        bool IsJumping()
        {
			return playerInput.PlayerMain.Jump.WasPressedThisFrame();
        }

        void Move(float yMove, float multiplier = 1)
        {
            float zMove = Time.deltaTime * forwardSpeed * multiplier;
            move.z = zMove;
            move.y = yMove;
            move.x = shiftSpeed * zMove * 0.75f;
            trans.Translate(move);
            distanceTraveled += zMove;
            Message.Send(new RunnerDistanceResponse(distanceTraveled));
        }

        void SetBodyTrailEffect(bool active)
        {
            if(bodyTrailEffect != null)
            {
                bodyTrailEffect.active = active;
            }
        }

        void SetWingTrailEffect(bool active)
        {
            if(wingTrailEffect != null)
            {
                wingTrailEffect.active = active;
            }
        }

        void SetBodyColliders(bool enabled)
        {
            foreach(Collider bodyCollider in bodyColliders)
            {
                bodyCollider.enabled = enabled;
            }
        }

        void SetHeadbutt(bool enabled)
        {
            HeadbuttAttackState = enabled ? AttackState.On : AttackState.Off;
            SetAttackCollider(enabled);
            SetHeadbuttAnim(enabled);
        }

        void SetAttackCollider(bool enabled)
        {
            if(attackCollider != null)
            {
                attackCollider.enabled = enabled;
            }
        }

        void SetHeadbuttAnim(bool enabled)
        {
            animator.SetBool("headbutt", enabled);
            if(dragonHeadbutt != null)
            {
                if(enabled)
                {
                    dragonHeadbutt.Enable();
                }
                else
                {
                    dragonHeadbutt.Disable();
                }
            }
        }

        void SetShiftAttack(bool enabled, bool skipHeadbutt = false)
        {
            ShiftAttackState = enabled ? AttackState.On : AttackState.Off;
            SetAttackCollider(enabled);
            SetBodyColliders(!enabled);
            
            if(!skipHeadbutt)
            {
                animator.SetBool("headbutt", enabled);
                SetBodyTrailEffect(enabled);
            }

            // if(shiftParticles != null)
            // {
            //     if(enabled)
            //     {
            //         shiftParticles.Enable();
            //     }
            //     else
            //     {
            //         shiftParticles.Disable();
            //     }
            // }
        }

        void StopHeadbutt()
        {
            if(HeadbuttAttackState != AttackState.Off)
            {
                SetHeadbuttAnim(false);
                SetBodyTrailEffect(false);
                HeadbuttAttackState = AttackState.Off;
                
                if(headbutt != null)
                {
                    StopCoroutine(headbutt);
                    headbutt = null;
                }
            }
        }

        void MakeGO(Vector3 curPos)
        {
            if(placeGOPrefab != null)
            {
                GameObject go = Instantiate(placeGOPrefab, curPos, Quaternion.identity);
            }
            else
            {
                GameObject go = new GameObject();
                go.transform.position = curPos;
            }
        }
        #endregion

        #region Coroutines
        IEnumerator _FlyToPosition(Transform target, float speed)
        {
            float distance = Vector3.Distance(trans.position, target.position);
            while(distance > 0.1f)
            {
                distance = Vector3.Distance(trans.position, target.position);
                move.z = Time.deltaTime * speed;
                move.y = 0;
                move.x = 0;
                trans.Translate(move);
                yield return null;
            }
            trans.position = target.position;
            trans.rotation = target.rotation;
            yield break;
        }

        IEnumerator FlyUp(float initY)
        {
            // StartCoroutine(PlaceGameObjs());
            canFly = false;
            SetWingTrailEffect(true);
            animator.SetBool("flyDown", false);
            animator.SetBool("flyUp", true);
            InJump = true;
            baselineY = initY;
            float currentY = trans.position.y;
            float absDiff = Mathf.Abs(currentY - baselineY);
            float multiScale = 1.5f;
            float multiplier = multiScale * (elevation - absDiff);
            while(multiplier > 0.05f)
            {
                currentY = trans.position.y;
                absDiff = Mathf.Abs(currentY - baselineY);
                multiplier = multiScale * (elevation - absDiff);
                // multiplier *= multiplier;
                multiplier = multiplier > 1 ? 1 : multiplier;
                multiplier = multiplier > 0 ? multiplier : 0;
                Move(flyUpSpeed * multiplier * Time.deltaTime);
                yield return null;
            }
            // float glideTime = 2 * Time.deltaTime;
            // float t = 0;
            // float intervals = 2;
            // while(t < intervals)
            // {
            //     t += 1;
            //     Move(0);
            //     yield return null;
            // }
            
            animator.SetBool("flyDown", true);
            animator.SetBool("flyUp", false);
            float timeMulti = 3;
            float timeMultiplier = 0.1f;
            while(absDiff > 0.1f && currentY > baselineY)
            {
                currentY = trans.position.y;
                absDiff = Mathf.Abs(currentY - baselineY);
                float downMultiplier = absDiff > 1 ? 1 : absDiff > 0.5f ? absDiff : 0.5f;
                Move(-flyDownSpeed * timeMultiplier * downMultiplier * Time.deltaTime);
                timeMultiplier += timeMulti * Time.deltaTime;
                timeMultiplier = timeMultiplier < 1 ? timeMultiplier : 1;
                yield return null;
            }
            canFly = true;
            SetWingTrailEffect(false);
            animator.SetBool("flyDown", false);
            while(absDiff > 0.01f && currentY > baselineY)
            {
                currentY = trans.position.y;
                absDiff = Mathf.Abs(currentY - baselineY);
                float downMultiplier = absDiff > 0.05f ? absDiff : 0.05f;
                Move(-flyDownSpeed * downMultiplier * Time.deltaTime);
                yield return null;
            }
            // animator.SetBool("flyDown", false);
            trans.position = new Vector3(trans.position.x, baselineY, trans.position.z);
            InJump = false;
            yield break;
        }

        IEnumerator _Headbutt(bool shouldWait = false)
        {
            Debug.Log("starting headbutt");
            float t = 0;
            if(shouldWait)
            {
                while(t < 0.25f)
                {
                    t += Time.deltaTime;
                    Move(0);
                    yield return null;
                }
            }
            SetBodyColliders(false);
            SetHeadbutt(true);
            SetBodyTrailEffect(true);
            t = 0;
            while(t < headbuttTime)
            {
                HeadbuttMove();
                t += Time.deltaTime;
                yield return null;
            }
            SetHeadbuttAnim(false);
            t = 0;
            HeadbuttAttackState = AttackState.Cooldown;
            while(t < 0.2f) //cooldown time
            {
                HeadbuttMove();
                t += Time.deltaTime;
                yield return null;
            }
            SetHeadbutt(false);
            SetBodyColliders(true);
            SetBodyTrailEffect(false);
        }

        IEnumerator _TailSlap()
        {
            TailSlapAttackState = AttackState.On;
            noShifting = true;
            animator.SetBool("flyDown", false);
            animator.SetBool("flyUp", false);
            canFly = false;
            SetWingTrailEffect(false);
            float upTime = dragonTailSlap.expansionTime;
            float t = 0;
            // animator.SetBool("tailSlapUp", true);
            // animator.SetBool("tailSlapDown", false);
            dragonTailSlap.Enable();
            float targetMulti = 5;
            SetBodyColliders(false);
            while(t < upTime)
            {
                t += Time.deltaTime;
                float diff = (1 - t / upTime);
                float moveMultiplier = 0.5f * diff * diff;
                // moveMultiplier = moveMultiplier > 0 ? moveMultiplier : 0;
                Move(0, moveMultiplier);
                float speedMulti = SpeedMultiplier;
                if(speedMulti < targetMulti)
                {
                    SpeedMultiplier = Mathf.Lerp(speedMulti, targetMulti, Time.deltaTime);
                }
                yield return null;
            }
            
            animator.SetBool("tailSlapUp", false);
            animator.SetBool("tailSlapDown", true);
            float currentY = trans.position.y;
            float absDiff = Mathf.Abs(currentY - baselineY);
            float moveMulti = 0;
            float hTime = 0;
            // float headbuttTime = 1f;
            float minHeadbuttTime = 0;
            bool startedHeadbutt = false;
            animator.SetBool("headbutt", true);
            SetAttackCollider(true);
            while(absDiff > 0.2f && currentY > baselineY)
            {
                moveMulti = dragonTailSlap.explosionEnabled ? 0.5f : 0;
                currentY = trans.position.y;
                absDiff = Mathf.Abs(trans.position.y - baselineY);
                float downMultiplier = absDiff > 1 ? 1 : absDiff > 0.5f ? absDiff : 0.5f;
                Move(-flyDownSpeed * Time.deltaTime * downMultiplier, moveMulti);
                float speedMulti = SpeedMultiplier;
                if(speedMulti < targetMulti)
                {
                    SpeedMultiplier = Mathf.Lerp(speedMulti, targetMulti, Time.deltaTime * 10);
                }
                hTime += Time.deltaTime;
                if(dragonTailSlap.explosionEnabled && !startedHeadbutt && hTime > minHeadbuttTime)
                {
                    SetHeadbuttAnim(true);
                    SetAttackCollider(true);
                    SetBodyTrailEffect(true);
                    startedHeadbutt = true;
                }
                yield return null;
            }

            while(!dragonTailSlap.explosionEnabled)
            {
                yield return null;
            }
            noShifting = false;
            // dragonTailSlap.EnableExplosion();
            currentY = trans.position.y;
            absDiff = Mathf.Abs(currentY - baselineY);
            bool disabledAnim = false;
            if(!startedHeadbutt && hTime > minHeadbuttTime)
            {
                SetHeadbuttAnim(true);
                SetAttackCollider(true);
                SetBodyTrailEffect(true);
                startedHeadbutt = true;
            }
            while(absDiff > 0.01f && currentY > baselineY)
            {
                moveMulti += Time.deltaTime;
                moveMulti = moveMulti > 1 ? 1 : moveMulti;
                SpeedMultiplier = Mathf.Lerp(SpeedMultiplier, 1, Time.deltaTime * 5);
                currentY = trans.position.y;
                absDiff = Mathf.Abs(currentY - baselineY);
                float downMultiplier = absDiff > 0.05f ? absDiff : 0.05f;
                Move(-flyDownSpeed * downMultiplier * Time.deltaTime, moveMulti);
                if(absDiff < 0.1f)
                {
                    disabledAnim = true;
                    animator.SetBool("tailSlapUp", false);
                    animator.SetBool("tailSlapDown", false);
                }
                hTime += Time.deltaTime;
                if(!startedHeadbutt && hTime > minHeadbuttTime)
                {
                    SetHeadbuttAnim(true);
                    SetAttackCollider(true);
                    SetBodyTrailEffect(true);
                    startedHeadbutt = true;
                }
                yield return null;
            }
            SpeedMultiplier = 1;
            dragonTailSlap.Disable();
            canFly = true;
            if(!disabledAnim)
            {
                animator.SetBool("tailSlapUp", false);
                animator.SetBool("tailSlapDown", false);
            }
            trans.position = new Vector3(trans.position.x, baselineY, trans.position.z);
            InJump = false;
            TailSlapAttackState = AttackState.Cooldown;
            if(!startedHeadbutt)
            {
                SetHeadbuttAnim(true);
                SetAttackCollider(true);
                SetBodyTrailEffect(true);
                startedHeadbutt = true;
            }
            while(hTime < headbuttTime)
            {
                hTime += headbuttTime;
                Move(0);
                yield return null;
            }
            SetHeadbuttAnim(false);
            SetAttackCollider(false);
            SetBodyTrailEffect(false);
            SetBodyColliders(true);
            TailSlapAttackState = AttackState.Off;
        }

        void HeadbuttMove()
        {
            float currentY = trans.position.y;
            float absDiff = Mathf.Abs(currentY - baselineY);
            float downSpeed = 0;
            if(absDiff > 0.01f && currentY > baselineY)
            {
                downSpeed = -flyDownSpeed * Time.deltaTime;
            }
            else if(absDiff > 0.01f)
            {
                trans.position = new Vector3(trans.position.x, baselineY, trans.position.z);
            }
            Move(downSpeed);
        }

        IEnumerator FinishShiftAndTailSlap()
        {
            if(flyUp != null)
            {
                StopCoroutine(flyUp);
                flyUp = null;
            }
            float t = 0;
            float maxT = 0.5f;
            while(inShift)
            {
                t += Time.deltaTime;
                float multiplier = 1 - t / maxT;
                multiplier = multiplier > 0 ? multiplier : 0;
                Move(0, multiplier);
                yield return null;
            }
            ShiftAttackState = AttackState.Off;
            tailSlap = _TailSlap();
            StartCoroutine(tailSlap);
        }

        IEnumerator SimulateTailSlap()
        {
            IsFlying = true;
            float t = 0;
            while(t < 0.5f)
            {
                t += Time.deltaTime;
                Move(0);
                yield return null;
            }
            Jump();
            float currentY = trans.position.y;
            float absDiff = Mathf.Abs(currentY - baselineY);
            bool shouldContinue = false;
            bool passedOnce = false;
            while(!shouldContinue)
            {
                currentY = trans.position.y;
                absDiff = Mathf.Abs(currentY - baselineY);
                if(!passedOnce)
                {
                    if(absDiff > 1.5f)
                    {
                        passedOnce = true;
                    }
                }
                else if(absDiff < 1.5f)
                {
                    shouldContinue = true;
                }
                yield return null;
            }
            Attack();
        }

        IEnumerator ShiftAttack()
        {
            SetShiftAttack(true);
            while(inShift)
            {
                yield return null;
            }
            float t = 0;
            float minCooldownTime = 0.25f;
            float shiftAttackCooldown = shiftParticles == null ? minCooldownTime : shiftParticles.deactivationTime;
            shiftAttackCooldown = shiftAttackCooldown < minCooldownTime ? minCooldownTime : shiftAttackCooldown;
            ShiftAttackState = AttackState.Cooldown;
            while(ShiftAttackState == AttackState.Cooldown && t < shiftAttackCooldown)
            {
                t += Time.deltaTime;
                yield return null;
            }
            if(ShiftAttackState == AttackState.Cooldown)
            {
                SetShiftAttack(false);
            }
        }
        IEnumerator FireBreath()
        {
            isBreathingFire = true;
            animator.SetBool("fireBreath", true);
            fire.Play();
            float fbTime = 1f;
            float time = 0;
            while(time < fbTime)
            {
                time += Time.deltaTime;
                yield return null;
            }
            isBreathingFire = false;
            animator.SetBool("fireBreath", false);
            fire.Stop();
            yield break;
        }

        IEnumerator PlaceGameObjs()
        {
            Vector3 curPos = transform.position;
            Vector3 prevPos = transform.position;
            // GameObject go = new GameObject();
            // go.transform.position = curPos;
            MakeGO(curPos);
            while(true)
            {
                curPos = transform.position;
                float diff = Mathf.Abs(curPos.z - prevPos.z);
                if(diff > 0.25f)
                {
                    prevPos = curPos;
                    // go = new GameObject();
                    // go.transform.position = curPos;
                    MakeGO(curPos);
                }
                yield return null;
            }
        }
        #endregion
    }

    public enum DragonState
    {
        Idle,
        Fly,
        FlyUp,
        FlyDown,
        Headbutt,
        TailSlap
    }

    public enum AttackState
    {
        Off,
        On,
        Cooldown
    }
}
