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

        void Awake()
        {
            Message.AddListener<SectionDeactivated>(OnSectionDeactivated);
        }

        void OnCollisionEnter(Collision other)
        {
            // RunnerController runner = other.gameObject.GetComponent<RunnerController>();
            Message.Send(new RunnerDie());
            // if(runner != null)
            // {
            //     runner.Die();
            // }
        }

        void OnDestroy()
        {
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
        Fire
    }
}
