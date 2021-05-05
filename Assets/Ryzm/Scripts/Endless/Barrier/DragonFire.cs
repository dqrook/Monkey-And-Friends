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
        float collisionTime;

        void Awake()
        {
            part = GetComponent<ParticleSystem>();
        }

        public void Play()
        {
            part.Play();
        }

        void OnDisable()
        {
            collisionTime = 0;
            if(part.isPlaying)
            {
                part.Stop();
            }
        }

        void OnParticleCollision(GameObject other)
        {
            // Debug.Log(other.tag);
            collisionTime += Time.deltaTime;
            if(collisionTime > Time.deltaTime * 5)
            {
                Message.Send(new RunnerDie());
            }
        }
    }
}
