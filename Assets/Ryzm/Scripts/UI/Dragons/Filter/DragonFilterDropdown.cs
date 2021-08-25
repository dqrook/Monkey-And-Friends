using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ryzm.Dragon;

namespace Ryzm.UI
{
    public class DragonFilterDropdown : FilterDropdown
    {
        #region Protected Functions
        protected override void Initialize()
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
                        if(filterValue.image != null)
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
                        if(filterValue.image != null)
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
}
