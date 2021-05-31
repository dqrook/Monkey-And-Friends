using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;
using TMPro;

namespace Ryzm.EndlessRunner.UI
{
    public class ScoreMenu : EndlessMenu
    {
        public TextMeshProUGUI score;
        int currentScore;

        public override bool IsActive
        {
            get
            {
                return base.IsActive;
            }
            set
            {
                if(value)
                {
                    Message.AddListener<TotalCoinsResponse>(OnTotalCoinsResponse);
                    Message.Send(new TotalCoinsRequest());
                }
                else
                {
                    Message.RemoveListener<TotalCoinsResponse>(OnTotalCoinsResponse);
                    currentScore = 0;
                }
                base.IsActive = value;
            }
        }

        void OnTotalCoinsResponse(TotalCoinsResponse response)
        {
            if(response.coinsCollected != currentScore)
            {
                currentScore = response.coinsCollected;
                score.text = currentScore.ToString();
            }
        }
    }
}
