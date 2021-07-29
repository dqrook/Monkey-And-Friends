using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class EndlessRow : EndlessItem
    {
        #region Public Variables
        public string id;
        public List<EndlessSection> sections = new List<EndlessSection>();
        // also have a single EndlessTurnSection (which will inherit from EndlessSection)
        public EndlessSection turnSection;
        public List<GameObject> environments = new List<GameObject>();
        public int rowId;
        #endregion

        #region Public Functions
        public virtual void Initialize(int numberOfSections)
        {
            CreateRowId();
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

            ChooseEnvironment();
            UpdateSectionsRowId();
        }

        public Transform FinalSpawn()
        {
            if(turnSection != null)
            {
                return turnSection.NextSectionSpawn();
            }
            return sections[sections.Count - 1].NextSectionSpawn();
        }
        #endregion

        #region Protected Functions
        protected virtual void CreateRowId()
        {
            rowId = Random.Range(1, 100000);
        }

        protected virtual void UpdateSectionsRowId()
        {
            Debug.Log("rowId " + rowId);
            foreach(EndlessSection section in sections)
            {
                section.rowId = rowId;
            }
            if(turnSection != null)
            {
                turnSection.rowId = rowId;
            }
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

        protected void PlaceSingleBarrier(EndlessSection section)
        {
            if(CanPlaceBarrier(section.barrierLikelihood))
            {
                CreateBarrier(section);
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
            _section.Initialize(rowId);
            _section.CancelDeactivation();
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

            SpawnLocation spawnLocation = _section.GetSpawnLocationForBarrier(_barrier.type);
            if(spawnLocation == null) 
            {
                return;
            }
            SpawnTransform spawnTransform = spawnLocation.RandomSpawnTransform();
            Transform spawn = spawnTransform.Location;
            
            _barrier.parentSection = _section;
            _barrier.transform.position = spawn.position;
            _barrier.transform.rotation = spawn.rotation;
            _barrier.Initialize(spawn, spawnTransform.position);
            _barrier.gameObject.SetActive(true);
        }

        protected bool CanPlaceBarrier(float barrierLikelihood)
        {
            return Random.Range(0, 1f) <= barrierLikelihood;
        }
        #endregion
    }
}
