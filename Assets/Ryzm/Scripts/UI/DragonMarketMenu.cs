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
    public class DragonMarketMenu : RyzmMenu
    {
        [Header("Dragon Panels")]
        public List<MarketDragonPanel> dragonPanels = new List<MarketDragonPanel>();

        [Header("Loading Panel")]
        public GameObject loadingPanel;

        [Header("Page Panel")]
        public GameObject pagePanel;
        public TextMeshProUGUI pageText;
        public GameObject nextPageButton;
        public GameObject previousPageButton;

        [Header("Buy Panel")]
        public GameObject buyPanel;
        public TextMeshProUGUI buyDragonText;
        public GameObject confirmBreedButton;
        public GameObject closeBreedingPanelButton;

        [Header("Single Dragon")]
        public GameObject singleDragonPanel;
        public GameObject singleDragonResetCameraButton;

        [Header("My Dragons")]
        public GameObject myDragonsButton;

        [Header("Misc")]
        public GameObject backButton;

        int currentPage = 0;
        bool cameraInitialized;
        List<MenuType> mainMenus = new List<MenuType> {};
        bool menuSetsInitialized;
        int numberOfDragonsOnMarket;
        bool numberOfDragonsInitialized;
        int maxPages;
        MarketDragonData[] data;
        int previousPage;
        bool dataInitialized;
        bool cameraZoomed;
        int dragon2BuyIndex;

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
                        foreach(MarketDragonPanel panel in dragonPanels)
                        {
                            panel.Deactivate();
                        }
                        buyPanel.SetActive(false);
                        singleDragonPanel.SetActive(false);
                        loadingPanel.SetActive(true);
                        myDragonsButton.SetActive(true);
                        pagePanel.SetActive(true);

                        Message.AddListener<DragonMarketResponse>(OnDragonMarketResponse);
                        Message.AddListener<MenuSetResponse>(OnMenuSetResponse);
                        Message.AddListener<NumberOfMarketDragonsResponse>(OnNumberOfMarketDragonsResponse);
                        
                        Message.Send(new DragonMarketRequest(MarketStatus.Start));
                        if(!numberOfDragonsInitialized)
                        {
                            Message.Send(new NumberOfMarketDragonsRequest());
                        }
                        else
                        {
                            UpdatePagePanel();
                        }
                        Message.Send(new DragonMarketRequest(currentPage));

                        if(!menuSetsInitialized)
                        {
                            menuSetsInitialized = true;
                            Message.Send(new MenuSetRequest(MenuSet.MainMenu));
                        }
                        if(!cameraInitialized)
                        {
                            cameraInitialized = true;
                            Message.Send(new MoveCameraRequest(CameraTransformType.Market));
                        }
                    }
                    else
                    {
                        currentPage = 0;
                        previousPage = 0;
                        cameraInitialized = false;
                        dataInitialized = false;
                        data = new MarketDragonData[0];
                        StopAllCoroutines();
                        Message.RemoveListener<DragonMarketResponse>(OnDragonMarketResponse);
                        Message.RemoveListener<MenuSetResponse>(OnMenuSetResponse);
                        Message.RemoveListener<NumberOfMarketDragonsResponse>(OnNumberOfMarketDragonsResponse);
                    }
                    base.IsActive = value;
                }
            }
        }

        void OnDragonMarketResponse(DragonMarketResponse response)
        {
            if(response.status == MarketStatus.Loading)
            {
                loadingPanel.SetActive(true);
            }
            else if(response.status == MarketStatus.Update)
            {
                loadingPanel.SetActive(false);
                data = new MarketDragonData[response.data.Length];
                response.data.CopyTo(data, 0);
                dataInitialized = true;
                int numPanels = dragonPanels.Count;
                int dataLength = data.Length;
                for(int i = 0; i < numPanels; i++)
                {
                    if(i < dataLength)
                    {
                        dragonPanels[i].Activate(data[i]);
                    }
                    else
                    {
                        dragonPanels[i].Deactivate();
                    }
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

        void OnNumberOfMarketDragonsResponse(NumberOfMarketDragonsResponse response)
        {
            numberOfDragonsOnMarket = response.numberOfDragonsOnMarket;
            numberOfDragonsInitialized = true;
            UpdatePagePanel();
        }

        void UpdatePagePanel()
        {
            maxPages = Mathf.CeilToInt(numberOfDragonsOnMarket / 5);
            maxPages = maxPages > 0 ? maxPages : 1;
            pageText.text = (currentPage + 1).ToString() + "/" + maxPages.ToString();
            previousPageButton.SetActive(currentPage > 0);
            nextPageButton.SetActive(currentPage < maxPages - 1);
        }

        public void CancelLoading()
        {
            if(dataInitialized) 
            {
                currentPage = previousPage;
                loadingPanel.SetActive(false);
                Message.Send(new DragonMarketRequest(MarketStatus.CancelLoading));
            }
            else
            {
                Exit();
            }
        }

        public void NextPage()
        {
            previousPage = currentPage;
            currentPage++;
            Message.Send(new DragonMarketRequest(currentPage));
            UpdatePagePanel();
        }

        public void PreviousPage()
        {
            previousPage = currentPage;
            currentPage--;
            Message.Send(new DragonMarketRequest(currentPage));
            UpdatePagePanel();
        }

        public void Exit()
        {
            if(cameraZoomed)
            {
                Message.Send(new UpdateVisibleMarketDragons());
                cameraZoomed = false;
                singleDragonPanel.SetActive(false);
                pagePanel.SetActive(true);
                myDragonsButton.SetActive(true);
                int numDragons = data.Length;
                for(int i = 0; i < numDragons; i++)
                {
                    dragonPanels[i].Activate(data[i]);
                }
                Message.Send(new MoveCameraRequest(CameraTransformType.Market));
            }
            else
            {
                Message.Send(new DragonMarketRequest(MarketStatus.Exit));
                Message.Send(new ActivateTimedLoadingMenu());
                Message.Send(new ActivateMenu(activatedTypes: mainMenus));
            }
        }

        public void Zoom(int index)
        {
            CameraTransformType type = CameraTransformType.MarketDragon1;
            switch(index)
            {
                case 0:
                    type = CameraTransformType.MarketDragon1;
                    break;
                case 1:
                    type = CameraTransformType.MarketDragon2;
                    break;
                case 2:
                    type = CameraTransformType.MarketDragon3;
                    break;
                case 3:
                    type = CameraTransformType.MarketDragon4;
                    break;
                case 4:
                    type = CameraTransformType.MarketDragon5;
                    break;
            }
            Message.Send(new MoveCameraRequest(type));
            Message.Send(new UpdateVisibleMarketDragons(index));
            singleDragonPanel.SetActive(true);
            pagePanel.SetActive(false);
            myDragonsButton.SetActive(false);
            cameraZoomed = true;
            int numDragons = data.Length;
            for(int i = 0; i < numDragons; i++)
            {
                dragonPanels[i].Deactivate();
            }
        }

        public void OpenBuyPanel(int index)
        {
            buyPanel.SetActive(true);
            dragon2BuyIndex = index;
        }

        public void ConfirmBuy()
        {
            // todo: logic dis beeeeotch
            int dragonId = data[dragon2BuyIndex].data.id;
            float price = data[dragon2BuyIndex].data.price;
            Message.Send(new BuyDragonRequest(dragonId, price));
            buyDragonText.text = "Breeding...";
            backButton.SetActive(false);
            confirmBreedButton.SetActive(false);
            closeBreedingPanelButton.SetActive(false);
        }

        public void CloseBuyPanel()
        {
            buyPanel.SetActive(false);
        }

        public void OpenMyDragons()
        {

        }
    }

    [System.Serializable]
    public struct MarketDragonPanel
    {
        public GameObject panel;
        public TextMeshProUGUI priceText;
        public GameObject buyButton;
        public GameObject zoomButton;

        public void Activate(MarketDragonData data)
        {
            panel.SetActive(true);
            zoomButton.SetActive(true);
            priceText.text = data.isUser ? "Your Dragon" : data.data.price + " Near";
            buyButton.SetActive(!data.isUser);
        }

        public void Deactivate()
        {
            panel.SetActive(false);
            zoomButton.SetActive(false);
        }
    }
}
