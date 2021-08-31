using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public abstract class EndlessHidingMonster : EndlessWaitingMonster
    {
        #region Public Variables
        public EndlessSection parentSection;
        public Transform childTransform;
        public bool enableDynamicSpawn;
        #endregion

        #region Protected Variables
        protected int runnerPosition;
        protected bool startedCoroutine;
        protected Vector3 initialPosition;
        protected Vector3 initialEulerAngles;
        protected GameObject childGO;
        protected IEnumerator moveThenAttack;
        protected BarrierType barrierType;
        #endregion

        #region Private Variables
        WaitForSeconds wait2Seconds = new WaitForSeconds(2);
        IEnumerator waitThenDisable;
        bool startedDisable;
        #endregion

        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            if(childTransform == null)
            {
                childTransform = GetComponentInChildren<Transform>();
            }
            childGO = childTransform.gameObject;
            initialPosition = childTransform.localPosition;
            initialEulerAngles = childTransform.localEulerAngles;
            attackDistance = 25;
            childGO.SetActive(false);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            Message.AddListener<CurrentPositionResponse>(OnCurrentPositionResponse);
            Message.Send(new CurrentPositionRequest());
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Message.RemoveListener<CurrentPositionResponse>(OnCurrentPositionResponse);
        }
        #endregion

        #region Listener Functions
        protected virtual void OnCurrentPositionResponse(CurrentPositionResponse response)
        {
            runnerPosition = response.position;
        }

        protected override void OnControllerTransformResponse(ControllerTransformResponse response)
        {
            if(gameStatus == GameStatus.Active)
            {
                currentDragonPosition = response.position;
                currentDistance = trans.InverseTransformPoint(currentDragonPosition).z;
                bool inFront = IsInFront(currentDragonPosition);
                float diff = Mathf.Abs((currentDragonPosition - trans.position).z);
                
                if(!startedAttack && inFront && currentDistance < attackDistance)
                {
                    Attack();
                }
                else if(!startedDisable && startedAttack && currentDistance < -10)
                {
                    waitThenDisable = WaitThenDisable();
                    StartCoroutine(waitThenDisable);
                }
            }
        }
        #endregion

        #region Public Functions
        public override void Reset()
        {
            base.Reset();
            startedDisable = false;
            childTransform.localPosition = initialPosition;
            childTransform.localEulerAngles = initialEulerAngles;
            childGO.SetActive(false);
            animator.SetBool("fireBreath", false);
        }
        #endregion

        #region Protected Functions
        protected override void Attack()
        {
            if(!startedAttack)
            {
                childGO.SetActive(true);
                startedAttack = true;
                SetSpawnLocation();
                moveThenAttack = MoveThenAttack();
                StartCoroutine(moveThenAttack);
            }
        }

        protected void SetSpawnLocation()
        {
            if(parentSection != null && enableDynamicSpawn)
            {
                Transform location = parentSection.GetSpawnTransformForBarrier(barrierType, runnerPosition);
                if(location != null)
                {
                    gameObject.transform.position = location.position;
                    gameObject.transform.rotation = location.rotation;
                }
            }
        }
        #endregion

        #region Coroutines
        protected virtual IEnumerator MoveThenAttack()
        {
            yield break;
        }

        IEnumerator WaitThenDisable()
        {
            startedDisable = true;
            yield return wait2Seconds;
            bool inFront = IsInFront(currentDragonPosition);
            bool setActive = !(!inFront && currentDistance < -10);
            childGO.SetActive(setActive);
        }
        #endregion
    }
}
