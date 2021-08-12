using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.UI
{
    public class ChildResizer : MonoBehaviour
    {
        #region Public Variables
        public RectTransform rectTransform;
        public float defaultHeight;
        public float defaultWidth;
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
        public void UpdateHeight(float newWidth)
        {
            float newHeight = newWidth / Ratio;
            RectTransform.sizeDelta = new Vector2(newWidth, newHeight);
        }
        #endregion
    }
}
