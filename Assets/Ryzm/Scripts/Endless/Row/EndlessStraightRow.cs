using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class EndlessStraightRow : EndlessModularRow
    {
        [Tooltip("Set to 0 or less for an infinite straight")]
        public int numberOfSections;
        int numberSectionsEntered;
        int modularSectionIndex;
        bool initialSectionEntered;

        protected override void Awake()
        {
            base.Awake();
            Message.AddListener<CurrentSectionChange>(OnCurrentSectionChange);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Message.RemoveListener<CurrentSectionChange>(OnCurrentSectionChange);
        }

        public override void Initialize(int numberOfSections)
        {
            base.Initialize(numberOfSections);
        }

        void OnCurrentSectionChange(CurrentSectionChange sectionChange)
        {
            if(sectionChange.rowId == rowId)
            {
                if(initialSectionEntered)
                {
                    if(numberOfSections <= 0 || numberSectionsEntered < numberOfSections)
                    {
                        ModularSection modularSection = modularSections[modularSectionIndex];
                        SectionType st = modularSection.types[Random.Range(0, modularSection.types.Length)];
                        EndlessSection _section = CreateSection(FinalSpawn(), st);
                        _section.rowId = rowId;
                        sections.Add(_section);
                        PlaceSingleBarrier(_section);
                        sections.RemoveAt(0);

                        modularSectionIndex++;
                        if(modularSectionIndex > modularSections.Count - 1)
                        {
                            modularSectionIndex = 0;
                        }
                    }
                }
                initialSectionEntered = true;
                numberSectionsEntered++;
            }
        }
    }
}
