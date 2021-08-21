using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.Dragon.Messages;
using CodeControl;
using Ryzm.Dragon;
using Ryzm.UI.Messages;

namespace Ryzm.UI
{
    public class MarketMenu : RyzmMenu
    {
        public Canvas loadingPanel;
        public Canvas failedPanel;
        public CanvasGroup scrollViewGroup;
        public GameObject noDragonsPanel;
        
        [Header("Header Row")]
        public GameObject arrowsPanel;
        public GameObject filterButton;

        #region Private Variables
        int currentPage;
        int totalNumDragons;
        int numNewDragons;
        DragonCardMetadata[] currentCardMetadata;
        bool inZoom;
        DragonGenes genes;
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
                        currentCardMetadata = new DragonCardMetadata[0];
                        Message.AddListener<QueryMarketResponse>(OnQueryMarketResponse);
                        Message.AddListener<DragonCardMetadataRequest>(OnDragonCardMetadataRequest);
                        Message.AddListener<DisplayDragonZoomRequest>(OnDisplayDragonZoomRequest);
                        Message.AddListener<DisplayDragonZoomResponse>(OnDisplayDragonZoomResponse);
                        Message.AddListener<FilterDragonZoomRequest>(OnFilterDragonZoomRequest);
                        Message.AddListener<DragonGenesResponse>(OnDragonGenesResponse);
                        Message.Send(new MoveCameraRequest(CameraTransformType.Market));
                        Message.Send(new QueryMarketRequest());
                        if(genes == null)
                        {
                            Message.Send(new DragonGenesRequest("marketMenu"));
                        }
                        scrollViewGroup.alpha = 0;
                        loadingPanel.enabled = true;
                        noDragonsPanel.SetActive(false);
                    }
                    else
                    {
                        Reset();
                        Message.Send(new QueryMarketRequest(MarketQueryType.Exit));
                        Message.RemoveListener<QueryMarketResponse>(OnQueryMarketResponse);
                        Message.RemoveListener<DragonCardMetadataRequest>(OnDragonCardMetadataRequest);
                        Message.RemoveListener<DisplayDragonZoomRequest>(OnDisplayDragonZoomRequest);
                        Message.RemoveListener<DisplayDragonZoomResponse>(OnDisplayDragonZoomResponse);
                        Message.RemoveListener<FilterDragonZoomRequest>(OnFilterDragonZoomRequest);
                        Message.RemoveListener<DragonGenesResponse>(OnDragonGenesResponse);
                    }
                    base.IsActive = value;
                }
            }
        }
        #endregion

        #region Listener Functions
        void OnQueryMarketResponse(QueryMarketResponse response)
        {
            currentPage = response.page;
            totalNumDragons = response.totalNumDragons;
            numNewDragons = response.numNewDragons;

            loadingPanel.enabled = false;
            // failedPanel.enabled = false;
            if(numNewDragons == 0)
            {
                scrollViewGroup.alpha = 0;
                noDragonsPanel.SetActive(true);
            }
            else
            {
                scrollViewGroup.alpha = 1;
                noDragonsPanel.SetActive(false);
            }

            // currentCardMetadata = response.dragons;
            // Message.Send(new DragonCardMetadataResponse(currentCardMetadata));
        }

        void OnDisplayDragonZoomRequest(DisplayDragonZoomRequest request)
        {
            loadingPanel.enabled = true;
            inZoom = true;
        }

        void OnDisplayDragonZoomResponse(DisplayDragonZoomResponse response)
        {
            loadingPanel.enabled = false;
            // failedPanel.enabled = response.failed;
            arrowsPanel.SetActive(response.failed);
            filterButton.SetActive(response.failed);
            inZoom = !response.failed;
        }

        void OnFilterDragonZoomRequest(FilterDragonZoomRequest request)
        {
            inZoom = true;
            arrowsPanel.SetActive(false);
            filterButton.SetActive(false);
        }

        void OnDragonGenesResponse(DragonGenesResponse response)
        {
            if(response.receiver == "marketMenu")
            {
                genes = response.genes;
            }
        }

        void OnDragonCardMetadataRequest(DragonCardMetadataRequest request)
        {
            Message.Send(new DragonCardMetadataResponse(currentCardMetadata));
        }
        #endregion

        #region Public Functions
        public void OnClickBackButton()
        {
            if(inZoom)
            {
                // todo: leave dragon panel
                Message.Send(new DisableDragonInfoPanel());
                Message.Send(new ReturnToMarket());
                inZoom = false;
                arrowsPanel.SetActive(true);
                filterButton.SetActive(true);
            }
            else
            {
                // todo: return to main menu
            }
        }

        public void OnClickFilterButton()
        {
            Message.Send(new FilterDragonZoomRequest());
        }
        #endregion

        #region Private Functions
        void Reset()
        {
            currentPage = 0;
            totalNumDragons = 0;
            inZoom = false;
        }
        #endregion
    }

}
