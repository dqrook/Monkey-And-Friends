using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class ControllerManager : MonoBehaviour
    {
        #region Public Variables
        public EndlessRyz ryz;
        public EndlessDragon dragon;
        public ControllerMode mode = ControllerMode.Monkey;
        public List<DragonByHorn> dragons = new List<DragonByHorn>();
        #endregion

        #region Event Functions
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
        #endregion

        #region Listener Functions
        void OnControllerModeRequest(ControllerModeRequest request)
        {
            ShoutMode(mode);
        }

        void OnUpdateControllerMode(UpdateControllerMode update)
        {
            ShoutMode(update.mode);
            if(update.mode == ControllerMode.MonkeyDragon)
            {
                ryz.RideDragon();
            }
        }

        void ShoutMode(ControllerMode mode)
        {
            this.mode = mode;
            Message.Send(new ControllerModeResponse(mode));
        }

        void OnControllersRequest(ControllersRequest request)
        {
            Message.Send(new ControllersResponse(ryz, dragon));
        }
        #endregion
    }

	public enum ControllerMode
	{
		Monkey,
		MonkeyDragon,
        Dragon,
        None
	}

    public struct DragonByHorn
    {
        public int hornType;
        public EndlessDragon dragon;
    }
}
