using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public class EndlessWorld : MonoBehaviour
    {
        #region Public Variables
        public List<EndlessRowPrefab> endlessRowPrefabs = new List<EndlessRowPrefab>();
        public List<string> prefabOrder = new List<string>();
        public Transform startingSpawn;
        #endregion

        #region Private Variables
        EndlessRowPrefab currentRow;
        int prefabIndex;
        EndlessRowPrefab defaultPrefab;
        Transform trans;
        #endregion

        #region Event Functions
        void Awake()
        {
            trans = this.transform;
            if(startingSpawn == null)
            {
                startingSpawn = gameObject.transform;
            }
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
            Message.AddListener<CreateSectionRow>(OnCreateSectionRow);
            Message.AddListener<GameStatusResponse>(OnGameStatusResponse);
            Message.AddListener<EndlessItemSpawn>(OnEndlessItemSpawn);
        }

        void OnDestroy()
        {
            Message.RemoveListener<CreateSectionRow>(OnCreateSectionRow);
            Message.RemoveListener<GameStatusResponse>(OnGameStatusResponse);
            Message.RemoveListener<EndlessItemSpawn>(OnEndlessItemSpawn);
        }
        #endregion

        #region Listener Functions
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

        void OnGameStatusResponse(GameStatusResponse response)
        {
            if(response.status == GameStatus.Restart || response.status == GameStatus.Exit)
            {
                foreach(EndlessRowPrefab prefab in endlessRowPrefabs)
                {
                    if(prefab.row != null)
                    {
                        Destroy(prefab.row.gameObject);
                        prefab.row = null;
                    }
                }
                currentRow = null;
                prefabIndex = 0;
            }
        }

        void OnEndlessItemSpawn(EndlessItemSpawn spawn)
        {
            spawn.item.transform.parent = trans;
        }
        #endregion

        #region Private Functions
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
        #endregion
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