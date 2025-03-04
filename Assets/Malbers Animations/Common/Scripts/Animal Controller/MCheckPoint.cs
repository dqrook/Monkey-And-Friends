﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace MalbersAnimations.Controller
{
    [AddComponentMenu("Malbers/Animal Controller/Check Point")]
    public class MCheckPoint : MonoBehaviour
    {
        /// <summary>List of all the CheckPoint on the Scene</summary>
        public static List<MCheckPoint> CheckPoints;
        
        /// <summary>Last CheckPoint the Animal use</summary>
        public static MCheckPoint LastCheckPoint;

        public UnityEvent OnEnter = new UnityEvent();
        [FormerlySerializedAs("OnActive")]
        public UnityEvent OnReset = new UnityEvent();

        public Collider Collider { get; set; } 

        // Use this for initialization
        void Start()
        {
            if (MRespawner.instance == null)
            {
                Debug.LogWarning(name + " has being destroyed since there's no Respawner");
                Destroy(gameObject); //Destroy the CheckPoint if is there no Respawner
            }

            if (MAnimal.MainAnimal == null)
            {
                Debug.LogWarning(name + " has being destroyed since there's no Main Animal Player, Set on your Main Character: Main Player = true");
                Destroy(gameObject); //Destroy the CheckPoint if is there no Respawner
            }

            Collider = GetComponent<Collider>();

            if (Collider)
            {
                Collider.isTrigger = true;
            }
            else
            {
                Debug.LogError(name + " needs a Collider");
            }
            OnReset.Invoke();
        }


        void OnTriggerEnter(Collider other)
        {
            if (LastCheckPoint == this) return; //Means the animal has already enter this CheckPoint
            var animal = other.GetComponentInParent<MAnimal>();

            if (!animal) return;        //Skip if there's no Animal

            if (animal != MAnimal.MainAnimal) return; //Skip if there's no the Player Animal

            MRespawner.instance.transform.position = transform.position;
            MRespawner.instance.transform.rotation = transform.rotation;
            MRespawner.instance.RespawnState = animal.ActiveStateID;        //Set on the Respawner the Last Animal State

            if (LastCheckPoint)  LastCheckPoint.OnReset.Invoke();
           

            LastCheckPoint = this;                                  //Check that the last check Point of entering was this one
            OnEnter.Invoke();
            Collider.enabled = false;
        }


        public static void ResetCheckPoint()
        {
            if (LastCheckPoint)
            {
                LastCheckPoint.Collider.enabled = true;
                LastCheckPoint.OnReset.Invoke();
                LastCheckPoint = null;
            }
        }

        void OnEnable()
        {
            if (CheckPoints == null) CheckPoints = new List<MCheckPoint>();
            CheckPoints.Add(this);          //Save tis CheckPoint  on the CheckPoint List
        }

        void OnDisable()
        {
            if (CheckPoints != null) CheckPoints.Remove(this);       //Remove this CheckPoint from the CheckPoint List
        }
    }
}