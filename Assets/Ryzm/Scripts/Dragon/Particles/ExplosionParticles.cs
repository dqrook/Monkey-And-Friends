using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.Dragon
{
    public class ExplosionParticles : TriggerParticles
    {
        #region Public Variables
        public Transform colliderTransform;
        #endregion

        #region Private Variables
        Vector3 colliderStartScale;
        IEnumerator expandCollider;
        IEnumerator shrinkThenDisable;
        #endregion

        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            colliderStartScale = colliderTransform.localScale;
            colliderTransform.localScale = Vector3.zero;
        }
        #endregion

        #region Public Functions
        public override void Enable()
        {
            base.Enable();
            expandCollider = ExpandCollider();
            StartCoroutine(expandCollider);
        }

        public override void Disable()
        {
            base.Disable();
            colliderTransform.localScale = Vector3.zero;
        }

        public void Disable(float shrinkTime = 0)
        {
            shrinkThenDisable = ShrinkThenDisable(shrinkTime);
            StartCoroutine(shrinkThenDisable);
        }

        public void OnTrigger(Collider other)
        {
            Debug.Log("sup bish");
            hasHit = false;
            CheckHit(other);
        }
        #endregion

        #region Coroutines
        IEnumerator ExpandCollider()
        {
            float t = 0;
            while(t < expansionTime)
            {
                t += Time.deltaTime;
                colliderTransform.localScale = colliderStartScale * t / expansionTime;
                yield return null;
            }
            colliderTransform.localScale = colliderStartScale;
        }

        IEnumerator ShrinkThenDisable(float shrinkTime)
        {
            float t = 0;
            while(t < shrinkTime)
            {
                t += Time.deltaTime;
                float multiplier = 1 - t / shrinkTime;
                multiplier = multiplier > 0 ? multiplier : 0;
                colliderTransform.localScale = colliderStartScale * multiplier;
                yield return null;
            }
            colliderTransform.localScale = Vector3.zero;
            PlayParticles(false);
            isEnabled = false;
        }
        #endregion
    }
}
