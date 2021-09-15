using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public class EndlessTregon : EndlessMonster
    {

        #region Public Functions
        public override void TakeDamage() {}

        public override void TakeSpecialDamage()
        {
            Die();
        }
        #endregion

        #region Protected Functions
        protected override void OnCollide(GameObject other)
        {
            if(!hasHit && other.GetComponent<EndlessController>())
            {
                hasHit = true;
                Message.Send(new RunnerHit(monsterMetadata.monsterType, AttackType.Physical));
            }
        }
        #endregion
    }
}
