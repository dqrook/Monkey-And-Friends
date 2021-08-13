using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.Dragon.Messages;
using Ryzm.Dragon;
using Ryzm.UI.Messages;
using TMPro;

namespace Ryzm.UI
{
    public class BreedingMenu : RyzmMenu
    {
        #region Public Variables
        [Header("Main")]
        public GameObject backButton;
        public GameObject noDragonsPanel;
        public GameObject mainPanel;
        public GameObject breedButton;

        [Header("Zoom")]
        public GameObject zoomPanel;
        public GameObject zoomedFwdButton;
        public GameObject zoomedBackButton;
        public GameObject resetCameraButton;

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

        [Header("Dragon 1")]
        public Transform dragon1Spawn;
        public Canvas dragon1OnMarketPanel;
        public TextMeshProUGUI dragon1MarketText;
        public GameObject dragon1MarketButton;

        [Header("Dragon 2")]
        public Transform dragon2Spawn;
        public Canvas dragon2OnMarketPanel;
        public TextMeshProUGUI dragon2MarketText;
        public GameObject dragon2MarketButton;

        [Header("Single Dragon")]
        public Transform singleDragonSpawn;
        public GameObject singleDragonPanel;
        public GameObject singleDragonResetCameraButton;
        #endregion

        #region Private Variables
        BaseDragon[] dragons;
        List<MenuType> mainMenus  = new List<MenuType>();
        bool initialized;
        bool movingCamera;
        BaseDragon dragon1;
        BaseDragon dragon2;
        int newDragonId;
        int dragon1Index;
        int dragon2Index;
        int currentZoomedDragon;
        int marketDragon;
        #endregion

        #region Properties
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
                        Message.AddListener<BreedDragonsResponse>(OnBreedDragonsResponse);
                        Message.AddListener<DragonInitialized>(OnDragonInitialized);
                        Message.AddListener<RemoveDragonFromMarketResponse>(OnRemoveDragonFromMarketResponse);
                        
                        Message.Send(new DragonsRequest("breedingMenu"));
                        if(mainMenus.Count == 0)
                        {
                            Message.Send(new MenuSetRequest(MenuSet.MainMenu));
                        }
                        InitializeCamera();
                    }
                    else
                    {
                        Reset();
                        Message.RemoveListener<DragonsResponse>(OnDragonsResponse);
                        Message.RemoveListener<MenuSetResponse>(OnMenuSetResponse);
                        Message.RemoveListener<BreedDragonsResponse>(OnBreedDragonsResponse);
                        Message.RemoveListener<DragonInitialized>(OnDragonInitialized);
                        Message.RemoveListener<RemoveDragonFromMarketResponse>(OnRemoveDragonFromMarketResponse);
                        // dragons.Clear();
                        dragons = new BaseDragon[0];
                    }
                    base.IsActive = value;
                }
            }
        }
        #endregion

        #region Event Listeners
        void OnDragonsResponse(DragonsResponse response)
        {
            if(response.sender == "breedingMenu")
            {
                dragons = response.dragons;
                InitializeDragons();
            }
            else if(response.sender == "newDragon")
            {
                dragons = response.dragons;
            }
            else if(response.sender == "marketUpdate")
            {
                dragons = response.dragons;
            }
        }

        void OnMenuSetResponse(MenuSetResponse response)
        {
            if(response.set == MenuSet.MainMenu)
            {
                mainMenus = response.menus;
            }
        }

        void OnBreedDragonsResponse(BreedDragonsResponse response)
        {
            if(response.status == TransactionStatus.Failed)
            {
                breedingPanelText.text = "Unable to breed, please try again later";
                closeBreedingPanelButton.SetActive(true);
            }
            else if (response.status == TransactionStatus.Success)
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
                foreach(BaseDragon dragon in dragons)
                {
                    if(dragon.data.id == newDragonId)
                    {
                        dragon.transform.position = singleDragonSpawn.position;
                        dragon.transform.rotation = singleDragonSpawn.rotation;
                        dragon.gameObject.SetActive(true);
                        Message.Send(new EnableDragonInfoPanel(dragon.data));
                    }
                    else
                    {
                        dragon.gameObject.SetActive(false);
                    }
                }
                Message.Send(new MoveCameraRequest(CameraTransformType.SingleDragon));
            }
        }

        void OnRemoveDragonFromMarketResponse(RemoveDragonFromMarketResponse response)
        {
            if(response.status == TransactionStatus.Success)
            {
                UpdateDragonMarketPanel(marketDragon == 1, true, false, "Successfully removed dragon from market!");
                if(marketDragon == 1)
                {
                    dragon1BackButton.SetActive(true);
                    dragon2FwdButton.SetActive(true);
                }
                else
                {
                    dragon2BackButton.SetActive(true);
                    dragon2FwdButton.SetActive(true);
                }
            }
            else if(response.status == TransactionStatus.Failed)
            {
                UpdateDragonMarketPanel(marketDragon == 1, true, true, "Unable to remove dragon from market. Please try again");
            }
        }
        #endregion

        #region Public Functions
        public void Zoom(int dragon)
        {
            backButton.SetActive(false);
            zoomPanel.SetActive(true);
            mainPanel.SetActive(false);
            arrowsPanel.SetActive(false);
            DeactivateDragonMarketPanels();
            currentZoomedDragon = dragon;
            if(dragon == 1)
            {
                Message.Send(new EnableDragonInfoPanel(dragon1.data));
                Message.Send(new MoveCameraRequest(CameraTransformType.Dragon1));
            }
            else
            {
                Message.Send(new EnableDragonInfoPanel(dragon2.data));
                Message.Send(new MoveCameraRequest(CameraTransformType.Dragon2));
            }
        }

        public void ChangeZoomedDragon(bool isNext)
        {
            if(isNext)
            {
                NextDragon(currentZoomedDragon);
            }
            else
            {
                PreviousDragon(currentZoomedDragon);
            }
            if(currentZoomedDragon == 1)
            {
                Message.Send(new EnableDragonInfoPanel(dragon1.data));
            }
            else
            {
                Message.Send(new EnableDragonInfoPanel(dragon2.data));
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
            zoomPanel.SetActive(false);
            mainPanel.SetActive(true);
            backButton.SetActive(true);
            arrowsPanel.SetActive(true);
            Message.Send(new DisableDragonInfoPanel());
            Message.Send(new MoveCameraRequest(CameraTransformType.BreedingMenu));
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

        public void CloseSingleDragonPanel()
        {
            singleDragonPanel.SetActive(false);
            backButton.SetActive(true);
            Message.Send(new DisableDragonInfoPanel());
            Message.Send(new MoveCameraRequest(CameraTransformType.BreedingMenu));
            InitializeDragons();
        }

        public void ExitMenu()
        {
            Message.Send(new ActivateTimedLoadingMenu(1.5f));
            Message.Send(new ActivateMenu(activatedTypes: mainMenus));
        }

        public void RemoveDragonFromMarket(bool is1)
        {
            marketDragon = is1 ? 1 : 2;
            UpdateDragonMarketPanel(is1, true, false);
            if(is1)
            {
                dragon1FwdButton.SetActive(false);
                dragon1BackButton.SetActive(false);
            }
            else
            {
                dragon2FwdButton.SetActive(false);
                dragon2BackButton.SetActive(false);
            }
            Message.Send(new RemoveDragonFromMarketRequest(is1 ? dragon1.data.id : dragon2.data.id));
        }

        public void ResetCamera()
        {

        }
        #endregion

        #region Private Functions
        void Reset()
        {
            zoomPanel.SetActive(false);
            breedingPanel.SetActive(false);
            singleDragonPanel.SetActive(false);
            breedButton.SetActive(true);
            DeactivateDragonMarketPanels();
            initialized = false;
            movingCamera = false;
            breedingPanelText.text = "Are you sure?";
            newDragonId = -1;
            dragon1Index = 0;
            dragon2Index = 1;
            currentZoomedDragon = 0;
            marketDragon = 0;
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
                UpdateDragons();
                dragon1BackButton.SetActive(dragons.Length > 2);
                dragon1FwdButton.SetActive(dragons.Length > 2);
                dragon2BackButton.SetActive(dragons.Length > 2);
                dragon2FwdButton.SetActive(dragons.Length > 2);
                zoomedBackButton.SetActive(dragons.Length > 2);
                zoomedFwdButton.SetActive(dragons.Length > 2);
            }
            else
            {
                noDragonsPanel.SetActive(true);
                mainPanel.SetActive(false);
                arrowsPanel.SetActive(false);
            }
        }

        void UpdateDragons()
        {
            dragon1 = dragons[dragon1Index];
            dragon2 = dragons[dragon2Index];
            bool forSale = false;
            foreach(BaseDragon dragon in dragons)
            {
                if(dragon == dragon1 || dragon == dragon2)
                {
                    if(dragon == dragon1)
                    {
                        dragon.transform.position = dragon1Spawn.position;
                        dragon.transform.rotation = dragon1Spawn.rotation;
                            if(dragon.Price > 0)
                            {
                                UpdateDragonMarketPanel(true);
                                forSale = true;
                            }
                    }
                    else
                    {
                        dragon.transform.position = dragon2Spawn.position;
                        dragon.transform.rotation = dragon2Spawn.rotation;
                        if(dragon.Price > 0)
                        {
                            UpdateDragonMarketPanel(false);
                            forSale = true;
                        }
                    }
                    dragon.gameObject.SetActive(true);
                }
                else
                {
                    dragon.gameObject.SetActive(false);
                }
            }
            breedButton.SetActive(!forSale);
            if(!forSale)
            {
                DeactivateDragonMarketPanels();
            }
        }

        void DeactivateDragonMarketPanels()
        {
            UpdateDragonMarketPanel(true, false);
            UpdateDragonMarketPanel(false, false);
        }

        void UpdateDragonMarketPanel(bool is1)
        {
            UpdateDragonMarketPanel(is1, true, true, "Cannot Breed With Dragon Currently On Market");
        }

        void UpdateDragonMarketPanel(bool is1, bool setActive)
        {
            UpdateDragonMarketPanel(is1, setActive, true, "");
        }

        void UpdateDragonMarketPanel(bool is1, bool setActive, bool activateButton)
        {
            string text = activateButton ? "" : "Removing dragon from market...";
            UpdateDragonMarketPanel(is1, setActive, activateButton, text);
        }

        void UpdateDragonMarketPanel(bool is1, bool setActive, bool activateButton, string text)
        {
            if(is1)
            {
                dragon1MarketText.text = text;
                dragon1MarketButton.SetActive(activateButton);
                dragon1OnMarketPanel.enabled = setActive;

            }
            else
            {
                dragon2MarketText.text = text;
                dragon2MarketButton.SetActive(activateButton);
                dragon2OnMarketPanel.enabled = setActive;
            }
        }

        void InitializeCamera()
        {
            if(!initialized)
            {
                initialized = true;
                Message.Send(new MoveCameraRequest(CameraTransformType.BreedingMenu));
            }
        }
        #endregion
    }
}
