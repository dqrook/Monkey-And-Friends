using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.Dragon
{
    public class DragonTailSlap : CustomParticles
    {
        #region Public Variables
        public ExplosionParticles explosion;
        public float shrinkTime = 0.5f;
        [HideInInspector]
        public bool explosionEnabled;
        #endregion

        #region Private Variables
        IEnumerator expand;
        IEnumerator shrinkThenDisable;
        Transform explosionTrans;
        #endregion

        protected override void Awake()
        {
            base.Awake();
            explosionTrans = explosion.transform;
            trans.localScale = Vector3.zero;
        }

        // void OnCollisionEnter(Collision other)
        // {
        //     Debug.Log("entered");
        //     EndlessMonster monster = other.gameObject.GetComponent<EndlessMonster>();
        //     if(monster != null)
        //     {
        //         monster.Die();
        //         EnableExplosion();
        //     }
        // }

        #region Public Functions
        public override void Enable()
        {
            // base.Enable();
            PlayParticles(true);
            isEnabled = true;
            if(shrinkThenDisable != null)
            {
                StopCoroutine(shrinkThenDisable);
                shrinkThenDisable = null;
            }
            trans.localScale = Vector3.zero;
            // expand = Expand();
            // StartCoroutine(expand);
        }

        public void EnableExplosion()
        {
            if(!explosionEnabled)
            {
                explosionEnabled = true;
                explosionTrans.parent = null;
                explosion.Enable();
            }
        }

        public override void Disable()
        {
            shrinkThenDisable = ShrinkThenDisable();
            StartCoroutine(shrinkThenDisable);
        }

        public void InstantDisable()
        {
            explosionEnabled = false;
            explosion.InstantDisable();
        }
        #endregion


        #region Coroutines
        IEnumerator Expand()
        {
            float t = 0;
            while(t < expansionTime)
            {
                t += Time.deltaTime;
                float val = t / expansionTime;
                trans.localScale = startLocalScale * val;
                yield return null;
            }
            trans.localScale = startLocalScale;
        }

        IEnumerator ShrinkThenDisable()
        {
            float t = 0;
            // while(t < 0.5f)
            // {
            //     t += Time.deltaTime;
            //     yield return null;
            // }
            explosion.Disable(shrinkTime);
            t = 0;
            Vector3 s = trans.localScale;
            explosionEnabled = false;
            while(t < shrinkTime)
            {
                t += Time.deltaTime;
                // float val = 1 - t / shrinkTime;
                // trans.localScale = s * val;
                yield return null;
            }
            explosionEnabled = false;
            trans.localScale = Vector3.zero;
            // explosionTrans.parent = trans.parent;
            // explosionTrans.localPosition = trans.localPosition;
            // explosionTrans.localRotation = trans.localRotation;
            PlayParticles(false);
        }
        #endregion
    }
}
