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

        protected override void Awake()
        {
            base.Awake();
            Message.AddListener<TotalCoinsResponse>(OnTotalCoinsResponse);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Message.RemoveListener<TotalCoinsResponse>(OnTotalCoinsResponse);
        }

        protected override void OnActivateMenu(ActivateMenu activate)
        {
            Message.Send(new TotalCoinsRequest());
            base.OnActivateMenu(activate);
        }

        protected override void OnDeactivateMenu(DeactivateMenu deactivate)
        {
            base.OnDeactivateMenu(deactivate);
            currentScore = 0;
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
