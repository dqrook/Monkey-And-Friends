using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class InputManager : MonoBehaviour
    {
        public KeyCode leftPress = KeyCode.A;
        public KeyCode rightPress = KeyCode.D;
        public KeyCode upPress = KeyCode.Space;
        public KeyCode downPress = KeyCode.S;

        private static InputManager _instance;
        public static InputManager Instance { get { return _instance; } }
        EndlessController monkey;
        EndlessController dragon;
        ControllerMode mode;
        GameStatus status;

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
            Message.AddListener<GameStatusResponse>(OnGameStatusResponse);
        }

        void OnDestroy()
        {
            Message.RemoveListener<ControllersResponse>(OnControllersResponse);
            Message.RemoveListener<ControllerModeResponse>(OnControllerModeResponse);
            Message.RemoveListener<GameStatusResponse>(OnGameStatusResponse);
        }

        void Start()
        {
            Message.Send(new ControllersRequest());
            Message.Send(new ControllerModeRequest());
        }

        void Update()
        {
            if(Input.GetKeyDown(leftPress))
            {
                Shift(Direction.Left);
            }
            else if(Input.GetKeyDown(rightPress))
            {
                Shift(Direction.Right);
            }
            else if(Input.GetKeyDown(upPress))
            {
                Jump();
            }
            else if(Input.GetKeyDown(downPress))
            {
                Slide();
            }
        }

        void OnGameStatusResponse(GameStatusResponse response)
        {
            status = response.status;
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
            if(!GameIsActive())
            {
                return;
            }
            if(mode == ControllerMode.Dragon || mode == ControllerMode.MonkeyDragon)
            {
                dragon.Shift(direction);
            }
            else if(mode == ControllerMode.Monkey)
            {
                monkey.Shift(direction);
            }
        }

        public void Jump()
        {
            if(!GameIsActive())
            {
                return;
            }
            if(mode == ControllerMode.Dragon || mode == ControllerMode.MonkeyDragon)
            {
                dragon.Jump();
            }
            else if(mode == ControllerMode.Monkey)
            {
                monkey.Jump();
            }
        }

        public void Slide()
        {
            if(!GameIsActive())
            {
                return;
            }
            if(mode == ControllerMode.Dragon || mode == ControllerMode.MonkeyDragon)
            {
                dragon.Attack();
            }
            else if(mode == ControllerMode.Monkey)
            {
                monkey.Slide();
            }
        }

        bool GameIsActive()
        {
            return status == GameStatus.Active;
        }
    }
}
