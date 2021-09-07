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
        #endregion
        
        #region Protected Functions
        protected override void OnCollide(GameObject other)
        {
            if(other.GetComponent<EndlessController>())
            {
                Message.Send(new RunnerHit());
            }
        }
        #endregion
    }
}
