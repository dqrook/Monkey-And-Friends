using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.UI
{
    public class HeaderRow : MonoBehaviour
    {
        #region Public Variables
        public RectTransform rectTransform;
        public float widthOffset;
        #endregion

        #region Properties
        public Vector2 Dimensions
        {
            get
            {
                return rectTransform.sizeDelta;
            }
        }
        #endregion

        #region Event Functions
        void Awake()
        {
            if(rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }
        }
        #endregion

        #region Public Variables
        public void UpdateWidth(float newWidth)
        {
            rectTransform.sizeDelta = new Vector2(newWidth, rectTransform.sizeDelta.y);
        }
        #endregion
    }
}
