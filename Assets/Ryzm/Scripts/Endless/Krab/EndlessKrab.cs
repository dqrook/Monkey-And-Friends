using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessKrab : EndlessMonster
    {
        #region Public Variables
        public List<Collider> bodyColliders = new List<Collider>();
        #endregion

        #region Protected Functions
        protected override void EnableCollider(bool shouldEnable)
        {
            foreach(Collider col in bodyColliders)
            {
                col.enabled = shouldEnable;
            }
        }
        #endregion
    }
}
