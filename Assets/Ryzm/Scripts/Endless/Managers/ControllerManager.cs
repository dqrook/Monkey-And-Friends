using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class ControllerManager : MonoBehaviour
    {
        public EndlessMonkey monkey;
        public EndlessDragon dragon;
        public ControllerMode mode = ControllerMode.Monkey;

        void Awake()
        {
            Message.AddListener<ControllerModeRequest>(OnControllerModeRequest);
            Message.AddListener<UpdateControllerMode>(OnUpdateControllerMode);
            Message.AddListener<ControllersRequest>(OnControllersRequest);
        }

        void OnDestroy()
        {
            Message.RemoveListener<ControllerModeRequest>(OnControllerModeRequest);
            Message.RemoveListener<UpdateControllerMode>(OnUpdateControllerMode);
            Message.RemoveListener<ControllersRequest>(OnControllersRequest);
        }

        void OnControllerModeRequest(ControllerModeRequest request)
        {
            ShoutMode(mode);
        }

        void OnUpdateControllerMode(UpdateControllerMode update)
        {
            ShoutMode(update.mode);
            if(update.mode == ControllerMode.Dragon)
            {
                monkey.RideDragon();
            }
        }

        void ShoutMode(ControllerMode mode)
        {
            this.mode = mode;
            Message.Send(new ControllerModeResponse(mode));
        }

        void OnControllersRequest(ControllersRequest request)
        {
            Message.Send(new ControllersResponse(monkey, dragon));
        }
    }

	public enum ControllerMode
	{
		Monkey,
		Dragon,
        None
	}
}
