using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Ryzm.UI
{
    public class ColumnResizer : MonoBehaviour
    {
        #region Public Variables
        public Canvas canvas;
        public RectTransform rectTransform;
        public RectTransform parentTransform;

        [Header("Resizers")]
        public List<ResizerPage> pages;
        public ArrowResizer arrowResizer;
        
        [Header("Constraints")]
        public float xOffset;
        public float yOffset;
        [Range(0, 100)]
        public float maxWidthPercent = 40;
        [Range(0, 100)]
        public float minWidthPercent = 20;
        [Range(0, 1000)]
        public float minWidthPixels = 100;
        [Range(0, 100)]
        public float maxHeightPercent = 90;
        #endregion

        #region Private Variables
        float _spacing = -1;
        float _totalInverseRatio;
        int numChildren;
        bool paginated;
        int currentPage;
        bool prevPaginated;
        float _highestInverseRatio;
        #endregion

        #region Properties
        float SpacingOffset
        {
            get
            {
                if(_spacing < 0)
                {
                    SetSpacing();
                }
                int _numChildren = numChildren;
                // if(paginated)
                // {
                //     _numChildren = 0;
                //     foreach(ResizerPage page in pages)
                //     {
                //         if(page.index == currentPage)
                //         {
                //             _numChildren += page.resizers.Count;
                //             break;
                //         }
                //     }
                //     _numChildren += 1; // for arrow resizer
                // }
                return _spacing * _numChildren;
            }
        }
        
        float TotalInverseRatio
        {
            get
            {
                if(_totalInverseRatio == 0)
                {
                    SetTotalInverseRatio();
                }
                return _totalInverseRatio;
            }
        }

        float HighestInverseRatio
        {
            get
            {
                if(_highestInverseRatio == 0)
                {
                    SetHighestInverseRatio();
                }
                return _highestInverseRatio;
            }
        }
        #endregion

        #region Event Functions
        void Awake()
        {
            if(canvas == null)
            {
                canvas = GetComponent<Canvas>();
            }
            if(rectTransform == null)
            {
                rectTransform = GetComponent<RectTransform>();
            }
            if(!paginated)
            {
                arrowResizer.Disable();
            }
            SetSpacing();
            SetTotalInverseRatio();
            SetHighestInverseRatio();
        }

        void OnRectTransformDimensionsChange()
        {
            UpdateChildDimensions();
        }
        #endregion

        #region Public Functions
        public void Enable()
        {
            canvas.enabled = true;
        }

        public void OnClickArrow(bool isForward)
        {
            if(isForward)
            {
                currentPage++;
            }
            else
            {
                currentPage--;
            }
            foreach(ResizerPage page in pages)
            {
                page.Activate(currentPage);
            }
            arrowResizer.UpdateArrows(currentPage, pages.Count);
            UpdateChildDimensions();
        }

        public void Disable()
        {
            currentPage = 0;
            paginated = false;
            prevPaginated = false;
            arrowResizer.Disable();
            canvas.enabled = false;
        }
        #endregion

        #region Private Functions
        void UpdateChildDimensions()
        {
            float _yOffset = yOffset + SpacingOffset;
            float widthAvailable = (rectTransform.rect.width - xOffset) * maxWidthPercent / 100;
            widthAvailable -= 10;
            widthAvailable = widthAvailable > 1 ? widthAvailable : 1;
            float heightAvailable = (rectTransform.rect.height - _yOffset) * maxHeightPercent / 100;
            heightAvailable = heightAvailable > 1 ? heightAvailable : 1;
            float minWidthAvailable = (rectTransform.rect.width - xOffset) * minWidthPercent / 100;
            minWidthAvailable += 10;
            minWidthAvailable = minWidthAvailable > minWidthPixels ? minWidthAvailable : minWidthPixels;

            float totalInverseRatio = TotalInverseRatio;
            float newWidth = heightAvailable / totalInverseRatio;
            if(newWidth > widthAvailable)
            {
                newWidth = widthAvailable;
            }
            if(!paginated)
            {
                if(newWidth < minWidthAvailable && widthAvailable > minWidthAvailable)
                {
                    paginated = true;
                    totalInverseRatio = GetInversePageRatio(0);
                    totalInverseRatio += 1 / arrowResizer.Ratio;
                    newWidth = heightAvailable / totalInverseRatio;
                    if(newWidth > widthAvailable)
                    {
                        newWidth = widthAvailable;
                    }
                }
                UpdateDimensions(newWidth);
            }
            else
            {
                if(newWidth > minWidthAvailable || widthAvailable < minWidthAvailable)
                {
                    paginated = false;
                }
                else
                {
                    totalInverseRatio = GetInversePageRatio(0);
                    newWidth = heightAvailable / totalInverseRatio;
                    if(newWidth > widthAvailable)
                    {
                        newWidth = widthAvailable;
                    }
                }
                UpdateDimensions(newWidth);
            }
            // Debug.Log(newWidth + " " + minWidthAvailable + " " + widthAvailable + " " + paginated);
        }

        float GetInversePageRatio()
        {
            return GetInversePageRatio(currentPage);
        }

        float GetInversePageRatio(int pageIndex)
        {
            // float ratio = 0;
            // foreach(ResizerPage page in pages)
            // {
            //     if(page.index == pageIndex)
            //     {
            //         ratio += page.TotalInverseRatio;
            //     }
            // }
            // if(paginated)
            // {
            //     ratio += 1 / arrowResizer.Ratio;
            // }
            // return ratio;
            return HighestInverseRatio +  1 / arrowResizer.Ratio;
        }

        void UpdateDimensions(float newWidth)
        {
            parentTransform.sizeDelta = new Vector2(newWidth, parentTransform.rect.height);
            foreach(ResizerPage page in pages)
            {
                page.UpdateDimensions(newWidth);
            }

            if(!paginated && prevPaginated)
            {
                currentPage = 0;
                arrowResizer.Disable();
                foreach(ResizerPage page in pages)
                {
                    page.Activate();
                }
            }
            else if (paginated && !prevPaginated)
            {
                arrowResizer.UpdateArrows(currentPage, pages.Count, newWidth);
                foreach(ResizerPage page in pages)
                {
                    page.Activate(currentPage);
                }
            }
            else if(paginated)
            {
                arrowResizer.UpdateDimensions(newWidth);
            }

            prevPaginated = paginated;
        }

        void SetSpacing()
        {
            VerticalLayoutGroup group = parentTransform.GetComponent<VerticalLayoutGroup>();
            _spacing = group != null ? group.spacing : 0;
            numChildren = 0;
            foreach(ResizerPage page in pages)
            {
                numChildren += page.resizers.Count;
            }
        }

        void SetTotalInverseRatio()
        {
            _totalInverseRatio = 0;
            foreach(ResizerPage page in pages)
            {
                _totalInverseRatio += page.TotalInverseRatio;
                // foreach(ChildResizer child in page.resizers)
                // {
                //     _totalInverseRatio += 1 / child.Ratio;
                // }
            }
        }

        void SetHighestInverseRatio()
        {
            _highestInverseRatio = 0;
            foreach(ResizerPage page in pages)
            {
                if(page.TotalInverseRatio > _highestInverseRatio)
                {
                    _highestInverseRatio = page.TotalInverseRatio;
                }
            }
        }
        #endregion
    }

    [System.Serializable]
    public class ResizerPage
    {
        public int index;
        public List<ChildResizer> resizers = new List<ChildResizer>();

        public float TotalInverseRatio
        {
            get
            {
                float ratio = 0;
                foreach(ChildResizer resizer in resizers)
                {
                    ratio += 1 / resizer.Ratio;
                }
                return ratio;
            }
        }

        public void UpdateDimensions(float newWidth)
        {
            foreach(ChildResizer resizer in resizers)
            {
                resizer.UpdateDimensions(newWidth);
            }
        }

        public void Activate()
        {
            foreach(ChildResizer resizer in resizers)
            {
                resizer.gameObject.SetActive(true);
            }
        }

        public void Activate(int currentPage)
        {
            foreach(ChildResizer resizer in resizers)
            {
                resizer.gameObject.SetActive(index == currentPage);
            }
        }
    }
}
