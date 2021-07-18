using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class EndlessScroller : EndlessItem
    {
        protected EndlessSection _currentSection;

        protected override void Awake()
        {
            base.Awake();
            Message.AddListener<CurrentSectionChange>(OnCurrentSectionChange);
            Message.AddListener<GameStatusResponse>(OnGameStatusResponse);
        }

        protected virtual void OnEnable()
        {
            Message.Send(new GameStatusRequest());
            Message.Send(new GameSpeedRequest());
        }

        protected virtual void OnDisable() {}

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Message.RemoveListener<CurrentSectionChange>(OnCurrentSectionChange);
            Message.RemoveListener<GameStatusResponse>(OnGameStatusResponse);
        }

        protected virtual bool CanMove()
        {
            return gameStatus == GameStatus.Active;
        }

        // protected void MoveInY()
        // {
        //     if(_currentSectionGO != null)
        //     {
        //         if(_currentSectionGO.tag == "stairsUp") 
        //         {
        //             trans.Translate(0, -0.06f, 0);
        //         }

        //         if(_currentSectionGO.tag == "stairsDown") 
        //         {
        //             trans.Translate(0, 0.06f, 0);
        //         }
        //     }
        // }

        protected virtual void OnCurrentSectionChange(CurrentSectionChange sectionChange)
        {
            _currentSection = sectionChange.endlessSection;
        }
    }
}
