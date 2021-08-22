using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ryzm.Dragon;
using CodeControl;
using Ryzm.Dragon.Messages;

namespace Ryzm.UI
{
    public class DragonFilterDropdown : MonoBehaviour
    {
        #region Public Variables
        public FilterType type;
        public TMP_Dropdown filterDropdown;
        #endregion

        #region Private Variables
        public MarketFilterMaps marketFilterMaps;
        public DragonGenes dragonGenes;
        bool initialized;
        List<FilterValue> filterValues = new List<FilterValue>();
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
        void OnMarketFilterMapsResponse(MarketFilterMapsResponse response)
        {
            bool isIt = response.receiver == type.ToString();
            if(response.receiver == type.ToString())
            {
                marketFilterMaps = response.marketFilterMaps;
                Intialize();
            }
        }

        void OnDragonGenesResponse(DragonGenesResponse response)
        {
            if(response.receiver == type.ToString())
            {
                dragonGenes = response.genes;
                Intialize();
            }
        }
        #endregion

        #region Public Functions

        public void Enable()
        {
            Intialize();
        }

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

        #region Private Functions
        void Intialize()
        {
            if(!initialized && dragonGenes != null && marketFilterMaps != null)
            {
                initialized = true;
                List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
                options.Add(new TMP_Dropdown.OptionData("Any"));
                MarketFilter filter = marketFilterMaps.GetMarketFilter(type);
                filterValues.Add(new FilterValue());
                if(type == FilterType.PrimaryColor || type == FilterType.SecondaryColor)
                {
                    foreach(DragonColor color in dragonGenes.colors)
                    {
                        FilterValue filterValue = new FilterValue(filter, color);
                        filterValues.Add(filterValue);
                        if(filterValue.image != null)
                        {
                            options.Add(new TMP_Dropdown.OptionData(filterValue.image));
                        }
                        else
                        {
                            options.Add(new TMP_Dropdown.OptionData(filterValue.displayName));
                        }
                    }
                }
                else if(type == FilterType.HornType)
                {
                    foreach(DragonGene gene in dragonGenes.hornGenes)
                    {
                        FilterValue filterValue = new FilterValue(filter, gene, true);
                        filterValues.Add(filterValue);
                        if(gene.image != null)
                        {
                            options.Add(new TMP_Dropdown.OptionData(filterValue.displayName, filterValue.image));
                        }
                        else
                        {
                            options.Add(new TMP_Dropdown.OptionData(filterValue.displayName));
                        }
                    }
                }
                else if(type == FilterType.BodyGenes)
                {
                    foreach(DragonGene gene in dragonGenes.bodyGenes)
                    {
                        FilterValue filterValue = new FilterValue(filter, gene);
                        filterValues.Add(filterValue);
                        if(gene.image != null)
                        {
                            options.Add(new TMP_Dropdown.OptionData(filterValue.displayName, filterValue.image));
                        }
                        else
                        {
                            options.Add(new TMP_Dropdown.OptionData(filterValue.displayName));
                        }
                    }
                }
                else
                {
                    foreach(DragonGene gene in dragonGenes.dragonGenes)
                    {
                        FilterValue filterValue = new FilterValue(filter, gene);
                        filterValues.Add(filterValue);
                        if(gene.image != null)
                        {
                            options.Add(new TMP_Dropdown.OptionData(filterValue.displayName, filterValue.image));
                        }
                        else
                        {
                            options.Add(new TMP_Dropdown.OptionData(filterValue.displayName));
                        }
                    }
                }
                filterDropdown.ClearOptions();
                filterDropdown.AddOptions(options);
            }
        }
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

        void SetValues(MarketFilter filter, DragonGene gene)
        {
            this.type = filter.type;
            this.filterName = filter.name;
            this.displayName = gene.name;
            this.image = gene.image;
            this.value = gene.sequence;
        }
    }
}
