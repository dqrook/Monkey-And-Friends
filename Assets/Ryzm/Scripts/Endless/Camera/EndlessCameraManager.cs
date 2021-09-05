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
        public Transform startTransform;
        public int startClipPlane = 1000;
        public int mainMenuFieldOfView = 50;
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
            cameraTrans = endlessCamera.gameObject.transform;
        }

        void OnDestroy()
        {
            Message.RemoveListener<GameStatusResponse>(OnGameStatusResponse);
            Message.RemoveListener<CameraRequest>(OnCameraRequest);
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
        #endregion
    }
}
