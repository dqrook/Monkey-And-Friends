using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class EndlessScroller : MonoBehaviour
    {
        protected RunnerController runner;
        protected Transform runnerTrans;
        protected GameObject _currentSection;
        protected GameStatus gameStatus;
        protected float gameSpeed;

        protected virtual void Awake()
        {
            Message.AddListener<CurrentSectionChange>(OnCurrentSectionChange);
            Message.AddListener<GameStatusResponse>(OnGameStatusResponse);
            Message.AddListener<GameSpeedResponse>(OnGameSpeedResponse);
            Message.AddListener<RunnerResponse>(OnRunnerResponse);
        }

        protected virtual void OnEnable()
        {
            Message.Send(new GameStatusRequest());
            Message.Send(new GameSpeedRequest());
            Message.Send(new RunnerRequest());
        }

        protected virtual void OnDestroy()
        {
            Message.RemoveListener<CurrentSectionChange>(OnCurrentSectionChange);
            Message.RemoveListener<GameStatusResponse>(OnGameStatusResponse);
            Message.RemoveListener<GameSpeedResponse>(OnGameSpeedResponse);
            Message.RemoveListener<RunnerResponse>(OnRunnerResponse);
        }

        protected virtual void FixedUpdate()
        {
            if(!CanMove())
            {
                return;
            }
            GetRunner();
            MoveForward(GameManager.Instance.speed);
            MoveInY();
        }

        protected bool CanMove()
        {
            return gameStatus == GameStatus.Active;
        }

        protected void GetRunner()
        {
            if(runnerTrans == null && runner != null)
            {
                runnerTrans = runner.gameObject.transform;
            }
        }

        protected virtual void MoveForward(float speed)
        {
            this.transform.position += runnerTrans.forward * - speed;
        }

        protected void MoveInY()
        {
            if(_currentSection != null)
            {
                if(_currentSection.tag == "stairsUp") 
                {
                    this.transform.Translate(0, -0.06f, 0);
                }

                if(_currentSection.tag == "stairsDown") 
                {
                    this.transform.Translate(0, 0.06f, 0);
                }
            }
        }

        void OnCurrentSectionChange(CurrentSectionChange sectionChange)
        {
            _currentSection = sectionChange.section;
        }

        void OnGameStatusResponse(GameStatusResponse gameStatusResponse)
        {
            gameStatus = gameStatusResponse.status;
        }

        void OnGameSpeedResponse(GameSpeedResponse response)
        {
            gameSpeed = response.speed;
        }

        void OnRunnerResponse(RunnerResponse response)
        {
            runner = response.runner;
            runnerTrans = runner.gameObject.transform;
        }
    }
}
