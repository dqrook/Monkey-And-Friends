using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class EndlessDeyonSpawn : EndlessSectionSpawn
    {
        #region Private Variables
        float runnerDistance;
        int difficultyLevel;
        bool shouldActivate;
        #endregion

        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            Message.AddListener<RunnerDistanceResponse>(OnRunnerDistanceResponse);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Message.RemoveListener<RunnerDistanceResponse>(OnRunnerDistanceResponse);
        }
        #endregion

        #region Listener Functions
        void OnRunnerDistanceResponse(RunnerDistanceResponse response)
        {
            runnerDistance = response.distance;
            difficultyLevel = Mathf.FloorToInt((runnerDistance + 50) / 100);
            // Debug.Log(difficultyLevel);
            if(shouldActivate)
            {
                shouldActivate = false;
                _Activate();
            }
        }
        #endregion

        #region Public Functions
        public override void Activate()
        {
            shouldActivate = true;
            Message.Send(new RunnerDistanceRequest());
        }
        #endregion

        void _Activate()
        {
            int numDeyons = 1;
            if(difficultyLevel > 10)
            {
                numDeyons = Random.Range(2, 5);
            }
            else if(difficultyLevel > 7)
            {
                numDeyons = Random.Range(2, 4);
            }
            else if(difficultyLevel > 4)
            {
                numDeyons = Random.Range(2, 5);
            }
            else if(difficultyLevel > 1)
            {
                numDeyons = Random.Range(1, 3);
            }

            int numSpawned = 0;

            foreach(EndlessMonster deyon in monsters)
            {
                if(numSpawned < numDeyons)
                {
                    deyon.gameObject.SetActive(true);
                    numSpawned++;
                }
                else
                {
                    deyon.gameObject.SetActive(false);
                }
            }
        }
    }
}
