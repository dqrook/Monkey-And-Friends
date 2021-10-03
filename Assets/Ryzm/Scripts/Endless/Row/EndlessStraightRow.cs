using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class EndlessStraightRow : EndlessModularRow
    {
        #region Public Variables
        [Tooltip("Set to 0 or less for an infinite straight")]
        public int numberOfSections;
        public List<ModularEnvironment> modularEnvironments = new List<ModularEnvironment>();
        #endregion

        #region Private Variables
        int numberSectionsEntered;
        int modularSectionIndex;
        bool initialSectionEntered;
        int modularEnvironmentIndex;
        int numModularEnvironments;
        #endregion

        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            Message.AddListener<CurrentSectionChange>(OnCurrentSectionChange);
            numModularEnvironments = modularEnvironments.Count;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Message.RemoveListener<CurrentSectionChange>(OnCurrentSectionChange);
        }
        #endregion

        #region Listener Functions
        void OnCurrentSectionChange(CurrentSectionChange sectionChange)
        {
            if(sectionChange.rowId == rowId)
            {
                if(initialSectionEntered)
                {
                    if(numberOfSections <= 0 || numberOfSectionsCreated < numberOfSections)
                    {
                        ModularSection modularSection = modularSections[modularSectionIndex];
                        SectionType st = modularSection.types[Random.Range(0, modularSection.types.Length)];
                        EndlessSection _section = CreateSection(FinalSpawn(), st);
                        _section.rowId = rowId;
                        sections.Add(_section);
                        PlaceSingleBarrier(_section);
                        AddEnvironment(_section);
                        sections.RemoveAt(0);

                        modularSectionIndex++;
                        if(modularSectionIndex > modularSections.Count - 1)
                        {
                            modularSectionIndex = 0;
                        }
                        numberOfSectionsCreated++;
                    }
                    else if(numberOfSectionsCreated == numberOfSections)
                    {
                        Message.Send(new AddTransitionRequest(rowId));
                    }
                }
                initialSectionEntered = true;
                numberSectionsEntered++;
            }
        }
        #endregion

        #region Public Functions
        public override void Initialize(int numberOfSections, ShiftDistanceType shiftDistanceType = ShiftDistanceType.x)
        {
            base.Initialize(numberOfSections, shiftDistanceType);
            InitializeEnvironment();
        }
        #endregion

        #region Private Functions
        void InitializeEnvironment()
        {
            modularEnvironmentIndex = 0;
            foreach(EndlessSection section in sections)
            {
                AddEnvironment(section);
            }
        }

        void AddEnvironment(EndlessSection section)
        {
            if(modularEnvironmentIndex < numModularEnvironments)
            {
                ModularEnvironment modularEnvironment = modularEnvironments[modularEnvironmentIndex];
                if(modularEnvironment.types.Length > 0)
                {
                    EndlessUtils.Shuffle(modularEnvironment.types);
                    EnvironmentType randomType = modularEnvironment.types[0];
                    GameObject envGO = EndlessPool.Instance.GetSpecifiedEnvironment(randomType);
                    if(envGO != null)
                    {
                        EndlessEnvironment environment = envGO.GetComponent<EndlessEnvironment>();
                        envGO.transform.position = section.transform.position;
                        envGO.transform.rotation = section.transform.rotation;
                        environment.Initialize(section);
                        envGO.SetActive(true);
                    }
                }
            }
            modularEnvironmentIndex++;
            if(modularEnvironmentIndex > numModularEnvironments - 1)
            {
                modularEnvironmentIndex = 0;
            }
        }
        #endregion
    }

    [System.Serializable]
    public struct ModularEnvironment
    {
        public EnvironmentType[] types;
    }
}
