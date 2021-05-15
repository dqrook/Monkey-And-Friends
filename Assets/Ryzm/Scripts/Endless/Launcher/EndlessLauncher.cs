using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class EndlessLauncher : MonoBehaviour
    {
        public EndlessLaunchZone launchZone;
        public EndlessLanding landing;
        public Transform targetDragonPosition;

        EndlessMonkey monkey;
        EndlessDragon dragon;
        Vector3 launchPosition = new Vector3();
        Vector3 dragonPosOffset = new Vector3();
        Vector3 newDragonPos = new Vector3();

        void Awake()
        {
            dragonPosOffset = targetDragonPosition.localPosition;
        }

        void OnEnable()
        {
            Message.AddListener<ControllersResponse>(OnControllersResponse);
            Message.Send(new ControllersRequest());
        }

        void OnDisable()
        {
            Message.RemoveListener<ControllersResponse>(OnControllersResponse);
        }

        void OnControllersResponse(ControllersResponse response)
        {
            monkey = response.monkey;
            dragon = response.dragon;
        }

        public void Launch(Vector3 launchPosition) 
        {
            this.launchPosition = launchPosition;
            newDragonPos.x = launchPosition.x + dragonPosOffset.x;
            newDragonPos.y = launchPosition.y + dragonPosOffset.y;
            newDragonPos.z = launchPosition.z + dragonPosOffset.z;
            targetDragonPosition.position = newDragonPos;
        }
    }
}
