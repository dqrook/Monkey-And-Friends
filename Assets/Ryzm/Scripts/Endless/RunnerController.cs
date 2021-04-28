using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Ryzm.EndlessRunner
{
    public class RunnerController : BaseController
    {
        public RuntimeAnimatorController animatorController;
        public float shiftDistance = 0.1f;

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
		}

        void Start()
        {
            // playerInput.Touch.PrimaryContact.started += ctx => OnStartTouch(ctx);
            // playerInput.Touch.PrimaryContact.canceled += ctx => OnStartTouch(ctx);
        }

        void OnStartTouch(InputAction.CallbackContext context)
        {
            Debug.Log("onstarttouch");
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();

            move = Vector3.zero;
			animator.SetFloat ("speed_z", input.z);
			animator.SetBool ("is_grounded", ctrl.isGrounded);
			animator.SetFloat ("time_to_idle", 0);
            
			if (IsJumping() && ctrl.isGrounded)
            {
				animator.SetTrigger("jump");
				AddImpact(Vector3.up, jumpPower);
			}

			UpdateImpact();
			UpdateMove();
        }

        public void Shift(Direction direction)
        {
            if(direction == Direction.Left)
            {
                trans.Translate(-shiftDistance, 0, 0);
            }
            else if(direction == Direction.Right)
            {
                trans.Translate(shiftDistance, 0, 0);
            }
        }

        public void Spin(Direction direction)
        {
            if(direction == Direction.Left)
            {
                trans.Rotate(Vector3.up * -90);
            }
            else if(direction == Direction.Right)
            {
                trans.Rotate(Vector3.up * 90);
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
