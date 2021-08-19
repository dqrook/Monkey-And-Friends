using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ryzm.UI
{
    public class ScrollCanvasGroup : MonoBehaviour
    {
        #region Public Variables
        public RectTransform rectTransform;
        public CanvasGroup canvasGroup;
        public HorizontalLayoutGroup layoutGroup;
        public List<DragonCard> cards = new List<DragonCard>();
        public int maxNumberOfCards = 5;
        public float cardMinWidth = 400;
        public float cardMaxWidth = 800;
        #endregion

        #region Private Variables
        int numActiveCards;
        bool hasCards;
        #endregion

        #region Properties
        Vector2 Dimensions 
        {
            get
            {
                return rectTransform.sizeDelta;
            }
        }

        float YPos
        {
            get
            {
                return rectTransform.anchoredPosition.y;
            }
        }
        #endregion

        #region Event Functions
        void Awake()
        {
            numActiveCards = maxNumberOfCards;
            if(rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }
            if(canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
            }
            if(layoutGroup == null)
            {
                layoutGroup = GetComponent<HorizontalLayoutGroup>();
            }
        }
        #endregion

        #region Public Functions
        public void UpdateVisibility()
        {
            if(hasCards)
            {
                bool isVisible = ScrollHider.HideObject(canvasGroup, YPos, Dimensions.y);
            }
            // if(hasCards && !isVisible)
            // {
            //     if(canvasGroup.alpha != 0)
            //     {

            //     }
            // }
            // Debug.Log(isVisible + " " + YPos + " " + Dimensions.y);
        }

        public void SetHasCards(bool hasCards)
        {
            this.hasCards = hasCards;
            if(hasCards && canvasGroup.alpha != 1)
            {
                canvasGroup.alpha = 1;
            }
            else if(!hasCards && canvasGroup.alpha != 0)
            {
                canvasGroup.alpha = 0;
            }
        }

        public void UpdateDimensions(float newRowWidth, float cardWidth, float cardHeight, int cards2Activate, int dragonIndex)
        {
            rectTransform.sizeDelta = new Vector2(newRowWidth, cardHeight);
            if(cards2Activate > 0)
            {
                SetHasCards(true);
                int cardsLeft = cards2Activate;
                int curDragonIndex = dragonIndex;
                foreach(DragonCard card in cards)
                {
                    if(cardsLeft > 0)
                    {
                        card.gameObject.SetActive(true);
                        card.UpdateCard(curDragonIndex, cardWidth, cardHeight);
                        curDragonIndex++;
                        cardsLeft--;
                    }
                    else
                    {
                        card.Deactivate();
                    }
                }
            }
            else
            {
                SetHasCards(false);
            }
        }
        #endregion
    }
}
