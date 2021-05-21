using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.Monkey
{
    public class MonkeyController : BaseController
    {
        protected int state = 0;

		public RuntimeAnimatorController animatorController;

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

		protected override void Update ()
		{
			base.Update();

			if (input.magnitude > 0.01)
            {
				timeToIdle = alertTime;
			}

			if (input.z >= 0)
            {
				move = input.z * trans.forward * forwardSpeed;
				turn = input.x * turn_speed * Time.deltaTime;
			} else
            {
				move = input.z * trans.forward * backwardSpeed;
				turn = -input.x * turn_speed * Time.deltaTime;
			}

			if (input.z > 0.01f || input.z < -0.01f)
            {
				if (state == 1)
                {
					state = 0;
				}
			}

			animator.SetFloat("speed_z", input.z);
			animator.SetFloat("speed_x", input.x);
			animator.SetBool("is_grounded", ctrl.isGrounded);
			animator.SetInteger("state", state);

			if (IsAttacking()) 
            {
				switch (attackType)
                {
                    case 0:
                        animator.SetTrigger ("attack"); 
                        break;
                    case 1: 
                        animator.SetTrigger ("attack_range");
                        break;
				}
			}

			timeToIdle -= Time.deltaTime;
			animator.SetFloat ("time_to_idle", timeToIdle);

			if (IsJumping() && ctrl.isGrounded)
            {
				animator.SetTrigger ("jump");
				AddImpact (Vector3.up, jumpPower);
			}

			isBlocking = IsBlocking();
			if (isBlocking)
            {
				move *= blockingSpeedMultiplier;
			}

			animator.SetBool("block", isBlocking);

			if (state != 0)
            {
				input = Vector3.zero;
				move.x = 0;
				move.z = 0;
				turn = 0;
			}

			UpdateImpact();
			UpdateMove();
		}

		public virtual void ClearState()
        {
			state = 0;
		}

		public virtual void Sit()
        {
			state = 1;
		}
			
		public virtual void Die()
        {
			state = 2;
		}

		public virtual void Cheer()
        {
			state = 3;
		}

		public virtual void Hit()
        {
			animator.SetTrigger ("hit");
		}
    }

}
