using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.Dragon
{
    public abstract class CustomParticles : MonoBehaviour
    {
        #region Public Variables
        public ParticleTarget target = ParticleTarget.Any;
        public List<ParticleSystem> particleSystems = new List<ParticleSystem>();
        public float expansionTime;
        [HideInInspector]
        public bool isEnabled;
        #endregion

        #region Protected Variables
        protected Transform trans;
        protected Vector3 startLocalPosition;
        protected Quaternion startLocalRotation;
        protected Vector3 startLocalScale;
        protected Transform parent;
        protected Vector3 move = new Vector3();
        #endregion

        #region Event Functions
        protected virtual void Awake()
        {
            trans = transform;
            parent = trans.parent;
            startLocalScale = trans.localScale;
            startLocalPosition = trans.localPosition;
            startLocalRotation = trans.localRotation;
            move = Vector3.zero;
            if(particleSystems.Count == 0)
            {
                int numChildren = trans.childCount;
                for(int i = 0; i < numChildren; i++)
                {
                    ParticleSystem ps =  trans.GetChild(i).GetComponent<ParticleSystem>();
                    if(ps != null)
                    {
                        particleSystems.Add(ps);
                    }
                }
            }
        }

        protected virtual void OnCollisionEnter(Collision other) {}

        protected virtual void OnTriggerEnter(Collider other) {}

        protected virtual void OnTriggerStay(Collider other) {}
        #endregion

        #region Public Functions
        public virtual void Enable()
        {
            PlayParticles(true);
            isEnabled = true;
        }

        public virtual void Disable()
        {
            PlayParticles(false);
            isEnabled = false;
        }
        #endregion

        #region Protected Functions
        protected virtual void PlayParticles(bool shouldPlay)
        {
            foreach(ParticleSystem system in particleSystems)
            {
                if(shouldPlay)
                {
                    system.Play();
                }
                else if(system != null)
                {
                    system.Stop();
                }
            }
        }

        protected void ResetLocalPosition()
        {
            if(trans != null)
            {
                trans.parent = parent;
                trans.localPosition = startLocalPosition;
                trans.localRotation = startLocalRotation;
            }
        }
        #endregion
    }

    public enum ParticleTarget
    {
        Enemy,
        User,
        Any
    }
}
