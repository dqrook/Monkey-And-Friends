using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class EndlessRunway : MonoBehaviour
    {
        #region Public Variables
        public EndlessMap map;
        public Transform nextSpawn;

        [Header("Camera")]
        public int initialClipPlane;
        public int initialFieldOfView;
        public Transform initialCameraSpawn;
        #endregion

        #region Private Variables
        EndlessDragon dragon;
        Transform dragonTrans;
        Camera mainCamera;
        Transform camTrans;
        int gameClipPlane;
        int gameFieldOfView;
        IEnumerator run;
        GameStatus status = GameStatus.MainMenu;
        #endregion

        #region Event Functions
        void Awake()
        {
            Message.AddListener<WorldItemsResponse>(OnWorldItemsResponse);
            Message.AddListener<ControllersResponse>(OnControllersResponse);
            Message.AddListener<GameStatusResponse>(OnGameStatusResponse);
            Message.Send(new WorldItemsRequest());
            Message.Send(new ControllersRequest());
        }

        void OnDestroy()
        {
            Message.RemoveListener<WorldItemsResponse>(OnWorldItemsResponse);
            Message.RemoveListener<ControllersResponse>(OnControllersResponse);
            Message.RemoveListener<GameStatusResponse>(OnGameStatusResponse);
        }
        #endregion

        #region Listener Functions
        void OnWorldItemsResponse(WorldItemsResponse response)
        {
            mainCamera = response.mainCamera;
            camTrans = mainCamera.transform;
            gameClipPlane = response.gameClipPlane;
            gameFieldOfView = response.gameFieldOfView;
        }

        void OnControllersResponse(ControllersResponse response)
        {
            dragon = response.dragon;
            dragonTrans = dragon.transform;
        }

        void OnGameStatusResponse(GameStatusResponse response)
        {
            status = response.status;
        }
        #endregion

        #region Public Functions
        public void Run()
        {
            float initialX = Mathf.Abs(dragonTrans.position.x - nextSpawn.position.x);
            float initialZ = Mathf.Abs(dragonTrans.position.z - nextSpawn.position.z);
            float initialDistance = initialX > initialZ ? initialX : initialZ;
            bool useX = initialX > initialZ;
            run = _Run(initialDistance, useX);
            StartCoroutine(run);
        }

        public void CrossedLine(RunwayLineType type) {}
        #endregion

        #region Private Functions
        float GetCurrentDistance(bool useX)
        {
            return useX ? Mathf.Abs(dragonTrans.position.x - nextSpawn.position.x) : Mathf.Abs(dragonTrans.position.z - nextSpawn.position.z);
        }
        #endregion


        #region Coroutines
        IEnumerator _Run(float initialDistance, bool useX)
        {
            while(dragon == null || mainCamera == null)
            {
                yield return null;
            }
            dragon.Fly();
            camTrans.parent = dragon.transform;
            camTrans.position = initialCameraSpawn.position;
            camTrans.rotation = initialCameraSpawn.rotation;
            Vector3 initialPos = camTrans.localPosition;
            Vector3 finalPos = dragon.localCameraSpawn.localPosition;
            Quaternion initialRot = camTrans.localRotation;
            Quaternion finalRot = dragon.localCameraSpawn.localRotation;

            mainCamera.farClipPlane = initialClipPlane;
            mainCamera.fieldOfView = initialFieldOfView;
            float fraction = GetCurrentDistance(useX) / initialDistance;
            float initialMultiplier = 0.5f;
            float maxLerpTime = 1;
            float lerpTime = 0;
            float cutoff = 0.7f;
            float denom = cutoff - 0.1f;
            while(fraction > 0.1f)
            {
                fraction = GetCurrentDistance(useX) / initialDistance;
                if(fraction < cutoff)
                {
                    float multiplier = initialMultiplier + (1 - initialMultiplier) * (cutoff - fraction) / denom;
                    multiplier *= 1.1f;
                    multiplier = multiplier < 0 ? multiplier : multiplier > 1 ? 1 : multiplier;
                    dragon.MoveWithMultiplier(multiplier);
                    lerpTime += Time.deltaTime;
                    float lerpFraction = lerpTime / maxLerpTime;
                    if(lerpFraction < 0.9)
                    {
                        mainCamera.farClipPlane = Mathf.Lerp(initialClipPlane, gameClipPlane, lerpFraction);
                        mainCamera.fieldOfView = Mathf.Lerp(initialFieldOfView, gameFieldOfView, lerpFraction);
                        camTrans.localPosition = Vector3.Lerp(initialPos, finalPos, lerpFraction);
                        camTrans.localRotation = Quaternion.Lerp(initialRot, finalRot, lerpFraction);
                    }
                    else
                    {
                        mainCamera.farClipPlane = Mathf.Lerp(mainCamera.farClipPlane, gameClipPlane, Time.deltaTime * 5);
                        mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, gameFieldOfView, Time.deltaTime * 5);
                        camTrans.localPosition = Vector3.Lerp(camTrans.localPosition, finalPos, Time.deltaTime * 5);
                        camTrans.localRotation = Quaternion.Lerp(camTrans.localRotation, finalRot, Time.deltaTime * 5);
                    }
                }
                else
                {
                    dragon.MoveWithMultiplier(initialMultiplier);
                }
                yield return null;
            }
            dragon.MoveWithMultiplier(1);
            mainCamera.farClipPlane = gameClipPlane;
            mainCamera.fieldOfView = gameFieldOfView;
            Debug.Log("distance diff: " + Vector3.Distance(camTrans.localPosition, finalPos));
            camTrans.localPosition = finalPos;
            camTrans.localRotation = finalRot;

            Message.Send(new StartGame());
            // while(status != GameStatus.Active)
            // {
            //     camTrans.position = dragon.localCameraSpawn.position;
            //     camTrans.rotation = dragon.localCameraSpawn.rotation;
            //     yield return null;
            // }
            camTrans.parent = null;
        }
        #endregion
    }
}
