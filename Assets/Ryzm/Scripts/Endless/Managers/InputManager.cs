using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class InputManager : MonoBehaviour
    {
        private static InputManager _instance;
        public static InputManager Instance { get { return _instance; } }
        EndlessController monkey;
        EndlessController dragon;
        ControllerMode mode;

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            } 
            else 
            {
                _instance = this;
            }
            Message.AddListener<ControllersResponse>(OnControllersResponse);
            Message.AddListener<ControllerModeResponse>(OnControllerModeResponse);
        }

        void OnDestroy()
        {
            Message.RemoveListener<ControllersResponse>(OnControllersResponse);
            Message.RemoveListener<ControllerModeResponse>(OnControllerModeResponse);
        }

        void Start()
        {
            Message.Send(new ControllersRequest());
            Message.Send(new ControllerModeRequest());
        }

        void OnControllersResponse(ControllersResponse response)
        {
            monkey = response.monkey;
            dragon = response.dragon;
        }

        void OnControllerModeResponse(ControllerModeResponse response)
        {
            mode = response.mode;
        }

        public void Shift(Direction direction)
        {
            if(mode == ControllerMode.Monkey)
            {
                monkey.Shift(direction);
            }
            else if(mode == ControllerMode.MonkeyDragon || mode == ControllerMode.Dragon)
            {
                dragon.Shift(direction);
            }
        }

        public void Jump()
        {
            monkey.Jump();
        }

        public void Slide()
        {
            monkey.Slide();
        }
    }
}
