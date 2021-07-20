using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public class DragonFire : MonoBehaviour
    {
        public ParticleSystem part;
        public FireType type = FireType.Enemy;
        float collisionTime;

        void Awake()
        {
            part = GetComponent<ParticleSystem>();
        }

        public void Play()
        {
            part.Play();
        }

        public void Stop()
        {
            part.Stop();
        }

        void OnDisable()
        {
            collisionTime = 0;
            if(part.isPlaying)
            {
                Stop();
            }
        }

        void OnParticleCollision(GameObject other)
        {
            if(type == FireType.Enemy)
            {
                collisionTime += Time.deltaTime;
                if(collisionTime > Time.deltaTime * 3)
                {
                    Message.Send(new RunnerDie());
                    collisionTime = 0;
                }
            }
            else
            {
                EndlessRabby rabby = other.GetComponent<EndlessRabby>();
                if(rabby != null)
                {
                    rabby.Die();
                }
            }
        }
    }

    public enum FireType
    {
        Enemy,
        User
    }
}
