using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using CodeControl;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner.UI
{
    public class EndGameMenu : EndlessMenu
    {
        public TextMeshProUGUI distance;
        public TextMeshProUGUI score;

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
                    Message.AddListener<RunnerDistanceResponse>(OnRunnerDistanceResponse);
                    Message.AddListener<TotalCoinsResponse>(OnTotalCoinsResponse);
                    Message.Send(new RunnerDistanceRequest());
                    Message.Send(new TotalCoinsRequest());
                }
                else
                {
                    Message.RemoveListener<RunnerDistanceResponse>(OnRunnerDistanceResponse);
                    Message.RemoveListener<TotalCoinsResponse>(OnTotalCoinsResponse);
                }
                base.IsActive = value;
            }
        }

        void OnRunnerDistanceResponse(RunnerDistanceResponse response)
        {
            int _distance = Mathf.RoundToInt(response.distance);
            distance.text = _distance.ToString() + " m";
        }

        void OnTotalCoinsResponse(TotalCoinsResponse response)
        {
            score.text = response.coinsCollected.ToString();
        }

        public void OnClickRestart()
        {
            Message.Send(new RestartGame());
        }

        public void OnClickReturnToMain()
        {

        }
    }
}
