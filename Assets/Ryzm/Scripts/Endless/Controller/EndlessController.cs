using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public class EndlessController : MonoBehaviour
    {
        #region Public Variables
        public RuntimeAnimatorController animatorController;
        public float forwardSpeed = 10;
		public Animator animator;
        public Collider playerCollider;
        public Transform rootTransform;
        public float distanceToGround = 0.5f;
		public LayerMask groundLayer;
        public Transform localCameraSpawn;
        public List<Collider> bodyColliders = new List<Collider>();
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
        public float shiftSpeed;
		protected ControllerMode mode;
        protected Camera mainCamera;
        protected bool inSlide;
        protected float maxShiftCooldown = 0.25f;
        protected RaycastHit hit;
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
                animator.SetInteger("state", state);
            }
        }
        #endregion
        
        #region Event Functions
		protected virtual void Awake()
		{
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
            rb.constraints = RigidbodyConstraints.FreezeAll;
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
            Message.AddListener<RunnerDistanceRequest>(OnRunnerDistanceRequest);
            Message.AddListener<CurrentSectionChange>(OnCurrentSectionChange);
            Message.AddListener<RunnerHit>(OnRunnerHit);
            Message.AddListener<CurrentPositionRequest>(OnCurrentPositionRequest);
            Message.AddListener<GameStatusResponse>(OnGameStatusResponse);
            Message.AddListener<ControllerModeResponse>(OnControllerModeResponse);
        }

        protected virtual void OnDisable()
		{
            Message.RemoveListener<RunnerDistanceRequest>(OnRunnerDistanceRequest);
            Message.RemoveListener<CurrentSectionChange>(OnCurrentSectionChange);
            Message.RemoveListener<RunnerHit>(OnRunnerHit);
            Message.RemoveListener<CurrentPositionRequest>(OnCurrentPositionRequest);
            Message.RemoveListener<GameStatusResponse>(OnGameStatusResponse);
            Message.RemoveListener<ControllerModeResponse>(OnControllerModeResponse);
		}

		protected virtual void OnDestroy()
        {
            // Message.RemoveListener<CurrentSectionChange>(OnCurrentSectionChange);
            // Message.RemoveListener<RunnerDie>(OnRunnerDie);
            // Message.RemoveListener<CurrentPositionRequest>(OnCurrentPositionRequest);
            // Message.RemoveListener<GameStatusResponse>(OnGameStatusResponse);
            // Message.RemoveListener<ControllerModeResponse>(OnControllerModeResponse);
        }
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
            Die();
        }

        protected virtual void OnCurrentPositionRequest(CurrentPositionRequest request)
        {
            Message.Send(new CurrentPositionResponse(CurrentPosition));
        }

        protected virtual void OnGameStatusResponse(GameStatusResponse response)
        {
            gameStatus = response.status;
        }

		protected virtual void OnControllerModeResponse(ControllerModeResponse response)
		{
			mode = response.mode;
		}

        protected virtual void OnRunnerDistanceRequest(RunnerDistanceRequest request) {}
        #endregion

        #region Public Variables
        public virtual void Shift(Direction direction)
        {
            if(!inShift && !InJump && !inSlide)
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
        
        public virtual void ShiftToPosition(Transform pos, ShiftDistanceType type)
        {
            shift = _Shift(pos, type);
            StartCoroutine(shift);
        }

        public void FinishShift()
        {
            inShift = false;
            // float diff = Time.time - shiftTime;
            // Debug.Log("shift finished " + diff);
        }

        public virtual void Attack() {}

		public virtual void Jump() {}

		public virtual void Slide() {}

        public virtual void Die() {}
        #endregion

        #region Protected Functions
        protected bool IsGrounded()
        {
            checkGround = new Ray(rootTransform.position, Vector3.down);
            bool grounded = Physics.Raycast(checkGround, out hit, distanceToGround, groundLayer);
            return grounded;
        }

        protected float GetShiftDistance(Transform target, ShiftDistanceType type = ShiftDistanceType.x)
        {
            return type == ShiftDistanceType.x ? target.InverseTransformPoint(trans.position).x : target.InverseTransformPoint(trans.position).z;
        }

		protected bool IsAttacking()
        {
			return false;
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

        protected virtual void Reset()
        {
            StopAllCoroutines();
            turned = false;
            // state = 0;
            // animator.SetInteger("state", state);
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
        }
        #endregion
        
        #region Coroutines
        protected IEnumerator _Shift(Transform target, ShiftDistanceType type = ShiftDistanceType.x, bool skipAnim = false)
        {
            inShift = true;
            float _shiftDistance = GetShiftDistance(target, type);
            float _distance = Mathf.Lerp(0, _shiftDistance, 0.1f);
            float signDistance = Mathf.Sign(_distance);
            
            if(!skipAnim)
            {
                if(signDistance == 1)
                {
                    animator.SetTrigger("shiftRight");
                }
                else
                {
                    animator.SetTrigger("shiftLeft");
                }
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
            _shiftDistance = GetShiftDistance(target, type);
            trans.Translate(_shiftDistance, 0, 0);
            shiftSpeed = 0;
            
            float cooldownTime = 0;
            while(inShift && cooldownTime < maxShiftCooldown) 
            {
                cooldownTime += Time.deltaTime;
                yield return null;
            }
            inShift = false;
        }
        #endregion
    }
}
