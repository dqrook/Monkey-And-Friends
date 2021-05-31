using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class HomeIsland : MonoBehaviour
    {
        public GameObject island;
        public Transform startingSpawn;

        void Awake() 
        {
            Message.AddListener<ActivateHome>(OnActivateHome);
            Message.AddListener<DeactivateHome>(OnDeactivateHome);
        }

        void OnDestroy() 
        {
            Message.RemoveListener<ActivateHome>(OnActivateHome);
            Message.RemoveListener<DeactivateHome>(OnDeactivateHome);
        }

        void OnActivateHome(ActivateHome activate)
        {
            island.SetActive(true);
        }

        void OnDeactivateHome(DeactivateHome deactivate)
        {
            island.SetActive(false);
        }
    }
}
