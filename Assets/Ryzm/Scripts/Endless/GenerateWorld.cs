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
        EndlessRow currentRow;

        void Awake()
        {
            if(startingSpawn == null)
            {
                startingSpawn = gameObject.transform;
            }
            Message.AddListener<CreateSectionRow>(OnCreateSectionRow);
        }

        void OnDestroy()
        {
            Message.RemoveListener<CreateSectionRow>(OnCreateSectionRow);
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
        }
    }
}

// void RunDummy()
// {
//     GameObject newSection = EndlessPool.Instance.GetRandomSection();
//     if(newSection == null) return;
    
//     if(lastSpawnedSection != null)
//     {
//         int move = newSection.GetComponent<EndlessSection>().spawnDistance;
//         dummyTransform.position = lastSpawnedSection.transform.position + runnerTrans.forward * move;
//         if(lastSpawnedSection.tag == "stairsUp")
//         {
//             dummyTransform.Translate(0, 5, 0);
//         }
//         else if (lastSpawnedSection.tag == "stairsDown")
//         {
//             dummyTransform.Translate(0, -5, 0);
//         }
//     }

//     lastSpawnedSection = newSection;
//     newSection.transform.position = dummyTransform.position;
//     newSection.transform.rotation = dummyTransform.rotation;
//     newSection.SetActive(true);
// }