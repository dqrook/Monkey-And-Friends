using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessCoconutTree : EndlessStonePillar
    {
        #region Public Functions
        public override void Reset()
        {
            base.Reset();
            trans.localEulerAngles = startEulerAngles;
        }
        #endregion

        #region Protected Functions
        protected override IEnumerator FallDown()
        {
            Die();
            startedFallDown = true;
            float t = 0;
            float totTime = 2;
            Vector3 angles = new Vector3();
            while(t < totTime)
            {
                t += Time.deltaTime;
                angles.y = -90 * t / totTime;
                trans.localEulerAngles = angles;
                yield return null;
            }
        }
        #endregion
    }
}
