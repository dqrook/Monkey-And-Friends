using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.UI
{
    public class ChildResizer : MonoBehaviour
    {
        #region Public Variables
        public RectTransform rectTransform;
        public float defaultWidth;
        public float defaultHeight;
        #endregion

        #region Properties
        public float Ratio
        {
            get
            {
                return defaultWidth / defaultHeight;
            }
        }

        RectTransform RectTransform
        {
            get
            {
                if(rectTransform == null)
                {
                    rectTransform = GetComponent<RectTransform>();
                }
                return rectTransform;
            }
        }
        #endregion

        #region Public Functions
        public void UpdateDimensions(float newWidth)
        {
            float newHeight = newWidth / Ratio;
            RectTransform.sizeDelta = new Vector2(newWidth, newHeight);
        }
        #endregion
    }
}
