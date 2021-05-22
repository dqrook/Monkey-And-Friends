using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;
using Ryzm.Monkey;

namespace Ryzm.EndlessRunner
{
    public class RunnerController : MonoBehaviour
    {
        public float jumpCooldown = 0.2f;
        public Transform rootTransform;
        public float distanceToGround = 0.7f;
        public float distanceToBarriers = 2f;
		public LayerMask groundLayer;
        public LayerMask barrierLayer;
        public RuntimeAnimatorController animatorController;
        public float forwardSpeed = 1.66f;
		public float jumpPower = 6.66f;
		
		public Animator animator;

		[Header("Monkey Emotion")]
		public MonkeyEmotions emotions;	
		public SkinnedMeshRenderer eyes;
		public SkinnedMeshRenderer mouth;
		public SkinnedMeshRenderer eyebrows;
		public SkinnedMeshRenderer blush;

		MonkeyEmotion currentEmotion = MonkeyEmotion.Happy;
		Player playerInput;
		Vector3 move;
		Transform trans;
        int _currentPosition = 1;
        // turned is set to true when on a TSection and the user has decided which direction they would like to go
        bool turned = false;
        RaycastHit hit;
        Ray checkGround;
        Ray checkBarriers;
        int state = 0;
        Rigidbody rb;
        IEnumerator monitorJump;
        bool inShift;
        IEnumerator shift;
        bool inJump;
        EndlessSection _endlessSection;
        EndlessTSection _endlessTSection;
        EndlessTurnSection _endlessTurnSection;
        GameStatus gameStatus;
        float distanceTraveled;
        float shiftSpeed;
        bool airAttacking;
        IEnumerator _airAttack;

        public int CurrentPosition
        {
            get
            {
                return _currentPosition;
            }
            set
            {
                _currentPosition = value;
                Message.Send(new CurrentPositionResponse(value));
            }
        }

		void Awake()
		{
            trans = GetComponent<Transform> ();
			playerInput = new Player();
			if(emotions == null)
			{
				emotions = gameObject.GetComponent<MonkeyEmotions>();
			}
			ChangeEmotion(MonkeyEmotion.Happy);

			if (animator == null)
            {
				animator = gameObject.GetComponentInChildren<Animator> ();
			}

			if (animatorController == null)
            {
				Debug.LogError ("Missing : animatorController.");
			}

			animator.runtimeAnimatorController = animatorController;
            rb = GetComponent<Rigidbody>();
            Message.AddListener<CurrentSectionChange>(OnCurrentSectionChange);
            Message.AddListener<RunnerDie>(OnRunnerDie);
            Message.AddListener<CurrentPositionRequest>(OnCurrentPositionRequest);
            Message.AddListener<GameStatusResponse>(OnGameStatusResponse);
		}

        void OnEnable()
        {
            playerInput.Enable();
            // Message.Send(new GameStatusRequest());
        }

        void OnDisable()
		{
			playerInput.Disable();
		}

        void OnDestroy()
        {
            Message.RemoveListener<CurrentSectionChange>(OnCurrentSectionChange);
            Message.RemoveListener<RunnerDie>(OnRunnerDie);
            Message.RemoveListener<CurrentPositionRequest>(OnCurrentPositionRequest);
            Message.RemoveListener<GameStatusResponse>(OnGameStatusResponse);
        }

        void OnCurrentSectionChange(CurrentSectionChange change)
        {
            turned = false;
            _endlessSection = change.endlessSection;
            _endlessTurnSection = change.endlessTurnSection;
            _endlessTSection = change.endlessTSection;
        }

        void OnRunnerDie(RunnerDie runnerDie)
        {
            Die();
        }

        void OnCurrentPositionRequest(CurrentPositionRequest request)
        {
            Message.Send(new CurrentPositionResponse(CurrentPosition));
        }

        void OnGameStatusResponse(GameStatusResponse response)
        {
            gameStatus = response.status;
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

        public void Jump()
        {
            bool isGrounded = IsGrounded();
            Jump(isGrounded);
        }

        public void Slide() {}

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

        // void FixedUpdate()
        // {
        //     if(gameStatus != GameStatus.Active)
        //     {
        //         return;
        //     }
        //     // rb.MovePosition(trans.position + trans.forward * forwardSpeed * Time.deltaTime);
        //     move = trans.forward * forwardSpeed;
        //     if(shiftSpeed != 0 && inShift)
        //     {
        //         move += trans.right * shiftSpeed * forwardSpeed * 0.75f ;
        //     }
        //     move *= Time.fixedDeltaTime;
        //     move += trans.position;
        //     UpdateMove(move);
        //     // rb.AddForce(move, ForceMode.VelocityChange);
        //     // Debug.Log(rb.velocity);
        // }
        
        void Update()
        {
            animator.SetInteger("state", state);
            if(gameStatus != GameStatus.Active)
            {
                return;
            }
            // move = trans.forward * forwardSpeed;
            // ctrl.Move(move * Time.deltaTime);
            // trans.position += trans.forward * GameManager.Instance.speed * 0.4f;
            // trans.Translate(0, 0, 0.4f * GameManager.Instance.speed);
            // if(!IsGrounded())
            // {
            //     jumpVelocity -= Time.deltaTime * Physics.gravity.y;
            // }
            // else
            // {
            //     jumpVelocity = 0;
            // }
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
            // move = new Vector3(0, 0, forwardSpeed);
            // UpdateImpact();
            // UpdateMove();
        }

        bool IsJumping()
        {
			return playerInput.PlayerMain.Jump.WasPressedThisFrame();
        }

        bool IsAttacking()
        {
			return playerInput.PlayerMain.Attack.WasPressedThisFrame();
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

        // protected override void UpdateMove(Vector3 move)
        // {
        //     // trans.Translate(Time.deltaTime * move);
        //     rb.MovePosition(move);
        // }
        
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
                // if(!HasBarrier(direction))
                // {
                // }
                if(_endlessTurnSection != null)
                {
                    // _endlessTurnSection.Shift(direction, this, turned);
                    turned = true;
                }
                else if(_endlessSection != null)
                {
                    // _endlessSection.Shift(direction, this);
                }
            }
        }
        
        public void ShiftToPosition(Transform pos, ShiftDistanceType type)
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
            shiftSpeed = signDistance;
            while(Mathf.Abs(_shiftDistance) > absDistance)
            {
                // add = true;
                _shiftDistance = GetShiftDistance(target, type);
                _move.x = _distance;
                // _move = transform.position + trans.right * forwardSpeed * Time.deltaTime;
                // UpdateMove(_move);
                // Debug.Log(_move + " " + trans.right);
                // trans.Translate(_move);
                yield return null;
            }
            _shiftDistance = GetShiftDistance(target, type);
            trans.Translate(_shiftDistance, 0, 0);
            // add = false;
            shiftSpeed = 0;
            yield return new WaitForSeconds(0.1f); // cooldown for shift
            inShift = false;
            yield break;
        }

        float GetShiftDistance(Transform target, ShiftDistanceType type = ShiftDistanceType.x)
        {
            return type == ShiftDistanceType.x ? target.InverseTransformPoint(trans.position).x : target.InverseTransformPoint(trans.position).z;
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

        public void Attack()
        {
            // animator.SetTrigger("attack");
            AirAttack();
        }

        public void Die()
        {
			state = 2;
            StopAllCoroutines();
		}

        void Reset()
        {
            turned = false;
            state = 0;
            inJump = false;
            inShift = false;
            distanceTraveled = 0;
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
