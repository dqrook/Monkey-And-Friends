using System.Collections;
using System.Collections.Generic;
using Ryzm.EndlessRunner.Messages;
using UnityEngine;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class EndlessRyz : EndlessController
    {
        #region Public Variables
        public EndlessPlayerController playerController;
        public float jumpCooldown = 0.1f;
        #endregion

        #region Private Variables
        IEnumerator monitorJump;
        WaitForSeconds initJumpWait;
        WaitForSeconds jumpCooldownWait;
        bool _isRunning;
        bool runningInitialized;
        // anim hashes
        int jumpAnimHash;
        int slideAnimHash;
        int runAnimHash;
        int attackAnimHash;
        ActionState _attackState;
        IEnumerator maintain;
        #endregion

        #region Properties
        bool IsRunning
        {
            get
            {
                return _isRunning;
            }
            set
            {
                if(value != _isRunning || !runningInitialized)
                {
                    _isRunning = value;
                    animator.SetBool(runAnimHash, value);
                    runningInitialized = true;
                }
            }
        }

        ActionState AttackState
        {
            get
            {
                return _attackState;
            }
            set
            {
                _attackState = value;
                SetBodyColliders(_attackState == ActionState.Off);
                SetAttackColliders(_attackState != ActionState.Off);
            }
        }
        #endregion

        #region Event Functions
        protected override void Awake()
        {
            jumpAnimHash = Animator.StringToHash("jump");
            slideAnimHash = Animator.StringToHash("slide");
            runAnimHash = Animator.StringToHash("run");
            attackAnimHash = Animator.StringToHash("attack");
            initJumpWait = new WaitForSeconds(0.3f);
            jumpCooldownWait = new WaitForSeconds(jumpCooldown);
            AttackState = ActionState.Off;
            base.Awake();
        }

        void Update()
        {
            UpdateControllerTransform();
            if(mode == ControllerMode.Monkey && gameStatus == GameStatus.Active)
            {
                IsRunning = true;
                StartMove();
                UpdateCharacterInputs();
            }
        }

        // void LateUpdate()
        // {
        //     playerController.PostInputUpdate(playerController.motor.CharacterUp);
        // }

        protected override void OnTriggerEnter(Collider col)
        {
            if(AttackState != ActionState.Off)
            {
                OnTrigger(col.gameObject);
            }
        }
        #endregion

        #region Listener Functions
        protected override void OnRunnerDistanceRequest(RunnerDistanceRequest request)
        {
            base.OnRunnerDistanceRequest(request);
            if(mode == ControllerMode.Monkey)
            {
                Message.Send(new RunnerDistanceResponse(distanceTraveled));
            }
        }

        protected override void OnGameStatusResponse(GameStatusResponse response)
        {
            base.OnGameStatusResponse(response);
            if(response.status == GameStatus.Exit) 
            {
                IsRunning = false;
            }
        }
        #endregion

        #region Public Functions
        public override void UpInput()
        {
            bool requestJump = playerController.CanJump && !inShift && !InJump;
            UpdateCharacterInputs(requestJump);

            if(requestJump)
            {
                if(monitorJump != null)
                {
                    StopCoroutine(monitorJump);
                    monitorJump = null;
                }
                monitorJump = MonitorJump();
                StartCoroutine(monitorJump);
            }
        }

        public override void DownInput()
        {
            // todo: handle sliding, attacking, down attacking???
            if(!inShift && AttackState == ActionState.Off)
            {
                if(!InJump)
                {
                    animator.SetTrigger(attackAnimHash);
                    AttackState = ActionState.On;
                    SetBodyColliders(true);
                    SetAttackColliders(false);
                    SetBodyTrailEffect(true);
                }
                else
                {
                    // handle it differently (e.g. sky attack or just drop down quickly and do the forward attack)
                }
            }
        }

        public override void StartMove()
        {
            IsRunning = true;
        }

        public void FinishSlide()
        {
            inSlide = false;
        }

        public void FinishAttack()
        {
            AttackState = ActionState.Off;
            SetBodyColliders(true);
            SetAttackColliders(false);
            SetBodyTrailEffect(false);
        }

        public void RideDragon()
        {
            rb.isKinematic = true;
            playerCollider.enabled = false;
            State = 4;
        }

        public void MaintainZeroPosition()
        {
            maintain = _MaintainZeroPosition();
            StartCoroutine(maintain);
        }
        #endregion

        #region Protected Functions
        protected override bool IsGrounded()
        {
            return playerController.IsGrounded;
        }

        protected override void Die()
        {
            State = 2;
            StopAllCoroutines();
            UpdateCharacterInputs();
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        protected override void Reset()
        {
            base.Reset();
            AttackState = ActionState.Off;
            animator.SetBool(jumpAnimHash, false);
            rb.constraints = RigidbodyConstraints.None;
            SetBodyColliders(true);
            SetAttackColliders(false);
        }
        #endregion

        #region Private Functions
        void UpdateCharacterInputs(bool requestJump = false)
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();
            characterInputs.zInput = State == 0 ? 0.8f * zSpeedMultiplier : 0;
            characterInputs.xInput = shiftSpeed * characterInputs.zInput * 0.75f;
            characterInputs.requestJump = requestJump;
            characterInputs.forwardAxis = playerController.motor.CharacterForward;
            playerController.SetInputs(ref characterInputs);
        }
        #endregion

        #region Coroutines
        IEnumerator MonitorJump()
        {
            animator.SetBool(jumpAnimHash, true);
            InJump = true;
            yield return initJumpWait;
            while(!IsGrounded())
            {
                yield return null;
            }
            animator.SetBool(jumpAnimHash, false);
            yield return jumpCooldownWait;
            InJump = false;
        }

        IEnumerator _MaintainZeroPosition()
        {
            while(true)
            {
                trans.localPosition = new Vector3(0, trans.localPosition.y, trans.localPosition.z);
                yield return null;
            }
        }
        #endregion
    }
}
