using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public class EndlessTregon : EndlessMonster
    {
        #region Event Functions
        protected override void OnCollisionEnter(Collision other)
        {
            if(other.gameObject.GetComponent<EndlessController>())
            {
                Message.Send(new RunnerDie());
            }
        }
        #endregion

        #region Public Functions
        public override void TakeDamage() {}
        #endregion
    }
}
