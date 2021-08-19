using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ryzm.Dragon;
using Ryzm.UI.Messages;
using CodeControl;

namespace Ryzm.UI
{
    public class Content : MonoBehaviour
    {
        #region Public Variables
        public RectTransform rectTransform;
        public VerticalLayoutGroup layoutGroup;
        public List<ScrollCanvasGroup> canvasGroups = new List<ScrollCanvasGroup>();
        public int maxNumberOfCards = 10;
        public int maxNumberOfCardsPerRow = 5;
        public int cardMinWidth = 400;
        public int cardMaxWidth = 800;
        public int cardSpacing = 20;
        public int rowSpacing = 50;
        public int scrollbarOffset = 100;
        public Vector2 cardDimensions = new Vector2(600, 800);
        #endregion

        #region Private Variables
        List<Breakpoint> widthBreakPoints = new List<Breakpoint>();
        float currentRowWidth;
        DragonCardMetadata[] dragonCards;
        #endregion

        #region Event Functions
        void Awake()
        {
            dragonCards = new DragonCardMetadata[0];
            if(rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }
            if(layoutGroup == null)
            {
                layoutGroup = GetComponent<VerticalLayoutGroup>();
            }
            Message.AddListener<DragonCardMetadataResponse>(OnDragonCardMetadataResponse);
            CreateBreakPoints();
        }

        void OnDestroy()
        {
            Message.RemoveListener<DragonCardMetadataResponse>(OnDragonCardMetadataResponse);
        }
        #endregion

        #region Listener Functions
        void OnDragonCardMetadataResponse(DragonCardMetadataResponse response)
        {
            dragonCards = response.dragonCards;
            if(currentRowWidth > 0)
            {
                UpdateDimensions(currentRowWidth);
            }
        }
        #endregion

        #region Public Functions
        public void UpdateVisibility()
        {
            foreach(ScrollCanvasGroup group in canvasGroups)
            {
                group.UpdateVisibility();
            }
        }

        public void UpdateDimensions(float newRowWidth)
        {
            currentRowWidth = newRowWidth;
            Breakpoint point = GetBreakpoint(newRowWidth);
            int noSpacingHeight = Mathf.FloorToInt((newRowWidth / point.numberOfCards) * cardDimensions.y / cardDimensions.x);
            int availableWidth = Mathf.FloorToInt(newRowWidth - cardSpacing * point.numberOfCards - scrollbarOffset);
            int cardWidth = Mathf.FloorToInt(availableWidth / point.numberOfCards);
            int cardHeight = Mathf.FloorToInt(cardWidth * cardDimensions.y / cardDimensions.x);
            // int cardsLeft = maxNumberOfCards;
            int cardsLeft = dragonCards.Length < maxNumberOfCards ? dragonCards.Length : maxNumberOfCards;
            int numRows = 0;
            int dragonIndex = 0;
            foreach(ScrollCanvasGroup group in canvasGroups)
            {
                int cards2Activate = cardsLeft >= point.numberOfCards ? point.numberOfCards : cardsLeft;
                cards2Activate = cards2Activate > 0 ? cards2Activate : 0;
                if(cards2Activate > 0)
                {
                    numRows += 1;
                }
                group.UpdateDimensions(newRowWidth, cardWidth, cardHeight, cards2Activate, dragonIndex);
                cardsLeft -= cards2Activate;
                dragonIndex += cards2Activate;
            }
            int newRowHeight = noSpacingHeight * numRows;
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, newRowHeight);
        }
        #endregion

        #region Private Functions
        void CreateBreakPoints()
        {
            if(widthBreakPoints.Count == 0)
            {
                for(int i = maxNumberOfCardsPerRow; i > 1; i--)
                {
                    Breakpoint point = new Breakpoint();
                    point.numberOfCards = i;
                    point.minWidth = (cardMinWidth + cardSpacing) * i;
                    widthBreakPoints.Add(point);
                }
            }
        }

        Breakpoint GetBreakpoint(float width)
        {
            CreateBreakPoints();
            foreach(Breakpoint point in widthBreakPoints)
            {
                if(point.minWidth < width)
                {
                    return point;
                }
            }
            return widthBreakPoints[widthBreakPoints.Count - 1];
        }
        #endregion
    }

    public struct Breakpoint
    {
        public int numberOfCards;
        public int minWidth;
    }
}
