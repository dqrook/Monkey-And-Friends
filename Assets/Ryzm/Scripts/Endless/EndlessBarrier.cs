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
        public EndlessSection section;

        protected override void Awake()
        {
            base.Awake();
            Message.AddListener<SectionDeactivated>(OnSectionDeactivated);
        }

        protected void OnCollisionEnter(Collision other)
        {
            Message.Send(new RunnerDie());
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Message.RemoveListener<SectionDeactivated>(OnSectionDeactivated);
        }

        void OnSectionDeactivated(SectionDeactivated sectionDeactivated)
        {
            if(sectionDeactivated.section == section)
            {
                gameObject.SetActive(false);
            }
        }
    }

    public enum BarrierType
    {
        Fire,
        Dragon
    }
}
