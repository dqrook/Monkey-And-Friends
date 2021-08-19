using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ryzm.UI
{
    public class ScrollView : MonoBehaviour
    {
        #region Public Variables
        public ScrollRect rect;
        public Content content;
        public float widthOffset = 100;
        public RectTransform rectTransform;
        #endregion

        #region Event Functions
        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        void Start()
        {
            UpdateVisibility();
        }
        #endregion
        
        #region Public Functions
        public void OnValueChanged(Vector2 value)
        {
            UpdateVisibility();
        }

        public void UpdateDimensions(float newWidth, float newHeight)
        {
            rectTransform.sizeDelta = new Vector2(newWidth, newHeight);
            content.UpdateDimensions(newWidth);
        }
        #endregion

        #region Private Functions
        void UpdateVisibility()
        {
            ScrollHider.Setup(rect);
            content.UpdateVisibility();
        }
        #endregion
    }
}
