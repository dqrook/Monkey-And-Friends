using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ryzm.EndlessRunner;
using Ryzm.EndlessRunner.Messages;
using CodeControl;
using Ryzm.UI.Messages;

namespace Ryzm.UI
{
    public class LoadingFadeMenu : RyzmMenu
    {
        #region Public Variables
        [Header("Background")]
        public Image background;
        public Color startColor;
        public Color endColor;

        [Header("Times")]
        public float fadeInTime = 1;
        public float pauseTime = 1;
        public float fadeOutTime = 1;
        [Range(0, 1)]
        public float startRunwayFractionTime = 0.5f;
        #endregion

        #region Private Variables
        MapType currentMap;
        IEnumerator startMenu;
        List<MenuType> activeMenus = new List<MenuType>();
        #endregion

        public override bool IsActive 
        { 
            get
            { 
                return base.IsActive;
            }
            set 
            {
                if(ShouldUpdate(value))
                {
                    if(value)
                    {
                        // Message.Send(new StartRunway())
                        Message.AddListener<CurrentMapResponse>(OnCurrentMapResponse);
                        Message.Send(new CurrentMapRequest());
                        startMenu = StartMenu();
                        StartCoroutine(startMenu);
                    }
                    else
                    {
                        StopAllCoroutines();
                        startMenu = null;
                        Message.RemoveListener<CurrentMapResponse>(OnCurrentMapResponse);
                    }
                    base.IsActive = value;
                }
            }
        }

        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            background.color = startColor;
        }
        #endregion

        #region Listener Functions
        void OnMenuSetResponse(MenuSetResponse response)
        {
            if(response.set == MenuSet.ActiveMenu)
            {
                activeMenus = response.menus;
            }
        }

        void OnCurrentMapResponse(CurrentMapResponse response)
        {
            currentMap = response.type;
        }
        #endregion

        #region Private Functions
        void SetColor(float fraction)
        {
            fraction = fraction < 0 ? 0 : fraction > 1 ? 1 : 0;
            Color clr = startColor * (1 - fraction) + endColor * fraction;
            background.color = clr;
        }
        #endregion

        #region Coroutines
        IEnumerator StartMenu()
        {
            float timer = 0;
            while(timer < fadeInTime)
            {
                timer += Time.deltaTime;
                float fraction = timer / fadeInTime;
                SetColor(fraction);
                yield return null;
            }
            SetColor(1);

            timer = 0;
            float startRunwayTime = startRunwayFractionTime * pauseTime;
            bool startedRunway = false;
            while(timer < pauseTime)
            {
                timer += Time.deltaTime;
                if(!startedRunway && timer > startRunwayTime)
                {
                    startedRunway = true;
                    Message.Send(new StartRunway(currentMap));
                }
                yield return null;
            }
            if(!startedRunway)
            {
                Message.Send(new StartRunway(currentMap));
            }
            
            timer = 0;
            while(timer < fadeOutTime)
            {
                timer += Time.deltaTime;
                float fraction = 1 - timer / fadeOutTime;
                SetColor(fraction);
                yield return null;
            }
            SetColor(0);
            Message.Send(new ActivateMenu(activatedTypes: activeMenus));
        }
        #endregion
    }
}
