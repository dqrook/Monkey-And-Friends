using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public abstract class EndlessWaitingMonster : EndlessMonster
    {
        #region Protected Variables
        protected bool startedAttack;
        protected float attackDistance = 20;
        protected float currentDistance = 100;
        protected Vector3 currentDragonPosition;
        protected Vector3 move;
        #endregion

        #region Event Functions
        protected override void OnEnable()
        {
            base.OnEnable();
            Message.AddListener<ControllerTransformResponse>(OnControllerTransformResponse);
            Message.Send(new ControllerTransformRequest());
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Message.RemoveListener<ControllerTransformResponse>(OnControllerTransformResponse);
        }
        #endregion

        #region Public Functions
        public override void Reset()
        {
            base.Reset();
            currentDistance = 100;
            startedAttack = false;
        }
        #endregion

        #region Listener Functions
        protected virtual void OnControllerTransformResponse(ControllerTransformResponse response)
        {
            if(gameStatus == GameStatus.Active)
            {
                currentDragonPosition = response.position;
                currentDistance = trans.InverseTransformPoint(currentDragonPosition).z;
                bool inFront = IsInFront(currentDragonPosition);
                float diff = Mathf.Abs((response.position - trans.position).z);
                if(inFront && currentDistance < attackDistance)
                {
                    Attack();
                }
            }
        }
        #endregion

        #region Protected Functions
        protected bool IsInFront(Vector3 targetPosition, float offset = 0)
        {
            return Vector3.Dot(trans.forward, trans.InverseTransformPoint(targetPosition)) > offset;
        }

        protected virtual void Attack() {}
        #endregion
    }
}
