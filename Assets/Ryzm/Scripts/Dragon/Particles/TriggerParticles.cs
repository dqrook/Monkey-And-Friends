using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.Dragon
{
    public abstract class TriggerParticles : CustomParticles
    {
        #region Public Variables
        public HitParticles hitParticles;
        #endregion

        #region Protected Variables
        protected bool hasHit;
        #endregion

        #region Event Functions
        protected override void OnTriggerEnter(Collider other)
        {
            CheckHit(other);
        }

        protected override void OnTriggerStay(Collider other)
        {
            CheckHit(other);
        }
        #endregion

        public override void Enable()
        {
            hasHit = false;
            base.Enable();
        }

        public override void Disable()
        {
            hasHit = false;
            base.Disable();
        }

        #region Protected Functions
        protected virtual void CheckHit(Collider other)
        {
            if(!hasHit)
            {
                bool checkUser = target == ParticleTarget.User || target == ParticleTarget.Any;
                bool checkEnemy = target == ParticleTarget.Enemy || target == ParticleTarget.Any;
                if(checkUser)
                {
                    // if(LayerMask.LayerToName(other.gameObject.layer) == "PlayerBody")
                    // {
                    //     Message.Send(new RunnerDie());
                    //     hasHit = true;
                    // }
                    if(other.gameObject.GetComponent<EndlessController>())
                    {
                        Message.Send(new RunnerHit(monsterMetadata.monsterType, AttackType.Special));
                        hasHit = true;
                    }
                }
                
                if(checkEnemy)
                {
                    MonsterBase monster = other.gameObject.GetComponent<MonsterBase>();
                    if(monster != null)
                    {
                        monster.TakeDamage();
                        hasHit = true;
                    }
                }

                if(hasHit && hitParticles != null)
                {
                    hitParticles.Enable();
                    PlayParticles(false);
                    isEnabled = false;
                    StopAllCoroutines();
                }
            }
        }
        #endregion
    }
}
