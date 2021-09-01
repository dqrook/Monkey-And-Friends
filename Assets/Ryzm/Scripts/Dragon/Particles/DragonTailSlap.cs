using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner;

namespace Ryzm.Dragon
{
    public class DragonTailSlap : CustomParticles
    {
        #region Public Variables
        public ExplosionParticles explosion;
        public float shrinkTime = 0.5f;
        #endregion

        #region Private Variables
        IEnumerator expand;
        IEnumerator shrinkThenDisable;
        bool explosionEnabled;
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
            base.Enable();
            expand = Expand();
            StartCoroutine(expand);
        }

        public void EnableExplosion()
        {
            if(!explosionEnabled)
            {
                explosionTrans.parent = null;
                explosion.Enable();
            }
        }

        public override void Disable()
        {
            shrinkThenDisable = ShrinkThenDisable();
            StartCoroutine(shrinkThenDisable);
        }
        #endregion


        #region Coroutines
        IEnumerator Expand()
        {
            trans.localScale = Vector3.zero;
            yield break;
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
            while(t < 0.5f)
            {
                t += Time.deltaTime;
                yield return null;
            }
            explosion.Disable(shrinkTime);
            t = 0;
            Vector3 s = trans.localScale;
            while(t < shrinkTime)
            {
                t += Time.deltaTime;
                float val = 1 - t / shrinkTime;
                trans.localScale = s * val;
                yield return null;
            }
            explosionEnabled = false;
            trans.localScale = Vector3.zero;
            explosionTrans.parent = trans.parent;
            explosionTrans.localPosition = trans.localPosition;
            explosionTrans.localRotation = trans.localRotation;
            PlayParticles(false);
        }
        #endregion
    }
}
