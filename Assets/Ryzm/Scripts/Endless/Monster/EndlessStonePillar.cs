using UnityEngine;
using System.Collections;

namespace Ryzm.EndlessRunner
{
    public class EndlessStonePillar : EndlessTregon
    {
        #region Private Variables
        bool startedFallDown;
        IEnumerator fallDown;
        #endregion

        #region Public Functions
        public override void Reset()
        {
            EnableCollider(true);
            trans.localPosition = startPosition;
            hasHit = false;
            startedFallDown = false;
        }

        public override void TakeSpecialDamage()
        {
            if(!startedFallDown)
            {
                fallDown = FallDown();
                StartCoroutine(fallDown);
            }
        }
        #endregion

        #region Protected Functions
        protected override void Die()
        {
            EnableCollider(false);
        }
        #endregion

        #region Coroutines
        IEnumerator FallDown()
        {
            Die();
            startedFallDown = true;
            float downSpeed = 5;
            float t = 0;
            Vector3 move = new Vector3();
            while(t < 2)
            {
                t += Time.deltaTime;
                downSpeed += 9.8f * Time.deltaTime;
                move.y = -downSpeed * Time.deltaTime;
                trans.Translate(move);
                yield return null;
            }
        }
        #endregion
    }
}
