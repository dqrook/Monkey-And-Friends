using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public class EndlessParticleBarrier : EndlessBarrier
    {
        public ParticleSystem part;
        public Collider _collider;
        
        protected override void Awake()
        {
            base.Awake();
            if(part == null)
            {
                part = GetComponentInChildren<ParticleSystem>();
            }
            _collider = GetComponent<Collider>();
            _collider.enabled = false;
        }
        
        protected override void OnCurrentSectionChange(CurrentSectionChange currentSectionChange)
        {
            base.OnCurrentSectionChange(currentSectionChange);
            if(_currentSection == parentSection)
            {
                _collider.enabled = true;
                part.Play();
            }
        }

        protected override void OnSectionDeactivated(SectionDeactivated sectionDeactivated)
        {
            if(sectionDeactivated.section == parentSection)
            {
                _collider.enabled = false;
                part.Stop();
                gameObject.SetActive(false);
            }
        }

        protected override void OnGameStatusResponse(GameStatusResponse gameStatusResponse)
        {
            base.OnGameStatusResponse(gameStatusResponse);
            if(gameStatusResponse.status == GameStatus.Restart)
            {
                _collider.enabled = false;
                if(part.isPlaying)
                {
                    part.Stop();
                }
                gameObject.SetActive(false);
            }
        }
    }
}
