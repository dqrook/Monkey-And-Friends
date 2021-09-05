﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.Dragon
{
    public class ExplosionParticles : TriggerParticles
    {
        #region Public Variables
        public SphereCollider explosionCollider;
        public float scale = 2;
        #endregion

        #region Private Variables
        Vector3 colliderStartScale;
        IEnumerator expandCollider;
        IEnumerator shrinkThenDisable;
        float finRadius;
        #endregion

        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            // colliderStartScale = colliderTransform.localScale;
            // colliderTransform.localScale = Vector3.zero;
            explosionCollider.enabled = false;
            finRadius = explosionCollider.radius;
            explosionCollider.radius = 0;
        }
        #endregion

        #region Public Functions
        public override void Enable()
        {
            ResetTransform();
            base.Enable();
            trans.parent = null;
            trans.rotation = Quaternion.identity;
            expandCollider = ExpandCollider();
            StartCoroutine(expandCollider);
            explosionCollider.enabled = true;
        }

        public override void Disable()
        {
            base.Disable();
            ResetTransform();
        }

        public void Disable(float shrinkTime = 0)
        {
            shrinkThenDisable = ShrinkThenDisable(shrinkTime);
            StartCoroutine(shrinkThenDisable);
        }

        public void OnTrigger(Collider other)
        {
            hasHit = false;
            CheckHit(other);
        }
        #endregion

        protected override void PlayParticles(bool shouldPlay)
        {
            foreach(ParticleSystem system in particleSystems)
            {
                if(shouldPlay)
                {
                    system.transform.localScale = Vector3.one * scale;
                    system.Play();
                }
                else
                {
                    system.transform.localScale = Vector3.zero;
                    system.Stop();
                }
            }
        }

        #region Private Functions
        void ResetTransform()
        {
            explosionCollider.enabled = false;
            explosionCollider.radius = 0;
            // colliderTransform.localScale = Vector3.zero;
            trans.parent = parent;
            trans.localPosition = startLocalPosition;
            trans.localRotation = startLocalRotation;
        }
        #endregion

        #region Coroutines
        IEnumerator ExpandCollider()
        {
            float t = 0;
            while(t < expansionTime)
            {
                t += Time.deltaTime;
                explosionCollider.radius = finRadius * t / expansionTime;
                // colliderTransform.localScale = colliderStartScale * t / expansionTime;
                yield return null;
            }
            // colliderTransform.localScale = colliderStartScale;
            explosionCollider.radius = finRadius;
            
        }

        IEnumerator ShrinkThenDisable(float shrinkTime)
        {
            float t = 0;
            // Vector3 start = colliderTransform.localScale;
            float _start = explosionCollider.radius;
            while(t < shrinkTime)
            {
                t += Time.deltaTime;
                float multiplier = 1 - t / shrinkTime;
                multiplier = multiplier > 0 ? multiplier : 0;
                // colliderTransform.localScale = start * multiplier;
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
