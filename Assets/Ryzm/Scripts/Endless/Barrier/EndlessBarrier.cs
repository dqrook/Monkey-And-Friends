using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class EndlessBarrier : EndlessScroller
    {
        public BarrierType type;
        // the section that the barrier belongs to
        public EndlessSection parentSection;
        public Transform[] spawnLocations;
        protected int runnerPosition;

        protected override void Awake()
        {
            base.Awake();
            Message.AddListener<SectionDeactivated>(OnSectionDeactivated);
            Message.AddListener<CurrentPositionResponse>(OnCurrentPositionResponse);
        }
        
        protected override void OnEnable()
        {
            base.OnEnable();
            Message.Send(new CurrentPositionRequest());
        }

        protected void OnCollisionEnter(Collision other)
        {
            Message.Send(new RunnerDie());
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Message.RemoveListener<SectionDeactivated>(OnSectionDeactivated);
            Message.RemoveListener<CurrentPositionResponse>(OnCurrentPositionResponse);
        }

        protected virtual void OnSectionDeactivated(SectionDeactivated sectionDeactivated)
        {
            if(sectionDeactivated.section == parentSection)
            {
                gameObject.SetActive(false);
            }
        }

        protected virtual void OnCurrentPositionResponse(CurrentPositionResponse response)
        {
            runnerPosition = response.position;
        }
    }

    public enum BarrierType
    {
        Fire,
        Dragon,
        InstantFire
    }
}
