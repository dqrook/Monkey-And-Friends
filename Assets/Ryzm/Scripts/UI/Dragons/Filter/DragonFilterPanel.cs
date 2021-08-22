using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.Dragon;
using Ryzm.Dragon.Messages;
using CodeControl;
using Ryzm.UI.Messages;

namespace Ryzm.UI
{
    public class DragonFilterPanel : MonoBehaviour
    {
        #region Public Variables
        public Canvas canvas;
        public List<DragonFilterDropdown> filterDropdowns = new List<DragonFilterDropdown>();
        #endregion

        #region Private Variables
        bool isActive;
        List<MarketFilter> filters = new List<MarketFilter>();
        #endregion

        #region Event Functions
        void Awake()
        {
            Message.AddListener<EnableDragonFilterPanel>(OnEnableDragonFilterPanel);
            Message.AddListener<DisableDragonFilterPanel>(OnDisableDragonFilterPanel);
            Message.AddListener<ResetDragonFilterPanel>(OnResetDragonFilterPanel);
        }

        void OnDestroy()
        {
            Message.RemoveListener<EnableDragonFilterPanel>(OnEnableDragonFilterPanel);
            Message.RemoveListener<DisableDragonFilterPanel>(OnDisableDragonFilterPanel);
            Message.RemoveListener<ResetDragonFilterPanel>(OnResetDragonFilterPanel);
        }
        #endregion

        #region Listener Functions
        void OnEnableDragonFilterPanel(EnableDragonFilterPanel enable)
        {
            if(!isActive)
            {
                isActive = true;
                foreach(DragonFilterDropdown dropdown in filterDropdowns)
                {
                    dropdown.Enable();
                }

                canvas.enabled = true;
            }
        }

        void OnDisableDragonFilterPanel(DisableDragonFilterPanel disable)
        {
            if(isActive)
            {
                isActive = false;
                foreach(DragonFilterDropdown dropdown in filterDropdowns)
                {
                    dropdown.Disable(disable.resetValue);
                }
                canvas.enabled = false;
            }
        }

        void OnResetDragonFilterPanel(ResetDragonFilterPanel reset)
        {
            ResetFilters();
        }
        #endregion

        #region Public Functions
        public void ConfirmFilter()
        {
            UpdateFilters(true);
        }

        public void ResetFilters()
        {
            foreach(DragonFilterDropdown filterDropdown in filterDropdowns)
            {
                filterDropdown.Reset();
            }
            UpdateFilterValue();
        }

        public void UpdateFilterValue()
        {
            UpdateFilters(false);
        }
        #endregion

        #region Private Functions
        void UpdateFilters(bool sendApiRequest = false)
        {
            filters.Clear();
            foreach(DragonFilterDropdown filterDropdown in filterDropdowns)
            {
                MarketFilter filter = filterDropdown.CurrentFilter;
                if(filter.value.ToLower() != "any")
                {
                    filters.Add(filter);
                }
            }
            Message.Send(new UpdateMarketFilters(filters, sendApiRequest));
        }
        #endregion
    }
}
