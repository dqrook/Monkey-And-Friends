using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.UI.Messages;
using CodeControl;
using Ryzm.Dragon.Messages;
using Ryzm.EndlessRunner;
using Ryzm.Dragon;
using TMPro;

namespace Ryzm.UI
{
    public class MyDragonsMenu : RyzmMenu
    {
        public Canvas noDragonsPanel;
        public Canvas arrowsPanel;

        [Header("Dragon Panel")]
        public Canvas onMarketCanvas;
        public Canvas offMarketCanvas;
        public TextMeshProUGUI dragonPrice;

        [Header("Add To Market Panel")]
        public Canvas addToMarketPanel;
        public TMP_InputField priceInput;
        public Canvas newListingPanel;
        public Canvas currentListingPanel;

        [Header("Remove From Market Panel")]
        public Canvas removeFromMarketPanel;

        [Header("Processing Panel")]
        public Canvas updatingPanel;
        public TextMeshProUGUI updateText;

        [Header("Success Panel")]
        public Canvas successPanel;
        public TextMeshProUGUI successText;

        [Header("Error Panel")]
        public Canvas errorPanel;
        public TextMeshProUGUI errorText;

        BaseDragon[] dragons;
        bool menuSetsInitialized;
        MenuSet previousMenuSet;
        List<MenuType> mainMenus  = new List<MenuType>();
        List<MenuType> marketMenus = new List<MenuType>();
        int dragonIndex;
        bool addingNewDragon;

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
                    Reset();
                    if(value)
                    {
                        Message.AddListener<DragonsResponse>(OnDragonsResponse);
                        Message.AddListener<MenuSetResponse>(OnMenuSetResponse);
                        Message.AddListener<AddDragonToMarketResponse>(OnAddDragonToMarketResponse);
                        Message.AddListener<RemoveDragonFromMarketResponse>(OnRemoveDragonFromMarketResponse);

                        Message.Send(new MoveCameraRequest(CameraTransformType.SingleDragon));
                        Message.Send(new DragonsRequest("myDragonsMenu"));
                        if(!menuSetsInitialized)
                        {
                            menuSetsInitialized = true;
                            Message.Send(new MenuSetRequest(MenuSet.MainMenu));
                            Message.Send(new MenuSetRequest(MenuSet.MarketMenu));
                        }
                    }
                    else
                    {
                        Message.RemoveListener<DragonsResponse>(OnDragonsResponse);
                        Message.RemoveListener<MenuSetResponse>(OnMenuSetResponse);
                        Message.RemoveListener<AddDragonToMarketResponse>(OnAddDragonToMarketResponse);
                        Message.RemoveListener<RemoveDragonFromMarketResponse>(OnRemoveDragonFromMarketResponse);
                    }
                    base.IsActive = value;
                }
            }
        }

        void Reset()
        {
            dragonIndex = 0;
            removeFromMarketPanel.enabled = false;
            addToMarketPanel.enabled = false;
            onMarketCanvas.enabled = false;
            offMarketCanvas.enabled = false;
            successPanel.enabled = false;
            updatingPanel.enabled = false;
            errorPanel.enabled = false;
            noDragonsPanel.enabled = false;
        }

        protected override void Awake()
        {
            base.Awake();
            Message.AddListener<PreviousMenusUpdate>(OnPreviousMenusUpdate);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Message.RemoveListener<PreviousMenusUpdate>(OnPreviousMenusUpdate);
        }

        void OnPreviousMenusUpdate(PreviousMenusUpdate update)
        {
            previousMenuSet = update.set;
        }

        void OnMenuSetResponse(MenuSetResponse response)
        {
            if(response.set == MenuSet.MainMenu)
            {
                mainMenus = response.menus;
            }
            else if(response.set == MenuSet.MarketMenu)
            {
                marketMenus = response.menus;
            }
        }

        void OnDragonsResponse(DragonsResponse response)
        {
            if(response.sender == "myDragonsMenu")
            {
                dragons = response.dragons;
                UpdateDragons();
            }
            else if(response.sender == "marketUpdate")
            {
                dragons = response.dragons;
            }
        }

        void OnAddDragonToMarketResponse(AddDragonToMarketResponse response)
        {
            if(response.status == TransactionStatus.Success)
            {
                successText.text = addingNewDragon ? "Successfully added dragon to market for " + response.price + " Near" : "Successfully updated price to " + priceInput + " Near";
                successPanel.enabled = true;
                errorPanel.enabled = false;
                updatingPanel.enabled = false;
            }
            else if(response.status == TransactionStatus.Failed)
            {
                errorText.text = addingNewDragon ? "Unable to add dragon to market" : "Unable to update the dragon's price";
                successPanel.enabled = false;
                errorPanel.enabled = true;
                updatingPanel.enabled = false;
            }
        }

        void OnRemoveDragonFromMarketResponse(RemoveDragonFromMarketResponse response)
        {
            if(response.status == TransactionStatus.Success)
            {
                successText.text = "Successfully removed dragon from market";
                successPanel.enabled = true;
                errorPanel.enabled = false;
                updatingPanel.enabled = false;
            }
            else if(response.status == TransactionStatus.Failed)
            {
                errorText.text = "Unable to remove dragon from market";
                successPanel.enabled = false;
                errorPanel.enabled = true;
                updatingPanel.enabled = false;
            }
        }

        void UpdateDragons()
        {
            noDragonsPanel.enabled = dragons.Length == 0;
            arrowsPanel.enabled = dragons.Length > 1;
            BaseDragon _dragon = CurrentDragon();
            if(_dragon != null)
            {
                int dragonId = _dragon.data.id;
                Debug.Log(dragonId + " " + _dragon.ForSale);
                Message.Send(new DragonTransformUpdate(dragonId, DragonSpawnType.SingleDragon));
                onMarketCanvas.enabled = _dragon.ForSale;
                offMarketCanvas.enabled = !_dragon.ForSale;
                if(_dragon.ForSale)
                {
                    dragonPrice.text = _dragon.Price.ToString() + " Near";
                }
                foreach(BaseDragon dragon in dragons)
                {
                    dragon.gameObject.SetActive(dragon.data.id == dragonId);
                }
            }
        }

        BaseDragon CurrentDragon()
        {
            if(dragons.Length == 0)
            {
                return null;
            }
            BaseDragon _dragon = dragons[dragonIndex];
            return _dragon;
        }

        public void OpenAddToMarketPanel()
        {
            priceInput.text = CurrentDragon().Price.ToString();
            bool forSale = CurrentDragon().ForSale;
            currentListingPanel.enabled = forSale;
            newListingPanel.enabled = !forSale;
            addToMarketPanel.enabled = true;
        }

        public void OpenRemoveFromMarketPanel()
        {
            addToMarketPanel.enabled = false;
            removeFromMarketPanel.enabled = true;
        }
        
        public void CloseMarketPanels()
        {
            addToMarketPanel.enabled = false;
            removeFromMarketPanel.enabled = false;
        }

        public void UpdatePrice()
        {
            string priceText = priceInput.text;
            float price = -1;
            bool isNumber = float.TryParse(priceText, out price);
            if(isNumber)
            {
                Debug.Log("new price is " + price);
                if(price <= 0)
                {
                    addToMarketPanel.enabled = false;
                    OpenRemoveFromMarketPanel();
                }
                else
                {
                    int dragonId = CurrentDragon().data.id;
                    Message.Send(new AddDragonToMarketRequest(dragonId, price));
                    addingNewDragon = !CurrentDragon().ForSale;
                    updateText.text = addingNewDragon ? "Adding dragon to market for " + price + " Near" : "Updating price to " + price + " Near";
                    updatingPanel.enabled = true;
                    CloseMarketPanels();
                }
            }
            else
            {
                Debug.LogError("Gotta b a number chief");
            }
        }

        public void RemoveFromMarket()
        {
            // todo: start request
            int dragonId = CurrentDragon().data.id;
            Message.Send(new RemoveDragonFromMarketRequest(dragonId));
            updateText.text = "Removing dragon from market";
            updatingPanel.enabled = true;
            CloseMarketPanels();
        }

        public void NextDragon()
        {
            dragonIndex++;
            dragonIndex = dragonIndex < dragons.Length ? dragonIndex : 0;
            UpdateDragons();
        }

        public void PreviousDragon()
        {
            dragonIndex--;
            dragonIndex = dragonIndex >= 0 ? dragonIndex : dragons.Length - 1;
            UpdateDragons();
        }

        public void Exit()
        {
            Message.Send(new ActivateTimedLoadingMenu(1.5f));
            Message.Send(new ResetDragons());
            Message.Send(new ActivateMenu(activatedTypes: previousMenuSet == MenuSet.MainMenu ? mainMenus : marketMenus));
        }

        public void CloseStatusPanels()
        {
            updatingPanel.enabled = false;
            successPanel.enabled = false;
            errorPanel.enabled = false;
        }
    }
}
