using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public class EndlessWorld : MonoBehaviour
    {
        public List<EndlessRowPrefab> endlessRowPrefabs = new List<EndlessRowPrefab>();
        public List<string> prefabOrder = new List<string>();
        public Transform startingSpawn;
        EndlessRowPrefab currentRow;
        int prefabIndex;
        EndlessRowPrefab defaultPrefab;

        void Awake()
        {
            if(startingSpawn == null)
            {
                startingSpawn = gameObject.transform;
            }
            Message.AddListener<CreateSectionRow>(OnCreateSectionRow);
            if(prefabOrder.Count == 0)
            {
                prefabOrder.Add("default");
            }
            foreach(EndlessRowPrefab prefab in endlessRowPrefabs)
            {
                if(prefab.Id == "default")
                {
                    defaultPrefab = prefab;
                    break;
                }
            }
        }

        void OnDestroy()
        {
            Message.RemoveListener<CreateSectionRow>(OnCreateSectionRow);
        }

        void OnCreateSectionRow(CreateSectionRow createSectionRow)
        {
            Transform spawnTransform = startingSpawn;
            if(currentRow != null && currentRow.row != null)
            {
                spawnTransform = currentRow.row.FinalSpawn();
            }
            string prefabType = prefabOrder[prefabIndex];
            
            // cant place the same row one after each other
            if(currentRow != null && prefabType == currentRow.Id)
            {
                prefabType = "default";
            }
            currentRow = GetPrefab(prefabType);
            
            if(currentRow.Id == "default" || currentRow.row == null)
            {
                currentRow.row = GameObject.Instantiate(currentRow.rowPrefab).GetComponent<EndlessRow>();
            }

            currentRow.row.transform.position = spawnTransform.position;
            currentRow.row.transform.rotation = spawnTransform.rotation;
            currentRow.row.Initialize(createSectionRow.numberOfSections);
            prefabIndex = prefabIndex < prefabOrder.Count - 1 ? prefabIndex + 1 : 0;
        }

        EndlessRowPrefab GetPrefab(string type)
        {
            if(type == "default")
            {
                return defaultPrefab;
            }
            foreach(EndlessRowPrefab fab in endlessRowPrefabs)
            {
                if(fab.Id == type)
                {
                    return fab;
                }
            }
            return defaultPrefab;
        }
    }

    [System.Serializable]
    public class EndlessRowPrefab 
    {
        public EndlessRow rowPrefab;
        [HideInInspector]
        public EndlessRow row;

        public string Id
        {
            get
            {
                return rowPrefab.id;
            }
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