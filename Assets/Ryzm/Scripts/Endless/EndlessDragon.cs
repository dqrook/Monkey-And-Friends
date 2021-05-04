using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessDragon : EndlessBarrier
    {
        protected override void Awake()
        {
            base.Awake();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if(!CanMove())
            {
                return;
            }
            GetRunner();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
