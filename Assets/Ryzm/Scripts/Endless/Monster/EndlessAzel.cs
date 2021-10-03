using System.Collections;
using System.Collections.Generic;
using Ryzm.EndlessRunner.Messages;
using UnityEngine;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class EndlessAzel : EndlessWaitingMonster
    {
        #region Public Variables
        public Transform rootTransform;
        #endregion

        #region Private Variables
        IEnumerator attack;
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

        protected override void OnTriggerEnter(Collider other)
        {
            if(LayerMask.LayerToName(other.gameObject.layer) == "PlayerBody")
            {
                HitRunner();
            }
            else if(LayerMask.LayerToName(other.gameObject.layer) == "Player")
            {
                TakeDamage();
            }
        }

        protected override void OnTriggerStay(Collider other)
        {
            if(LayerMask.LayerToName(other.gameObject.layer) == "PlayerBody")
            {
                HitRunner();
            }
            else if(LayerMask.LayerToName(other.gameObject.layer) == "Player")
            {
                TakeDamage();
            }
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
            animator.SetBool("attack", false);
            canKill = false;
            checkedShift = false;
        }

        public override void TakeDamage()
        {
            UpdateCanKill();
            StopCoroutine(attack);
            if(canKill)
            {
                HitRunner();
            }
            else
            {
                Die();
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
            if(LayerMask.LayerToName(other.gameObject.layer) == "PlayerBody")
            {
                HitRunner();
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
            if(headbuttAttackState != ActionState.Off || tailSlapAttackState != ActionState.Off)
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
            bool inFront = IsInFront(currentControllerPosition, -5);
            bool startedAttackAnim = false;
            bool lockRotation = false;
            Vector3 distanceVec = trans.InverseTransformPoint(currentControllerPosition);
            EnableCollider(true);
            while(inFront)
            {
                currentControllerPosition = CurrentTransform.position;
                currentControllerPosition.y = rootTransform.position.y;
                inFront = IsInFront(currentControllerPosition);
                distanceVec = trans.InverseTransformPoint(currentControllerPosition);
                float curX = Mathf.Abs(distanceVec.x);

                currentDistance = distanceVec.z;
                float distance = distanceVec.z * distanceVec.z + distanceVec.x * distanceVec.x;
                if((distance < 3f || currentDistance < 2.25f) && !startedAttackAnim)
                {
                    startedAttackAnim = true;
                    animator.SetBool("attack", true);
                }
                if(currentDistance < -1)
                {
                    lockRotation = true;
                }
                if(!lockRotation)
                {
                    rootTransform.LookAt(currentControllerPosition);
                }
                UpdateCanKill();
                yield return null;
            }
            animator.SetBool("attack", false);
        }
        #endregion
    }
}
