using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;
using Ryzm.EndlessRunner;

namespace Ryzm.Test
{
    public class TestRyzMovement : MonoBehaviour
    {
        public bool setGameActive;
        public bool endGame;

        bool gameActive;

        void Update()
        {
            if(setGameActive && !gameActive)
            {
                setGameActive = false;
                gameActive = true;
                Message.Send(new GameStatusResponse(GameStatus.Active));
            }
        }
    }
}
