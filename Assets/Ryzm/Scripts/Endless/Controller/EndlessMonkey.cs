﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.Monkey;
using CodeControl;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public class EndlessMonkey : EndlessController
    {
        public float jumpCooldown = 0.1f;
        public Transform rootTransform;
        public float distanceToGround = 0.5f;
        public float distanceToBarriers = 2;
		public LayerMask groundLayer;
        public LayerMask barrierLayer;
		public float jumpPower = 5;

        [Header("Monkey Emotion")]
		public MonkeyEmotions emotions;	
		public SkinnedMeshRenderer eyes;
		public SkinnedMeshRenderer mouth;
		public SkinnedMeshRenderer eyebrows;
		public SkinnedMeshRenderer blush;

		MonkeyEmotion currentEmotion = MonkeyEmotion.Happy;
        RaycastHit hit;
        Ray checkGround;
        Ray checkBarriers;
        Rigidbody rb;
        IEnumerator monitorJump;
        bool airAttacking;
        IEnumerator _airAttack;

        protected override void Awake()
        {
            base.Awake();
			if(emotions == null)
			{
				emotions = gameObject.GetComponent<MonkeyEmotions>();
			}
			ChangeEmotion(MonkeyEmotion.Happy);
            rb = GetComponent<Rigidbody>();
        }

        void Update()
        {
            animator.SetInteger("state", state);
            if(gameStatus != GameStatus.Active)
            {
                return;
            }
            float speedZ = state == 0 ? 1 : 0;
            float zMove = Time.deltaTime * forwardSpeed * speedZ;
            move.z = zMove;
            move.y = 0;
            move.x = shiftSpeed * zMove * 0.75f;
            trans.Translate(move);
            distanceTraveled += zMove;
            Message.Send(new RunnerDistanceResponse(distanceTraveled));
            
            bool isGrounded = IsGrounded();
            animator.SetFloat("speed_z", speedZ);
			animator.SetFloat("speed_x", 0);
			animator.SetBool("is_grounded", isGrounded);
            
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
        
        void ChangeEmotion(MonkeyEmotion emotion)
		{
			if(emotions == null)
			{
				return;
			}
			
			MonkeyEmotionPrefab emotionPrefab = emotions.GetEmotion(emotion);
			if(emotionPrefab == null)
			{
				return;
			}
			
			Material _eyes = emotionPrefab.eyes;
			if(_eyes != null)
			{
				eyes.material = _eyes;
			}

			Material _mouth = emotionPrefab.mouth;
			if(_mouth != null)
			{
				mouth.material = _mouth;
			}
			
			Material _eyebrows = emotionPrefab.eyebrows;
			if(_eyebrows != null)
			{
				eyebrows.material = _eyebrows;
			}
			eyebrows.gameObject.SetActive(_eyebrows != null);

			Material _blush = emotionPrefab.blush;
			if(_blush != null)
			{
				blush.material = _blush;
			}
			blush.gameObject.SetActive(_blush != null);
			
			currentEmotion = emotion;
		}

        bool IsJumping()
        {
			return playerInput.PlayerMain.Jump.WasPressedThisFrame();
        }

        bool IsGrounded()
        {
            checkGround = new Ray(rootTransform.position, Vector3.down);
            bool grounded = Physics.Raycast(checkGround, out hit, distanceToGround, groundLayer);
            return grounded;
        }

        bool HasBarrier(Direction direction)
        {
            int sign = direction == Direction.Right ? 1 : -1;
            Vector3 vec = trans.right * sign;

            checkBarriers = new Ray(rootTransform.position, vec);
            bool hasBarrier = Physics.Raycast(checkBarriers, out hit, distanceToBarriers, barrierLayer);

            // Vector3 vector45 = Quaternion.Euler(0, -45 * sign, 0) * vec;
            // if(!hasBarrier)
            // {
            //     checkBarriers = new Ray(rootTransform.position, vector45);
            //     hasBarrier = Physics.Raycast(checkBarriers, out hit, distanceToBarriers * 1.4f, barrierLayer);
            // }

            Vector3 vector30 = Quaternion.Euler(0, -30 * sign, 0) * vec;
            if(!hasBarrier)
            {
                checkBarriers = new Ray(rootTransform.position, vector30);
                hasBarrier = Physics.Raycast(checkBarriers, out hit, distanceToBarriers * 1.15f, barrierLayer);
            }

            Vector3 vector15 = Quaternion.Euler(0, -15 * sign, 0) * vec;
            if(!hasBarrier)
            {
                checkBarriers = new Ray(rootTransform.position, vector15);
                hasBarrier = Physics.Raycast(checkBarriers, out hit, distanceToBarriers, barrierLayer);
            }

            // Debug.DrawRay(rootTransform.position, vector15 * distanceToBarriers, Color.red);
            // Debug.DrawRay(rootTransform.position, vector30 * distanceToBarriers * 1.15f, Color.red);
            // Debug.DrawRay(rootTransform.position, vector45 * distanceToBarriers * 1.4f, Color.red);
            // Debug.DrawRay(rootTransform.position, vec * distanceToBarriers, Color.red);
            return hasBarrier;
        }

        public override void Jump()
        {
            bool isGrounded = IsGrounded();
            Jump(isGrounded);
        }

        public override void Slide() {}

        public void Jump(bool isGrounded)
        {
            if(!InJump && isGrounded)
            {
                monitorJump = MonitorJump();
                StartCoroutine(monitorJump);
                animator.SetTrigger("jump");
                rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            }
        }

        IEnumerator MonitorJump()
        {
            InJump = true;
            yield return new WaitForSeconds(0.3f);
            while(!IsGrounded())
            {
                yield return null;
            }
            animator.SetBool("is_grounded", true);
            yield return new WaitForSeconds(jumpCooldown);
            InJump = false;
            yield break;
        }

        public void AirAttack()
        {
            if(airAttacking || !IsGrounded())
            {
                return;
            }
            _airAttack = _AirAttack();
            StartCoroutine(_airAttack);
        }

        IEnumerator _AirAttack()
        {
            ChangeEmotion(MonkeyEmotion.Focused);
            airAttacking = true;
            animator.SetTrigger("jump");
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            float _time = 0f;
            float maxT = 0.45f;
            float maxSpeed = 0.2f;
            float targetSpeed = 0.1f;
            while(_time < maxT)
            {
                _time += Time.deltaTime;
                float ratio = _time / maxT * 1.1f;
                ratio = ratio < 1 ? ratio : 1;
                float speed = Mathf.Lerp(maxSpeed, targetSpeed, ratio); 
                Message.Send(new RequestGameSpeedChange(speed));
                yield return null;
            }
            // Message.Send(new ChangeGameSpeed(targetSpeed, 0.4f));
            Message.Send(new RequestGameSpeedChange(targetSpeed));
            animator.SetTrigger("airAttack");
            ChangeEmotion(MonkeyEmotion.Angry);
            yield return new WaitForSeconds(0.4f);
            Message.Send(new RequestGameSpeedChange(0.2f, 0.2f));
            _time = 0;
            float timeGrounded = 0;
            float cooldownTime = 0.15f;
            while(timeGrounded < cooldownTime)
            {
                if(IsGrounded())
                {
                    timeGrounded += Time.deltaTime;
                }
                else
                {
                    timeGrounded = 0f;
                }
                _time += Time.deltaTime; 
                yield return null;
            }
            ChangeEmotion(MonkeyEmotion.Focused);
            airAttacking = false;
            yield return new WaitForSeconds(0.5f);
            ChangeEmotion(MonkeyEmotion.Happy);
            yield break;
        }

        public override void Attack()
        {
            // animator.SetTrigger("attack");
            AirAttack();
        }

        public void RideDragon()
        {
            state = 4;
        }
    }
}
