using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessModularRow : EndlessRow
    {
        #region Public Variables
        public List<ModularSection> modularSections = new List<ModularSection>();
        #endregion
        
        #region Protected Variables
        protected int numberOfSectionsCreated;
        #endregion

        #region Public Functions
        public override void Initialize(int numberOfSections)
        {
            CreateRowId();
            sections.Clear();
            turnSection = null;
            Transform trans = gameObject.transform;
            numberOfSections = modularSections.Count;
            for(int i = 0; i < numberOfSections; i++)
            {
                if(i > 0)
                {
                    trans = sections[i-1].NextSectionSpawn();
                }
                
                ModularSection modularSection = modularSections[i];
                EndlessSection _section = null;
                EndlessSection _turnSection = null;
                if(modularSection.section == null)
                {
                    if(modularSection.types.Length == 0)
                    {
                        _section = CreateSection(trans, false);
                    }
                    else
                    {
                        SectionType st = modularSection.types[Random.Range(0, modularSection.types.Length)];
                        if(modularSection.isTurn)
                        {
                            _turnSection = CreateSection(trans, st, true);
                        }
                        else
                        {
                            _section = CreateSection(trans, st);
                        }
                    }
                }
                else 
                {
                    if(modularSection.isTurn)
                    {
                        // _turnSection = modularSection.section;
                        _turnSection = CreateSection(sections[sections.Count - 1].transform, modularSection.section.gameObject);
                        _turnSection.gameObject.SetActive(true);
                    }
                    else
                    {
                        _section = modularSection.section;
                        _section.gameObject.SetActive(true);
                    }
                }
                if(_turnSection != null)
                {
                    turnSection = _turnSection;
                }
                else if(_section != null)
                {
                    sections.Add(_section);
                }
            }

            PlaceBarriers();
            ChooseEnvironment();
            UpdateSectionsRowId();
            numberOfSectionsCreated = numberOfSections;
        }
        #endregion

        #region Protected Functions
        protected EndlessSection CreateSection(Transform spawnTransform, SectionType type, bool isTurn = false, GameObject newSection = null)
        {
            if(newSection == null)
            {
                newSection = EndlessPool.Instance.GetSpecifiedOrRandomSection(type, isTurn);
            }

            if(newSection == null) return null;

            return CreateSection(spawnTransform, newSection);
        }

        protected EndlessSection CreateSection(Transform spawnTransform, GameObject newSection)
        {
            newSection.transform.position = spawnTransform.position;
            newSection.transform.rotation = spawnTransform.rotation;

            EndlessSection _section = newSection.GetComponent<EndlessSection>();
            _section.Initialize(rowId);
            _section.CancelDeactivation();
            newSection.SetActive(true);
            return _section;
        }
        #endregion
    }

    [System.Serializable]
    public struct ModularSection
    {
        public SectionType[] types;
        public bool isTurn;
        public EndlessSection section;
    }
}
