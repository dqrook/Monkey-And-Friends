using System.Collections;
using System.Collections.Generic;
using Ryzm.Dragon.Messages;
using TMPro;
using Ryzm.Dragon;
using UnityEngine;

namespace Ryzm.UI
{
    public class ProbabilityFilterDropdown : FilterDropdown
    {
        #region Public Variables
        public TextMeshProUGUI probability;
        #endregion

        #region Private Variables
        List<GeneProbability> geneProbabilities = new List<GeneProbability>();
        List<FilterProbabilityValue> filterProbabilityValues = new List<FilterProbabilityValue>();
        float prevProbability;
        #endregion

        #region Listener Functions
        protected override void OnDragonGenesResponse(DragonGenesResponse response)
        {
            if(IsReceiver(response.receiver))
            {
                dragonGenes = response.genes;
            }
        }

        protected override void OnMarketFilterMapsResponse(MarketFilterMapsResponse response)
        {
            if(IsReceiver(response.receiver))
            {
                marketFilterMaps = response.marketFilterMaps;
            }
        }
        #endregion

        #region Public Functions
        public override void Enable(int[] genes1, int[] genes2)
        {
            prevProbability = -1;
            geneProbabilities.Clear();
            filterProbabilityValues.Clear();
            MarketFilter filter = marketFilterMaps.GetMarketFilter(type);
            if(type == FilterType.PrimaryColor || type == FilterType.SecondaryColor)
            {
                geneProbabilities = dragonGenes.GetColorProbablities(genes1[0].ToString(), genes2[0].ToString());
                foreach(GeneProbability prob in geneProbabilities)
                {
                    DragonColor color = dragonGenes.GetDragonColor(prob.value);
                    FilterProbabilityValue fpv = new FilterProbabilityValue(filter, color, prob.probablity);
                    filterProbabilityValues.Add(fpv);
                }
            }
            else
            {
                geneProbabilities = dragonGenes.GetGeneProbablities(genes1, genes2);
                bool isHornType = type == FilterType.HornType;
                foreach(GeneProbability prob in geneProbabilities)
                {
                    if(type == FilterType.BodyGenes)
                    {
                        DragonGene gene = dragonGenes.GetGeneBySequence(prob.value, GeneType.Body);
                        FilterProbabilityValue fpv = new FilterProbabilityValue(filter, gene, prob.probablity);
                        filterProbabilityValues.Add(fpv);
                    }
                    else
                    {
                        DragonGene gene = dragonGenes.GetGeneBySequence(prob.value, GeneType.Horn);
                        FilterProbabilityValue fpv = new FilterProbabilityValue(filter, gene, isHornType, prob.probablity);
                        filterProbabilityValues.Add(fpv);
                    }
                }
            }
            OnFilterValueChanged();
        }

        public float CurrentProbability()
        {
            if(filterDropdown.value < filterProbabilityValues.Count)
            {
                return filterProbabilityValues[filterDropdown.value].probability;
            }
            return 0;
        }

        public void OnFilterValueChanged()
        {
            float curProb = CurrentProbability();
            if(prevProbability != curProb)
            {
                prevProbability = curProb;
                if(curProb < 1)
                {
                    probability.text = "<1%";
                }
                else
                {
                    probability.text = Mathf.Round(curProb * 100).ToString() + "%";
                }
            }
        }
        #endregion
    }

    public class FilterProbabilityValue: FilterValue
    {
        public float probability;

        public FilterProbabilityValue(MarketFilter filter, DragonGene gene, float probability)
        {
            this.probability = probability;
            SetValues(filter, gene);
        }

        public FilterProbabilityValue(MarketFilter filter, DragonGene gene, bool isHornType, float probability)
        {
            this.probability = probability;
            SetValues(filter, gene);
            if(isHornType)
            {
                this.value = gene.hornType;
            }
        }

        public FilterProbabilityValue(MarketFilter filter, DragonColor color, float probability)
        {
            this.probability = probability;
            this.type = filter.type;
            this.filterName = filter.name;
            this.displayName = color.name;
            this.image = color.image;
            this.value = color.value;
        }
    }
}
