using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.Dragon
{
    public class ProjectileParticles : TriggerParticles
    {
        #region Public Variables
        public float speed;
        #endregion

        #region Private Variables
        IEnumerator expandAndFire;
        Vector3 startPosition;
        #endregion

        #region Public Variables
        public override void Enable()
        {
            trans.localScale = Vector3.zero;
            ResetLocalPosition();
            base.Enable();
            expandAndFire = ExpandAndFire();
            StartCoroutine(expandAndFire);
            startPosition = trans.position;
        }

        public override void Disable()
        {
            base.Disable();
            ResetLocalPosition();
            trans.localScale = Vector3.zero;
            if(hitParticles != null)
            {
                hitParticles.Disable();
            }
        }
        #endregion

        #region Coroutines
        IEnumerator ExpandAndFire()
        {
            float t = 0;
            while(t < expansionTime)
            {
                t += Time.deltaTime;
                float frac = t / expansionTime;
                trans.localScale = startLocalScale * frac;
                yield return null;
            }
            trans.localScale = startLocalScale;
            trans.parent = null;
            Vector3 currentPosition = trans.position;
            float diff = Vector3.Distance(currentPosition, startPosition);
            while(diff < 20)
            {
                move.z = Time.deltaTime * speed;
                trans.Translate(move);
                currentPosition = trans.position;
                diff = Vector3.Distance(currentPosition, startPosition);
                yield return null;
            }
            Disable();
        }
        #endregion
    }
}
