using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.Dragon.Messages;
using Ryzm.Dragon;
using Ryzm.UI.Messages;
using TMPro;
using UnityEngine.UI;

namespace Ryzm.UI
{
    public class DragonMarketMenu : RyzmMenu
    {
        public enum PanelType
        {
            Loading,
            Page,
            Buy,
            SingleDragon
        }

        struct MarketPanel
        {
            public PanelType type;
            public GameObject panel;
            Canvas panelCanvas;
            bool initialized;
            bool status;

            void Initialize()
            {
                initialized = true;
                panelCanvas = panel.GetComponent<Canvas>();
            }

            public void ChangeStatus(bool isActive)
            {
                if(!initialized || isActive != status)
                {
                    if(!initialized)
                    {
                        Initialize();
                    }
                    if(panelCanvas != null)
                    {
                        panelCanvas.enabled = isActive;
                    }
                    else
                    {
                        panel.SetActive(isActive);
                    }
                    status = isActive;
                }
            }
        }

        #region Public Variables
        [Header("Dragon Panels")]
        public List<MarketDragonPanel> dragonPanels = new List<MarketDragonPanel>();

        [Header("Loading Panel")]
        public GameObject loadingPanel;

        [Header("Page Panel")]
        public GameObject pagePanel;
        public TextMeshProUGUI pageText;
        public TextMeshProUGUI noDragonsText;
        public GameObject nextPageButton;
        public GameObject previousPageButton;

        [Header("Buy Panel")]
        public GameObject buyPanel;
        public TextMeshProUGUI buyDragonText;
        public GameObject confirmBuyButton;
        public GameObject closeBuyPanelButton;
        public GameObject cancelBuyButton;

        [Header("Single Dragon")]
        public GameObject singleDragonPanel;
        public GameObject singleDragonResetCameraButton;

        [Header("My Dragons")]
        public GameObject myDragonsButton;

        [Header("Misc")]
        public GameObject backButton;
        #endregion

        #region Private Variables
        int currentPage = 0;
        bool cameraInitialized;
        List<MenuType> mainMenus = new List<MenuType> {};
        List<MenuType> singleDragonMenus = new List<MenuType> {};
        List<MenuType> myDragonsMenus = new List<MenuType> {};
        bool menuSetsInitialized;
        int numberOfDragonsOnMarket;
        bool numberOfDragonsInitialized;
        int maxPages;
        MarketDragonData[] data;
        int previousPage;
        bool dataInitialized;
        bool cameraZoomed;
        int dragon2BuyIndex;
        List<MarketPanel> marketPanels = new List<MarketPanel>();
        bool initialized;
        List<PanelType> activatedPanelTypes = new List<PanelType>();
        WaitForSeconds initializingWait;
        IEnumerator waitForInitialization;
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
                        foreach(MarketDragonPanel panel in dragonPanels)
                        {
                            panel.Deactivate();
                        }
                        if(!initialized)
                        {
                            initializingWait = new WaitForSeconds(2 * Time.deltaTime);
                            initialized = true;
                            MarketPanel _buyPanel = new MarketPanel();
                            _buyPanel.panel = buyPanel;
                            _buyPanel.type = PanelType.Buy;
                            marketPanels.Add(_buyPanel);

                            MarketPanel _singleDragonPanel = new MarketPanel();
                            _singleDragonPanel.panel = singleDragonPanel;
                            _singleDragonPanel.type = PanelType.SingleDragon;
                            marketPanels.Add(_singleDragonPanel);

                            MarketPanel _loadingPanel = new MarketPanel();
                            _loadingPanel.panel = loadingPanel;
                            _loadingPanel.type = PanelType.Loading;
                            marketPanels.Add(_loadingPanel);

                            MarketPanel _pagePanel = new MarketPanel();
                            _pagePanel.panel = pagePanel;
                            _pagePanel.type = PanelType.Page;
                            marketPanels.Add(_pagePanel);
                        }
                        activatedPanelTypes.Clear();
                        activatedPanelTypes.Add(PanelType.Loading);
                        activatedPanelTypes.Add(PanelType.Page);
                        ActivatePanels(activatedPanelTypes);
                        // buyPanel.SetActive(false);
                        // singleDragonPanel.SetActive(false);
                        // loadingPanel.SetActive(true);
                        // pagePanel.SetActive(true);
                        myDragonsButton.SetActive(true);

                        Message.AddListener<DragonMarketResponse>(OnDragonMarketResponse);
                        Message.AddListener<MenuSetResponse>(OnMenuSetResponse);
                        Message.AddListener<NumberOfMarketDragonsResponse>(OnNumberOfMarketDragonsResponse);
                        Message.AddListener<BuyDragonResponse>(OnBuyDragonResponse);
                        
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
                            Message.Send(new MenuSetRequest(MenuSet.SingleDragonMenu));
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
                        Message.RemoveListener<BuyDragonResponse>(OnBuyDragonResponse);
                    }
                    base.IsActive = value;
                }
            }
        }
        #endregion

        #region Event Listeners
        void OnDragonMarketResponse(DragonMarketResponse response)
        {
            if(response.status == MarketStatus.Loading)
            {
                if(!numberOfDragonsInitialized || numberOfDragonsOnMarket > 0)
                {
                    // loadingPanel.SetActive(true);
                    ActivatePanel(PanelType.Loading);
                }
                else
                {
                    // loadingPanel.SetActive(false);
                    DeactivatePanel(PanelType.Loading);
                }
            }
            else if(response.status == MarketStatus.Update)
            {
                DeactivatePanel(PanelType.Loading);
                // loadingPanel.SetActive(false);
                // data = new MarketDragonData[response.data.Length];
                // response.data.CopyTo(data, 0);
                waitForInitialization = WaitForInitialization(data);
                StartCoroutine(waitForInitialization);
                // dataInitialized = true;
                // int numPanels = dragonPanels.Count;
                // int dataLength = data.Length;
                // for(int i = 0; i < numPanels; i++)
                // {
                //     if(i < dataLength)
                //     {
                //         dragonPanels[i].Activate(data[i]);
                //     }
                //     else
                //     {
                //         dragonPanels[i].Deactivate();
                //     }
                // }
            }
        }

        void OnMenuSetResponse(MenuSetResponse response)
        {
            if(response.set == MenuSet.MainMenu)
            {
                mainMenus = response.menus;
            }
            else if(response.set == MenuSet.SingleDragonMenu)
            {
                singleDragonMenus = response.menus;
            }
            else if(response.set == MenuSet.MyDragonsMenu)
            {
                myDragonsMenus = response.menus;
            }
        }

        void OnNumberOfMarketDragonsResponse(NumberOfMarketDragonsResponse response)
        {
            numberOfDragonsOnMarket = response.numberOfDragonsOnMarket;
            numberOfDragonsInitialized = true;
            UpdatePagePanel();
        }

        void OnBuyDragonResponse(BuyDragonResponse response)
        {
            if(response.status == TransactionStatus.Failed)
            {
                buyDragonText.text = "Unable to buy, please try again later";
                closeBuyPanelButton.SetActive(true);
            }
            else if(response.status == TransactionStatus.Success)
            {
                dragon2BuyIndex = response.dragonId;
                Message.Send(new DragonMarketRequest(MarketStatus.Exit));
                Message.Send(new ActivateTimedLoadingMenu(true));
                Message.Send(new ActivateMenu(activatedTypes: singleDragonMenus));
            }
        }
        #endregion

        #region Public Functions
        public void CancelLoading()
        {
            if(dataInitialized) 
            {
                currentPage = previousPage;
                // loadingPanel.SetActive(false);
                DeactivatePanel(PanelType.Loading);
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
                // singleDragonPanel.SetActive(false);
                // pagePanel.SetActive(true);
                activatedPanelTypes.Clear();
                activatedPanelTypes.Add(PanelType.Page);
                ActivatePanels(activatedPanelTypes);

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
                Message.Send(new ActivateTimedLoadingMenu(2.5f));
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
            
            // singleDragonPanel.SetActive(true);
            // pagePanel.SetActive(false);
            activatedPanelTypes.Clear();
            activatedPanelTypes.Add(PanelType.SingleDragon);
            ActivatePanels(activatedPanelTypes);

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
            confirmBuyButton.SetActive(true);
            closeBuyPanelButton.SetActive(true);
            cancelBuyButton.SetActive(false);
            // buyPanel.SetActive(true);
            activatedPanelTypes.Clear();
            activatedPanelTypes.Add(PanelType.Buy);
            activatedPanelTypes.Add(PanelType.Page);
            ActivatePanels(activatedPanelTypes);
            dragon2BuyIndex = index;
        }

        public void ConfirmBuy()
        {
            if(IsActive)
            {
                int dragonId = data[dragon2BuyIndex].data.id;
                float price = data[dragon2BuyIndex].data.price;
                Message.Send(new BuyDragonRequest(dragonId, price));
                buyDragonText.text = "Authorizing Transaction...";
                backButton.SetActive(false);
                confirmBuyButton.SetActive(false);
                closeBuyPanelButton.SetActive(false);
                cancelBuyButton.SetActive(true);
            }
            else
            {
                Debug.Log("what the hell is going on");
            }
        }

        public void CancelBuy()
        {
            Message.Send(new CancelTransaction());
            CloseBuyPanel();
        }

        public void CloseBuyPanel()
        {
            // buyPanel.SetActive(false);
            activatedPanelTypes.Clear();
            activatedPanelTypes.Add(PanelType.Page);
            ActivatePanels(activatedPanelTypes);
        }

        public void OpenMyDragons()
        {
            Message.Send(new DragonMarketRequest(MarketStatus.Exit));
            Message.Send(new ActivateTimedLoadingMenu(2.5f));
            Message.Send(new ActivateMenu(activatedTypes: myDragonsMenus));
            Message.Send(new PreviousMenusUpdate(MenuSet.MarketMenu));
        }
        #endregion

        #region Private Functions
        void ActivatePanels(List<PanelType> activatedTypes)
        {
            foreach(MarketPanel panel in marketPanels)
            {
                panel.ChangeStatus(activatedTypes.Contains(panel.type));
            }
        }

        void ActivatePanel(PanelType activatedType)
        {
            foreach(MarketPanel panel in marketPanels)
            {
                if(panel.type == activatedType)
                {
                    panel.ChangeStatus(true);
                    break;
                }
            }
        }

        void DeactivatePanel(PanelType deactivatedType)
        {
            foreach(MarketPanel panel in marketPanels)
            {
                if(panel.type == deactivatedType)
                {
                    panel.ChangeStatus(false);
                    break;
                }
            }
        }

        void UpdatePagePanel()
        {
            if(numberOfDragonsOnMarket == 0)
            {
                noDragonsText.gameObject.SetActive(true);
                pageText.gameObject.SetActive(false);
                previousPageButton.SetActive(false);
                nextPageButton.SetActive(false);
            }
            else
            {
                noDragonsText.gameObject.SetActive(false);
                pageText.gameObject.SetActive(true);
                maxPages = Mathf.CeilToInt(numberOfDragonsOnMarket / 5);
                maxPages = maxPages > 0 ? maxPages : 1;
                pageText.text = (currentPage + 1).ToString() + "/" + maxPages.ToString();
                previousPageButton.SetActive(currentPage > 0);
                nextPageButton.SetActive(currentPage < maxPages - 1);
            }
        }
        #endregion

        #region Coroutines
        IEnumerator WaitForInitialization(MarketDragonData[] data)
        {
            bool allInitialized = false;
            while(!allInitialized)
            {
                bool init = true;
                foreach(MarketDragonData dragon in data)
                {
                    if(!dragon.Initialized)
                    {
                        init = false;
                        break;
                    }
                }
                allInitialized = init;
                yield return initializingWait;
                yield return null;
            }
            DeactivatePanel(PanelType.Loading);
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
            yield break;
        }
        #endregion
    }

    [System.Serializable]
    public struct MarketDragonPanel
    {
        public GameObject panel;
        public TextMeshProUGUI priceText;
        public GameObject buyButton;
        public GameObject zoomButton;
        bool intialized;
        Canvas panelCanvas;

        void Intialize()
        {
            intialized = true;
            panelCanvas = panel.GetComponent<Canvas>();
        }

        public void Activate(MarketDragonData data)
        {
            if(data != null)
            {
                panel.SetActive(true);
                zoomButton.SetActive(true);
                priceText.text = data.isUser ? "Your Dragon" : data.data.price + " Near";
                buyButton.SetActive(!data.isUser);
            }
            else
            {
                Deactivate();
            }
        }

        public void Deactivate()
        {
            panel.SetActive(false);
            zoomButton.SetActive(false);
        }
    }
}
