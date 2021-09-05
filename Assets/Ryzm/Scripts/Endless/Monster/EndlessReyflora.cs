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
        EndlessDragon dragon;
        AttackState shiftAttackState;
        AttackState headbuttAttackState;
        bool canKill;
        bool checkedShift;
        #endregion

        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            attackDistance = 15;
            EnableCollider(false);
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
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Message.RemoveListener<ControllersResponse>(OnControllersResponse);
            Message.RemoveListener<ShiftAttackStateResponse>(OnShiftAttackStateResponse);
            Message.RemoveListener<HeadbuttAttackStateResponse>(OnHeadbuttAttackStateResponse);
        }

        
        #endregion

        #region Listener Functions
        void OnControllersResponse(ControllersResponse response)
        {
            dragon = response.dragon;
            dragonTrans = dragon.transform;
        }

        void OnShiftAttackStateResponse(ShiftAttackStateResponse response)
        {
            shiftAttackState = response.shiftAttackState;
            if(checkedShift && shiftAttackState == AttackState.Off)
            {
                checkedShift = false;
            }
        }

        void OnHeadbuttAttackStateResponse(HeadbuttAttackStateResponse response)
        {
            headbuttAttackState = response.headbuttAttackState;
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
            Debug.Log("canKill " + canKill);
            StopCoroutine(attack);
            if(canKill)
            {
                Message.Send(new RunnerDie());
                animator.SetBool("attack", false);
            }
            else
            {
                animator.SetBool("dead", true);
                EnableCollider(false);
                // StartCoroutine(FinishDaJob());
            }
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
        #endregion
        bool gotEm = false;
        protected override void OnCollide(GameObject other)
        {
            base.OnCollide(other);
            gotEm = true;
        }

        #region Coroutines
        IEnumerator _Attack()
        {
            bool inFront = IsInFront(currentDragonPosition);
            bool startedAttackAnim = false;
            bool lockRotation = false;
            bool checkShiftDirection = false;
            bool canAttack = true;
            // bool startedShift = false;
            Vector3 distanceVec = trans.InverseTransformPoint(currentDragonPosition);
            float prevX = 0;
            EnableCollider(true);
            while(inFront)
            {
                // dragon.MoveWithMultiplier(1);
                currentDragonPosition = dragonTrans.position;
                currentDragonPosition.y = rootTransform.position.y;
                inFront = IsInFront(currentDragonPosition);
                distanceVec = trans.InverseTransformPoint(currentDragonPosition);
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

                if(shiftAttackState != AttackState.Off)
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
                    rootTransform.LookAt(currentDragonPosition);
                }
                if(headbuttAttackState != AttackState.Off)
                {
                    canKill = curX > 0.5f;
                }
                yield return null;
            }
            animator.SetBool("attack", false);
            // float t = 0;
            // while(t < 0.5f)
            // {
            //     t += Time.deltaTime;
            //     dragon.MoveWithMultiplier(1);
            //     yield return null;
            // }
        }

        IEnumerator FinishDaJob()
        {
            float t = 0;
            while(t < 0.5f)
            {
                t += Time.deltaTime;
                dragon.MoveWithMultiplier(1);
                yield return null;
            }
        }
        #endregion
    }
}
