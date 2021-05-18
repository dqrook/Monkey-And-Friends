using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner.UI
{
    public class MainMenu : EndlessMenu
    {
        public void OnClickStart()
        {
            // Message.Send(new GameStatusResponse(GameStatus.Starting));
            Message.Send(new MakeWorld());
        }
    }
}
