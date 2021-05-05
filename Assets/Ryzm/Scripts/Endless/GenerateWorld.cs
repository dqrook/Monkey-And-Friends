using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public class GenerateWorld : MonoBehaviour
    {
        public EndlessRow endlessRowPrefab;
        public Transform startingSpawn;
        static public Transform dummyTransform;
        // last platform added
        static public GameObject lastSpawnedSection;
        int numberSectionsSinceBarrier;
        Transform runnerTrans;
        GameObject _currentSection;
        EndlessRow currentRow;

        void Awake()
        {
            if(startingSpawn == null)
            {
                startingSpawn = gameObject.transform;
            }
            dummyTransform = new GameObject("dummy").transform;
            Message.AddListener<CreateSection>(OnCreateSection);
            Message.AddListener<CreateBarrier>(OnCreateBarrier);
            Message.AddListener<CurrentSectionChange>(OnCurrentSectionChange);
            Message.AddListener<RunnerResponse>(OnRunnerResponse);
            Message.AddListener<CreateSectionRow>(OnCreateSectionRow);
        }

        void OnEnable()
        {
            Message.Send(new RunnerRequest());
        }

        void OnDestroy()
        {
            Message.RemoveListener<CreateSection>(OnCreateSection);
            Message.RemoveListener<CreateBarrier>(OnCreateBarrier);
            Message.RemoveListener<CurrentSectionChange>(OnCurrentSectionChange);
            Message.RemoveListener<RunnerResponse>(OnRunnerResponse);
            Message.RemoveListener<CreateSectionRow>(OnCreateSectionRow);
        }

        void OnCreateSection(CreateSection createSection)
        {
            // CreateSections(createSection.numberOfSections);
        }

        void OnCreateBarrier(CreateBarrier createBarrier)
        {
            // _CreateBarrier();
        }

        void OnCurrentSectionChange(CurrentSectionChange sectionChange)
        {
            _currentSection = sectionChange.section;
            // if(CanPlaceBarrier())
            // {
            //     _CreateBarrier();
            // }
            // else
            // {
            //     numberSectionsSinceBarrier++;
            // }
        }

        void OnCreateSectionRow(CreateSectionRow createSectionRow)
        {
            Transform spawnTransform = startingSpawn;
            if(currentRow != null)
            {
                spawnTransform = currentRow.FinalSpawn();
            }
            
            currentRow = null;
            currentRow = GameObject.Instantiate(endlessRowPrefab.gameObject).GetComponent<EndlessRow>();
            currentRow.transform.position = spawnTransform.position;
            currentRow.transform.rotation = spawnTransform.rotation;
            currentRow.Initialize(createSectionRow.numberOfSections);

            // CreateSections(createSectionRow.numberOfSections);
        }

        void CreateSections(int numberOfSections)
        {
            if(numberOfSections <= 1)
            {
                RunDummy();
            }
            else
            {
                for(int i = 0; i < numberOfSections; i++)
                {
                    RunDummy();
                }
            }
        }

        void RunDummy()
        {
            GameObject newSection = EndlessPool.Instance.GetRandomSection();
            if(newSection == null) return;
            
            if(lastSpawnedSection != null)
            {
                int move = newSection.GetComponent<EndlessSection>().spawnDistance;
                dummyTransform.position = lastSpawnedSection.transform.position + runnerTrans.forward * move;
                if(lastSpawnedSection.tag == "stairsUp")
                {
                    dummyTransform.Translate(0, 5, 0);
                }
                else if (lastSpawnedSection.tag == "stairsDown")
                {
                    dummyTransform.Translate(0, -5, 0);
                }
            }

            lastSpawnedSection = newSection;
            newSection.transform.position = dummyTransform.position;
            newSection.transform.rotation = dummyTransform.rotation;
            newSection.SetActive(true);
        }

        void _CreateBarrier()
        {
            if(_currentSection == null) 
            {
                return;
            }

            EndlessSection _section = _currentSection.GetComponent<EndlessSection>();
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

        void OnRunnerResponse(RunnerResponse response)
        {
            runnerTrans = response.runner.gameObject.transform;
        }
    }
}
