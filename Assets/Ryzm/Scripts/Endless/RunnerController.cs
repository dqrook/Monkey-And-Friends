using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

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
        // turned is set to true when on a TSection and the user has decided which direction they would like to go
        bool turned = false;
        RaycastHit hit;
        Ray checkGround;
        int state = 0;
        Rigidbody rb;
        IEnumerator monitorJump;
        bool inShift;
        IEnumerator shift;
        bool inJump;
        EndlessSection _endlessSection;
        EndlessTSection _endlessTSection;

        // void OnCollisionEnter(Collision other)
        // {
        //     if(other.gameObject.tag == "Fire")
        //     {
        //         Die();
        //     }
        // }

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
            Message.AddListener<CurrentSectionChange>(OnCurrentSectionChange);
            Message.AddListener<RunnerDie>(OnRunnerDie);
		}

        void OnDestory()
        {
            Message.RemoveListener<CurrentSectionChange>(OnCurrentSectionChange);
            Message.RemoveListener<RunnerDie>(OnRunnerDie);
        }

        void OnCurrentSectionChange(CurrentSectionChange change)
        {
            turned = false;
            _endlessSection = change.endlessSection;
            _endlessTSection = change.endlessTSection;
        }

        void OnRunnerDie(RunnerDie runnerDie)
        {
            Die();
        }
        
        bool IsGrounded()
        {
            checkGround = new Ray (rootTransform.position, Vector3.down);
            bool grounded = Physics.Raycast(checkGround, out hit, distanceToGround, groundLayer);
            return grounded;
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
            // input.z = 1;
            // move = Vector3.zero;
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
                if(_endlessTSection != null)
                {
                    _endlessTSection.Shift(direction, this, turned);
                    turned = true;
                }
                else
                {
                    _endlessSection.Shift(direction, this);
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

        bool airAttacking;
        IEnumerator _airAttack;
        public void AirAttack()
        {
            if(airAttacking)
            {
                return;
            }
            _airAttack = _AirAttack();
            StartCoroutine(_airAttack);
        }

        IEnumerator _AirAttack()
        {
            airAttacking = true;
            animator.SetTrigger("jump");
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            float _time = 0f;
            float maxT = 0.4f;
            float maxSpeed = 0.2f;
            float targetSpeed = 0.1f;
            while(_time < maxT)
            {
                _time += Time.deltaTime;
                float ratio = _time / maxT * 1.1f;
                ratio = ratio < 1 ? ratio : 1;
                float speed = Mathf.Lerp(maxSpeed, targetSpeed, ratio); 
                Message.Send(new ChangeGameSpeed(speed));
                yield return null;
            }
            // Message.Send(new ChangeGameSpeed(targetSpeed, 0.4f));
            Message.Send(new ChangeGameSpeed(targetSpeed));
            animator.SetTrigger("airAttack");
            yield return new WaitForSeconds(0.4f);
            Message.Send(new ChangeGameSpeed(0.2f, 0.2f));
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
            Debug.Log("time not grounded: " + _time);
            // yield return new WaitForSeconds(0.2f); // cooldown
            airAttacking = false;
            yield break;
        }

        public void Attack()
        {
            // animator.SetTrigger("attack");
            AirAttack();
        }

        public void Die()
        {
            isDead = true;
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
