using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessRow : MonoBehaviour
    {
        public List<EndlessSection> sections = new List<EndlessSection>();
        // also have a single EndlessTurnSection (which will inherit from EndlessSection)
        EndlessSection turnSection;
        int numberSectionsSinceBarrier;

        public void Initialize(int numberOfSections)
        {
            Transform trans = gameObject.transform;
            for(int i = 0; i < numberOfSections; i++)
            {
                if(i > 0)
                {
                    trans = sections[i-1].nextSectionSpawn;
                }
                CreateSection(trans);
            }
            foreach(EndlessSection section in sections)
            {
                if(CanPlaceBarrier())
                {
                    CreateBarrier(section);
                }
                else
                {
                    numberSectionsSinceBarrier++;
                }
            }
            sections[sections.Count - 1].isLastSection = true;
            // todo: add a turn section
        }

        void CreateSection(Transform spawnTransform)
        {
            GameObject newSection = EndlessPool.Instance.GetRandomSection();
            if(newSection == null) return;

            newSection.transform.position = spawnTransform.position;
            newSection.transform.rotation = spawnTransform.rotation;

            EndlessSection _section = newSection.GetComponent<EndlessSection>();
            newSection.SetActive(true);
            sections.Add(_section);
        }

        void CreateBarrier(EndlessSection _section)
        {
            if(_section == null)
            {
                return;
            }

            GameObject newBarrier = EndlessPool.Instance.GetRandomBarrier(_section.PossibleBarrierTypes);
            if(newBarrier == null) 
            {
                return;
            }
            
            EndlessBarrier _barrier = newBarrier.GetComponent<EndlessBarrier>();
            if(_barrier == null)
            {
                return;
            }

            Transform spawnLocation = _section.GetBarrierSpawnLocation(_barrier.type);
            if(spawnLocation == null) 
            {
                return;
            }
            
            _barrier.parentSection = _section;
            _barrier.transform.position = spawnLocation.position;
            _barrier.transform.rotation = spawnLocation.rotation;
            _barrier.gameObject.SetActive(true);
            numberSectionsSinceBarrier = 0;
        }

        bool CanPlaceBarrier()
        {
            return Random.Range(0, 2) < numberSectionsSinceBarrier;
        }

        public Transform FinalSpawn()
        {
            if(turnSection != null)
            {
                return turnSection.nextSectionSpawn;
            }
            return sections[sections.Count - 1].nextSectionSpawn;
        }
    }
}
