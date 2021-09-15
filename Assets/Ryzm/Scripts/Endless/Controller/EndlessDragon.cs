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
        public int maxHealth = 100;
        public int maxSpecial = 50;
        public GameObject placeGOPrefab;
        public LayerMask monsterLayer;
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
        public Transform testShiftTransform;
        public bool testTakeDamage;

        [Header("Trail Effects")]
        public TrailEffect bodyTrailEffect;
        public TrailEffect wingTrailEffect;

        [Header("Attacks")]
        public float headbuttTime = 1;
        public DragonTailSlap dragonTailSlap;
        public DragonHeadbutt dragonHeadbutt;
        public Collider attackCollider;
        public ShiftParticles shiftParticles;
        public BurstParticles burstParticles;
        public EndlessMonsterMetadata monsterMetadata;

        [Header("Damage")]
        public BasicParticles damageParticles;

        [HideInInspector]
        public Vector3 monkeyOffset;
        #endregion

        #region Private Variables
        int _health = 100;
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
        bool isDamaged;
        IEnumerator takeDamage;
        IEnumerator testShiftWithDamage;
        float ySpeed;
        float zSpeedMultiplier;
        float _special;
        bool _usingSpecial;
        // anim hashes
        int flyAnimHash;
        int fireBreahAnimHash;
        int flyDownAnimHash;
        int flyUpAnimHash;
        int damagedAnimHash;
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

        int Health
        {
            get
            {
                return _health;
            }
            set
            {
                _health = value;
                if(_health > maxHealth)
                {
                    _health = maxHealth;
                }
                else if(_health < 0)
                {
                    _health = 0;
                }
                Message.Send(new RunnerHealthResponse(_health, maxHealth));
            }
        }

        float Special
        {
            get
            {
                return _special;
            }
            set
            {
                _special = value;
                if(_special > maxSpecial)
                {
                    _special = maxSpecial;
                }
                else if(_special < 0)
                {
                    _special = 0;
                }
                Message.Send(new RunnerSpecialResponse(_special, maxSpecial));
            }
        }

        bool UsingSpecial
        {
            get
            {
                return _usingSpecial;
            }
            set
            {
                _usingSpecial = value;
                Message.Send(new SpecialAttackResponse(_usingSpecial ? AttackState.On : AttackState.Off));
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
            damagedAnimHash = Animator.StringToHash("damaged");

            base.Awake();
            Health = maxHealth;
            monkeyOffset = monkeyPos.position - trans.position;
            maxShiftCooldown = 1f;
            if(fire != null)
            {
                fire.type = FireType.User;
            }
            SetBodyTrailEffect(false);
            SetWingTrailEffect(false);
            baselineY = trans.position.y;
            Message.AddListener<MonsterMetadataResponse>(OnMonsterMetadataResponse);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Message.AddListener<ControllerTransformRequest>(OnControllerTransformRequest);
            Message.AddListener<ShiftAttackStateRequest>(OnShiftAttackStateRequest);
            Message.AddListener<HeadbuttAttackStateRequest>(OnHeadbuttAttackStateRequest);
            Message.AddListener<RunnerHealthRequest>(OnRunnerHealthRequest);
            Message.AddListener<RunnerSpecialRequest>(OnRunnerSpecialRequest);
            Message.AddListener<CollectGem>(OnCollectGem);
            Message.AddListener<SpecialAttackRequest>(OnSpecialAttackRequest);
        }

        void Start()
        {
            SpeedMultiplier = 1;
            zSpeedMultiplier = 1;
            SetHeadbutt(false);
            SetShiftAttack(false);
            prevPosition = transform.position;
            Message.Send(new ControllerTransformResponse(prevPosition));
            Message.Send(new MonsterMetadataRequest());
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

            if(!InJump && testJump)
            {
                IsFlying = true;
                Jump();
                testJump = false;
            }

            if(HeadbuttAttackState == AttackState.Off && testHeadbutt)
            {
                IsFlying = true;
                if(headbutt != null)
                {
                    StopCoroutine(headbutt);
                    headbutt = null;
                }
                headbutt = _Headbutt(true);
                StartCoroutine(headbutt);
                testHeadbutt = false;
            }

            if(TailSlapAttackState == AttackState.Off && testTailSlap)
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
        }

        void OnTriggerEnter(Collider col)
        {
            if(HeadbuttAttackState != AttackState.Off || TailSlapAttackState != AttackState.Off) //  || UsingSpecial (if using trigger)
            {
                MonsterBase monster = col.gameObject.GetComponent<MonsterBase>();
                if(monster != null)
                {
                    if(UsingSpecial)
                    {
                        monster.TakeSpecialDamage();
                    }
                    else
                    {
                        monster.TakeDamage();
                    }
                }
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Message.RemoveListener<ControllerTransformRequest>(OnControllerTransformRequest);
            Message.RemoveListener<ShiftAttackStateRequest>(OnShiftAttackStateRequest);
            Message.RemoveListener<HeadbuttAttackStateRequest>(OnHeadbuttAttackStateRequest);
            Message.RemoveListener<RunnerHealthRequest>(OnRunnerHealthRequest);
            Message.RemoveListener<RunnerSpecialRequest>(OnRunnerSpecialRequest);
            Message.RemoveListener<CollectGem>(OnCollectGem);
            Message.RemoveListener<SpecialAttackRequest>(OnSpecialAttackRequest);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Message.RemoveListener<MonsterMetadataResponse>(OnMonsterMetadataResponse);
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

        protected override void OnRunnerHit(RunnerHit runnerHit)
        {
            // Debug.Log("runner hit");
            if(!isDamaged)
            {
                int damage = runnerHit.damage;
                if(damage <= 0)
                {
                    MonsterMetadata metadata = monsterMetadata.GetMonsterMetadata(runnerHit.monsterType);
                    damage = runnerHit.attackType == AttackType.Physical ? metadata.physicalDamage : metadata.specialDamage;
                }
                
                Health -= damage;
                if(Health <= 0)
                {
                    Die();
                }
                else
                {
                    // todo: stop all attacking coroutines and run damaged animation 
                    TakeDamage();
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

        void OnRunnerHealthRequest(RunnerHealthRequest request)
        {
            Message.Send(new RunnerHealthResponse(Health, maxHealth));
        }

        void OnRunnerSpecialRequest(RunnerSpecialRequest request)
        {
            Message.Send(new RunnerSpecialResponse(Special, maxHealth));
        }

        void OnMonsterMetadataResponse(MonsterMetadataResponse response)
        {
            monsterMetadata = response.monsterMetadata;
        }

        void OnCollectGem(CollectGem collectGem)
        {
            if(!UsingSpecial)
            {
                Special += 1;
            }
        }

        void OnSpecialAttackRequest(SpecialAttackRequest request)
        {
            if(!UsingSpecial)
            {
                if(Special >= maxSpecial)
                {
                    Message.Send(new SpecialAttackResponse(AttackState.On));
                    fireBreath = FireBreath();
                    StartCoroutine(fireBreath);
                }
                else
                {
                    Message.Send(new SpecialAttackResponse(AttackState.Off));
                }
            }
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
            if(!UsingSpecial && HeadbuttAttackState == AttackState.Off && TailSlapAttackState != AttackState.On)
            {
                float absDiff = Mathf.Abs(trans.position.y - baselineY);
                if(absDiff > 0.1f && InJump) // high enough above ground to do ground pound move
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
                    if(TailSlapAttackState == AttackState.Cooldown)
                    {
                        TailSlapAttackState = AttackState.Off;
                        StopCoroutine(tailSlap);
                        tailSlap = null;
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
            else if(!InJump && HeadbuttAttackState != AttackState.Off && numHeadbutts < maxHeadbutts)
            {
                StopCoroutine(headbutt);
                headbutt = null;
                headbutt = _Headbutt();
                StartCoroutine(headbutt);
            }
        }

        public override void Jump()
        {
            if(!inShift && canFly)
            {
                if(IsFlying)
                {
                    if(!InJump)
                    {
                        StopHeadbutt();
                        flyUp = FlyUp(trans.position.y);
                        StartCoroutine(flyUp);
                    }
                    else
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

        public void Fly(bool shouldFly = true)
        {
            IsFlying = shouldFly;
        }

        public void MoveWithMultiplier(float multi)
        {
            SpeedMultiplier = multi;
            zSpeedMultiplier = multi;
            Move(0, multi);
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
            if(TailSlapAttackState == AttackState.On)
            {
                dragonTailSlap.EnableExplosion();
            }
        }
        #endregion

        #region Protected Functions
        protected override void Reset()
        {
            animator.SetBool(flyDownAnimHash, false);
            animator.SetBool(flyUpAnimHash, false);
            animator.SetBool(tailSlapUpAnimHash, false);
            animator.SetBool(tailSlapDownAnimHash, false);
            SetHeadbutt(false);
            HeadbuttAttackState = AttackState.Off;
            TailSlapAttackState = AttackState.Off;
            noShifting = false;
            ShiftAttackState = AttackState.Off;
            if(InJump)
            {
                ResetY();
            }
            dragonTailSlap.InstantDisable();
            Health = maxHealth;
            ResetDamaged();
            base.Reset();
            canFly = true;
            SetBodyColliders(true);
            zSpeedMultiplier = 1;
            Special = 0;
            UsingSpecial = false;
        }
        #endregion

        #region Private Functions
        void EndlessRun()
        {
            IsFlying = true;
            // if(!InJump && HeadbuttAttackState == AttackState.Off && TailSlapAttackState == AttackState.Off)
            // {
            // }
            Move(0);

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

        void Move(float yMove, float multiplier = 1)
        {
            float zMove = Time.deltaTime * forwardSpeed * multiplier;
            zMove = Time.deltaTime * forwardSpeed * zSpeedMultiplier;
            move.z = zMove;
            // move.y = yMove;
            move.y = ySpeed;
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
            ShiftAttackState = enabled ? AttackState.On : AttackState.Off;
            SetAttackCollider(enabled);
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
            if(TailSlapAttackState != AttackState.Off)
            {
                zSpeedMultiplier = 1;
                dragonTailSlap.Disable();
                TailSlapAttackState = AttackState.Off;
                SetAttackCollider(false);
                SetBodyTrailEffect(false);
                SetBodyColliders(true);
                SetHeadbuttAnim(false);
                animator.SetBool(tailSlapUpAnimHash, false);
                animator.SetBool(tailSlapDownAnimHash, false);

                if(tailSlap != null)
                {
                    StopCoroutine(tailSlap);
                    tailSlap = null;
                }
            }
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

        void TakeDamage()
        {
            // if(TailSlapAttackState != AttackState.Off)
            // {
            //     TailSlapAttackState = AttackState.Off;
            //     StopCoroutine(tailSlap);
            //     tailSlap = null;
            // }
            if(takeDamage != null)
            {
                StopCoroutine(takeDamage);
                takeDamage = null;
            }
            takeDamage = _TakeDamage();
            StartCoroutine(takeDamage);
        }

        void ResetDamaged()
        {
            if(isDamaged)
            {
                isDamaged = false;
                animator.SetBool(damagedAnimHash, false);
                baseDragon.EnableMaterials();
                playerCollider.enabled = true;
                if(TailSlapAttackState == AttackState.Off && HeadbuttAttackState == AttackState.Off)
                {
                    SetBodyColliders(true);
                }
                damageParticles.Disable();
                // if(!InJump)
                // {
                //     canFly = true;
                // }
            }
        }

        void ResetY()
        {
            trans.position = new Vector3(trans.position.x, baselineY, trans.position.z);
            ySpeed = 0;
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
            // if(inShift)
            // {
            //     TakeDamageAndShift();
            // }
            animator.SetBool(damagedAnimHash, true);
            playerCollider.enabled = false;
            SetBodyColliders(false);
            SetAttackCollider(false);
            damageParticles.Enable();
            float t = 0;
            int framesSinceLastFlash = 0;
            baseDragon.DisableMaterials();
            bool materialsEnabled = false;
            bool startedInShift = inShift;
            bool stoppedShift = false;
            bool startedInJump = InJump;
            bool stoppedJump = false;
            bool startedInTailSlap = TailSlapAttackState != AttackState.Off;
            bool stoppedTailSlap = false;
            bool notInFlight = !ForceFlyDown();
            canFly = notInFlight;
            while(t < 2)
            {
                if(!notInFlight)
                {
                    notInFlight = !ForceFlyDown();
                    canFly = notInFlight;
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

        IEnumerator TestShiftWithDamage()
        {
            ShiftToPosition(testShiftTransform, ShiftDistanceType.x);
            float t = 0;
            while(t < 0.1f)
            {
                t += Time.deltaTime;
                Move(0);
                yield return null;
            }
            TakeDamage();
            t = 0;
            while(t < 1)
            {
                t += Time.deltaTime;
                Move(0);
                yield return null;
            }
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

        IEnumerator FlyUp(float initY)
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

        IEnumerator _Headbutt(bool shouldWait = false)
        {
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
                ForceFlyDown(true);
                t += Time.deltaTime;
                yield return null;
            }
            SetHeadbuttAnim(false);
            t = 0;
            HeadbuttAttackState = AttackState.Cooldown;
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

        void ActivateTailSlapHeadbutt()
        {
            if(!isDamaged)
            {
                SetHeadbuttAnim(true);
                SetAttackCollider(true);
                SetBodyTrailEffect(true);
            }
        }

        void DeactivateTailSlapAnims()
        {
            animator.SetBool(tailSlapUpAnimHash, false);
            animator.SetBool(tailSlapDownAnimHash, false);
            SetHeadbuttAnim(false);
            SetBodyTrailEffect(false);
            SetWingTrailEffect(false);
            dragonTailSlap.Disable();
        }

        IEnumerator _TailSlap()
        {
            TailSlapAttackState = AttackState.On;
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
                SetAttackCollider(true);
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
            TailSlapAttackState = AttackState.Cooldown;
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
            SetAttackCollider(false);
            SetBodyTrailEffect(false);
            if(!isDamaged)
            {
                SetBodyColliders(true);
            }
            TailSlapAttackState = AttackState.Off;
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
            UsingSpecial = true;
            animator.SetBool(fireBreahAnimHash, true);
            fire.Play();
            float fbTime = 8f;
            float time = 0;
            StopHeadbutt();
            StopTailSlap();
            SetBodyColliders(false);
            SetAttackCollider(false);
            while(time < fbTime)
            {
                time += Time.deltaTime;
                Special = maxSpecial * (1 - time / fbTime);
                yield return null;
            }
            UsingSpecial = false;
            animator.SetBool(fireBreahAnimHash, false);
            SetBodyColliders(true);
            SetAttackCollider(true);
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
