using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.Dragon.Messages;
using CodeControl;
using Ryzm.Dragon;
using Ryzm.Utils;
using UnityEngine.Networking;
using Ryzm.UI.Messages;

namespace Ryzm.UI
{
    public class MarketMenu : RyzmMenu
    {
        public Canvas loadingPanel;
        public CanvasGroup scrollViewGroup;
        public GameObject noDragonsPanel;

        #region Private Variables
        int currentPage;
        int totalNumDragons;
        DragonCardMetadata[] currentCardMetadata;
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
                        Message.Send(new QueryMarketRequest());
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
            int numNewDragons = response.dragons.Length;
            currentCardMetadata = response.dragons;
            if(numNewDragons == 0)
            {
                scrollViewGroup.alpha = 0;
                loadingPanel.enabled = false;
                noDragonsPanel.SetActive(true);
            }
            else
            {
                scrollViewGroup.alpha = 1;
                loadingPanel.enabled = false;
                noDragonsPanel.SetActive(false);
            }
            Message.Send(new DragonCardMetadataResponse(currentCardMetadata));
        }

        void OnDragonCardMetadataRequest(DragonCardMetadataRequest request)
        {
            Message.Send(new DragonCardMetadataResponse(currentCardMetadata));
        }
        #endregion

        #region Private Functions
        void Reset()
        {
            currentPage = 0;
            totalNumDragons = 0;
        }
        #endregion
    }

}
