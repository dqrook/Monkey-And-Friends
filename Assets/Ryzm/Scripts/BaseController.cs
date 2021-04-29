using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm
{
    public class BaseController : MonoBehaviour
    {
		public float mass = 1f;
		public float forwardSpeed = 1.66f;
		public float backwardSpeed = 0.66f;
		public float blockingSpeedMultiplier = 0.5f;
		public float turn_speed = 266.66f;
		public float jumpPower = 6.66f;
		public float gravity_multiplier = 1.66f;
		public float alertTime = 1f;

		public Animator animator;

		protected Player playerInput;

		protected float timeToIdle = 0f;
		protected bool isBlocking = true;
		protected int attackType = 0;
		protected Vector3 impact;

		protected Vector3 input;
		protected Vector3 move;
		protected float turn;

		protected CharacterController ctrl;
		protected Transform trans;
		Vector2 _temp = new Vector2();

		protected virtual void Awake()
        {
			trans = GetComponent<Transform> ();
			ctrl = GetComponent<CharacterController> ();
			playerInput = new Player();
		}
		
		protected virtual void OnEnable()
		{
			playerInput.Enable();
		}

		protected virtual void OnDisable()
		{
			playerInput.Disable();
		}

        protected virtual void GetMovement()
        {
			// Debug.Log(playerInput.Touch.Primary.ReadValue<float>());
			_temp = playerInput.PlayerMain.Move.ReadValue<Vector2>();
			input.x = _temp.x;
			input.z = _temp.y;
			// Debug.Log(input);
            // # if UNITY_STANDALONE || UNITY_EDITOR
            // input.x = Input.GetAxis (axisX);
			// input.z = Input.GetAxis (axisZ);

            // # elif UNITY_IOS || UNITY_ANDROID

            // # endif
        }

        protected virtual bool IsJumping()
        {
			return playerInput.PlayerMain.Jump.WasPressedThisFrame();
			// return playerInput.PlayerMain.Jump.ReadValue<float>() > 0;
            // # if UNITY_STANDALONE || UNITY_EDITOR
            // return Input.GetKey(keyJump);

            // # elif UNITY_IOS || UNITY_ANDROID
            // return false;

            // # else
            // return false;

            // # endif
        }

        protected virtual bool IsBlocking()
        {
			return false;
            // # if UNITY_STANDALONE || UNITY_EDITOR
            // return Input.GetKey(keyBlock);

            // # elif UNITY_IOS || UNITY_ANDROID
            // return false;

            // # else
            // return false;

            // # endif
        }

        protected virtual bool IsAttacking()
        {
			return playerInput.PlayerMain.Attack.WasPressedThisFrame();
			// return playerInput.PlayerMain.Attack.ReadValue<float>() > 0;
            // # if UNITY_STANDALONE || UNITY_EDITOR
            // return Input.GetKeyDown(keyAttack);
            
            // # elif UNITY_IOS || UNITY_ANDROID
            // return false;

            // # else
            // return false;

            // # endif
        }

		protected virtual void Update()
        {
            GetMovement();
		}

		protected virtual void UpdateImpact()
        {
			if (!ctrl.isGrounded)
            {
				impact += Physics.gravity * gravity_multiplier * Time.deltaTime;
			}

			impact = Vector3.Lerp(impact, Vector3.zero, Time.deltaTime);
			if (impact.magnitude > 0.2f)
            {
				move += impact;
			}
		}

		protected virtual void UpdateMove()
        {
			trans.Rotate (Vector3.up,turn);
			ctrl.Move (move * Time.deltaTime);
		}

		protected virtual void AddImpact(Vector3 dir,float force)
        {
			dir.Normalize ();
			if (dir.y < 0)
            {
				dir.y = -dir.y;
			}
			impact = dir.normalized * force / mass;
		}

		public virtual void SwitchAttackType()
        {
			if (attackType == 0) {
				attackType = 1;
			} else {
				attackType = 0;
			}		
		}

		public virtual string GetAttackType(string pre_text)
        {
			string tmp = pre_text;

			switch (attackType) {
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
