using System.Collections;
using System.Collections.Generic;
using Ryzm.EndlessRunner.Messages;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessRabbyRow : EndlessBarrier
    {
        public List<EndlessRabby> rabbies = new List<EndlessRabby>();
        public List<Material> rabbyMaterials = new List<Material>();

        public override void Initialize(Transform parentTransform, int position)
        {
            base.Initialize(parentTransform, position);
            int materialIndex = -1;
            if(rabbyMaterials.Count > 0)
            {
                materialIndex = Random.Range(0, rabbyMaterials.Count);

            }
            foreach(EndlessRabby rabby in rabbies)
            {
                if(materialIndex >= 0)
                {
                    rabby.Initialize(rabbyMaterials[materialIndex]);
                }
            }
        }

        protected override void OnSectionDeactivated(SectionDeactivated sectionDeactivated)
        {
            base.OnSectionDeactivated(sectionDeactivated);
            if(sectionDeactivated.section == parentSection)
            {
                foreach(EndlessRabby rabby in rabbies)
                {
                    rabby.Reset();
                }
            }
        }
    }
}
