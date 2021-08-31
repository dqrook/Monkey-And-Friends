using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.Dragon
{
    public class HitParticles : CustomParticles
    {
        public override void Disable()
        {
            base.Disable();
            ResetLocalPosition();
        }
    }
}
