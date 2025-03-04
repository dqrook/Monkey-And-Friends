﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner;

namespace Ryzm.Dragon
{
    public class ParticlesContainer : MonoBehaviour
    {
        #region Public Variables
        public CustomParticles particlesPrefab;
        public List<CustomParticles> customParticles = new List<CustomParticles>();
        public int maxParticles = 3;
        public MonsterMetadata monsterMetadata = new MonsterMetadata();
        #endregion

        #region Private Variables
        Transform trans;
        #endregion

        #region Properties
        public float ExpansionTime
        {
            get
            {
                if(particlesPrefab != null)
                {
                    return particlesPrefab.expansionTime;
                }
                return 0;
            }
        }
        #endregion

        #region Event Functions
        void Awake()
        {
            trans = transform;
        }
        #endregion

        #region Public Functions
        public bool EnableParticle()
        {
            int activeParticles = 0;
            CustomParticles currentParticle = null;
            foreach(CustomParticles particle in customParticles)
            {
                if(!particle.isEnabled && currentParticle == null)
                {
                    particle.Disable();
                    currentParticle = particle;
                }
                else if(particle.isEnabled)
                {
                    activeParticles++;
                }
            }

            if(activeParticles < maxParticles)
            {
                if(currentParticle == null)
                {
                    currentParticle = Instantiate(particlesPrefab.gameObject, trans).GetComponent<CustomParticles>();
                    currentParticle.transform.parent = trans;
                    currentParticle.transform.localPosition = Vector3.zero;
                    currentParticle.transform.localEulerAngles = Vector3.zero;
                    customParticles.Add(currentParticle);
                }
                currentParticle.monsterMetadata = monsterMetadata;
                currentParticle.Enable();
                return true;
            }
            return false;
        }

        public void DisableParticles()
        {
            foreach(CustomParticles particle in customParticles)
            {
                particle.Disable();
            }
        }
        #endregion
    }
}
