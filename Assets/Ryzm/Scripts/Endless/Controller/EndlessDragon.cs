using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;
using Ryzm.Dragon;

namespace Ryzm.EndlessRunner
{
    public class EndlessDragon : EndlessController
    {
        #region Public Variables
        public BaseDragon baseDragon;
        public Transform monkeyPos;
        public DragonFire fire;
        public float flyUpSpeed = 10;
        public float flyDownSpeed = 5;
        
        [Header("Testing")]
        public bool testJump;
        public bool testHeadbutt;
        public bool testTailSlap;
        public bool testShift;
        public bool testJumpWithDamage;
        public Transform testShiftTransform;
        public bool testTakeDamage;

        [Header("Attacks")]
        public float headbuttTime = 1;
        public DragonTailSlap dragonTailSlap;
        public DragonHeadbutt dragonHeadbutt;
        public ShiftParticles shiftParticles;
        public BurstParticles burstParticles;

        [Header("Damage")]
        public BasicParticles damageParticles;

        [HideInInspector]
        public Vector3 monkeyOffset;
        #endregion

        #region Private Variables
        IEnumerator flyToPosition;
        IEnumerator fireBreath;
        bool _isFlying;
        bool flyingInitialized;
        float elevation = 1.5f;
        IEnumerator flyUp;
        float baselineY;
        IEnumerator headbutt;
        bool canFly = true;
        IEnumerator tailSlap;
        IEnumerator simulateTailSlap;
        IEnumerator finishShiftThenTailSlap;
        IEnumerator shiftAttack;
        ActionState _shiftAttackState;
        ActionState _headbuttAttackState;
        int numHeadbutts;
        int maxHeadbutts = 2;
        ActionState _tailSlapAttackState;
        IEnumerator takeDamage;
        IEnumerator testShiftWithDamage;
        IEnumerator testFlyUpWithDamage;
        float ySpeed;
        // anim hashes
        int flyAnimHash;
        int fireBreahAnimHash;
        int flyDownAnimHash;
        int flyUpAnimHash;
        int tailSlapUpAnimHash;
        int tailSlapDownAnimHash;
        int headbuttAnimHash;
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
                    animator.SetBool(flyAnimHash, value);
                    flyingInitialized = true;
                }
            }
        }

        ActionState ShiftAttackState
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

        ActionState HeadbuttAttackState
        {
            get
            {
                return _headbuttAttackState;
            }
            set
            {
                if(value == ActionState.Off)
                {
                    numHeadbutts = 0;
                }
                else if(value == ActionState.On)
                {
                    numHeadbutts++;
                }
                _headbuttAttackState = value;
                Message.Send(new HeadbuttAttackStateResponse(_headbuttAttackState));
            }
        }

        ActionState TailSlapAttackState
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
            flyAnimHash = Animator.StringToHash("fly");
            flyUpAnimHash = Animator.StringToHash("flyUp");
            flyDownAnimHash = Animator.StringToHash("flyDown");
            fireBreahAnimHash = Animator.StringToHash("fireBreath");
            tailSlapUpAnimHash = Animator.StringToHash("tailSlapUp");
            tailSlapDownAnimHash = Animator.StringToHash("tailSlapDown");
            headbuttAnimHash = Animator.StringToHash("headbutt");

            base.Awake();
            monkeyOffset = monkeyPos.position - trans.position;
            maxShiftCooldown = 1f;
            if(fire != null)
            {
                fire.type = FireType.User;
            }
            SetBodyTrailEffect(false);
            SetWingTrailEffect(false);
            baselineY = trans.position.y;
            canFly = true;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Message.AddListener<ShiftAttackStateRequest>(OnShiftAttackStateRequest);
            Message.AddListener<HeadbuttAttackStateRequest>(OnHeadbuttAttackStateRequest);
        }

        protected override void Start()
        {
            base.Start();
            SpeedMultiplier = 1;
            SetHeadbutt(false);
            SetShiftAttack(false);
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
            UpdateControllerTransform();

            // animator.SetInteger("state", state);
            if((mode == ControllerMode.MonkeyDragon || mode == ControllerMode.Dragon) && gameStatus == GameStatus.Active)
            {
                StartMove();
                Move();
            }

            if(!InJump && testJump)
            {
                IsFlying = true;
                UpInput();
                testJump = false;
            }

            if(HeadbuttAttackState == ActionState.Off && testHeadbutt)
            {
                IsFlying = true;
                if(headbutt != null)
                {
                    StopCoroutine(headbutt);
                    headbutt = null;
                }
                headbutt = _Headbutt();
                StartCoroutine(headbutt);
                testHeadbutt = false;
            }

            if(TailSlapAttackState == ActionState.Off && testTailSlap)
            {
                simulateTailSlap = SimulateTailSlap();
                StartCoroutine(simulateTailSlap);
                testTailSlap = false;
            }

            if(testTakeDamage && !isDamaged)
            {
                TakeDamage();
                testTakeDamage = false;
            }

            if(testShift && !isDamaged && !inShift)
            {
                testShiftWithDamage = TestShiftWithDamage();
                StartCoroutine(testShiftWithDamage);
                testShift = false;
            }

            if(testJumpWithDamage && !isDamaged && !InJump)
            {
                testFlyUpWithDamage = TestFlyUpWithDamage();
                StartCoroutine(testFlyUpWithDamage);
                testJumpWithDamage = false;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Message.RemoveListener<ShiftAttackStateRequest>(OnShiftAttackStateRequest);
            Message.RemoveListener<HeadbuttAttackStateRequest>(OnHeadbuttAttackStateRequest);
        }

        protected override void OnTriggerEnter(Collider col)
        {
            if(HeadbuttAttackState != ActionState.Off || TailSlapAttackState != ActionState.Off) //  || UsingSpecial (if using trigger)
            {
                OnTrigger(col.gameObject);
            }
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
            else if(response.status == GameStatus.Exit) 
            {
                IsFlying = false;
            }
        }

        protected override void OnSpecialAttackRequest(SpecialAttackRequest request)
        {
            if(!UsingSpecial)
            {
                if(Special >= maxSpecial)
                {
                    Message.Send(new SpecialAttackResponse(ActionState.On));
                    fireBreath = FireBreath();
                    StartCoroutine(fireBreath);
                }
                else
                {
                    Message.Send(new SpecialAttackResponse(ActionState.Off));
                }
            }
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

        // public override void ShiftToPosition(Transform pos, ShiftDistanceType type)
        // {
        //     StopHeadbutt();
        //     if(ShiftAttackState != AttackState.Off)
        //     {
        //         ShiftAttackState = AttackState.Off;
        //         StopCoroutine(shiftAttack);
        //     }
        //     float _shiftDistance = GetShiftDistance(pos, type);
        //     float _distance = Mathf.Lerp(0, _shiftDistance, 0.1f);
        //     float direction = Mathf.Sign(_distance);
        //     bool hitIt = CheckIfHit(direction);
            
        //     shiftAttack = ShiftAttack();
        //     StartCoroutine(shiftAttack);
        // }

        public override void DownInput()
        {
            // Debug.Log("numHeadbutts " + numHeadbutts + "HeadbuttAttackState " + HeadbuttAttackState.ToString());
            if(!UsingSpecial && HeadbuttAttackState == ActionState.Off && TailSlapAttackState != ActionState.On)
            {
                float absDiff = Mathf.Abs(trans.position.y - baselineY);
                if(absDiff > 0.35f && InJump) // high enough above ground to do ground pound move
                {
                    if(finishShiftThenTailSlap != null)
                    {
                        StopCoroutine(finishShiftThenTailSlap);
                        finishShiftThenTailSlap = null;
                    }
                    if(tailSlap != null)
                    {
                        StopCoroutine(tailSlap);
                        tailSlap = null;
                    }
                    finishShiftThenTailSlap = FinishShiftAndTailSlap();
                    StartCoroutine(finishShiftThenTailSlap);
                }
                else
                {
                    if(TailSlapAttackState == ActionState.Cooldown)
                    {
                        TailSlapAttackState = ActionState.Off;
                        if(tailSlap != null)
                        {
                            StopCoroutine(tailSlap);
                            tailSlap = null;
                        }
                        dragonTailSlap.Disable();
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
            }
            else if(!InJump && HeadbuttAttackState != ActionState.Off && numHeadbutts < maxHeadbutts)
            {
                StopCoroutine(headbutt);
                headbutt = null;
                headbutt = _Headbutt();
                StartCoroutine(headbutt);
            }
        }

        public override void UpInput()
        {
            if(!inShift && canFly)
            {
                if(IsFlying)
                {
                    if(!InJump)
                    {
                        StopHeadbutt();
                        // flyUp = FlyUp(trans.position.y);
                        flyUp = _FlyUp(baselineY);
                        StartCoroutine(flyUp);
                    }
                    else
                    {
                        StopHeadbutt();
                        if(flyUp != null)
                        {
                            StopCoroutine(flyUp);
                            flyUp = null;
                        }
                        flyUp = _FlyUp(baselineY);
                        StartCoroutine(flyUp);
                    }
                }
            }
        }

        protected override void Die()
        {
            StopAllCoroutines();
            State = 2;
            if(fire != null)
            {
                fire.Stop();
                animator.SetBool(fireBreahAnimHash, false);
            }
            rb.constraints = RigidbodyConstraints.FreezeAll;
            SetHeadbutt(false);
            SetBodyTrailEffect(false);
            SetWingTrailEffect(false);
            SetShiftAttack(false);
            ResetDamaged();
            Message.Send(new RunnerDie());
            animator.SetBool(tailSlapUpAnimHash, false);
            animator.SetBool(tailSlapDownAnimHash, false);
        }

        public override void StartMove()
        {
            IsFlying = true;
        }

        public override void MoveWithMultiplier(float multi)
        {
            base.MoveWithMultiplier(multi);
            Move(multi);
        }

        public void FlyToPosition(Transform t)
        {
            IsFlying = true;
            flyToPosition = _FlyToPosition(t, forwardSpeed * 2.5f);
            StartCoroutine(flyToPosition);
        }

        public void ForceEnableTailSlapExplosion()
        {
            // Debug.Log("forcing explosion " + dragonTailSlap.explosionEnabled + " " + TailSlapAttackState);
            if(TailSlapAttackState == ActionState.On)
            {
                dragonTailSlap.EnableExplosion();
            }
        }
        #endregion

        #region Protected Functions
        protected override void TakeDamage()
        {
            if(takeDamage != null)
            {
                StopCoroutine(takeDamage);
                takeDamage = null;
            }
            takeDamage = _TakeDamage();
            StartCoroutine(takeDamage);
        }

        protected override void Reset()
        {
            animator.SetBool(flyDownAnimHash, false);
            animator.SetBool(flyUpAnimHash, false);
            animator.SetBool(tailSlapUpAnimHash, false);
            animator.SetBool(tailSlapDownAnimHash, false);
            SetHeadbutt(false);
            HeadbuttAttackState = ActionState.Off;
            TailSlapAttackState = ActionState.Off;
            ShiftAttackState = ActionState.Off;
            if(InJump)
            {
                ResetY();
            }
            dragonTailSlap.InstantDisable();
            base.Reset();
            canFly = true;
            SetBodyColliders(true);
        }

        protected override void ResetDamaged()
        {
            if(isDamaged)
            {
                baseDragon.EnableMaterials();
                if(TailSlapAttackState == ActionState.Off && HeadbuttAttackState == ActionState.Off)
                {
                    SetBodyColliders(true);
                }
                damageParticles.Disable();
            }
        }
        #endregion

        #region Private Functions
        void Move(float multiplier = 1)
        {
            float zMove = Time.deltaTime * forwardSpeed * multiplier;
            zMove = Time.deltaTime * forwardSpeed * zSpeedMultiplier;
            move.z = zMove;
            move.y = ySpeed;
            move.x = shiftSpeed * zMove * 0.75f;
            trans.Translate(move);
            distanceTraveled += zMove;
            Message.Send(new RunnerDistanceResponse(distanceTraveled));
        }

        void SetHeadbutt(bool enabled)
        {
            HeadbuttAttackState = enabled ? ActionState.On : ActionState.Off;
            SetAttackColliders(enabled);
            SetHeadbuttAnim(enabled);
        }

        void SetHeadbuttAnim(bool enabled)
        {
            animator.SetBool(headbuttAnimHash, enabled);
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
            ShiftAttackState = enabled ? ActionState.On : ActionState.Off;
            SetAttackColliders(enabled);
            SetBodyColliders(!enabled);
            
            if(!skipHeadbutt)
            {
                animator.SetBool(headbuttAnimHash, enabled);
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

        void StopTailSlap()
        {
            if(TailSlapAttackState != ActionState.Off)
            {
                zSpeedMultiplier = 1;
                dragonTailSlap.Disable();
                TailSlapAttackState = ActionState.Off;
                SetAttackColliders(false);
                SetBodyTrailEffect(false);
                SetBodyColliders(true);
                SetHeadbuttAnim(false);
                noShifting = false;
                animator.SetBool(tailSlapUpAnimHash, false);
                animator.SetBool(tailSlapDownAnimHash, false);

                if(tailSlap != null)
                {
                    StopCoroutine(tailSlap);
                    tailSlap = null;
                }
                if(finishShiftThenTailSlap != null)
                {
                    StopCoroutine(finishShiftThenTailSlap);
                    finishShiftThenTailSlap = null;
                }
            }
        }

        void StopHeadbutt()
        {
            if(HeadbuttAttackState != ActionState.Off)
            {
                SetHeadbuttAnim(false);
                SetBodyTrailEffect(false);
                HeadbuttAttackState = ActionState.Off;
                
                if(headbutt != null)
                {
                    StopCoroutine(headbutt);
                    headbutt = null;
                }
            }
        }

        void StopJump()
        {
            if(InJump)
            {
                if(flyUp != null)
                {
                    StopCoroutine(flyUp);
                    flyUp = null;
                }
            }
        }

        bool ForceFlyDown(bool withMultiplier = false)
        {
            float currentY = trans.position.y;
            float absDiff = Mathf.Abs(currentY - baselineY);
            if(absDiff > 0.01f && currentY > baselineY)
            {
                float downMultiplier = absDiff > 0.05f ? absDiff : 0.05f;
                downMultiplier = withMultiplier ? downMultiplier : 1;
                ySpeed = -flyDownSpeed * downMultiplier * Time.deltaTime;
                return true;
            }
            else
            {
                if(absDiff > 0.01f)
                {
                    ResetY();
                }
                ySpeed = 0;
                return false;
            }
        }

        void ResetY()
        {
            trans.position = new Vector3(trans.position.x, baselineY, trans.position.z);
            ySpeed = 0;
            animator.SetBool(flyUpAnimHash, false);
            animator.SetBool(flyDownAnimHash, false);
            SetWingTrailEffect(false);
            InJump = false;
        }

        void ActivateTailSlapHeadbutt()
        {
            if(!isDamaged)
            {
                SetHeadbuttAnim(true);
                SetAttackColliders(true);
                SetBodyTrailEffect(true);
            }
        }
        #endregion

        #region Coroutines
        IEnumerator _TakeDamage()
        {
            isDamaged = true;
            // SetHeadbutt(false);
            StopHeadbutt();
            // DeactivateTailSlapAnims();
            StopTailSlap();
            StopJump();
            // if(inShift)
            // {
            //     TakeDamageAndShift();
            // }
            animator.SetBool(damagedAnimHash, true);
            playerCollider.enabled = false;
            SetBodyColliders(false);
            SetAttackColliders(false);
            damageParticles.Enable();
            float t = 0;
            int framesSinceLastFlash = 0;
            baseDragon.DisableMaterials();
            bool materialsEnabled = false;
            bool startedInShift = inShift;
            bool stoppedShift = false;
            bool startedInJump = InJump;
            bool stoppedJump = false;
            bool startedInTailSlap = TailSlapAttackState != ActionState.Off;
            bool stoppedTailSlap = false;
            bool notInFlight = !ForceFlyDown();
            SpeedMultiplier = 1;
            zSpeedMultiplier = 1;
            canFly = notInFlight;
            if(!canFly)
            {
                animator.SetBool(flyDownAnimHash, true);
            }
            else
            {
                ResetY();
            }
            while(t < 2)
            {
                if(!notInFlight)
                {
                    notInFlight = !ForceFlyDown();
                    canFly = notInFlight;
                    if(canFly)
                    {
                        ResetY();
                    }
                }
                t += Time.deltaTime;
                // if((inShift && (startedInShift && stoppedShift || !startedInShift)) || (InJump && (startedInJump && stoppedJump || !startedInJump)) || HeadbuttAttackState != AttackState.Off || (TailSlapAttackState != AttackState.Off && (startedInTailSlap && stoppedTailSlap || !startedInTailSlap)))
                // {
                //     break;
                // }
                // if(startedInJump && !InJump)
                // {
                //     stoppedJump = true;
                // }
                // if(startedInShift && !inShift)
                // {
                //     stoppedShift = true;
                // }
                // if(startedInTailSlap && TailSlapAttackState == AttackState.Off)
                // {
                //     stoppedTailSlap = true;
                // }
                if(framesSinceLastFlash >= 8)
                {
                    framesSinceLastFlash = 0;
                    if(materialsEnabled)
                    {
                        baseDragon.DisableMaterials();
                    }
                    else
                    {
                        baseDragon.EnableMaterials();
                    }
                    materialsEnabled = !materialsEnabled;
                }
                else
                {
                    framesSinceLastFlash++;
                }
                yield return null;
            }
            ResetDamaged();
        }

        IEnumerator _FlyUp(float initY)
        {
            // StartCoroutine(PlaceGameObjs());
            canFly = false;
            SetWingTrailEffect(true);
            animator.SetBool(flyDownAnimHash, false);
            animator.SetBool(flyUpAnimHash, true);
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
                ySpeed = flyUpSpeed * multiplier * Time.deltaTime;
                // Move(ySpeed);
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
            
            animator.SetBool(flyDownAnimHash, true);
            animator.SetBool(flyUpAnimHash, false);
            float timeMulti = 3;
            float timeMultiplier = 0.1f;
            while(absDiff > 0.1f && currentY > baselineY)
            {
                currentY = trans.position.y;
                absDiff = Mathf.Abs(currentY - baselineY);
                float downMultiplier = absDiff > 1 ? 1 : absDiff > 0.5f ? absDiff : 0.5f;
                ySpeed = -flyDownSpeed * timeMultiplier * downMultiplier * Time.deltaTime;
                // Move(ySpeed);
                timeMultiplier += timeMulti * Time.deltaTime;
                timeMultiplier = timeMultiplier < 1 ? timeMultiplier : 1;
                yield return null;
            }
            canFly = true;
            SetWingTrailEffect(false);
            animator.SetBool(flyDownAnimHash, false);
            while(absDiff > 0.01f && currentY > baselineY)
            {
                currentY = trans.position.y;
                absDiff = Mathf.Abs(currentY - baselineY);
                float downMultiplier = absDiff > 0.05f ? absDiff : 0.05f;
                ySpeed = -flyDownSpeed * downMultiplier * Time.deltaTime;
                // Move(ySpeed);
                yield return null;
            }
            ResetY();
            InJump = false;
        }

        IEnumerator _Headbutt()
        {
            SetBodyColliders(false);
            SetHeadbutt(true);
            SetBodyTrailEffect(true);
            float t = 0;
            while(t < headbuttTime)
            {
                ForceFlyDown(true);
                t += Time.deltaTime;
                yield return null;
            }
            SetHeadbuttAnim(false);
            t = 0;
            HeadbuttAttackState = ActionState.Cooldown;
            while(t < 0.2f) //cooldown time
            {
                ForceFlyDown(true);
                t += Time.deltaTime;
                yield return null;
            }
            SetHeadbutt(false);
            SetBodyColliders(true);
            SetBodyTrailEffect(false);
        }

        IEnumerator _TailSlap()
        {
            TailSlapAttackState = ActionState.On;
            noShifting = true;
            animator.SetBool(flyDownAnimHash, false);
            animator.SetBool(flyUpAnimHash, false);
            canFly = false;
            SetWingTrailEffect(false);
            float upTime = dragonTailSlap.expansionTime;
            float t = 0;
            // animator.SetBool(tailSlapUpAnimHash, true);
            // animator.SetBool(tailSlapDownAnimHash, false);
            dragonTailSlap.Enable();
            float targetMulti = 5;
            SetBodyColliders(false);
            while(t < upTime)
            {
                t += Time.deltaTime;
                float diff = (1 - t / upTime);
                float moveMultiplier = 0.5f * diff * diff;
                // moveMultiplier = moveMultiplier > 0 ? moveMultiplier : 0;
                zSpeedMultiplier = moveMultiplier;
                // Move(0, moveMultiplier);
                float speedMulti = SpeedMultiplier;
                if(speedMulti < targetMulti)
                {
                    SpeedMultiplier = Mathf.Lerp(speedMulti, targetMulti, Time.deltaTime);
                }
                yield return null;
            }
            
            float currentY = trans.position.y;
            float absDiff = Mathf.Abs(currentY - baselineY);
            float moveMulti = 0;
            float hTime = 0;
            // float headbuttTime = 1f;
            float minHeadbuttTime = 0;
            bool startedHeadbutt = false;
            if(!isDamaged)
            {
                animator.SetBool(tailSlapUpAnimHash, false);
                animator.SetBool(tailSlapDownAnimHash, true);
                animator.SetBool(headbuttAnimHash, true);
                SetAttackColliders(true);
            }
            while(absDiff > 0.2f && currentY > baselineY)
            {
                moveMulti = dragonTailSlap.explosionEnabled ? 0.5f : 0;
                currentY = trans.position.y;
                absDiff = Mathf.Abs(currentY - baselineY);
                float downMultiplier = absDiff > 1 ? 1 : absDiff > 0.5f ? absDiff : 0.5f;
                ySpeed = -flyDownSpeed * Time.deltaTime * downMultiplier;
                zSpeedMultiplier = moveMulti;
                // Move(ySpeed, moveMulti);
                float speedMulti = SpeedMultiplier;
                if(speedMulti < targetMulti)
                {
                    SpeedMultiplier = Mathf.Lerp(speedMulti, targetMulti, Time.deltaTime * 10);
                }
                hTime += Time.deltaTime;
                if(dragonTailSlap.explosionEnabled && !startedHeadbutt && hTime > minHeadbuttTime)
                {
                    ActivateTailSlapHeadbutt();
                    startedHeadbutt = true;
                }
                yield return null;
            }
            ySpeed = 0;
            t = 0;
            float quitTime = 0.25f;
            while(!dragonTailSlap.explosionEnabled && !isDamaged && t < quitTime)
            {
                t += Time.deltaTime;
                // Debug.Log("explosion not enabled " + t);
                yield return null;
            }
            noShifting = false;
            dragonTailSlap.EnableExplosion();
            currentY = trans.position.y;
            absDiff = Mathf.Abs(currentY - baselineY);
            bool disabledAnim = false;
            if(!startedHeadbutt && hTime > minHeadbuttTime)
            {
                ActivateTailSlapHeadbutt();
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
                ySpeed = -flyDownSpeed * downMultiplier * Time.deltaTime;
                zSpeedMultiplier = moveMulti;
                // Move(ySpeed, moveMulti);
                if(absDiff < 0.1f)
                {
                    disabledAnim = true;
                    animator.SetBool(tailSlapUpAnimHash, false);
                    animator.SetBool(tailSlapDownAnimHash, false);
                }
                hTime += Time.deltaTime;
                if(!startedHeadbutt && hTime > minHeadbuttTime)
                {
                    ActivateTailSlapHeadbutt();
                    startedHeadbutt = true;
                }
                yield return null;
            }
            zSpeedMultiplier = 1;
            SpeedMultiplier = 1;
            dragonTailSlap.Disable();
            canFly = true;
            if(!disabledAnim)
            {
                animator.SetBool(tailSlapUpAnimHash, false);
                animator.SetBool(tailSlapDownAnimHash, false);
            }
            ResetY();
            InJump = false;
            TailSlapAttackState = ActionState.Cooldown;
            if(!startedHeadbutt)
            {
                ActivateTailSlapHeadbutt();
                startedHeadbutt = true;
            }
            while(hTime < headbuttTime)
            {
                hTime += Time.deltaTime;
                // Move(0);
                yield return null;
            }
            SetHeadbuttAnim(false);
            t = 0;
            while(t < 0.2f)
            {
                t += Time.deltaTime;
                // Move(0);
                yield return null;
            }
            SetAttackColliders(false);
            SetBodyTrailEffect(false);
            if(!isDamaged)
            {
                SetBodyColliders(true);
            }
            TailSlapAttackState = ActionState.Off;
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
                zSpeedMultiplier = multiplier;
                // Move(0, multiplier);
                yield return null;
            }
            ShiftAttackState = ActionState.Off;
            tailSlap = _TailSlap();
            StartCoroutine(tailSlap);
        }

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
            ShiftAttackState = ActionState.Cooldown;
            while(ShiftAttackState == ActionState.Cooldown && t < shiftAttackCooldown)
            {
                t += Time.deltaTime;
                yield return null;
            }
            if(ShiftAttackState == ActionState.Cooldown)
            {
                SetShiftAttack(false);
            }
        }

        IEnumerator FireBreath()
        {
            UsingSpecial = true;
            animator.SetBool(fireBreahAnimHash, true);
            fire.Play();
            float fbTime = 8f;
            float time = 0;
            StopHeadbutt();
            StopTailSlap();
            SetBodyColliders(false);
            SetAttackColliders(false);
            while(time < fbTime)
            {
                time += Time.deltaTime;
                Special = maxSpecial * (1 - time / fbTime);
                yield return null;
            }
            UsingSpecial = false;
            animator.SetBool(fireBreahAnimHash, false);
            SetBodyColliders(true);
            SetAttackColliders(true);
            fire.Stop();
            yield break;
        }

        #endregion
        
        #region Test Coroutines
        IEnumerator SimulateTailSlap()
        {
            IsFlying = true;
            float t = 0;
            while(t < 0.5f)
            {
                t += Time.deltaTime;
                Move();
                yield return null;
            }
            UpInput();
            float currentY = trans.position.y;
            float absDiff = Mathf.Abs(currentY - baselineY);
            bool shouldContinue = false;
            bool passedOnce = false;
            while(!passedOnce)
            {
                Move();
                currentY = trans.position.y;
                absDiff = Mathf.Abs(currentY - baselineY);
                if(!passedOnce)
                {
                    if(absDiff > 1f)
                    {
                        passedOnce = true;
                    }
                }
                else if(absDiff < 1f)
                {
                    shouldContinue = true;
                }
                yield return null;
            }
            DownInput();
            while(absDiff > 0.4f)
            {
                currentY = trans.position.y;
                absDiff = Mathf.Abs(currentY - baselineY);
                Move();
                yield return null;
            }
            TakeDamage();
            t = 0;
            while(t < 2)
            {
                t += Time.deltaTime;
                Move();
                Debug.Log(canFly);
                yield return null;
            }
        }

        IEnumerator TestShiftWithDamage()
        {
            ShiftToPosition(testShiftTransform, ShiftDistanceType.x);
            float t = 0;
            while(t < 0.1f)
            {
                t += Time.deltaTime;
                Move();
                yield return null;
            }
            TakeDamage();
            t = 0;
            while(t < 1)
            {
                t += Time.deltaTime;
                Move();
                yield return null;
            }
        }

        IEnumerator TestFlyUpWithDamage()
        {
            Move();
            UpInput();
            float t = 0;
            while(t < 0.1f)
            {
                t += Time.deltaTime;
                Move();
                yield return null;
            }
            TakeDamage();
            t = 0;
            while(t < 1)
            {
                t += Time.deltaTime;
                Move();
                yield return null;
            }
            UpInput();
            t = 0;
            while(t < 1f)
            {
                t += Time.deltaTime;
                Move();
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
}
