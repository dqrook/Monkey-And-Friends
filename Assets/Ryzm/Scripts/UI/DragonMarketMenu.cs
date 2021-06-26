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

        [Header("Single Dragon")]
        public GameObject singleDragonPanel;
        public GameObject singleDragonResetCameraButton;

        int currentPage = 0;
        bool cameraInitialized;
        List<MenuType> mainMenus = new List<MenuType> {};
        bool menuSetsInitialized;
        int numberOfDragonsOnMarket;
        int maxPages;
        MarketDragonData[] data;
        int previousPage;
        bool dataInitialized;

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
                        loadingPanel.SetActive(true);

                        Message.AddListener<DragonMarketResponse>(OnDragonMarketResponse);
                        Message.AddListener<MenuSetResponse>(OnMenuSetResponse);
                        Message.AddListener<NumberOfMarketDragonsResponse>(OnNumberOfMarketDragonsResponse);
                        
                        Message.Send(new DragonMarketRequest(MarketStatus.Start));
                        Message.Send(new NumberOfMarketDragonsRequest());
                        Message.Send(new DragonMarketRequest(currentPage));

                        if(!menuSetsInitialized)
                        {
                            menuSetsInitialized = true;
                            Message.Send(new MenuSetRequest(MenuSet.MainMenu));
                        }
                        if(!cameraInitialized)
                        {
                            cameraInitialized = true;
                            Message.Send(new MoveCameraRequest(TransformType.Market));
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
                data = new MarketDragonData[0];
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
            Message.Send(new DragonMarketRequest(MarketStatus.Exit));
            Message.Send(new ActivateMenu(activatedTypes: mainMenus));
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
