using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class ScoreManager : MonoBehaviour
    {
        public float distanceTraveled;
        public int coinsCollected;

        void Awake()
        {
            Message.AddListener<GameStatusResponse>(OnGameStatusResponse);
            Message.AddListener<CollectCoin>(OnCollectCoin);
            Message.AddListener<RunnerDistanceResponse>(OnRunnerDistanceResponse);
            Message.AddListener<TotalCoinsRequest>(OnTotalCoinsRequest);
        }

        void OnDestroy()
        {
            Message.RemoveListener<GameStatusResponse>(OnGameStatusResponse);
            Message.RemoveListener<CollectCoin>(OnCollectCoin);
            Message.RemoveListener<RunnerDistanceResponse>(OnRunnerDistanceResponse);
            Message.RemoveListener<TotalCoinsRequest>(OnTotalCoinsRequest);
        }

        #region Listener Functions
        void OnGameStatusResponse(GameStatusResponse response)
        {
            if(response.status == GameStatus.Restart || response.status == GameStatus.Exit)
            {
                coinsCollected = 0;
                distanceTraveled = 0;
                UpdateCoins();
            }
        }

        void OnRunnerDistanceResponse(RunnerDistanceResponse response)
        {
            distanceTraveled = response.distance;
        }
       
        void OnCollectCoin(CollectCoin collectCoin)
        {
            coinsCollected += 1;
            UpdateCoins();
        }

        void OnTotalCoinsRequest(TotalCoinsRequest request)
        {
            UpdateCoins();
        }
        #endregion

        #region Private Functions
        void UpdateCoins()
        {
            Message.Send(new TotalCoinsResponse(coinsCollected));
        }
        #endregion
        
    }
}
