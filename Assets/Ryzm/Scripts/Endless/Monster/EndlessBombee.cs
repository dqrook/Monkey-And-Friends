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
        public ExplosionParticles explosionParticles;
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

        protected override void OnMonsterMetadataResponse(MonsterMetadataResponse response)
        {
            base.OnMonsterMetadataResponse(response);
            explosionParticles.monsterMetadata = monsterMetadata;
        }

        #region Public Functions
        public override void TakeDamage()
        {
            Explode();
        }

        public override void TakeSpecialDamage()
        {
            Die(); // or Explode() ?
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
            if(!hasHit && other.gameObject.GetComponent<EndlessController>())
            {
                hasHit = true;
                Explode();
                Message.Send(new RunnerHit(monsterMetadata.monsterType, AttackType.Physical));
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
                Die();
            }
        }
        #endregion
    }
}
