using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class EndlessScroller : MonoBehaviour
    {
        protected Transform runner;
        protected GameObject _currentSection;
        protected GameStatus gameStatus;

        void OnEnable()
        {
            Message.AddListener<CurrentSectionChange>(OnCurrentSectionChange);
            Message.AddListener<GameStatusResponse>(OnGameStatusResponse);
            Message.Send(new GameStatusRequest());
        }

        void OnDisable()
        {
            Message.RemoveListener<CurrentSectionChange>(OnCurrentSectionChange);
            Message.RemoveListener<GameStatusResponse>(OnGameStatusResponse);
        }

        protected void FixedUpdate()
        {
            if(runner == null)
            {
                runner = RunnerController.player.transform;
            }
            if(RunnerController.isDead)
            {
                return;
            }
            this.transform.position += runner.forward * - GameManager.Instance.speed;

            if(_currentSection == null) return;

            if(_currentSection.tag == "stairsUp") 
            {
                this.transform.Translate(0, -0.06f, 0);
            }

            if(_currentSection.tag == "stairsDown") 
            {
                this.transform.Translate(0, 0.06f, 0);
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
    }
}
