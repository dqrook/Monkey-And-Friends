using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;
using TrailsFX;

namespace Ryzm.EndlessRunner
{
    public class EndlessController : MonoBehaviour
    {
        #region Public Variables
        public int maxHealth = 100;
        public int maxSpecial = 150;
        public RuntimeAnimatorController animatorController;
        public float forwardSpeed = 10;
		public Animator animator;
        public Collider playerCollider;
        public Transform rootTransform;
        public float distanceToGround = 0.5f;
		public LayerMask groundLayer;
        public Transform localCameraSpawn;
        public EndlessMonsterMetadata monsterMetadata;
        public LayerMask monsterLayer;
        public GameObject placeGOPrefab;

        [Header("Colliders")]
        public Collider[] bodyColliders;
        public Collider[] attackColliders;

        [Header("Trail Effects")]
        public TrailEffect bodyTrailEffect;
        public TrailEffect wingTrailEffect;
        public TrailEffect shiftTrailEffect;
        #endregion

        #region Events
        public delegate void StartTouch(Vector2 position, float time);
        public event StartTouch OnStartTouch;
        public delegate void EndTouch(Vector2 position, float time);
        public event EndTouch OnEndTouch;
        #endregion

        #region Protected Variables
        protected Rigidbody rb;
		protected Vector3 move;
		protected Transform trans;
        protected int _currentPosition = 1;
        // turned is set to true when on a TSection and the user has decided which direction they would like to go
        protected bool turned = false;
        protected int state = 0;
        protected bool inShift;
        protected IEnumerator shift;
        protected bool _inJump;
        protected EndlessSection _endlessSection;
        protected EndlessTSection _endlessTSection;
        protected EndlessTurnSection _endlessTurnSection;
        protected GameStatus gameStatus;
        protected float distanceTraveled;
        protected float shiftSpeed;
		protected ControllerMode mode;
        protected Camera mainCamera;
        protected bool inSlide;
        protected float maxShiftCooldown = 0.25f;
        protected RaycastHit hit;
        protected int _health = 100;
        protected float _special;
        protected bool _usingSpecial;
        protected Vector3 prevPosition;
        protected Vector3 currentPosition;
        protected bool isDamaged;
        protected bool noShifting;
        protected int damagedAnimHash;
        protected int stateAnimHash;
        protected float zSpeedMultiplier;
        #endregion

        #region Private Variables
        Ray checkGround;
        #endregion

        #region Properties
        public int CurrentPosition
        {
            get
            {
                return _currentPosition;
            }
            set
            {
                _currentPosition = value;
                Message.Send(new CurrentPositionResponse(value));
            }
        }

        protected bool InJump
        {
            get
            {
                return _inJump;
            }
            set
            {
                _inJump = value;
                Message.Send(new JumpStatusResponse(value));
            }
        }

        protected int State
        {
            get
            {
                return state;
            }
            set
            {
                state = value;
                animator.SetInteger(stateAnimHash, state);
            }
        }

        protected int Health
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

        protected float Special
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

        protected bool UsingSpecial
        {
            get
            {
                return _usingSpecial;
            }
            set
            {
                _usingSpecial = value;
                Message.Send(new SpecialAttackResponse(_usingSpecial ? ActionState.On : ActionState.Off));
            }
        }
        #endregion
        
        #region Event Functions
		protected virtual void Awake()
		{
            stateAnimHash = Animator.StringToHash("state");
            damagedAnimHash = Animator.StringToHash("damaged");
            trans = GetComponent<Transform> ();
            rb = GetComponent<Rigidbody>();
            if(playerCollider == null)
            {
                playerCollider = gameObject.GetComponent<Collider>();
            }

			if (animator == null)
            {
				animator = gameObject.GetComponentInChildren<Animator> ();
			}

			if (animatorController == null)
            {
				Debug.LogError ("Missing : animatorController.");
			}
            animator.runtimeAnimatorController = animatorController;

            mainCamera = Camera.main;
            State = 0;
            Health = maxHealth;
            SetBodyColliders(true);
            SetAttackColliders(false);
            CurrentPosition = 1;
            Message.AddListener<MonsterMetadataResponse>(OnMonsterMetadataResponse);

            // rb.constraints = RigidbodyConstraints.FreezeAll;
            // playerInput.Touch.PrimaryContact.started += (ctx) => StartTouchPrimary(ctx);
            // playerInput.Touch.PrimaryContact.canceled += (ctx) => EndTouchPrimary(ctx);
		}

        // void StartTouchPrimary(InputAction.CallbackContext ctx)
        // {
        //     if(OnStartTouch != null)
        //     {
        //         OnStartTouch(EndlessUtils.ScreenToWorld(mainCamera, playerInput.Touch.PrimaryPosition.ReadValue<Vector2>()), (float)ctx.startTime);
        //     }
        // }

        // void EndTouchPrimary(InputAction.CallbackContext ctx)
        // {
        //     if(OnEndTouch != null)
        //     {
        //         OnEndTouch(EndlessUtils.ScreenToWorld(mainCamera, playerInput.Touch.PrimaryPosition.ReadValue<Vector2>()), (float)ctx.time);
        //     }
        // }

        // public Vector2 PrimaryPosition()
        // {
        //     return EndlessUtils.ScreenToWorld(mainCamera, playerInput.Touch.PrimaryPosition.ReadValue<Vector2>());
        // }

        protected virtual void OnEnable()
        {
			Message.Send(new ControllerModeRequest());
            Message.AddListener<ControllerTransformRequest>(OnControllerTransformRequest);
            Message.AddListener<RunnerDistanceRequest>(OnRunnerDistanceRequest);
            Message.AddListener<CurrentSectionChange>(OnCurrentSectionChange);
            Message.AddListener<RunnerHit>(OnRunnerHit);
            Message.AddListener<CurrentPositionRequest>(OnCurrentPositionRequest);
            Message.AddListener<GameStatusResponse>(OnGameStatusResponse);
            Message.AddListener<ControllerModeResponse>(OnControllerModeResponse);
            Message.AddListener<LocalCameraSpawnResponse>(OnLocalCameraSpawnResponse);
            Message.AddListener<RunnerHealthRequest>(OnRunnerHealthRequest);
            Message.AddListener<RunnerSpecialRequest>(OnRunnerSpecialRequest);
            Message.AddListener<CollectGem>(OnCollectGem);
            Message.AddListener<SpecialAttackRequest>(OnSpecialAttackRequest);
            Message.Send(new LocalCameraSpawnRequest());
        }

        protected virtual void Start()
        {
            prevPosition = transform.position;
            Message.Send(new MonsterMetadataRequest());
            zSpeedMultiplier = 1;
        }

        protected virtual void OnDisable()
		{
            Message.RemoveListener<ControllerTransformRequest>(OnControllerTransformRequest);
            Message.RemoveListener<RunnerDistanceRequest>(OnRunnerDistanceRequest);
            Message.RemoveListener<CurrentSectionChange>(OnCurrentSectionChange);
            Message.RemoveListener<RunnerHit>(OnRunnerHit);
            Message.RemoveListener<CurrentPositionRequest>(OnCurrentPositionRequest);
            Message.RemoveListener<GameStatusResponse>(OnGameStatusResponse);
            Message.RemoveListener<ControllerModeResponse>(OnControllerModeResponse);
            Message.RemoveListener<LocalCameraSpawnResponse>(OnLocalCameraSpawnResponse);
            Message.RemoveListener<RunnerHealthRequest>(OnRunnerHealthRequest);
            Message.RemoveListener<RunnerSpecialRequest>(OnRunnerSpecialRequest);
            Message.RemoveListener<CollectGem>(OnCollectGem);
            Message.RemoveListener<SpecialAttackRequest>(OnSpecialAttackRequest);
		}

		protected virtual void OnDestroy()
        {
            Message.RemoveListener<MonsterMetadataResponse>(OnMonsterMetadataResponse);
        }

        protected virtual void OnTriggerEnter(Collider col) {}
        #endregion

        #region Listener Functions
        protected virtual void OnCurrentSectionChange(CurrentSectionChange change)
        {
            turned = false;
            _endlessSection = change.endlessSection;
            _endlessTurnSection = change.endlessTurnSection;
            _endlessTSection = change.endlessTSection;
        }

        protected virtual void OnRunnerHit(RunnerHit runnerHit)
        {
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

        protected virtual void OnCurrentPositionRequest(CurrentPositionRequest request)
        {
            Message.Send(new CurrentPositionResponse(CurrentPosition));
        }

        protected virtual void OnGameStatusResponse(GameStatusResponse response)
        {
            gameStatus = response.status;
            if(response.status == GameStatus.Restart || response.status == GameStatus.Exit)
            {
                Reset();
            }
        }

		protected virtual void OnControllerModeResponse(ControllerModeResponse response)
		{
			mode = response.mode;
		}

        protected virtual void OnRunnerDistanceRequest(RunnerDistanceRequest request) {}

        protected void OnLocalCameraSpawnResponse(LocalCameraSpawnResponse update)
        {
            Vector3 angleDiff = update.localEulerAngles - localCameraSpawn.localEulerAngles;
            Quaternion newAngles = Quaternion.Euler(localCameraSpawn.eulerAngles + angleDiff);
            Vector3 newPos = trans.TransformPoint(update.localPosition);
            localCameraSpawn.SetPositionAndRotation(newPos, newAngles);
        }

        protected void OnControllerTransformRequest(ControllerTransformRequest request)
        {
            currentPosition = trans.position;
            Message.Send(new ControllerTransformResponse(currentPosition));
        }

        protected void OnMonsterMetadataResponse(MonsterMetadataResponse response)
        {
            monsterMetadata = response.monsterMetadata;
        }

        protected void OnRunnerHealthRequest(RunnerHealthRequest request)
        {
            Message.Send(new RunnerHealthResponse(Health, maxHealth));
        }

        protected void OnRunnerSpecialRequest(RunnerSpecialRequest request)
        {
            Message.Send(new RunnerSpecialResponse(Special, maxSpecial));
        }

        protected void OnCollectGem(CollectGem collectGem)
        {
            if(!UsingSpecial)
            {
                Special += 1;
            }
        }

        protected virtual void OnSpecialAttackRequest(SpecialAttackRequest request) {}
        #endregion

        #region Public Functions
        public virtual void Shift(Direction direction)
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
                else
                {
                    float xDistance = 0;
                    // if(direction == Direction.Left && CurrentPosition > 0)
                    // {
                    //     xDistance = -1.5f;
                    //     CurrentPosition--;
                    // }
                    // else if(direction == Direction.Right && CurrentPosition < 2)
                    // {
                    //     xDistance = 1.5f;
                    //     CurrentPosition++;
                    // }
                    xDistance = direction == Direction.Left ? -1.25f : 1.25f;
                    if(xDistance != 0)
                    {
                        shift = _Shift(trans.TransformPoint(new Vector3(xDistance, 0, 0)));
                        StartCoroutine(shift);
                    }
                }
            }
        }
        
        public virtual void ShiftToPosition(Transform target, ShiftDistanceType type)
        {
            shift = _Shift(target, type);
            StartCoroutine(shift);
        }

        public void FinishShift()
        {
            inShift = false;
        }

        public virtual void DownInput() {}

		public virtual void UpInput() {}

        #endregion

        #region Protected Functions
        protected virtual void OnTrigger(GameObject target)
        {
            MonsterBase monster = target.GetComponent<MonsterBase>();
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

        protected void SetBodyColliders(bool enabled)
        {
            foreach(Collider bodyCollider in bodyColliders)
            {
                bodyCollider.enabled = enabled;
            }
        }

        protected void SetAttackColliders(bool enabled)
        {
            foreach(Collider attackCollider in attackColliders)
            {
                attackCollider.enabled = enabled;
            }
        }

        protected void SetBodyTrailEffect(bool active)
        {
            if(bodyTrailEffect != null)
            {
                bodyTrailEffect.active = active;
            }
        }

        protected void SetWingTrailEffect(bool active)
        {
            if(wingTrailEffect != null)
            {
                wingTrailEffect.active = active;
            }
        }

        protected void SetShiftTrailEffect(bool active)
        {
            if(shiftTrailEffect != null)
            {
                shiftTrailEffect.active = active;
            }
        }

        protected virtual void Die() {}
        
        protected virtual bool IsGrounded()
        {
            checkGround = new Ray(rootTransform.position, Vector3.down);
            bool grounded = Physics.Raycast(checkGround, out hit, distanceToGround, groundLayer);
            return grounded;
        }

        protected float GetShiftDistance(Transform target, ShiftDistanceType type = ShiftDistanceType.x)
        {
            return type == ShiftDistanceType.x ? target.InverseTransformPoint(trans.position).x : target.InverseTransformPoint(trans.position).z;
        }

        protected float GetShiftDistance(Vector3 position, ShiftDistanceType type = ShiftDistanceType.x)
        {
            return type == ShiftDistanceType.x ? trans.InverseTransformPoint(position).x : trans.InverseTransformPoint(position).z;
        }

        protected bool IsShifting(Direction direction)
        {
            if(direction == Direction.Left)
            {
                return false;
            }
            if(direction == Direction.Right)
            {
                return false;
            }
            return false;
        }

        protected void UpdateControllerTransform()
        {
            currentPosition = trans.position;
            Vector3 distance = (currentPosition - prevPosition);
            float sqrDistance = distance.sqrMagnitude;
            if(sqrDistance > 1)
            {
                Message.Send(new ControllerTransformResponse(currentPosition));
                prevPosition = currentPosition;
            }
        }

        protected virtual void TakeDamage() {}

        protected virtual void Reset()
        {
            ResetDamaged();
            StopAllCoroutines();
            SetShiftTrailEffect(false);
            turned = false;
            State = 0;
            InJump = false;
            inShift = false;
            inSlide = false;
            distanceTraveled = 0;
            CurrentPosition = 1;
            shiftSpeed = 0;
            rb.isKinematic = true;
            rb.isKinematic = false;
            _endlessSection = null;
            _endlessTSection = null;
            _endlessTurnSection = null;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
            Message.Send(new RunnerDistanceResponse(distanceTraveled));
            zSpeedMultiplier = 1;
            Health = maxHealth;
            Special = 0;
            UsingSpecial = false;
            noShifting = false;
        }

        protected bool CheckIfHit(float direction)
        {
            Vector3 spherePos = currentPosition;
            spherePos.x += trans.right.x * 1.5f * direction;
            bool hitIt = Physics.SphereCast(spherePos, 1, trans.forward, out hit, 3, (int)monsterLayer);
            return hitIt;
        }

        protected virtual void ResetDamaged()
        {
            if(isDamaged)
            {
                isDamaged = false;
                animator.SetBool(damagedAnimHash, false);
                playerCollider.enabled = true;
            }
        }
        protected void MakeGO(Vector3 curPos)
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
        protected IEnumerator _Shift(Transform target, ShiftDistanceType type)
        {
            inShift = true;
            float _shiftDistance = GetShiftDistance(target, type);
            float _distance = Mathf.Lerp(0, _shiftDistance, 0.1f);
            float signDistance = Mathf.Sign(_distance);
            
            if(signDistance == 1)
            {
                animator.SetTrigger("shiftRight");
            }
            else
            {
                animator.SetTrigger("shiftLeft");
            }
            float absDistance = _distance * signDistance;
            shiftSpeed = signDistance;
            float signShiftDistance = Mathf.Sign(_shiftDistance);
            while(_shiftDistance * signShiftDistance > absDistance && signShiftDistance == signDistance)
            {
                _shiftDistance = GetShiftDistance(target, type);
                signShiftDistance = Mathf.Sign(_shiftDistance);
                yield return null;
            }
            shiftSpeed = 0;
            _shiftDistance = GetShiftDistance(target, type);
            trans.Translate(_shiftDistance, 0, 0);
            
            float cooldownTime = 0;
            while(inShift && cooldownTime < maxShiftCooldown) 
            {
                cooldownTime += Time.deltaTime;
                yield return null;
            }
            _shiftDistance = GetShiftDistance(target, type);
            trans.Translate(_shiftDistance, 0, 0);
            inShift = false;
        }
        protected IEnumerator _Shift(Vector3 targetPosition)
        {
            SetShiftTrailEffect(true);
            inShift = true;
            float _shiftDistance = GetShiftDistance(targetPosition);
            float _distance = Mathf.Lerp(0, _shiftDistance, 0.1f);
            float signDistance = Mathf.Sign(_distance);
            
            if(signDistance == 1)
            {
                animator.SetTrigger("shiftRight");
            }
            else
            {
                animator.SetTrigger("shiftLeft");
            }
            float absDistance = _distance * signDistance;
            shiftSpeed = signDistance;
            float signShiftDistance = Mathf.Sign(_shiftDistance);
            while(_shiftDistance * signShiftDistance > absDistance && signShiftDistance == signDistance)
            {
                _shiftDistance = GetShiftDistance(targetPosition);
                signShiftDistance = Mathf.Sign(_shiftDistance);
                yield return null;
            }
            shiftSpeed = 0;
            _shiftDistance = GetShiftDistance(targetPosition);
            trans.Translate(_shiftDistance, 0, 0);
            
            float cooldownTime = 0;
            while(inShift && cooldownTime < maxShiftCooldown) 
            {
                cooldownTime += Time.deltaTime;
                yield return null;
            }
            SetShiftTrailEffect(false);
            _shiftDistance = GetShiftDistance(targetPosition);
            trans.Translate(_shiftDistance, 0, 0);
            inShift = false;
        }

        protected IEnumerator PlaceGameObjs()
        {
            Vector3 curPos = transform.position;
            Vector3 prevPos = transform.position;
            MakeGO(curPos);
            while(true)
            {
                curPos = transform.position;
                float diff = Mathf.Abs(curPos.z - prevPos.z);
                if(diff > 0.25f)
                {
                    prevPos = curPos;
                    MakeGO(curPos);
                }
                yield return null;
            }
        }
        #endregion
    }
}
