using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class TotalCoinsResponse : Message
    {
        public int coinsCollected;

        public TotalCoinsResponse() {}

        public TotalCoinsResponse(int coinsCollected)
        {
            this.coinsCollected = coinsCollected;
        }
    }
}
