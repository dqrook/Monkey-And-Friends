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
        public Transform endTransform;
        public int gameClipPlane = 50;
        #endregion

        #region Private Variables
        GameStatus gameStatus;
        IEnumerator rotateCamera;
        Transform cameraTrans;
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
            }
            else if(gameStatus == GameStatus.Starting)
            {
                rotateCamera = null;
                rotateCamera = RotateCamera(endTransform);
                StartCoroutine(rotateCamera);
                endlessCamera.cam.farClipPlane = gameClipPlane;
            }
            else if(gameStatus == GameStatus.Restart)
            {
                cameraTrans.position = startTransform.position;
                cameraTrans.rotation = startTransform.rotation;
            }
        }

        void OnCameraRequest(CameraRequest request)
        {
            Message.Send(new CameraResponse(endlessCamera.gameObject));
        }
        #endregion


        #region Private Functions
        float GetTotalDifference(Transform target)
        {
            float posDiff = Vector3.Distance(cameraTrans.position, target.position);
            float rotDiff = Vector3.Distance(cameraTrans.eulerAngles, target.eulerAngles);
            return posDiff + rotDiff;
        }
        #endregion

        #region Coroutines
        IEnumerator RotateCamera(Transform target)
        {
            float diff = GetTotalDifference(target);
            while(diff > 0.1f)
            {
                cameraTrans.position = Vector3.Lerp(cameraTrans.position, target.position, Time.deltaTime * 2f);
                cameraTrans.rotation = Quaternion.Lerp(cameraTrans.rotation, target.rotation, Time.deltaTime * 2f);
                diff = GetTotalDifference(target);
                yield return null;
            }
            cameraTrans.position = target.position;
            cameraTrans.rotation = target.rotation;
            Message.Send(new StartGame());
            yield break;
        }
        #endregion
    }
}
