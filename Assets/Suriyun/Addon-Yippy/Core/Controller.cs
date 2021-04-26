using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Suriyun
{
	public class Controller : MonoBehaviour
	{

		public string axis_x = "Horizontal";
		public string axis_z = "Vertical";

		public KeyCode key_jump = KeyCode.Space;
		public KeyCode key_attack = KeyCode.Mouse0;
		public KeyCode key_block = KeyCode.LeftControl;

		public float mass = 1f;
		public float forward_speed = 3.33f;
		public float backward_speed = 1.33f;
		public float blocking_speed_multiplier = 0.5f;
		public float turn_speed = 66.66f;
		public float jump_power = 1f;
		public float gravity_multiplier = 2f;
		public float alert_time = 2f;

		public Animator animator;

		protected float time_to_idle = 0f;
		protected bool is_blocking = true;
		protected int attack_type = 0;
		protected Vector3 impact;

		protected Vector3 input;
		protected Vector3 move;
		protected float turn;

		protected CharacterController ctrl;
		protected Transform trans;

		protected virtual void Awake(){
			trans = GetComponent<Transform> ();
			ctrl = GetComponent<CharacterController> ();
		}

		protected virtual void Update(){
			// TODO : Can change input method here //
			input.x = Input.GetAxis (axis_x);
			input.z = Input.GetAxis (axis_z);
		}

		protected virtual void UpdateImpact(){

			if (!ctrl.isGrounded) {
				impact += Physics.gravity * gravity_multiplier * Time.deltaTime;
			}

			impact = Vector3.Lerp (impact, Vector3.zero, Time.deltaTime);
			if (impact.magnitude > 0.2f) {
				move += impact;
			}
		}

		protected virtual void UpdateMove(){
			trans.Rotate (Vector3.up,turn);
			ctrl.Move (move * Time.deltaTime);
		}

		protected virtual void AddImpact(Vector3 dir,float force){
			dir.Normalize ();
			if (dir.y < 0) {
				dir.y = -dir.y;
			}
			impact = dir.normalized * force / mass;
		}

		public virtual void SwitchAttackType(){
			if (attack_type == 0) {
				attack_type = 1;
			} else {
				attack_type = 0;
			}		
		}

		public virtual string GetAttackType(string pre_text){
			string tmp = pre_text;

			switch (attack_type) {
			case 0:
				tmp += "Melee";
				break;
			case 1:
				tmp += "Range";
				break;
			}

			return tmp;
		}
	}
}
