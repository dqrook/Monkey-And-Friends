using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class InputManager : MonoBehaviour
    {
        #region Public Variables
        public KeyCode leftPress = KeyCode.A;
        public KeyCode rightPress = KeyCode.D;
        public KeyCode upPress = KeyCode.Space;
        public KeyCode downPress = KeyCode.S;
        public EndlessRyz ryz;
        public static InputManager Instance { get { return _instance; } }
        #endregion
        
        #region Private Variables
        static InputManager _instance;
        EndlessController monkey;
        EndlessController dragon;
        ControllerMode mode;
        GameStatus status;
        #endregion

        #region Properties
        EndlessController CurrentController
        {
            get
            {
                if(ryz != null)
                {
                    return ryz;
                }
                if(mode == ControllerMode.Monkey)
                {
                    return monkey;
                }
                return dragon;
            }
        }
        #endregion

        #region Event Functions
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

        void Start()
        {
            Message.Send(new ControllersRequest());
            Message.Send(new ControllerModeRequest());
        }

        void Update()
        {
            if(Input.GetKeyDown(leftPress))
            {
                SetInput(Direction.Left);
            }
            else if(Input.GetKeyDown(rightPress))
            {
                SetInput(Direction.Right);
            }
            else if(Input.GetKeyDown(upPress))
            {
                SetInput(Direction.Up);
            }
            else if(Input.GetKeyDown(downPress))
            {
                SetInput(Direction.Down);
            }
        }

        void OnDestroy()
        {
            Message.RemoveListener<ControllersResponse>(OnControllersResponse);
            Message.RemoveListener<ControllerModeResponse>(OnControllerModeResponse);
            Message.RemoveListener<GameStatusResponse>(OnGameStatusResponse);
        }
        #endregion

        #region Listener Functions
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
        #endregion

        #region Public Functions
        public void SetInput(Direction direction)
        {
            if(!CanMove())
            {
                return;
            }

            if(direction == Direction.Up)
            {
                CurrentController.UpInput();
            }
            else if(direction == Direction.Down)
            {
                CurrentController.DownInput();
            }
            else
            {
                CurrentController.Shift(direction);
            }
        }
        #endregion

        #region Private Functions
        bool CanMove()
        {
            return status == GameStatus.Active || status == GameStatus.Starting;
        }
        #endregion
    }
}
