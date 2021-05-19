using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner.UI
{
    public abstract class EndlessMenu : MonoBehaviour
    {
        #region Public Variables
        public Canvas canvas;
        public MenuType type;
        [Header("Resize")]
        public bool restricWidth;
        public bool restricHeight;
        #endregion

        #region Nonserialized Public Variables
        [NonSerialized] public bool placedResizer;
        #endregion

        protected Resolution prevResolution;
        protected RectTransform canvasResizer;
        protected List<Transform> Children = new List<Transform>();
        protected bool _isActive;

        #region Properties
        public virtual bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                PlaceResizer();
                if(value != _isActive)
                {
                    _isActive = value;
                    if(canvas != null)
                    {
                        canvas.enabled = value;
                    }
                    foreach(Transform child in gameObject.transform)
                    {
                        child.gameObject.SetActive(value);
                    }
                }
            }
        }
        #endregion

        protected virtual void Awake()
        {
            if(canvas == null)
            {
                canvas = GetComponent<Canvas>();
            }
            Message.AddListener<ActivateMenu>(OnActivateMenu);
            Message.AddListener<DeactivateMenu>(OnDeactivateMenu);
        }

        protected virtual void OnDestroy()
        {
            Message.RemoveListener<ActivateMenu>(OnActivateMenu);
            Message.RemoveListener<DeactivateMenu>(OnDeactivateMenu);
        }

        protected virtual void OnEnable() {}

        protected virtual void OnDisable() {}

        protected virtual void OnActivateMenu(ActivateMenu activate)
        {
            if(activate.type == type)
            {
                IsActive = true;
            }
            else if(activate.useActivated)
            {
                if(activate.activatedTypes.Contains(type))
                {
                    IsActive = true;
                }
            }
            else if(!activate.deactivatedTypes.Contains(type))
            {
                IsActive = true;
            }
        }

        protected virtual void OnDeactivateMenu(DeactivateMenu deactivate)
        {
            if(deactivate.useActivated)
            {
                if(!deactivate.activatedTypes.Contains(type))
                {
                    IsActive = false; 
                }
            } 
            else if(deactivate.type == type || deactivate.deactivatedTypes.Contains(type))
            {
                IsActive = false;
            }
        }

        // todo: figure out how to use this (if necessary)
        protected void PlaceResizer()
        {
            // if(!restricWidth && !restricHeight)
            // {
            //     return;
            // }
            // if (!placedResizer || menuManager.rYZ.settingsManager.Resolution != prevResolution)
            // {
            //     var r = Screen.safeArea;
            //     if (Application.isEditor && menuManager.Sim != SimDevice.None)
            //     {
            //         Rect nsa = new Rect(0, 0, Screen.width, Screen.height);

            //         switch (menuManager.Sim)
            //         {
            //             case SimDevice.iPhoneX:
            //                 if (Screen.height > Screen.width)  // Portrait
            //                     nsa = menuManager.NSA_iPhoneX[0];
            //                 else  // Landscape
            //                     nsa = menuManager.NSA_iPhoneX[1];
            //                 break;
            //             default:
            //                 break;
            //         }

            //         r = new Rect(Screen.width * nsa.x, Screen.height * nsa.y, Screen.width * nsa.width, Screen.height * nsa.height);
            //     }
            //     if(!restricWidth)
            //     {
            //         r.x = 0;
            //         r.width = Screen.width;
            //     }
            //     if(!restricHeight)
            //     {
            //         r.y = 0;
            //         r.height = Screen.height;
            //     }

            //     if (r.width != Screen.width || r.height != Screen.height)
            //     {
            //         if (!placedResizer)
            //         {
            //             Children.Clear();
            //             foreach (Transform child in gameObject.transform)
            //             {
            //                 Children.Add(child);
            //             }
            //             canvasResizer = Instantiate(menuManager.canvasResizerPrefab, transform).GetComponent<RectTransform>();
            //         }
            //         canvasResizer.gameObject.SetActive(false);
            //         foreach (Transform child in Children)
            //         {
            //             child.parent = canvasResizer.transform;
            //             child.gameObject.SetActive(true);
            //         }
            //         var anchorMin = r.position;
            //         var anchorMax = r.position + r.size;

            //         anchorMin.x /= Screen.width;
            //         anchorMin.y /= Screen.height;
            //         anchorMax.x /= Screen.width;
            //         anchorMax.y /= Screen.height;

            //         canvasResizer.anchorMin = anchorMin;
            //         canvasResizer.anchorMax = anchorMax;
            //         Debug.LogFormat("New safe area applied to {0}: x={1}, y={2}, w={3}, h={4} on full extents w={5}, h={6}",
            //         name, r.x, r.y, r.width, r.height, Screen.width, Screen.height);
            //     }
            //     placedResizer = true;
            // }
            // prevResolution = menuManager.rYZ.settingsManager.Resolution;
        }
    }
    public enum SimDevice { None, iPhoneX }

    public enum MenuType
    {
        None,
        Score,
        Distance,
        SwipeZone,
        Pause,
        EndGame,
        Main
    }
}
