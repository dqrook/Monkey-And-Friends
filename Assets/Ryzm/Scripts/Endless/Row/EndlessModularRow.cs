using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessModularRow : EndlessRow
    {
        public List<ModularSection> modularSections = new List<ModularSection>();

        public override void Initialize(int numberOfSections)
        {
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

            if(turnSection == null)
            {
                sections[sections.Count - 1].isLastSection = true;
            }

            ChooseEnvironment();
        }

        EndlessSection CreateSection(Transform spawnTransform, SectionType type)
        {
            GameObject newSection = EndlessPool.Instance.GetSpecifiedSection(type);
            if(newSection == null) return null;

            newSection.transform.position = spawnTransform.position;
            newSection.transform.rotation = spawnTransform.rotation;

            EndlessSection _section = newSection.GetComponent<EndlessSection>();
            newSection.SetActive(true);
            return _section;
        }
    }

    [System.Serializable]
    public struct ModularSection
    {
        public SectionType[] types;
        public bool isTurn;
    }
}
