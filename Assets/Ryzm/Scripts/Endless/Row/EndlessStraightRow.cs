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
        public int maximumNumberOfSections;
        int numberSectionsEntered;
        int modularSectionIndex;
        bool initialSectionEntered;

        void Awake()
        {
            Message.AddListener<CurrentSectionChange>(OnCurrentSectionChange);
        }

        void OnDestroy()
        {
            Message.RemoveListener<CurrentSectionChange>(OnCurrentSectionChange);
        }

        public override void Initialize(int numberOfSections)
        {
            base.Initialize(numberOfSections);
        }

        void OnCurrentSectionChange(CurrentSectionChange sectionChange)
        {
            Debug.Log("sectionChange.rowId" + sectionChange.rowId);
            if(sectionChange.rowId == rowId)
            {
                if(initialSectionEntered)
                {
                    if(maximumNumberOfSections <= 0 || numberSectionsEntered < maximumNumberOfSections)
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
