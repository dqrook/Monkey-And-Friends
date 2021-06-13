using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessRow : MonoBehaviour
    {
        public string id;
        public List<EndlessSection> sections = new List<EndlessSection>();
        // also have a single EndlessTurnSection (which will inherit from EndlessSection)
        public EndlessSection turnSection;
        public List<GameObject> environments = new List<GameObject>();

        public virtual void Initialize(int numberOfSections)
        {
            Transform trans = gameObject.transform;

            // only create sections if none provided
            if(sections.Count == 0)
            {
                for(int i = 0; i < numberOfSections; i++)
                {
                    if(i > 0)
                    {
                        trans = sections[i-1].NextSectionSpawn();
                    }
                    EndlessSection _section = CreateSection(trans, false);
                    if(_section != null)
                    {
                        sections.Add(_section);
                    }
                }
            }
            else
            {
                numberOfSections = sections.Count;
                for(int i = 0; i < numberOfSections; i++)
                {
                    if(i > 0)
                    {
                        trans = sections[i-1].NextSectionSpawn();
                    }
                    CreateSection(trans, false, sections[i].gameObject);
                }
            }

            PlaceBarriers();

            trans = sections[sections.Count - 1].NextSectionSpawn();
            if(turnSection == null)
            {
                turnSection = CreateSection(trans, true);
            }
            else
            {
                CreateSection(trans, true, turnSection.gameObject);
            }

            if(turnSection == null)
            {
                sections[sections.Count - 1].isLastSection = true;
            }

            ChooseEnvironment();
        }

        protected void PlaceBarriers()
        {
            foreach(EndlessSection section in sections)
            {
                if(CanPlaceBarrier(section.barrierLikelihood))
                {
                    CreateBarrier(section);
                }
            }
        }

        protected void ChooseEnvironment()
        {
            if(environments.Count > 0)
            {
                EndlessUtils.Shuffle(environments);
                int i = 0;
                foreach(GameObject environment in environments)
                {
                    environment.SetActive(i == 0);
                    i++;
                }
            }
        }

        protected EndlessSection CreateSection(Transform spawnTransform, bool isTurn, GameObject newSection = null)
        {
            if(newSection == null)
            {
                newSection = EndlessPool.Instance.GetRandomSection(isTurn);
            }
            
            if(newSection == null) return null;

            newSection.transform.position = spawnTransform.position;
            newSection.transform.rotation = spawnTransform.rotation;

            EndlessSection _section = newSection.GetComponent<EndlessSection>();
            newSection.SetActive(true);
            _section.gameObject.SetActive(true);
            
            return _section;
        }

        protected void CreateBarrier(EndlessSection _section)
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

            Transform spawnLocation = _section.GetBarrierSpawnTransform(_barrier.type);
            if(spawnLocation == null) 
            {
                return;
            }
            
            _barrier.parentSection = _section;
            _barrier.transform.position = spawnLocation.position;
            _barrier.transform.rotation = spawnLocation.rotation;
            _barrier.gameObject.SetActive(true);
        }

        protected bool CanPlaceBarrier(float barrierLikelihood)
        {
            return Random.Range(0, 1f) < barrierLikelihood;
        }

        public Transform FinalSpawn()
        {
            if(turnSection != null)
            {
                return turnSection.NextSectionSpawn();
            }
            return sections[sections.Count - 1].NextSectionSpawn();
        }
    }
}
