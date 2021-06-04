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
        }

        void OnDestroy()
        {
            Message.RemoveListener<ControllersResponse>(OnControllersResponse);
        }

        void Start()
        {
            Message.Send(new ControllersRequest());
        }

        void OnControllersResponse(ControllersResponse response)
        {
            monkey = response.monkey;
            dragon = response.dragon;
        }

        public void Shift(Direction direction)
        {
            monkey.Shift(direction);
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
