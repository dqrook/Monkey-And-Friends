using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessMonodon : EndlessWaitingMonster
    {
        #region Private Variables
        int forwardSpeed = 6;
        IEnumerator attack;
        #endregion

        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            move = Vector3.zero;
            ResetAnim();
        }
        #endregion

        #region Public Functions
        public override void Reset()
        {
            base.Reset();
            StopAllCoroutines();
            attack = null;
            ResetAnim();
            trans.localPosition = startPosition;
        }

        public override void TakeDamage()
        {
            base.TakeDamage();
            ResetAnim();
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
        void ResetAnim()
        {
            SetIsAttacking(false);
            SetIsMoving(false);
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

        #region Coroutines
        IEnumerator _Attack()
        {
            if(type == MonsterType.MovingMonodon)
            {
                SetIsMoving(true);
                SetIsAttacking(false);
                float zMove = Time.deltaTime * forwardSpeed;
                while(currentDistance > 2)
                {
                    currentDistance = trans.InverseTransformPoint(currentDragonPosition).z;
                    move.z = zMove;
                    trans.Translate(move);
                    yield return null;
                }
            }
            SetIsMoving(false);
            SetIsAttacking(true);
            while(currentDistance > -2)
            {
                currentDistance = trans.InverseTransformPoint(currentDragonPosition).z;
                yield return null;
            }
            ResetAnim();
        }
        #endregion
    }
}
