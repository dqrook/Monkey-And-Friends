using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class HomeIsland : MonoBehaviour
    {
        #region Public Variables
        public GameObject island;
        public Transform monkeySpawn;
        #endregion

        #region Private Variables
        Transform ryzTrans;
        #endregion

        
        #region Event Functions
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
        #endregion

        #region Listener Functions
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
            ryzTrans = response.ryz.transform;
        }

        void OnGameStatusResponse(GameStatusResponse response)
        {
            if(ryzTrans != null)
            {
                if(response.status == GameStatus.MainMenu || response.status == GameStatus.Exit)
                {
                    island.SetActive(true);
                    ryzTrans.position = monkeySpawn.position;
                    ryzTrans.rotation = monkeySpawn.rotation;
                }
            }
        }
        #endregion
    }
}
