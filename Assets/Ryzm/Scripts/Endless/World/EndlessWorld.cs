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
        public Transform startingSpawn;
        public Camera mainCamera;
        public Light mainLight;
        public int gameClipPlane;
        public int gameFieldOfView;

        [Header("Map")]
        public List<EndlessMapPrefab> endlessMapPrefabs = new List<EndlessMapPrefab>();
        public List<MapType> mapOrder = new List<MapType>();
        #endregion

        #region Private Variables
        int prefabIndex;
        Transform trans;
        EndlessMapPrefab currentMap;
        #endregion

        #region Event Functions
        void Awake()
        {
            if(mainCamera == null)
            {
                mainCamera = Camera.main;
            }
            trans = this.transform;
            if(startingSpawn == null)
            {
                startingSpawn = trans;
            }
            Message.AddListener<CreateMap>(OnCreateMap);
            Message.AddListener<GameStatusResponse>(OnGameStatusResponse);
            Message.AddListener<EndlessItemSpawn>(OnEndlessItemSpawn);
            Message.AddListener<WorldItemsRequest>(OnWorldItemsRequest);
            Message.AddListener<MapSettingsRequest>(OnMapSettingsRequest);
            Message.AddListener<CurrentMapRequest>(OnCurrentMapRequest);
        }

        void OnDestroy()
        {
            Message.RemoveListener<CreateMap>(OnCreateMap);
            Message.RemoveListener<GameStatusResponse>(OnGameStatusResponse);
            Message.RemoveListener<EndlessItemSpawn>(OnEndlessItemSpawn);
            Message.RemoveListener<WorldItemsRequest>(OnWorldItemsRequest);
            Message.RemoveListener<MapSettingsRequest>(OnMapSettingsRequest);
            Message.RemoveListener<CurrentMapRequest>(OnCurrentMapRequest);
        }
        #endregion

        #region Listener Functions
        void OnCreateMap(CreateMap createMap)
        {
            Debug.Log("Create Map");
            CreateMap();
        }

        void OnGameStatusResponse(GameStatusResponse response)
        {
            if(response.status == GameStatus.Restart || response.status == GameStatus.Exit)
            {
                prefabIndex = 0;
                foreach(EndlessMapPrefab prefab in endlessMapPrefabs)
                {
                    if(prefab.map != null)
                    {
                        prefab.map.Reset();
                        Destroy(prefab.map.gameObject);
                        prefab.map = null;
                    }
                }
            }
        }

        void OnEndlessItemSpawn(EndlessItemSpawn spawn)
        {
            spawn.item.transform.parent = trans;
        }

        void OnWorldItemsRequest(WorldItemsRequest request)
        {
            Message.Send(new WorldItemsResponse(mainCamera, mainLight, gameClipPlane, gameFieldOfView));
        }

        void OnMapSettingsRequest(MapSettingsRequest request)
        {
            if(request.getCurrent)
            {
                if(currentMap != null && currentMap.map != null)
                {
                    Message.Send(new MapSettingsResponse(currentMap.settings, request.requestId));
                }
            }
            else
            {
                foreach(EndlessMapPrefab map in endlessMapPrefabs)
                {
                    if(map.Type == request.type)
                    {
                        Message.Send(new MapSettingsResponse(map.settings, request.requestId));
                        break;
                    }
                }
            }
        }

        void OnCurrentMapRequest(CurrentMapRequest request)
        {
            if(currentMap != null && currentMap.map != null)
            {
                Message.Send(new CurrentMapResponse(currentMap.Type));
            }
        }

        #endregion

        #region Private Functions
        void CreateMap()
        {
            // Transform spawnTransform = startingSpawn;
            // if(currentMap != null && currentMap.map != null)
            // {
            //     spawnTransform = currentMap.map.FinalSpawn();
            // }
            currentMap = GetMapPrefab(mapOrder[prefabIndex]);

            if(currentMap.map == null)
            {
                currentMap.map = GameObject.Instantiate(currentMap.mapPrefab).GetComponent<EndlessMap>();
            }
            currentMap.map.transform.position = currentMap.settings.mapSpawn.position;
            currentMap.map.transform.rotation = currentMap.settings.mapSpawn.rotation;
            currentMap.map.Initialize(currentMap.settings.mapSpawn);
            prefabIndex = prefabIndex < mapOrder.Count - 1 ? prefabIndex + 1 : 0;
        }

        EndlessMapPrefab GetMapPrefab(MapType type)
        {
            foreach(EndlessMapPrefab fab in endlessMapPrefabs)
            {
                if(fab.Type == type)
                {
                    return fab;
                }
            }
            return null;
        }
        #endregion
    }

    [System.Serializable]
    public struct MapSettings
    {
        public Transform mapSpawn;
        public int farClipPlane;
    }

    [System.Serializable]
    public class EndlessMapPrefab
    {
        public MapSettings settings;
        public EndlessMap mapPrefab;
        [HideInInspector]
        public EndlessMap map;

        public MapType Type
        {
            get
            {
                return mapPrefab.type;
            }
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