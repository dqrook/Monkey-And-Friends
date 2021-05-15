using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class InputManager : MonoBehaviour
    {
        public EndlessController runner;
        private static InputManager _instance;
        public static InputManager Instance { get { return _instance; } }

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
            if(runner == null)
            {
                runner = FindObjectOfType<EndlessController>();
            }
        }

        public void Shift(Direction direction)
        {
            runner.Shift(direction);
        }

        public void Jump()
        {
            runner.Jump();
        }

        public void Slide()
        {
            runner.Slide();
        }
    }
}
