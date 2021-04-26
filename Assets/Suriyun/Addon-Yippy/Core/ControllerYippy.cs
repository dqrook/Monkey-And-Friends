using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Suriyun
{
	public class ControllerYippy : Controller
	{
		protected int state = 0;

		public RuntimeAnimatorController animator_controller;

		protected override void Awake ()
		{
			base.Awake ();

			if (animator == null) {
				animator = gameObject.GetComponentInChildren<Animator> ();
			}

			if (animator_controller == null) {
				Debug.LogError ("Missing : animator_controller.");
			}

			animator.runtimeAnimatorController = animator_controller;
		}

		protected override void Update ()
		{
			base.Update ();

			if (input.magnitude > 0.01) {
				time_to_idle = alert_time;
			}

			if (input.z >= 0) {
				move = input.z * trans.forward * forward_speed;
				turn = input.x * turn_speed * Time.deltaTime;
			} else {
				move = input.z * trans.forward * backward_speed;
				turn = -input.x * turn_speed * Time.deltaTime;
			}

			if (input.z > 0.01f || input.z < -0.01f) {
				if (state == 1) {
					state = 0;
				}
			}

			animator.SetFloat ("speed_z", input.z);
			animator.SetFloat ("speed_x", input.x);
			animator.SetBool ("is_grounded", ctrl.isGrounded);
			animator.SetInteger ("state",state);

			if (Input.GetKeyDown(key_attack) 
                && !EventSystem.current.IsPointerOverGameObject ()) {
				switch (attack_type) {
				case 0:
					animator.SetTrigger ("attack"); 
					break;
				case 1: 
					animator.SetTrigger ("attack_range");
					break;
				}
			}

			time_to_idle -= Time.deltaTime;
			animator.SetFloat ("time_to_idle", time_to_idle);

			if (Input.GetKeyDown (key_jump) && ctrl.isGrounded) {
				animator.SetTrigger ("jump");
				AddImpact (Vector3.up, jump_power);
			}

			is_blocking = Input.GetKey (key_block);
			if (is_blocking) {
				move *= blocking_speed_multiplier;
			}

			animator.SetBool ("block", is_blocking);

			if (state != 0) {
				input = Vector3.zero;
				move.x = 0;
				move.z = 0;
				turn = 0;
			}

			UpdateImpact ();
			UpdateMove ();

		}

		public virtual void ClearState(){
			state = 0;
		}

		public virtual void Sit(){
			state = 1;
		}
			
		public virtual void Die(){
			state = 2;
		}

		public virtual void Cheer(){
			state = 3;
		}

		public virtual void Hit(){
			animator.SetTrigger ("hit");
		}
			
	}
}
