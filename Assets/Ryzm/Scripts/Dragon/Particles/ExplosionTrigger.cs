using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.Dragon
{
    public class ExplosionTrigger : MonoBehaviour
    {
        #region Public Variables
        public ExplosionParticles explosion;
        #endregion

        #region Private Functions
        void OnTriggerEnter(Collider other)
        {
            explosion.OnTrigger(other);
        }

        void OnTriggerStay(Collider other)
        {
            explosion.OnTrigger(other);
        }
        #endregion
    }
}
