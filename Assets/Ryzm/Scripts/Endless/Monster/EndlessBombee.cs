using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.Dragon;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class EndlessBombee : EndlessWaitingMonster
    {
        #region Public Variables
        public CustomParticles explosionParticles;
        #endregion

        #region Private Variables
        bool exploded;
        #endregion

        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            attackDistance = 10;
        }
        #endregion

        #region Public Functions
        public override void TakeDamage()
        {
            Explode();
        }

        public override void Reset()
        {
            base.Reset();
            explosionParticles.Disable();
            animator.SetBool("changeColor", false);
            exploded = false;
        }
        #endregion

        #region Protected Functions
        protected override void OnCollide(GameObject other)
        {
            if(other.gameObject.GetComponent<EndlessController>())
            {
                Explode();
                Message.Send(new RunnerDie());
            }
        }
        protected override void Attack()
        {
            if(!startedAttack)
            {
                startedAttack = true;
                animator.SetBool("changeColor", true);
            }
        }
        #endregion

        #region Private Functions
        void Explode()
        {
            if(!exploded)
            {
                exploded = true;
                explosionParticles.Enable();
                animator.SetBool("dead", true);
            }
        }
        #endregion
    }
}
