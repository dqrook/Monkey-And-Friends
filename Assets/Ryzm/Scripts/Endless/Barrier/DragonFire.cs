using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public class DragonFire : MonoBehaviour
    {
        #region Public Variables
        public ParticleSystem part;
        public FireType type = FireType.Enemy;
        public MonsterMetadata monsterMetadata;
        [Header("Collider")]
        public Transform colliderParent;
        public Collider fireCollider;
        public float finalScale = 15;
        public float scaleTime = 1;
        public bool disableCollider;
        #endregion

        #region Private Variables
        float collisionTime;
        bool hasHit;
        IEnumerator scaleCollider;
        #endregion

        #region Event Functions
        void Awake()
        {
            part = GetComponent<ParticleSystem>();
        }

        void OnEnable()
        {
            hasHit = false;
            Stop();
        }

        void OnDisable()
        {
            collisionTime = 0;
            hasHit = false;
            Stop();
        }

        void OnParticleCollision(GameObject other)
        {
            if(type == FireType.Enemy)
            {
                collisionTime += Time.deltaTime;
                if(collisionTime > Time.deltaTime * 3 && !hasHit)
                {
                    Message.Send(new RunnerHit(monsterMetadata.monsterType, AttackType.Special));
                    collisionTime = 0;
                    hasHit = true;
                }
            }
            else
            {
                MonsterBase monster = other.GetComponent<MonsterBase>();
                if(monster != null)
                {
                    monster.TakeSpecialDamage();
                }
            }
        }
        #endregion

        #region Public Functions
        public void Play()
        {
            ResetCollider();
            part.Play();
            ScaleCollider();
        }

        public void Stop()
        {
            if(scaleCollider != null)
            {
                StopCoroutine(scaleCollider);
                scaleCollider = null;
            }
            ResetCollider();
            part.Stop();
        }
        #endregion

        #region Private Functions
        void ScaleCollider()
        {
            if(!disableCollider && colliderParent != null)
            {
                scaleCollider = _ScaleCollider();
                StartCoroutine(scaleCollider);
            }
        }
        
        void SetCollider(bool enabled)
        {
            if(fireCollider != null)
            {
                fireCollider.enabled = enabled && !disableCollider;
            }
        }

        void ResetCollider()
        {
            if(colliderParent != null)
            {
                colliderParent.localScale = new Vector3(1, 1, 0);
            }
            SetCollider(false);
        }
        #endregion

        #region Coroutines
        IEnumerator _ScaleCollider()
        {
            SetCollider(true);
            float t = 0;
            while(t < scaleTime)
            {
                t += Time.deltaTime;
                colliderParent.localScale = new Vector3(1, 1, finalScale * t / scaleTime);
                yield return null;
            }
            colliderParent.localScale = new Vector3(1, 1, finalScale);
        }
        #endregion
    }

    public enum FireType
    {
        Enemy,
        User
    }
}
