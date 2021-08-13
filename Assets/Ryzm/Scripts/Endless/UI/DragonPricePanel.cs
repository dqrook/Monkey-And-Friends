using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.Dragon;
using TMPro;

namespace Ryzm.UI
{
    public class DragonPricePanel : MonoBehaviour
    {
        #region Public Variables
        public PricePanelType type;
        public TextMeshProUGUI dragonPrice;
        public GameObject buyButton;
        public GameObject editButton;
        #endregion

        #region Public Functions
        public void Initialize(DragonResponse data, bool isUser)
        {
            dragonPrice.text = data.price > 0 ? data.price + " Near" : type == PricePanelType.Sell ? "NOT FOR SALE" : "NOT FOR RENT";
            buyButton.SetActive(!isUser);
            editButton.SetActive(isUser);
        }
        #endregion
    }

    public enum PricePanelType
    {
        Sell,
        Rent
    }
}
