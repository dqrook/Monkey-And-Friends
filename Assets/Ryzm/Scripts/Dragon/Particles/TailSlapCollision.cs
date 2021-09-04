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
            MonsterBase monster = other.GetComponent<MonsterBase>();
            if(monster != null)
            {
                monster.TakeDamage();
                // tailSlap.EnableExplosion();
            }
        }
    }
}
