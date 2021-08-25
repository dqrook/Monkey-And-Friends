using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ryzm.Dragon;
using CodeControl;
using Ryzm.Dragon.Messages;

namespace Ryzm.UI
{
    public abstract class FilterDropdown : MonoBehaviour
    {
        #region Public Variables
        public FilterType type;
        public TMP_Dropdown filterDropdown;
        #endregion

        #region Protected  Variables
        protected MarketFilterMaps marketFilterMaps;
        protected DragonGenes dragonGenes;
        protected bool initialized;
        protected List<FilterValue> filterValues = new List<FilterValue>();
        #endregion

        #region Properties
        public MarketFilter CurrentFilter
        {
            get
            {
                FilterValue currentFilterValue = filterValues[filterDropdown.value];
                return currentFilterValue.CurrentFilter;
            }
        }
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
        protected virtual void OnMarketFilterMapsResponse(MarketFilterMapsResponse response)
        {
            if(IsReceiver(response.receiver))
            {
                marketFilterMaps = response.marketFilterMaps;
                Initialize();
            }
        }

        protected virtual void OnDragonGenesResponse(DragonGenesResponse response)
        {
            if(IsReceiver(response.receiver))
            {
                dragonGenes = response.genes;
                Initialize();
            }
        }
        #endregion

        #region Public Functions
        public void Enable()
        {
            Initialize();
        }

        public virtual void Enable(int[] genes1, int[] genes2) {}

        public void Disable(bool resetValue)
        {
            if(resetValue)
            {
                Reset();
            }
        }

        public void Reset()
        {
            filterDropdown.value = 0;
        }
        #endregion
        
        #region Protected Functions
        protected bool IsReceiver(string receiver)
        {
            return receiver == type.ToString();
        }

        protected virtual void Initialize() {}
        #endregion
    }

    public class FilterValue
    {
        public FilterType type;
        public string filterName;
        public string displayName;
        public Sprite image;
        public string value;

        public MarketFilter CurrentFilter
        {
            get
            {
                MarketFilter filter = new MarketFilter();
                filter.type = type;
                filter.name = filterName;
                filter.value = value;
                return filter;
            }
        }

        public FilterValue()
        {
            this.filterName = "";
            this.type = FilterType.PrimaryColor; // doesnt matter the type
            this.value = "Any";
        }

        public FilterValue(MarketFilter filter, DragonGene gene)
        {
            SetValues(filter, gene);
        }

        public FilterValue(MarketFilter filter, DragonGene gene, bool isHornType)
        {
            SetValues(filter, gene);
            if(isHornType)
            {
                this.value = gene.hornType;
            }
        }

        public FilterValue(MarketFilter filter, DragonColor color)
        {
            this.type = filter.type;
            this.filterName = filter.name;
            this.displayName = color.name;
            this.image = color.image;
            this.value = color.value;
        }

        protected void SetValues(MarketFilter filter, DragonGene gene)
        {
            this.type = filter.type;
            this.filterName = filter.name;
            this.displayName = gene.name;
            this.image = gene.image;
            this.value = gene.sequence;
        }
    }
}
