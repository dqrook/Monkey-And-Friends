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
        public Transform monkeySpawn;
        public Transform dragonSpawn;
        EndlessMonkey monkey;
        EndlessDragon dragon;
        

        void Awake() 
        {
            Message.AddListener<ActivateHome>(OnActivateHome);
            Message.AddListener<DeactivateHome>(OnDeactivateHome);
            Message.AddListener<ControllersResponse>(OnControllersResponse);
            Message.AddListener<GameStatusResponse>(OnGameStatusResponse);
        }

        void Start()
        {
            Message.Send(new ControllersRequest());
        }

        void OnDestroy() 
        {
            Message.RemoveListener<ActivateHome>(OnActivateHome);
            Message.RemoveListener<DeactivateHome>(OnDeactivateHome);
            Message.RemoveListener<ControllersResponse>(OnControllersResponse);
            Message.RemoveListener<GameStatusResponse>(OnGameStatusResponse);
        }

        void OnActivateHome(ActivateHome activate)
        {
            island.SetActive(true);
        }

        void OnDeactivateHome(DeactivateHome deactivate)
        {
            island.SetActive(false);
        }

        void OnControllersResponse(ControllersResponse response)
        {
            monkey = response.monkey;
            dragon = response.dragon;
        }

        void OnGameStatusResponse(GameStatusResponse response)
        {
            if(response.status == GameStatus.Restart)
            {
                island.SetActive(true);
                monkey.transform.position = monkeySpawn.position;
                monkey.transform.rotation = monkeySpawn.rotation;
            }
        }
    }
}
