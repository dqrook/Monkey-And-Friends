using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.Dragon;
using Ryzm.Dragon.Messages;
using CodeControl;
using Ryzm.UI.Messages;
using TMPro;

namespace Ryzm.UI
{
    public class ProbabilityFilterPanel : MonoBehaviour
    {
        #region Public Variables
        public Canvas canvas;
        public TextMeshProUGUI probability;
        public List<ProbabilityFilterDropdown> filterDropdowns = new List<ProbabilityFilterDropdown>();
        #endregion

        #region Private Variables
        bool isActive;
        List<MarketFilter> filters = new List<MarketFilter>();
        #endregion

        #region Public Functions
        public void ConfirmFilter()
        {
            UpdateFilters(true);
        }

        public void ResetFilters()
        {
            foreach(ProbabilityFilterDropdown filterDropdown in filterDropdowns)
            {
                filterDropdown.Reset();
            }
            UpdateFilterValue();
        }

        public void UpdateFilterValue()
        {
            UpdateFilters(false);
            float currentProbability = 0;
            foreach(ProbabilityFilterDropdown filterDropdown in filterDropdowns)
            {
                filterDropdown.OnFilterValueChanged();
            }
            if(currentProbability < 1)
            {
                probability.text = "<1%";
            }
            else
            {
                probability.text = Mathf.Round(currentProbability * 100).ToString();
            }
        }
        #endregion


        #region Private Functions
        void UpdateFilters(bool sendApiRequest = false)
        {
            filters.Clear();
            foreach(ProbabilityFilterDropdown filterDropdown in filterDropdowns)
            {
                MarketFilter filter = filterDropdown.CurrentFilter;
                if(filter.value.ToLower() != "any")
                {
                    filters.Add(filter);
                }
            }
            Message.Send(new UpdateDragonFilters(filters, sendApiRequest));
        }
        #endregion
    }
}
