using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ryzm.Dragon;
using CodeControl;
using Ryzm.Dragon.Messages;

namespace Ryzm.UI
{
    public class FilterDropdown : MonoBehaviour
    {
        #region Public Variables
        public FilterType type;
        #endregion

        #region Private Variables
        MarketFilterMaps marketFilterMaps;
        DragonGenes dragonGenes;
        #endregion

        #region Event Functions
        void Awake()
        {
            Message.AddListener<MarketFilterMapsResponse>(OnMarketFilterMapsResponse);
            Message.AddListener<DragonGenesResponse>(OnDragonGenesResponse);
        }

        void Start()
        {
            Message.Send(new MarketFilterMapsRequest(type.ToString()));
            Message.Send(new DragonGenesRequest(type.ToString()));
        }

        void OnDestroy()
        {
            Message.RemoveListener<MarketFilterMapsResponse>(OnMarketFilterMapsResponse);
            Message.RemoveListener<DragonGenesResponse>(OnDragonGenesResponse);
        }
        #endregion

        #region Listener Functions
        void OnMarketFilterMapsResponse(MarketFilterMapsResponse response)
        {
            if(response.receiver == type.ToString())
            {
                marketFilterMaps = response.marketFilterMaps;
            }
        }

        void OnDragonGenesResponse(DragonGenesResponse response)
        {
            if(response.receiver == type.ToString())
            {
                dragonGenes = response.genes;
            }
        }
        #endregion
    }
}
