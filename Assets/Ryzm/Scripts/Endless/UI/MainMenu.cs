using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;
using Ryzm.Blockchain.Messages;

namespace Ryzm.EndlessRunner.UI
{
    public class MainMenu : EndlessMenu
    {
        protected override void Awake()
        {
            base.Awake();
            StartCoroutine(Wait2Send());
        }
        
        public void OnClickStart()
        {
            // Message.Send(new GameStatusResponse(GameStatus.Starting));
            Message.Send(new MakeWorld());
        }

        IEnumerator Wait2Send(float time = 1)
        {
            yield return new WaitForSeconds(time);
            Message.Send(new LoginRequest());
        }
    }
}
