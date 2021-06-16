using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class BreedDragonsResponse : Message
    {
        public bool isSuccess;

        public BreedDragonsResponse(bool isSuccess)
        {
            this.isSuccess = isSuccess;
        }
    }
}
