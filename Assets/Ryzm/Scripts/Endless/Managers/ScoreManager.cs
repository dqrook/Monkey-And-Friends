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

        void OnGameStatusResponse(GameStatusResponse response)
        {
            if(response.status == GameStatus.Restart)
            {
                coinsCollected = 0;
                distanceTraveled = 0;
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

        void UpdateCoins()
        {
            Message.Send(new TotalCoinsResponse(coinsCollected));
        }
        
    }
}
