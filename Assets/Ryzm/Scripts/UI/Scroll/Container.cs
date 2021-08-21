using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ryzm.UI
{
    public class Container : MonoBehaviour
    {
        #region Public Variables
        public RectTransform rectTransform;
        public VerticalLayoutGroup layoutGroup;
        public HeaderRow header;
        public ScrollView scrollView;
        public float yOffset = 20;
        public RectTransform noDragonsPanel;
        public bool disableScrollView;
        #endregion

        #region Event Functions
        void OnRectTransformDimensionsChange()
        {
            Debug.Log("changed");
            if(rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }
            float totalWidth = rectTransform.sizeDelta.x;
            float totalHeight = rectTransform.sizeDelta.y;
            header.UpdateWidth(totalWidth - header.widthOffset);
            if(!disableScrollView)
            {
                float availableHeight = totalHeight - header.Dimensions.y - yOffset - layoutGroup.spacing;
                float newWidth = totalWidth - scrollView.widthOffset;
                float newHeight = availableHeight;
                scrollView.UpdateDimensions(newWidth, newHeight);
                if(noDragonsPanel != null)
                {
                    noDragonsPanel.sizeDelta = new Vector2(newWidth, newHeight);
                }
            }
        }
        #endregion
    }
}
