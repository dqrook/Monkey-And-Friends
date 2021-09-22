using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class EndlessCameraManager : MonoBehaviour
    {
        #region Public Variables
        public EndlessCamera endlessCamera;
        public CameraCull[] culls;
        public Transform startTransform;
        public int startClipPlane = 1000;
        public int mainMenuFieldOfView = 50;
        public List<Vector3> cameraLocations = new List<Vector3>();
        public EndlessCameraSpawns cameraSpawns;
        public int currentCameraSpawn;
        #endregion

        #region Private Variables
        GameStatus gameStatus;
        Transform cameraTrans;
        bool initializedCamera;
        #endregion

        #region Event Functions
        void Awake()
        {
            Message.AddListener<GameStatusResponse>(OnGameStatusResponse);
            Message.AddListener<CameraRequest>(OnCameraRequest);
            Message.AddListener<CameraSpawnsRequest>(OnCameraSpawnsRequest);
            Message.AddListener<LocalCameraSpawnRequest>(OnLocalCameraSpawnRequest);
            Message.AddListener<UpdateCurrentCameraSpawn>(OnUpdateCurrentCameraSpawn);
            cameraTrans = endlessCamera.gameObject.transform;
        }

        void Start()
        {
            float[] distances = new float[32];
            foreach (CameraCull c in culls)
            {
                distances[c.layer] = c.distance;
            }
            endlessCamera.cam.layerCullDistances = distances;
        }

        void OnDestroy()
        {
            Message.RemoveListener<GameStatusResponse>(OnGameStatusResponse);
            Message.RemoveListener<CameraRequest>(OnCameraRequest);
            Message.RemoveListener<CameraSpawnsRequest>(OnCameraSpawnsRequest);
            Message.RemoveListener<LocalCameraSpawnRequest>(OnLocalCameraSpawnRequest);
            Message.RemoveListener<UpdateCurrentCameraSpawn>(OnUpdateCurrentCameraSpawn);
        }
        #endregion

        #region Listener Functions
        void OnGameStatusResponse(GameStatusResponse response)
        {
            gameStatus = response.status;
            if(gameStatus == GameStatus.MainMenu || gameStatus == GameStatus.Exit)
            {
                cameraTrans.position = startTransform.position;
                cameraTrans.rotation = startTransform.rotation;
                endlessCamera.cam.farClipPlane = startClipPlane;
                initializedCamera = false;
                if(gameStatus == GameStatus.MainMenu)
                {
                    endlessCamera.cam.fieldOfView = mainMenuFieldOfView;
                }
            }
        }

        void OnCameraRequest(CameraRequest request)
        {
            Message.Send(new CameraResponse(endlessCamera.gameObject));
        }

        void OnCameraSpawnsRequest(CameraSpawnsRequest request)
        {
            Message.Send(new CameraSpawnsResponse(cameraSpawns));
        }

        void OnLocalCameraSpawnRequest(LocalCameraSpawnRequest request)
        {
            UpdateCameraSpawn();
        }

        void OnUpdateCurrentCameraSpawn(UpdateCurrentCameraSpawn update)
        {
            cameraSpawns.currentCameraSpawn = update.currentCameraSpawn;
            UpdateCameraSpawn();
        }
        #endregion

        #region Private Functions
        void UpdateCameraSpawn()
        {
            CameraSpawn camSpawn = cameraSpawns.CurrentCameraSpawn;
            Message.Send(new LocalCameraSpawnResponse(camSpawn));
        }
        #endregion
    }

    [System.Serializable]
    public struct CameraCull
    {
        public int layer;
        public float distance;
    }
}
