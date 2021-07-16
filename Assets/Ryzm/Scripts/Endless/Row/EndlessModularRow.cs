﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessModularRow : EndlessRow
    {
        public List<ModularSection> modularSections = new List<ModularSection>();

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
                            _turnSection = CreateSection(trans, st);
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
                        _turnSection = modularSection.section;
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
        }

        protected EndlessSection CreateSection(Transform spawnTransform, SectionType type)
        {
            GameObject newSection = EndlessPool.Instance.GetSpecifiedSection(type);
            if(newSection == null) return null;

            newSection.transform.position = spawnTransform.position;
            newSection.transform.rotation = spawnTransform.rotation;

            EndlessSection _section = newSection.GetComponent<EndlessSection>();
            _section.CancelDeactivation();
            newSection.SetActive(true);
            return _section;
        }
    }

    [System.Serializable]
    public struct ModularSection
    {
        public SectionType[] types;
        public bool isTurn;
        public EndlessSection section;
    }
}
