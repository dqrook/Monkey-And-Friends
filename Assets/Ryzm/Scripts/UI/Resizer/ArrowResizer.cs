using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.UI
{
    public class ArrowResizer : ChildResizer
    {
        #region Public Variables
        public GameObject forwardArrow;
        public GameObject backwardArrow;
        #endregion

        #region Public Functions
        public void UpdateArrows(int currentPage, int maxPages)
        {
            backwardArrow.SetActive(currentPage > 0);
            forwardArrow.SetActive(currentPage < maxPages - 1);
        }

        public void UpdateArrows(int currentPage, int maxPages, float newWidth)
        {
            UpdateArrows(currentPage, maxPages);
            UpdateHeight(newWidth);
            if(!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
        #endregion
    }
}
