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
        protected GameObject _currentSectionGO;
        protected EndlessSection _currentSection;
        protected GameStatus gameStatus;
        protected float gameSpeed;
        protected Transform trans;

        protected virtual void Awake()
        {
            Message.AddListener<CurrentSectionChange>(OnCurrentSectionChange);
            Message.AddListener<GameStatusResponse>(OnGameStatusResponse);
            Message.AddListener<GameSpeedResponse>(OnGameSpeedResponse);
            Message.AddListener<RunnerResponse>(OnRunnerResponse);
            trans = gameObject.transform;
        }

        protected virtual void OnEnable()
        {
            Message.Send(new GameStatusRequest());
            Message.Send(new GameSpeedRequest());
            Message.Send(new RunnerRequest());
        }

        protected virtual void OnDisable() {}

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

        protected virtual bool CanMove()
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
            // trans.position += runnerTrans.forward * - speed;
        }

        protected void MoveInY()
        {
            if(_currentSectionGO != null)
            {
                if(_currentSectionGO.tag == "stairsUp") 
                {
                    trans.Translate(0, -0.06f, 0);
                }

                if(_currentSectionGO.tag == "stairsDown") 
                {
                    trans.Translate(0, 0.06f, 0);
                }
            }
        }

        protected virtual void OnCurrentSectionChange(CurrentSectionChange sectionChange)
        {
            _currentSectionGO = sectionChange.section;
            _currentSection = sectionChange.endlessSection;
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
