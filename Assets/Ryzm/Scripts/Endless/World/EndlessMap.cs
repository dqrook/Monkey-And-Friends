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
        public Transform initialDragonSpawn;

        [Header("Fog & Sky")]
        public FogType fogType = FogType.AtmosphericHeightFog;
        public HeightFogGlobal fog;
        public Color fogColor;

        [Header("Settings")]
        public int gameClipPlane = 50;
        
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
        Transform dragonTrans;
        Camera mainCamera;
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
            Message.AddListener<EnterTransition>(OnEnterTransition);
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
            Message.RemoveListener<EnterTransition>(OnEnterTransition);
        }
        #endregion

        #region Listener Functions
        void OnWorldItemsResponse(WorldItemsResponse response)
        {
            mainCamera = response.mainCamera;
            if(fogType == FogType.AtmosphericHeightFog && fog != null)
            {
                RenderSettings.fog = false;
                fog.mainCamera = response.mainCamera;
                fog.mainDirectional = response.mainLight;
                fog.gameObject.SetActive(true);
            }
            else if(fogType == FogType.BuiltIn)
            {
                RenderSettings.fog = true;
                RenderSettings.fogColor = fogColor;
                response.mainCamera.clearFlags = CameraClearFlags.SolidColor;
                response.mainCamera.backgroundColor = fogColor;
            }
            else
            {
                RenderSettings.fog = false;
                response.mainCamera.clearFlags = CameraClearFlags.Skybox;
            }
        }

        void OnRowComplete(RowComplete complete)
        {
            if(currentRow != null && currentRow.row != null && currentRow.row.rowId == complete.rowId)
            {
                if(numberOfRowLoopsCompleted < numberOfRowLoops || numberOfRowLoops == 0)
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
            dragonTrans = dragon.transform;
        }

        void OnStartRunway(StartRunway start)
        {
            runway.gameObject.SetActive(true);
            if(start.type == type && !startedRunway)
            {
                dragonTrans.position = initialDragonSpawn.position;
                dragonTrans.rotation = initialDragonSpawn.rotation;
                mainCamera.transform.position = dragon.localCameraSpawn.position;
                mainCamera.transform.rotation = dragon.localCameraSpawn.rotation;
                
                if(runway != null)
                {
                    runway.Run(gameClipPlane);
                }
                else
                {
                    mainCamera.farClipPlane = gameClipPlane;
                }
                // else
                // {
                //     Message.Send(new StartGame());
                // }
                Message.Send(new StartRunwayResponse(runway != null, type));
                startedRunway = true;
            }
        }

        void OnEnterTransition(EnterTransition enter)
        {
            AddRow(enter.nextSpawn);
        }
        #endregion

        #region Public Functions
        public void Initialize(Transform spawnTransform)
        {
            if(!initialized && runway != null)
            {
                AddRow(runway.nextRowSpawn);
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
        void AddRow(Transform spawnTransform)
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
            prefabIndex++;
            if(prefabIndex > prefabOrder.Count - 1)
            {
                prefabIndex = 0;
                numberOfRowLoopsCompleted++;
            }
            
            Message.Send(new RowChange(currentRowId, currentRow.row.rowId));
        }

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
        Water
    }

    public enum FogType
    {
        AtmosphericHeightFog,
        BuiltIn,
        None
    }
}
