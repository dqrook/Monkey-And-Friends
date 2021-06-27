using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class UpdateVisibleMarketDragons : Message
    {
        public int visibleIndex;
        public bool allVisible;

        public UpdateVisibleMarketDragons()
        {
            this.allVisible = true;
        }

        public UpdateVisibleMarketDragons(int visibleIndex)
        {
            this.allVisible = false;
            this.visibleIndex = visibleIndex;
        }
    }
}
