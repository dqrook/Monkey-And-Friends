using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.Dragon
{
    public class ProjectileParticles : CustomParticles
    {
        #region Public Variables
        public float speed;
        public HitParticles hitParticles;
        #endregion

        #region Private Variables
        IEnumerator expandAndFire;
        bool hasHit;
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

        #region Public Variables
        public override void Enable()
        {
            trans.localScale = Vector3.zero;
            ResetLocalPosition();
            hasHit = false;
            base.Enable();
            expandAndFire = ExpandAndFire();
            StartCoroutine(expandAndFire);
        }

        public override void Disable()
        {
            base.Disable();
            StopAllCoroutines();
            ResetLocalPosition();
            hasHit = false;
            if(hitParticles != null)
            {
                hitParticles.Disable();
            }
        }
        #endregion

        void CheckHit(Collider other)
        {
            if(!hasHit)
            {
                bool checkUser = target == ParticleTarget.User || target == ParticleTarget.Any;
                bool checkEnemy = target == ParticleTarget.Enemy || target == ParticleTarget.Any;
                Debug.Log(LayerMask.LayerToName(other.gameObject.layer));
                if(checkUser)
                {
                    if(LayerMask.LayerToName(other.gameObject.layer) == "PlayerBody")
                    {
                        Message.Send(new RunnerDie());
                        hasHit = true;
                    }
                }

                if(checkEnemy)
                {
                    EndlessMonster monster = other.gameObject.GetComponent<EndlessMonster>();
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

        #region Coroutines
        IEnumerator ExpandAndFire()
        {
            float t = 0;
            while(t < expansionTime)
            {
                t += Time.deltaTime;
                float frac = t / expansionTime;
                trans.localScale = startLocalScale * frac;
                yield return null;
            }
            trans.localScale = startLocalScale;
            trans.parent = null;
            while(true)
            {
                move.z = Time.deltaTime * speed;
                trans.Translate(move);
                yield return null;
            }
        }
        #endregion
    }
}
