using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.Dragon
{
    public class ExplosionParticles : TriggerParticles
    {
        #region Public Variables
        public SphereCollider explosionCollider;
        public bool keepParent;
        #endregion

        #region Private Variables
        IEnumerator shrinkThenDisable;
        float finRadius;
        #endregion

        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            explosionCollider.enabled = false;
            finRadius = explosionCollider.radius;
            explosionCollider.radius = 0;
        }
        #endregion

        #region Public Functions
        public override void Enable()
        {
            if(isEnabled)
            {
                PlayParticles(false);
            }
            if(shrinkThenDisable != null)
            {
                StopCoroutine(shrinkThenDisable);
                shrinkThenDisable = null;
            }
            ResetTransform();
            base.Enable();
            if(!keepParent)
            {
                trans.parent = null;
                trans.rotation = Quaternion.identity;
            }
            explosionCollider.radius = finRadius;
            explosionCollider.enabled = true;
        }

        public override void Disable()
        {
            base.Disable();
            ResetTransform();
        }

        public void Disable(float shrinkTime = 0)
        {
            if(shrinkThenDisable != null)
            {
                StopCoroutine(shrinkThenDisable);
                shrinkThenDisable = null;
            }
            if(shrinkTime > 0)
            {
                shrinkThenDisable = ShrinkThenDisable(shrinkTime);
                StartCoroutine(shrinkThenDisable);
            }
            else
            {
                PlayParticles(false);
                ResetTransform();
                isEnabled = false;
            }
        }

        public void InstantDisable()
        {
            PlayParticles(false);
            ResetTransform();
            isEnabled = false;
            explosionCollider.radius = finRadius;
        }

        public void OnTrigger(Collider other)
        {
            hasHit = false;
            CheckHit(other);
        }
        #endregion

        #region Private Functions
        void ResetTransform()
        {
            explosionCollider.enabled = false;
            explosionCollider.radius = 0;
            if(!keepParent)
            {
                trans.parent = parent;
                trans.localPosition = startLocalPosition;
                trans.localRotation = startLocalRotation;
            }
        }
        #endregion

        #region Coroutines
        IEnumerator ShrinkThenDisable(float shrinkTime)
        {
            float t = 0;
            float _start = explosionCollider.radius;
            while(t < shrinkTime)
            {
                t += Time.deltaTime;
                float multiplier = 1 - t / shrinkTime;
                multiplier = multiplier > 0 ? multiplier : 0;
                explosionCollider.radius = _start * multiplier;
                yield return null;
            }
            PlayParticles(false);
            ResetTransform();
            isEnabled = false;
        }
        #endregion
    }
}
