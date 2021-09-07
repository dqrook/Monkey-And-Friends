using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;
using TMPro;

namespace Ryzm.UI
{
    public class ScoreMenu : RyzmMenu
    {
        #region Public Variables
        public TextMeshProUGUI score;
        #endregion

        #region Private Variables
        int currentScore;
        #endregion

        #region Properties
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
                        Message.AddListener<TotalCoinsResponse>(OnTotalCoinsResponse);
                        Message.Send(new TotalCoinsRequest());
                    }
                    else
                    {
                        Message.RemoveListener<TotalCoinsResponse>(OnTotalCoinsResponse);
                        currentScore = -1;
                    }
                    base.IsActive = value;
                }
            }
        }
        #endregion

        #region Listener Functions
        void OnTotalCoinsResponse(TotalCoinsResponse response)
        {
            if(response.coinsCollected != currentScore)
            {
                currentScore = response.coinsCollected;
                score.text = currentScore.ToString();
            }
        }
        #endregion
    }
}
