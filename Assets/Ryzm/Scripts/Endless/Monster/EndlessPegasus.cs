using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrailsFX;

namespace Ryzm.EndlessRunner
{
    public class EndlessPegasus : EndlessWaitingMonster
    {
        public TrailEffect bodyTrailEffect;
        
        #region Private Variables
        int forwardSpeed = 8;
        IEnumerator attack;
        #endregion

        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            move = Vector3.zero;
            ActivateAttack(false);
        }
        #endregion

        #region Public Functions
        public override void Reset()
        {
            base.Reset();
            StopAllCoroutines();
            attack = null;
            ActivateAttack(false);
            trans.localPosition = startPosition;
        }

        public override void TakeDamage()
        {
            base.TakeDamage();
            ActivateAttack(false);
            StopAllCoroutines();
            attack = null;
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

        #region Private Functions
        void ActivateAttack(bool activate)
        {
            SetBodyTrailEffect(activate);
            SetIsAttacking(activate);
            SetIsMoving(activate);
        }

        void SetBodyTrailEffect(bool active)
        {
            bodyTrailEffect.active = active;
        }

        void SetIsMoving(bool isMoving)
        {
            animator.SetBool("move", isMoving);
        }

        void SetIsAttacking(bool isAttacking)
        {
            animator.SetBool("attack", isAttacking);
        }
        #endregion

        IEnumerator _Attack()
        {
            ActivateAttack(true);
            Vector3 attackStartPosition = trans.position;
            float distanceTraveled = Mathf.Abs(trans.InverseTransformPoint(attackStartPosition).z);
            float zMove = Time.deltaTime * forwardSpeed;
            while(currentDistance > -10 && distanceTraveled < 25)
            {
                currentDistance = trans.InverseTransformPoint(currentDragonPosition).z;
                move.z = zMove;
                trans.Translate(move);
                distanceTraveled = Mathf.Abs(trans.InverseTransformPoint(attackStartPosition).z);
                yield return null;
            }
            ActivateAttack(false);
        }
    }
}
