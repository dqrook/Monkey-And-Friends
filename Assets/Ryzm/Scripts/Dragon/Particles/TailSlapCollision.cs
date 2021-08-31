using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner;

namespace Ryzm.Dragon
{
    public class TailSlapCollision : ParticleCollision
    {
        public DragonTailSlap tailSlap;
        protected override void OnParticleCollision(GameObject other)
        {
            Debug.Log("particle collision");
            EndlessMonster monster = other.GetComponent<EndlessMonster>();
            if(monster != null)
            {
                monster.TakeDamage();
                // tailSlap.EnableExplosion();
            }
        }
    }
}
