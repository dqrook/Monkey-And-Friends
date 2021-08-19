using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.UI.Messages;

namespace Ryzm.Dragon
{
    public class MarketTest : MonoBehaviour
    {
        void Start()
        {
            Message.Send(new ActivateMenu(UI.MenuType.DragonMarket));
        }
    }
}
