using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;
using AtmosphericHeightFog;

namespace Ryzm.EndlessRunner
{
    public class EndlessMap : EndlessItem
    {
        #region Public Variables
        public MapType type;
        public EndlessTransition transition;
        public HeightFogGlobal fog;
        public Transform initialDragonSpawn;
        
        [Header("Runway")]
        public EndlessRunway runway;

        [Header("Rows")]
        [Tooltip("Set to 0 or less if this map is infinite")]
        public int numberOfRowLoops;
        public List<EndlessRowPrefab> endlessRowPrefabs = new List<EndlessRowPrefab>();
        public List<string> prefabOrder = new List<string>();
        #endregion

        #region Private Variables
        EndlessRowPrefab currentRow;
        int prefabIndex;
        EndlessRowPrefab defaultPrefab;
        int numberOfRowLoopsCompleted;
        bool initialized;
        EndlessDragon dragon;
        Camera mainCamera;
        int gameClipPlane;
        bool startedRunway;
        #endregion

        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            if(transition != null)
            {
                transition.gameObject.SetActive(false);
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
            Message.AddListener<WorldItemsResponse>(OnWorldItemsResponse);
            Message.AddListener<RowComplete>(OnRowComplete);
            Message.AddListener<AddTransitionRequest>(OnAddTransitionRequest);
            Message.AddListener<ControllersResponse>(OnControllersResponse);
            Message.AddListener<StartRunway>(OnStartRunway);
            Message.Send(new WorldItemsRequest());
            Message.Send(new ControllersRequest());
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Message.RemoveListener<WorldItemsResponse>(OnWorldItemsResponse);
            Message.RemoveListener<RowComplete>(OnRowComplete);
            Message.RemoveListener<AddTransitionRequest>(OnAddTransitionRequest);
            Message.RemoveListener<ControllersResponse>(OnControllersResponse);
            Message.RemoveListener<StartRunway>(OnStartRunway);
        }
        #endregion

        #region Listener Functions
        void OnWorldItemsResponse(WorldItemsResponse response)
        {
            mainCamera = response.mainCamera;
            gameClipPlane = response.gameClipPlane;
            if(fog != null)
            {
                fog.mainCamera = response.mainCamera;
                fog.mainDirectional = response.mainLight;
                fog.gameObject.SetActive(true);
            }
        }

        void OnRowComplete(RowComplete complete)
        {
            if(currentRow != null && currentRow.row != null && currentRow.row.rowId == complete.rowId)
            {
                if(numberOfRowLoopsCompleted < numberOfRowLoops)
                {
                    // place next row
                    AddRow(currentRow.row.FinalSpawn());
                }
                else if(transition != null)
                {
                    // place the transition section
                    AddTransition(currentRow.row.FinalSpawn());
                }
            }
        }

        void OnAddTransitionRequest(AddTransitionRequest request)
        {
            if(transition != null && currentRow != null && currentRow.row != null && currentRow.row.rowId == request.rowId)
            {
                AddTransition(currentRow.row.FinalSpawn());
            }
        }

        void OnControllersResponse(ControllersResponse response)
        {
            dragon = response.dragon;
        }

        void OnStartRunway(StartRunway start)
        {
            if(start.type == type && !startedRunway)
            {
                dragon.transform.position = initialDragonSpawn.position;
                dragon.transform.rotation = initialDragonSpawn.rotation;

                if(runway != null)
                {
                    runway.Run();
                }
                else
                {
                    Message.Send(new StartGame());
                }
                startedRunway = true;
            }
        }
        #endregion

        #region Public Functions
        public void Initialize(Transform spawnTransform)
        {
            if(!initialized && runway != null)
            {
                AddRow(runway.nextSpawn);
                // runway.Run();
                // // has initial row
                // currentRow = GetPrefab(initialRowType);
                // if(currentRow.row == null)
                // {
                //     currentRow.row = GameObject.Instantiate(currentRow.rowPrefab).GetComponent<EndlessRow>();
                // }
                // currentRow.row.transform.position = spawnTransform.position;
                // currentRow.row.transform.rotation = spawnTransform.rotation;
                // currentRow.row.Initialize(5);
                // Message.Send(new RowChange(0, currentRow.row.rowId));
            }
            else
            {
                AddRow(spawnTransform);
            }
            initialized = true;
        }

        public void AddRow(Transform spawnTransform)
        {
            string prefabType = prefabOrder[prefabIndex];
            int currentRowId = currentRow == null || currentRow.row == null ? 0 : currentRow.row.rowId;
            // cant place the same row one after each other
            // if(currentRow != null && prefabType == currentRow.Id)
            // {
            //     prefabType = "default";
            // }
            currentRow = GetPrefab(prefabType);
            
            if(currentRow.Id == "default" || currentRow.row == null)
            {
                currentRow.row = GameObject.Instantiate(currentRow.rowPrefab).GetComponent<EndlessRow>();
            }

            currentRow.row.transform.position = spawnTransform.position;
            currentRow.row.transform.rotation = spawnTransform.rotation;
            currentRow.row.Initialize(5);
            if(prefabIndex > prefabOrder.Count - 1)
            {
                prefabIndex = 0;
                numberOfRowLoopsCompleted++;
            }
            else
            {
                prefabIndex = prefabIndex + 1;
            }
            
            Message.Send(new RowChange(currentRowId, currentRow.row.rowId));
        }

        public void Reset()
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
            initialized = false;
            startedRunway = false;
        }

        public Transform FinalSpawn()
        {
            if(transition != null)
            {
                return transition.nextSpawn;
            }
            else if(currentRow != null)
            {
                return currentRow.row.FinalSpawn();
            }
            return null;
        }
        #endregion

        #region Private Functions
        void AddTransition(Transform spawnTransform)
        {
            transition.transform.position = spawnTransform.position;
            transform.transform.rotation = spawnTransform.rotation;
            transition.Initialize(spawnTransform);
            transition.gameObject.SetActive(true);
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
        #endregion
    }

    public enum MapType
    {
        Floating,
        Rock
    }
}
