using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessSideDragon : EndlessDiveDragon
    {
        protected override void Awake()
        {
            base.Awake();
            dropSpeed = 15;
        }
    }
}
