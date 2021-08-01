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
                        Debug.Log("open loading fade menu");
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
        protected override void OnUpdateLoadingFadeMenu(UpdateLoadingFadeMenu update)
        {
            base.OnUpdateLoadingFadeMenu(update);
            SetColor(update.fadeFraction);
        }
        #endregion

        #region Private Functions
        void SetColor(float fraction)
        {
            fraction = fraction < 0 ? 0 : fraction > 1 ? 1 : fraction;
            Color clr = startColor * (1 - fraction) + endColor * fraction;
            background.color = clr;
        }
        #endregion
    }
}
