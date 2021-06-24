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
        public Transform singleDragonCameraPivot;
        public Transform singleDragonCameraXform;

        [Header("Dragon 1")]
        public Transform dragon1Spawn;
        public GameObject dragon1Panel;
        public GameObject dragon1ResetCameraButton;

        [Header("Dragon 2")]
        public Transform dragon2Spawn;
        public GameObject dragon2Panel;
        public GameObject dragon2ResetCameraButton;

        [Header("Single Dragon")]
        public Transform singleDragonSpawn;
        public GameObject singleDragonPanel;
        public GameObject singleDragonResetCameraButton;

        EndlessDragon[] dragons;
        List<MenuType> mainMenus  = new List<MenuType>();
        Transform camTrans;
        bool initialized;
        IEnumerator moveCamera;
        bool movingCamera;
        EndlessDragon dragon1;
        EndlessDragon dragon2;
        int newDragonId;
        int dragon1Index;
        int dragon2Index;

        public override bool IsActive
        {
            get
            {
                return base.IsActive;
            }
            set
            {
                if(ShouldUpdate(value))
                {
                    if(value)
                    {
                        Reset();
                        Message.AddListener<DragonsResponse>(OnDragonsResponse);
                        Message.AddListener<MenuSetResponse>(OnMenuSetResponse);
                        Message.AddListener<CameraResponse>(OnCameraResponse);
                        Message.AddListener<BreedDragonsResponse>(OnBreedDragonsResponse);
                        Message.AddListener<DragonInitialized>(OnDragonInitialized);
                        
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
                        Message.RemoveListener<BreedDragonsResponse>(OnBreedDragonsResponse);
                        Message.RemoveListener<CameraResponse>(OnCameraResponse);
                        Message.RemoveListener<DragonInitialized>(OnDragonInitialized);
                        // dragons.Clear();
                        dragons = new EndlessDragon[0];
                    }
                    base.IsActive = value;
                }
            }
        }

        void Reset()
        {
            StopAllCoroutines();
            zoomPanel.SetActive(false);
            dragon1Panel.SetActive(false);
            dragon2Panel.SetActive(false);
            breedingPanel.SetActive(false);
            singleDragonPanel.SetActive(false);
            moveCamera = null;
            initialized = false;
            movingCamera = false;
            breedingPanelText.text = "Are you sure?";
            newDragonId = -1;
            dragon1Index = 0;
            dragon2Index = 1;
        }

        void OnDragonsResponse(DragonsResponse response)
        {
            if(response.sender == "breedingMenu")
            {
                dragons = response.dragons;
                InitializeDragons();
                // InitializeMenus();
                // if(dragons.Count > 1)
                // {
                //     dragon1 = dragons[0];
                //     dragon2 = dragons[1];
                //     UpdateDragons();
                // }
            }
            else if(response.sender == "newDragon")
            {
                dragons = response.dragons;
            }
        }

        void InitializeMenus()
        {
            if(dragons.Length > 1)
            {
                noDragonsPanel.SetActive(false);
                mainPanel.SetActive(true);
                arrowsPanel.SetActive(true);
                
                if(dragons.Length > 2)
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

        void InitializeDragons()
        {
            if(dragons.Length > 1)
            {
                noDragonsPanel.SetActive(false);
                mainPanel.SetActive(true);
                arrowsPanel.SetActive(true);
                dragon1Index = 0;
                dragon2Index = 1;
                // dragon1 = dragons[0];
                // dragon2 = dragons[1];
                UpdateDragons();
                if(dragons.Length == 2)
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

        void OnMenuSetResponse(MenuSetResponse response)
        {
            if(response.set == MenuSet.MainMenu)
            {
                mainMenus = response.menus;
            }
        }

        void OnCameraResponse(CameraResponse response)
        {
            Debug.Log("camera response");
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
                MoveCamera(dragon1CameraXform);
            }
            else
            {
                dragon2Panel.SetActive(true);
                camTrans.parent = dragon2CameraPivot;
                MoveCamera(dragon2CameraXform);
            }
        }

        void UpdateDragons()
        {
            // availableDragons.Clear();
            dragon1 = dragons[dragon1Index];
            dragon2 = dragons[dragon2Index];
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
                    // availableDragons.Add(dragon);
                }
            }
        }

        public void NextDragon(int dragon)
        {
            if(dragons.Length > 2)
            {
                if(dragon == 1)
                {
                    dragon1Index++;
                    if(dragon1Index == dragon2Index)
                    {
                        dragon1Index++;
                    }
                    if(dragon1Index > dragons.Length - 1)
                    {
                        dragon1Index = dragon2Index == 0 ? 1 : 0;
                    }
                }
                else
                {
                    dragon2Index++;
                    if(dragon2Index == dragon1Index)
                    {
                        dragon2Index++;
                    }
                    if(dragon2Index > dragons.Length - 1)
                    {
                        dragon2Index = dragon1Index == 0 ? 1 : 0;
                    }
                }
                UpdateDragons();
            }
        }

        public void PreviousDragon(int dragon)
        {
            if(dragons.Length > 2)
            {
                if(dragon == 1)
                {
                    dragon1Index--;
                    if(dragon1Index == dragon2Index)
                    {
                        dragon1Index--;
                    }
                    if(dragon1Index < 0)
                    {
                        dragon1Index = dragon2Index == dragons.Length - 1 ? dragons.Length - 2 : dragons.Length - 1;
                    }
                }
                else
                {
                    dragon2Index--;
                    if(dragon2Index == dragon1Index)
                    {
                        dragon2Index--;
                    }
                    if(dragon2Index < 0)
                    {
                        dragon2Index = dragon1Index == dragons.Length - 1 ? dragons.Length - 2 : dragons.Length - 1;
                    }
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
            MoveCamera(breedingCameraXform);
        }

        void InitializeCamera(Transform cam)
        {
            camTrans = cam;
            if(!initialized)
            {
                initialized = true;
                MoveCamera(breedingCameraXform);
            }
        }

        void MoveCamera(Transform target)
        {
            if(moveCamera != null)
            {
                StopCoroutine(moveCamera);
            }
            moveCamera = null;
            moveCamera = _MoveCamera(target);
            StartCoroutine(moveCamera);
        }

        IEnumerator _MoveCamera(Transform target)
        {
            Debug.Log("moving camera");
            movingCamera = true;
            Vector3 endPos = target.position;
            Quaternion endRot = target.rotation;
            float distance = Vector3.Distance(camTrans.position, endPos);
            float rotDiff = Mathf.Abs(Quaternion.Angle(camTrans.rotation, endRot));
            while(distance > 0.1f || rotDiff > 1)
            {
                camTrans.position = Vector3.Lerp(camTrans.position, endPos, Time.deltaTime);
                camTrans.rotation = Quaternion.Lerp(camTrans.rotation, endRot, Time.deltaTime);
                distance = Vector3.Distance(camTrans.position, endPos);
                rotDiff = Mathf.Abs(Quaternion.Angle(camTrans.rotation, endRot));
                yield return null;
            }
            // camTrans.position = target.position;
            // camTrans.rotation = target.rotation;
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
            backButton.SetActive(true);
            confirmBreedButton.SetActive(true);
            breedingPanelText.text = "Are you sure?";
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
            if(response.status == BreedingStatus.Failed)
            {
                breedingPanelText.text = "Unable to breed, please try again later";
                closeBreedingPanelButton.SetActive(true);
            }
            else if (response.status == BreedingStatus.Success)
            {
                newDragonId = response.dragonId;
            }
        }

        void OnDragonInitialized(DragonInitialized initialized)
        {
            if(newDragonId == initialized.id)
            {
                breedingPanel.SetActive(false);
                singleDragonPanel.SetActive(true);
                foreach(EndlessDragon dragon in dragons)
                {
                    if(dragon.data.id == newDragonId)
                    {
                        dragon.transform.position = singleDragonSpawn.position;
                        dragon.transform.rotation = singleDragonSpawn.rotation;
                        dragon.gameObject.SetActive(true);
                    }
                    else
                    {
                        dragon.gameObject.SetActive(false);
                    }
                }
                camTrans.parent = singleDragonCameraPivot;
                MoveCamera(singleDragonCameraXform);
            }
        }

        public void CloseSingleDragonPanel()
        {
            camTrans.parent = null;
            singleDragonPanel.SetActive(false);
            backButton.SetActive(true);
            MoveCamera(breedingCameraXform);
            InitializeDragons();
        }

        public void ExitMenu()
        {
            Message.Send(new ActivateMenu(activatedTypes: mainMenus));
            Message.Send(new DeactivateMenu(activatedTypes: mainMenus));
        }
    }
}
