using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ryzm.EndlessRunner
{
    public class RunnerController : BaseController
    {
        public int currentPosition = 1;
        public RuntimeAnimatorController animatorController;
        public float shiftDistance = 0.1f;
        public static GameObject player;
        bool canTurn = false;
        static GameObject _currentPlatform;
        static EndlessSection currentSection;

        public static GameObject CurrentPlatform
        {
            get
            {
                return _currentPlatform;
            }
            set
            {
                _currentPlatform = value;
                currentSection = value.GetComponent<EndlessSection>();
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
            // ctrl.attachedRigidbody.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;
		}

        void Start()
        {
            GenerateWorld.RunDummy();
        }

        void OnTriggerEnter(Collider other)
        {
            if(other is BoxCollider && GenerateWorld.lastPlatform.tag != "platformTSection")
            {
                Debug.Log("entered lol");
                GenerateWorld.RunDummy();
            }
            if(other is SphereCollider)
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

        protected override void Update()
        {
            base.Update();

            move = Vector3.zero;
			animator.SetFloat("speed_z", input.z);
			animator.SetBool("is_grounded", ctrl.isGrounded);
			// animator.SetFloat("time_to_idle", 0);
            bool isGrounded = ctrl.isGrounded || !ctrl.enabled;

			if (IsJumping() && isGrounded)
            {
				animator.SetTrigger("jump");
				AddImpact(Vector3.up, jumpPower);
			}

			UpdateImpact();
            if(ctrl.enabled)
            {
			    UpdateMove();
            }
        }

        protected override void GetMovement()
        {
            input.x = 0;
            input.z = 1;
        }

        public void Shift(Direction direction)
        {
            currentSection.Shift(direction, this);
        }

        public void Spin(Direction direction)
        {
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
