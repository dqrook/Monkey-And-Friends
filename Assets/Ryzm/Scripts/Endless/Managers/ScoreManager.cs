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
        public int gemsCollected;

        void Awake()
        {
            Message.AddListener<GameStatusResponse>(OnGameStatusResponse);
            Message.AddListener<CollectCoin>(OnCollectCoin);
            Message.AddListener<RunnerDistanceResponse>(OnRunnerDistanceResponse);
            Message.AddListener<TotalCoinsRequest>(OnTotalCoinsRequest);
            Message.AddListener<CollectGem>(OnCollectGem);
        }

        void OnDestroy()
        {
            Message.RemoveListener<GameStatusResponse>(OnGameStatusResponse);
            Message.RemoveListener<CollectCoin>(OnCollectCoin);
            Message.RemoveListener<RunnerDistanceResponse>(OnRunnerDistanceResponse);
            Message.RemoveListener<TotalCoinsRequest>(OnTotalCoinsRequest);
            Message.RemoveListener<CollectGem>(OnCollectGem);
        }

        #region Listener Functions
        void OnGameStatusResponse(GameStatusResponse response)
        {
            if(response.status == GameStatus.Restart || response.status == GameStatus.Exit)
            {
                coinsCollected = 0;
                distanceTraveled = 0;
                gemsCollected = 0;
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

        void OnCollectGem(CollectGem collectGem)
        {
            gemsCollected += 1;
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
