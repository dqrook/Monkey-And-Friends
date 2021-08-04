﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;
using Ryzm.UI;
using Ryzm.UI.Messages;

namespace Ryzm.EndlessRunner
{
    public class EndlessRunway : MonoBehaviour
    {
        #region Public Variables
        public EndlessMap map;
        public Transform nextRowSpawn;
        public Transform nextSpawn;
        public float deactivationTime = 1;
        public EndlessSection section;

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
        WaitForSeconds deactivationWaitForSeconds;
        IEnumerator waitAndDeactivate;
        List<MenuType> activeMenus = new List<MenuType>();
        Vector3 currentSectionPos;
        #endregion

        #region Event Functions
        void Awake()
        {
            Message.AddListener<WorldItemsResponse>(OnWorldItemsResponse);
            Message.AddListener<ControllersResponse>(OnControllersResponse);
            Message.AddListener<GameStatusResponse>(OnGameStatusResponse);
            Message.AddListener<MenuSetResponse>(OnMenuSetResponse);
            Message.Send(new WorldItemsRequest());
            Message.Send(new ControllersRequest());
            Message.Send(new MenuSetRequest(MenuSet.ActiveMenu));
            deactivationWaitForSeconds = new WaitForSeconds(deactivationTime);
        }

        void OnDestroy()
        {
            Message.RemoveListener<WorldItemsResponse>(OnWorldItemsResponse);
            Message.RemoveListener<ControllersResponse>(OnControllersResponse);
            Message.RemoveListener<GameStatusResponse>(OnGameStatusResponse);
            Message.RemoveListener<MenuSetResponse>(OnMenuSetResponse);
        }
        #endregion

        #region Listener Functions
        void OnWorldItemsResponse(WorldItemsResponse response)
        {
            mainCamera = response.mainCamera;
            camTrans = mainCamera.transform;
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

        void OnMenuSetResponse(MenuSetResponse response)
        {
            activeMenus = response.menus;
        }
        #endregion

        #region Public Functions
        public void Run(int gameClipPlane)
        {
            this.gameClipPlane = gameClipPlane;
            float initialX = Mathf.Abs(dragonTrans.position.x - nextSpawn.position.x);
            float initialZ = Mathf.Abs(dragonTrans.position.z - nextSpawn.position.z);
            float initialDistance = initialX > initialZ ? initialX : initialZ;
            bool useX = initialX > initialZ;
            run = _Run(initialDistance, useX);
            StartCoroutine(run);
            currentSectionPos = section.GetPosition(1).position;
        }

        public void CrossedLine(RunwayLineType type)
        {
            if(type == RunwayLineType.End)
            {
                waitAndDeactivate = WaitAndDeactivate();
                StartCoroutine(waitAndDeactivate);
            }
        }
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
            float maxLerpTime = 0.75f;
            float lerpTime = 0;
            float cutoff = 0.7f;
            float denom = cutoff - 0.1f;
            bool activatedMenus = false;
            
            while(fraction > 0.1f)
            {
                fraction = GetCurrentDistance(useX) / initialDistance;
                // finalPos.x = dragonTrans.InverseTransformPoint(currentSectionPos).x;
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
            if(!activatedMenus)
            {
                activatedMenus = true;
                Message.Send(new ActivateMenu(activeMenus));
                if(section != null)
                {
                    Message.Send(new CurrentSectionChange(section.gameObject));
                }
            }
            mainCamera.farClipPlane = gameClipPlane;
            mainCamera.fieldOfView = gameFieldOfView;
            camTrans.localPosition = finalPos;
            camTrans.localRotation = finalRot;
            camTrans.parent = null;
            float timer = 0;
            while(timer < 2 * Time.deltaTime)
            {
                timer += Time.deltaTime;
                yield return null;
            }
            Message.Send(new StartGame());
        }

        IEnumerator WaitAndDeactivate()
        {
            yield return deactivationWaitForSeconds;
            gameObject.SetActive(false);
        }
        #endregion
    }
}
