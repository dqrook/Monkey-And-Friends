﻿using UnityEngine;
using System.Collections;

public class OneShotParticle : MonoBehaviour {

    private ParticleSystem[] particleSystems;

    private void Awake() {
        particleSystems = GetComponentsInChildren<ParticleSystem>();
    }

    private void Update() {
        foreach (ParticleSystem particleSystem in particleSystems) {
            if (particleSystem.IsAlive()) {
                return;
            }
        }
        GameObject.Destroy(gameObject);
    }

}
