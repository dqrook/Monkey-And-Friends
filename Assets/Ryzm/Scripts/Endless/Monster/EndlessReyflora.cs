using System.Collections;
using System.Collections.Generic;
using Ryzm.EndlessRunner.Messages;
using UnityEngine;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class EndlessReyflora : EndlessWaitingMonster
    {
        #region Public Variables
        public Transform rootTransform;
        public Transform test;
        #endregion

        #region Private Variables
        IEnumerator attack;
        Vector3 currentPosition;
        Quaternion targetRotation;
        Transform dragonTrans;
        Transform ryzTrans;
        ControllerMode controllerMode;
        ActionState shiftAttackState;
        ActionState headbuttAttackState;
        ActionState tailSlapAttackState;
        bool canKill;
        bool checkedShift;
        Vector3 _distanceVec;
        #endregion

        #region Properties
        Transform CurrentTransform
        {
            get
            {
                return controllerMode == ControllerMode.Monkey ? ryzTrans : dragonTrans;
            }
        }
        #endregion

        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            attackDistance = 15;
            EnableCollider(false);
            Message.AddListener<ControllerModeResponse>(OnControllerModeResponse);
        }
        
        void Start()
        {
            Message.Send(new ControllersRequest());
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Message.AddListener<ControllersResponse>(OnControllersResponse);
            Message.AddListener<ShiftAttackStateResponse>(OnShiftAttackStateResponse);
            Message.AddListener<HeadbuttAttackStateResponse>(OnHeadbuttAttackStateResponse);
            Message.AddListener<TailSlapAttackStateResponse>(OnTailSlapAttackStateResponse);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Message.RemoveListener<ControllersResponse>(OnControllersResponse);
            Message.RemoveListener<ShiftAttackStateResponse>(OnShiftAttackStateResponse);
            Message.RemoveListener<HeadbuttAttackStateResponse>(OnHeadbuttAttackStateResponse);
            Message.RemoveListener<TailSlapAttackStateResponse>(OnTailSlapAttackStateResponse);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Message.RemoveListener<ControllerModeResponse>(OnControllerModeResponse);
        }
        #endregion

        #region Listener Functions
        void OnControllersResponse(ControllersResponse response)
        {
            dragonTrans = response.dragon.transform;
            ryzTrans = response.ryz.transform;
        }

        void OnControllerModeResponse(ControllerModeResponse response)
        {
            controllerMode = response.mode;
        }

        void OnShiftAttackStateResponse(ShiftAttackStateResponse response)
        {
            shiftAttackState = response.shiftAttackState;
            if(checkedShift && shiftAttackState == ActionState.Off)
            {
                checkedShift = false;
            }
        }

        void OnHeadbuttAttackStateResponse(HeadbuttAttackStateResponse response)
        {
            headbuttAttackState = response.headbuttAttackState;
        }

        void OnTailSlapAttackStateResponse(TailSlapAttackStateResponse response)
        {
            tailSlapAttackState = response.tailSlapAttackState;
        }
        #endregion

        #region Public Functions
        public override void Reset()
        {
            base.Reset();
            canKill = false;
            checkedShift = false;
        }

        public override void TakeDamage()
        {
            // Debug.Log("canKill " + canKill + " tailslapattackstate " + tailSlapAttackState);
            UpdateCanKill();
            StopCoroutine(attack);
            if(canKill)
            {
                HitRunner();
            }
            else
            {
                animator.SetBool("dead", true);
                EnableCollider(false);
            }
        }

        public override void TakeSpecialDamage()
        {
            Die();
        }
        #endregion

        #region Protected Functions
        protected override void Attack()
        {
            if(!startedAttack)
            {
                startedAttack = true;
                attack = _Attack();
                StartCoroutine(attack);
            }
        }

        protected override void OnCollide(GameObject other)
        {
            if(LayerMask.LayerToName(other.layer) == "PlayerBody")
            {
                HitRunner();
            }
            else if(LayerMask.LayerToName(other.layer) == "Player")
            {
                TakeDamage();
            }
        }
        #endregion

        #region Private Methods
        void HitRunner()
        {
            if(!hasHit)
            {
                hasHit = true;
                Message.Send(new RunnerHit(monsterMetadata.monsterType, AttackType.Physical));
                animator.SetBool("attack", false);
            }
        }

        void UpdateCanKill()
        {
            _distanceVec = trans.InverseTransformPoint(currentControllerPosition);
            float curX = Mathf.Abs(_distanceVec.x);
            // Debug.Log(tailSlapAttackState + " " + curX);
            if(tailSlapAttackState == ActionState.On)
            {
                canKill = false;
            }
            else if(headbuttAttackState != ActionState.Off || tailSlapAttackState != ActionState.Off)
            {
                canKill = curX > 0.5f;
            }
            else if(headbuttAttackState == ActionState.Off && tailSlapAttackState == ActionState.Off && shiftAttackState == ActionState.Off)
            {
                canKill = true;
            }
        }
        #endregion

        #region Coroutines
        IEnumerator _Attack()
        {
            bool inFront = IsInFront(currentControllerPosition);
            bool startedAttackAnim = false;
            bool lockRotation = false;
            bool checkShiftDirection = false;
            bool canAttack = true;
            // bool startedShift = false;
            Vector3 distanceVec = trans.InverseTransformPoint(currentControllerPosition);
            float prevX = 0;
            EnableCollider(true);
            while(inFront)
            {
                currentControllerPosition = CurrentTransform.position;
                currentControllerPosition.y = rootTransform.position.y;
                inFront = IsInFront(currentControllerPosition);
                distanceVec = trans.InverseTransformPoint(currentControllerPosition);
                float curX = Mathf.Abs(distanceVec.x);
                if(checkShiftDirection && !checkedShift)
                {
                    float diff = Mathf.Abs(prevX - curX);
                    if(diff > 0.05f)
                    {
                        if(curX < prevX)
                        {
                            if(prevX <= 1.6f)
                            {
                                canAttack = false;
                                canKill = false;
                                // lockRotation = true;
                            }
                            else
                            {
                                // moving towards but farther away
                                canAttack = true;
                                lockRotation = false;
                                canKill = true;
                            }
                        }
                        else
                        {
                            // moving away
                            if(prevX >= 1.4f)
                            {
                                canAttack = false;
                                lockRotation = true;
                                canKill = false;
                            }
                            else
                            {
                                canAttack = true;
                                lockRotation = false;
                                canKill = true;
                            }
                        }
                        checkShiftDirection = false;
                        checkedShift = true;
                    }
                }

                currentDistance = distanceVec.z;
                
                // if(currentDistance < 3 && !startedShift)
                // {
                //     startedShift = true;
                //     dragon.ShiftToPosition(test, ShiftDistanceType.x);
                //     dragon.CurrentPosition--;
                // }
                // if(startedShift)
                // {
                //     dragon.MoveWithMultiplier(1);
                // }

                if(shiftAttackState != ActionState.Off)
                {
                    checkShiftDirection = true;
                    prevX = curX;
                }

                if(currentDistance < 2f)
                {
                    if(canAttack && !startedAttackAnim)
                    {
                        startedAttackAnim = true;
                        animator.SetBool("attack", true);
                    }
                }
                if(currentDistance < 0.2f)
                {
                    lockRotation = true;
                }
                if(!lockRotation)
                {
                    rootTransform.LookAt(currentControllerPosition);
                }
                UpdateCanKill();
                // Debug.Log("canKill " + canKill + " tailslapattackstate " + tailSlapAttackState);
                yield return null;
            }
            animator.SetBool("attack", false);
        }
        #endregion
    }
}
