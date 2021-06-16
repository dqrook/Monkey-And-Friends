using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.Dragon.Messages;
using Ryzm.Dragon;
using Ryzm.EndlessRunner.Messages;
using TMPro;

namespace Ryzm.EndlessRunner.UI
{
    public class BreedingMenu : BaseMenu
    {
        [Header("Main")]
        public GameObject backButton;
        public GameObject noDragonsPanel;
        public GameObject mainPanel;
        public GameObject zoomPanel;

        [Header("Breeding")]
        public GameObject breedingPanel;
        public GameObject confirmBreedButton;
        public GameObject closeBreedingPanelButton;
        public TextMeshProUGUI breedingPanelText;

        [Header("Arrows")]
        public GameObject arrowsPanel;
        public GameObject dragon1BackButton;
        public GameObject dragon1FwdButton;
        public GameObject dragon2BackButton;
        public GameObject dragon2FwdButton;

        [Header("Camera")]
        public Transform breedingCameraXform;
        public Transform dragon1CameraPivot;
        public Transform dragon1CameraXform;
        public Transform dragon2CameraPivot;
        public Transform dragon2CameraXform;

        [Header("Dragon 1")]
        public Transform dragon1Spawn;
        public GameObject dragon1Panel;
        public GameObject dragon1ResetCameraButton;

        [Header("Dragon 2")]
        public Transform dragon2Spawn;
        public GameObject dragon2Panel;
        public GameObject dragon2ResetCameraButton;

        List<EndlessDragon> dragons;
        List<EndlessDragon> availableDragons;
        List<MenuType> mainMenus;
        Transform camTrans;
        bool initialized;
        IEnumerator moveCamera;
        bool movingCamera;
        EndlessDragon dragon1;
        EndlessDragon dragon2;

        public override bool IsActive
        {
            get
            {
                return base.IsActive;
            }
            set
            {
                if(value != _isActive && !disable)
                {
                    if(value)
                    {
                        Reset();
                        Message.AddListener<DragonsResponse>(OnDragonsResponse);
                        Message.AddListener<MenuSetResponse>(OnMenuSetResponse);
                        Message.AddListener<CameraResponse>(OnCameraResponse);
                        
                        Message.Send(new DragonsRequest("breedingMenu"));
                        if(mainMenus.Count == 0)
                        {
                            Message.Send(new MenuSetRequest(MenuSet.MainMenu));
                        }
                        if(camTrans == null)
                        {
                            Message.Send(new CameraRequest());
                        }
                        else 
                        {
                            InitializeCamera(camTrans);
                        }
                    }
                    else
                    {
                        Reset();
                        Message.RemoveListener<DragonsResponse>(OnDragonsResponse);
                        Message.RemoveListener<MenuSetResponse>(OnMenuSetResponse);
                        Message.RemoveListener<CameraResponse>(OnCameraResponse);
                        dragons.Clear();
                    }
                }
                base.IsActive = value;
            }
        }

        void Reset()
        {
            StopAllCoroutines();
            zoomPanel.SetActive(false);
            dragon1Panel.SetActive(false);
            dragon2Panel.SetActive(false);
            breedingPanel.SetActive(false);
            moveCamera = null;
            initialized = false;
            movingCamera = false;
            breedingPanelText.text = "Are you sure?";
        }

        void OnDragonsResponse(DragonsResponse response)
        {
            if(response.sender == "breedingMenu")
            {
                dragons = response.dragons;
                if(dragons.Count > 1)
                {
                    noDragonsPanel.SetActive(false);
                    mainPanel.SetActive(true);
                    arrowsPanel.SetActive(true);
                    dragon1 = dragons[0];
                    dragon2 = dragons[1];
                    UpdateDragons();
                    if(dragons.Count == 2)
                    {
                        dragon1BackButton.SetActive(false);
                        dragon1FwdButton.SetActive(false);
                        dragon2BackButton.SetActive(false);
                        dragon2FwdButton.SetActive(false);
                    }
                    else
                    {
                        dragon1BackButton.SetActive(true);
                        dragon1FwdButton.SetActive(true);
                        dragon2BackButton.SetActive(true);
                        dragon2FwdButton.SetActive(true);
                    }
                }
                else
                {
                    noDragonsPanel.SetActive(true);
                    mainPanel.SetActive(false);
                    arrowsPanel.SetActive(false);
                }
            }
        }

        void OnMenuSetResponse(MenuSetResponse response)
        {
            if(response.set == MenuSet.MainMenu)
            {
                mainMenus = response.menus;
            }
        }

        void OnCameraResponse(CameraResponse response)
        {
            InitializeCamera(response.camera.transform);
        }

        public void Zoom(int dragon)
        {
            backButton.SetActive(false);
            zoomPanel.SetActive(true);
            mainPanel.SetActive(false);
            if(dragon == 1)
            {
                dragon1Panel.SetActive(true);
                camTrans.parent = dragon1CameraPivot;
                MoveCamera(dragon1CameraXform, 1);
            }
            else
            {
                dragon2Panel.SetActive(true);
                camTrans.parent = dragon2CameraPivot;
                MoveCamera(dragon2CameraXform, 1);
            }
        }

        void UpdateDragons()
        {
            availableDragons.Clear();
            foreach(EndlessDragon dragon in dragons)
            {
                if(dragon == dragon1 || dragon == dragon2)
                {
                    if(dragon == dragon1)
                    {
                        dragon.transform.position = dragon1Spawn.position;
                        dragon.transform.rotation = dragon1Spawn.rotation;
                    }
                    else
                    {
                        dragon.transform.position = dragon2Spawn.position;
                        dragon.transform.rotation = dragon2Spawn.rotation;
                    }
                    dragon.gameObject.SetActive(true);
                }
                else
                {
                    dragon.gameObject.SetActive(false);
                    availableDragons.Add(dragon);
                }
            }
        }

        public void NextDragon(int dragon)
        {
            if(availableDragons.Count > 0)
            {
                if(dragon == 1)
                {
                    dragon1 = availableDragons[0];
                }
                else
                {
                    dragon2 = availableDragons[0];
                }
                UpdateDragons();
            }
        }

        public void PreviousDragon(int dragon)
        {
            if(availableDragons.Count > 0)
            {
                if(dragon == 1)
                {
                    dragon1 = availableDragons[availableDragons.Count - 1];
                }
                else
                {
                    dragon2 = availableDragons[availableDragons.Count - 1];
                }
                UpdateDragons();
            }
        }

        public void ExitZoom()
        {
            camTrans.parent = null;
            zoomPanel.SetActive(false);
            mainPanel.SetActive(true);
            dragon1Panel.SetActive(false);
            dragon2Panel.SetActive(false);
            backButton.SetActive(true);
            MoveCamera(breedingCameraXform, 1);
        }

        void InitializeCamera(Transform cam)
        {
            camTrans = cam;
            if(!initialized)
            {
                initialized = true;
                MoveCamera(breedingCameraXform, 2);
            }
        }

        void MoveCamera(Transform target, float timeToMove)
        {
            if(moveCamera != null)
            {
                StopCoroutine(moveCamera);
            }
            moveCamera = null;
            moveCamera = _MoveCamera(target, timeToMove);
        }

        IEnumerator _MoveCamera(Transform target, float timeToMove)
        {
            movingCamera = true;
            Vector3 startPos = camTrans.position;
            Quaternion startRot = camTrans.rotation;
            Vector3 endPos = target.position;
            Quaternion endRot = target.rotation;
            float distance = Vector3.Distance(camTrans.position, endPos);
            float rotDiff = Mathf.Abs(Quaternion.Angle(camTrans.rotation, endRot));
            float t = 0;
            while(distance > 0.1f || rotDiff > 1)
            {
                t += Time.deltaTime;
                camTrans.position = Vector3.Lerp(startPos, endPos, t / timeToMove);
                camTrans.rotation = Quaternion.Lerp(startRot, endRot, t / timeToMove);
                distance = Vector3.Distance(camTrans.position, endPos);
                rotDiff = Mathf.Abs(Quaternion.Angle(camTrans.rotation, endRot));
                yield return null;
            }
            camTrans.position = target.position;
            camTrans.rotation = target.rotation;
            movingCamera = false;
            yield break;
        }

        public void OpenBreedingPanel()
        {
            confirmBreedButton.SetActive(true);
            breedingPanel.SetActive(true);
            arrowsPanel.SetActive(false);
            mainPanel.SetActive(false);
        }

        public void CloseBreedingPanel()
        {
            breedingPanel.SetActive(false);
            arrowsPanel.SetActive(true);
            mainPanel.SetActive(true);
        }

        public void BreedDragons()
        {
            if(dragon1 != null && dragon2 != null)
            {
                Message.Send(new BreedDragonsRequest(dragon1.data.id, dragon2.data.id));
                breedingPanelText.text = "Breeding...";
                backButton.SetActive(false);
                confirmBreedButton.SetActive(false);
                closeBreedingPanelButton.SetActive(false);
            }
        }

        void OnBreedDragonsResponse(BreedDragonsResponse response)
        {
            closeBreedingPanelButton.SetActive(true);
        }

        public void ExitMenu()
        {
            Message.Send(new ActivateMenu(activatedTypes: mainMenus));
            Message.Send(new DeactivateMenu(activatedTypes: mainMenus));
        }
    }
}
