using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;
using Ryzm.Dragon;

namespace Ryzm.EndlessRunner
{
    public class EndlessMonafly : EndlessWaitingMonster
    {
        #region Public Variables
        public ParticlesContainer particlesContainer;
        #endregion

        #region Private Variables
        IEnumerator stationarySpecial;
        float firePauseRate = 1;
        WaitForSeconds firePause;
        #endregion

        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            firePause = new WaitForSeconds(firePauseRate);
        }
        #endregion

        #region Listener Functions
        protected override void OnMonsterMetadataResponse(MonsterMetadataResponse response)
        {
            if(!gotMetadata)
            {
                base.OnMonsterMetadataResponse(response);
                particlesContainer.monsterMetadata = monsterMetadata;
            }
        }
        #endregion

        #region Public Functions
        public override void Reset()
        {
            base.Reset();
            StopAllCoroutines();
            stationarySpecial = null;
            particlesContainer.DisableParticles();
        }

        public override void TakeDamage()
        {
            base.TakeDamage();
            StopAllCoroutines();
            stationarySpecial = null;
            particlesContainer.DisableParticles();
        }
        #endregion

        #region Protected Functions
        protected override void Attack()
        {
            if(!startedAttack)
            {
                startedAttack = true;
                stationarySpecial = StationarySpecial();
                StartCoroutine(stationarySpecial);
            }
        }
        #endregion

        #region Private Functions
        protected void SetSpecial()
        {
            animator.SetTrigger("special");
        }
        #endregion

        #region Coroutines
        protected IEnumerator StationarySpecial()
        {
            while(true)
            {
                float t = 0;
                bool hasParticle = particlesContainer.EnableParticle();
                while(t < particlesContainer.ExpansionTime)
                {
                    t += Time.deltaTime;
                    yield return null;
                }
                if(hasParticle)
                {
                    SetSpecial();
                }
                yield return firePause;
            }
        }
        #endregion
    }
}
