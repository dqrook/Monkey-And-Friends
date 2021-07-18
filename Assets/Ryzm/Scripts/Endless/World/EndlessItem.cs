using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class EndlessItem : MonoBehaviour
    {
        protected GameStatus gameStatus;
        
        protected virtual void Awake()
        {
            Message.AddListener<GameStatusResponse>(OnGameStatusResponse);
        }
        
        protected virtual void Start()
        {
            Message.Send(new EndlessItemSpawn(this.gameObject));
        }

        protected virtual void OnDestroy()
        {
            Message.RemoveListener<GameStatusResponse>(OnGameStatusResponse);
        }

        protected virtual void OnGameStatusResponse(GameStatusResponse gameStatusResponse)
        {
            gameStatus = gameStatusResponse.status;
            if(gameStatus == GameStatus.Exit)
            {
                Destroy(this.gameObject);
            }
        }
    }
}
