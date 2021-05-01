using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ryzm.EndlessRunner
{
    public class RunnerController : BaseController
    {
        public float jumpCooldown = 0.2f;
        public static bool isDead;
        public Transform rootTransform;
        public float distanceToGround = 0.7f;
		public LayerMask groundLayer;
        public int currentPosition = 1;
        public RuntimeAnimatorController animatorController;
        public static GameObject player;
        static GameObject _currentPlatform;
        static EndlessSection currentSection;
        static EndlessTSection currentTSection;
        // turned is set to true when on a TSection and the user has decided which direction they would like to go
        static bool turned = false;
        RaycastHit hit;
        Ray checkGround;
        int state = 0;
        Rigidbody rb;
        IEnumerator monitorJump;
        bool inShift;
        IEnumerator shift;
        bool inJump;

        public static GameObject CurrentPlatform
        {
            get
            {
                return _currentPlatform;
            }
            set
            {
                _currentPlatform = value;
                GameObject _ = _currentPlatform.GetComponent<DeactivateSection>().section.gameObject;
                currentTSection = _.GetComponent<EndlessTSection>();
                currentSection = _.GetComponent<EndlessSection>();
                turned = false;
            }
        }

        public static EndlessSection CurrentSection
        {
            get
            {
                return currentTSection != null ? currentTSection : currentSection;
            }
        }

        void OnCollisionEnter(Collision other)
        {
            if(other.gameObject.tag == "Fire")
            {
                isDead = true;
                Die();
            }
            else
            {
                CurrentPlatform = other.gameObject;
            }
        }

		protected override void Awake ()
		{
			base.Awake();

			if (animator == null)
            {
				animator = gameObject.GetComponentInChildren<Animator> ();
			}

			if (animatorController == null)
            {
				Debug.LogError ("Missing : animatorController.");
			}

			animator.runtimeAnimatorController = animatorController;
            player = this.gameObject;
            rb = GetComponent<Rigidbody>();
		}
        
        bool IsGrounded()
        {
            checkGround = new Ray (rootTransform.position, Vector3.down);
            bool grounded = Physics.Raycast(checkGround, out hit, distanceToGround, groundLayer);
            return grounded;
        }

        void Start()
        {
            GenerateWorld.RunDummy();
        }

        void OnTriggerEnter(Collider other)
        {
            if(other is BoxCollider && GenerateWorld.lastPlatform.tag != "platformTSection")
            {
                GenerateWorld.RunDummy();
            }
        }

        public void Jump()
        {
            bool isGrounded = IsGrounded();
            Jump(isGrounded);
        }

        public void Jump(bool isGrounded)
        {
            if(!inJump && isGrounded)
            {
                monitorJump = MonitorJump();
                StartCoroutine(monitorJump);
                animator.SetTrigger("jump");
                rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            }
        }
        
        protected override void Update()
        {
            bool isGrounded = IsGrounded();
            input.z = 1;
            move = Vector3.zero;
			animator.SetFloat("speed_z", 1);
			animator.SetFloat("speed_x", 0);
			animator.SetBool("is_grounded", isGrounded);
            animator.SetInteger("state", state);
            
            if(IsShifting(Direction.Left))
            {
                Shift(Direction.Left);
            }
            else if(IsShifting(Direction.Right))
            {
                Shift(Direction.Right);
            }

            if (IsJumping())
            {
                Jump(isGrounded);
			}
            if(IsAttacking())
            {
                Attack();
            }
        }
        
        IEnumerator MonitorJump()
        {
            inJump = true;
            yield return new WaitForSeconds(0.3f);
            while(!IsGrounded())
            {
                yield return null;
            }
            animator.SetBool("is_grounded", true);
            yield return new WaitForSeconds(jumpCooldown);
            inJump = false;
            yield break;
        }

        bool IsShifting(Direction direction)
        {
            if(direction == Direction.Left)
            {
                return playerInput.Endless.ShiftLeft.WasPressedThisFrame();
            }
            if(direction == Direction.Right)
            {
                return playerInput.Endless.ShiftRight.WasPressedThisFrame();
            }
            return false;
        }

        public void Shift(Direction direction)
        {
            if(!inShift && !inJump)
            {
                if(currentTSection != null)
                {
                    currentTSection.Shift(direction, this, turned);
                    turned = true;
                }
                else
                {
                    currentSection.Shift(direction, this);
                }
            }
        }
        
        public void ShiftToPosition(Transform pos, ShiftDistanceType type = ShiftDistanceType.x)
        {
            shift = _Shift(pos, type);
            StartCoroutine(shift);
        }

        IEnumerator _Shift(Transform target, ShiftDistanceType type = ShiftDistanceType.x)
        {
            inShift = true;
            float _shiftDistance = GetShiftDistance(target, type);
            Vector3 _move = Vector3.zero;
            float _distance = Mathf.Lerp(0, _shiftDistance, 0.04f);
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
            while(Mathf.Abs(_shiftDistance) > absDistance)
            {
                _shiftDistance = GetShiftDistance(target, type);
                _move.x = _distance;
                trans.Translate(_move);
                yield return null;
            }
            _shiftDistance = GetShiftDistance(target, type);
            trans.Translate(_shiftDistance, 0, 0);
            yield return new WaitForSeconds(0.1f); // cooldown for shift
            inShift = false;
            yield break;
        }

        float GetShiftDistance(Transform target, ShiftDistanceType type = ShiftDistanceType.x)
        {
            return type == ShiftDistanceType.x ? target.InverseTransformPoint(trans.position).x : target.InverseTransformPoint(trans.position).z;
        }

        public void Attack()
        {
            animator.SetTrigger("attack");
        }

        public void Die()
        {
			state = 2;
            StopAllCoroutines();
		}

        void Reset()
        {
            turned = false;
            state = 1;
            inJump = false;
            inShift = false;
        }
    }

    public enum ShiftDistanceType
    {
        x,
        z
    }

    public enum Direction 
    {
        Left,
        Right,
        Up,
        Down
    }
}
