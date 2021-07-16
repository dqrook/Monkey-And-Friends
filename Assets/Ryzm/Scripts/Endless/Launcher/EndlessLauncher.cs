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
        public Transform startingDragonPosition;

        EndlessMonkey monkey;
        EndlessDragon dragon;
        Vector3 launchPosition = new Vector3();
        Vector3 dragonPosOffset = new Vector3();
        Vector3 newDragonPos = new Vector3();
        Vector3 newStartPos = new Vector3();
        Vector3 startPosOffset = new Vector3();
        IEnumerator monitorMonkey;

        void Awake()
        {
            dragonPosOffset = targetDragonPosition.localPosition;
            startPosOffset = startingDragonPosition.localPosition;
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
            
            newDragonPos.x = launchPosition.x - dragonPosOffset.x;
            newDragonPos.y = launchPosition.y - dragonPosOffset.y;
            newDragonPos.z = launchPosition.z - dragonPosOffset.z;
            targetDragonPosition.position = newDragonPos;

            newStartPos.x = launchPosition.x - startPosOffset.x;
            newStartPos.y = launchPosition.y - startPosOffset.y;
            newStartPos.z = launchPosition.z - startPosOffset.z;
            startingDragonPosition.position = newStartPos;

            dragon.transform.position = startingDragonPosition.position;
            dragon.transform.rotation = startingDragonPosition.rotation;
            dragon.FlyToPosition(targetDragonPosition);

            Vector3 monkeyPosition = dragon.monkeyOffset + targetDragonPosition.position;
            monitorMonkey = MonitorMonkey(monkeyPosition, dragon.monkeyPos);
            StartCoroutine(monitorMonkey);
        }
        
        IEnumerator MonitorMonkey(Vector3 finalPos, Transform monkeyPos)
        {
            Transform monkeyTrans = monkey.gameObject.transform;
            float difference = Vector3.Distance(monkeyTrans.position, finalPos);
            while(difference > 1)
            {
                difference = Vector3.Distance(monkeyTrans.position, finalPos);
                yield return null;
            }

            difference = Vector3.Distance(monkeyTrans.position, finalPos);
            monkeyTrans.parent = monkeyPos;
            Message.Send(new UpdateControllerMode(ControllerMode.MonkeyDragon));
            while(difference > 0.01f)
            {
                monkeyTrans.rotation = Quaternion.Lerp(monkeyTrans.rotation, monkeyPos.rotation, 5 * Time.deltaTime);
                monkeyTrans.localPosition = Vector3.Lerp(monkeyTrans.localPosition, Vector3.zero, 5 * Time.deltaTime);
                difference = Vector3.Distance(monkeyTrans.localPosition, Vector3.zero);
                yield return null;
            }
            monkeyTrans.rotation = monkeyPos.rotation;
            monkeyTrans.localPosition = Vector3.zero;
            monkey.MaintainZeroPosition();
            yield break;
        }
    }
}
