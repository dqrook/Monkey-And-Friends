using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner;

namespace Ryzm.Dragon
{
    public class ParticleCollision : MonoBehaviour
    {
        #region Protected Functions
        protected virtual void OnParticleCollision(GameObject other)
        {
            EndlessMonster monster = other.GetComponent<EndlessMonster>();
            if(monster != null)
            {
                monster.TakeDamage();
            }
        }
        #endregion
    }
}
