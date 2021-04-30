using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ryzm.EndlessRunner
{
    public class RunnerController : BaseController
    {
        public Transform rootTransform;
        public float distanceToGround = 0.7f;
		public LayerMask groundLayer;
        public int currentPosition = 1;
        public RuntimeAnimatorController animatorController;
        public float shiftDistance = 0.1f;
        public static GameObject player;
        bool canTurn = false;
        static GameObject _currentPlatform;
        static EndlessSection currentSection;
        static EndlessTSection currentTSection;
        // turned is set to true when on a TSection and the user has decided which direction they would like to go
        static bool turned = false;
        Vector3 _raycastPos;
        RaycastHit hit;
        Ray checkGround;

        public static GameObject CurrentPlatform
        {
            get
            {
                return _currentPlatform;
            }
            set
            {
                _currentPlatform = value;
                currentTSection = value.GetComponent<EndlessTSection>();
                currentSection = value.GetComponent<EndlessSection>();
                turned = false;
            }
        }

        void OnCollisionEnter(Collision other)
        {
            CurrentPlatform = other.gameObject;
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            CurrentPlatform = hit.gameObject;
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
            // means we are in the T section and we can turn
            if(other is SphereCollider && other.gameObject.tag == "platformTSection")
            {
                canTurn = true;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if(other is SphereCollider)
            {
                canTurn = false;
            }
        }

        protected override void UpdateImpact()
        {
			if (!IsGrounded())
            {
				impact += Physics.gravity * gravity_multiplier * Time.deltaTime;
			}

			impact = Vector3.Lerp(impact, Vector3.zero, Time.deltaTime);
			if (impact.magnitude > 0.2f)
            {
				move += impact;
			}
		}

        protected override void UpdateMove()
        {
            trans.Translate(move * Time.deltaTime);
        }

        protected override void Update()
        {
            base.Update();
            
            move = Vector3.zero;
			animator.SetFloat("speed_z", input.z);
			animator.SetBool("is_grounded", ctrl.isGrounded);
			// animator.SetFloat("time_to_idle", 0);

			if (IsJumping() && IsGrounded())
            {
                animator.SetTrigger("jump");
                AddImpact(Vector3.up, jumpPower);
			}

			// UpdateImpact();
			// UpdateMove();
        }

        protected override void GetMovement()
        {
            input.x = 0;
            input.z = 1;
        }

        public void Shift(Direction direction)
        {
            if(IsGrounded())
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

        public void Spin(Direction direction)
        {
            return;
            if(canTurn)
            {
                if(direction == Direction.Left)
                {
                    trans.Rotate(Vector3.up * -90);
                    GenerateWorld.dummyTransform.forward = -trans.forward;
                    GenerateWorld.RunDummy();
                }
                else if(direction == Direction.Right)
                {
                    trans.Rotate(Vector3.up * 90);
                    GenerateWorld.dummyTransform.forward = -trans.forward;
                    GenerateWorld.RunDummy();
                }
            }
        }

        public void Attack()
        {
            animator.SetTrigger("attack");
        }
    }
    public enum Direction 
    {
        Left,
        Right,
        Up,
        Down
    }
}
