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

        void OnCollisionEnter(Collision other)
        {
            // Debug.Log(LayerMask.LayerToName(other.gameObject.layer) + " " + groundLayer);
            if(other.gameObject.tag == "Fire")
            {
                isDead = true;
                Die();
            }
            // else
            // {
            //     turned = false;
            //     GameManager.Instance.CurrentPlatform = other.gameObject;
            // }
        }

        void OnTriggerEnter(Collider other)
        {
            // if(other is BoxCollider && GenerateWorld.lastSpawnedPlatform.tag != "platformTSection")
            // {
            //     GenerateWorld.RunDummy();
            // }
            // if(other is SphereCollider)
            // {
            //     Debug.Log(other.name);
            // }
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
            Message.AddListener<CurrentPlatformChange>(OnCurrentPlatformChange);
		}

        void OnDestory()
        {
            Message.RemoveListener<CurrentPlatformChange>(OnCurrentPlatformChange);
        }

        void OnCurrentPlatformChange(CurrentPlatformChange change)
        {
            turned = false;
        }

        void Start()
        {
            GenerateWorld.RunDummy();
            // GenerateWorld.RunDummy();
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
                if(GameManager.Instance.CurrentTSection != null)
                {
                    GameManager.Instance.CurrentTSection.Shift(direction, this, turned);
                    turned = true;
                }
                else
                {
                    GameManager.Instance.CurrentSection.Shift(direction, this);
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

        IEnumerator _airAttack;
        public void AirAttack()
        {
            _airAttack = _AirAttack();
            StartCoroutine(_airAttack);
        }

        IEnumerator _AirAttack()
        {
            animator.SetTrigger("jump");
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            yield return new WaitForSeconds(0.4f);
            animator.SetTrigger("attack");
            yield break;
        }

        public void Attack()
        {
            animator.SetTrigger("attack");
            // AirAttack();
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
