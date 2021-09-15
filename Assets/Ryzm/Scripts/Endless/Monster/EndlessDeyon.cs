using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessDeyon : EndlessWaitingMonster
    {
        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            attackDistance = 10;
        }
        #endregion

        #region Public Functions
        public override void Reset()
        {
            base.Reset();
            SetIsAttacking(false);
        }
        #endregion

        #region Protected Functions
        protected override void Attack()
        {
            SetIsAttacking(true);
        }
        #endregion

        #region Private Functions
        void SetIsAttacking(bool isAttacking)
        {
            animator.SetBool("attack", isAttacking);
        }
        #endregion
    }
}
