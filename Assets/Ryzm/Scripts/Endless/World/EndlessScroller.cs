using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class EndlessScroller : MonoBehaviour
    {
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
            trans = gameObject.transform;
        }

        protected virtual void OnEnable()
        {
            Message.Send(new GameStatusRequest());
            Message.Send(new GameSpeedRequest());
        }

        protected virtual void OnDisable() {}

        protected virtual void OnDestroy()
        {
            Message.RemoveListener<CurrentSectionChange>(OnCurrentSectionChange);
            Message.RemoveListener<GameStatusResponse>(OnGameStatusResponse);
            Message.RemoveListener<GameSpeedResponse>(OnGameSpeedResponse);
        }

        protected virtual void FixedUpdate()
        {
            if(!CanMove())
            {
                return;
            }
            MoveInY();
        }

        protected virtual bool CanMove()
        {
            return gameStatus == GameStatus.Active;
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

        protected virtual void OnGameStatusResponse(GameStatusResponse gameStatusResponse)
        {
            gameStatus = gameStatusResponse.status;
        }

        void OnGameSpeedResponse(GameSpeedResponse response)
        {
            gameSpeed = response.speed;
        }
    }
}
