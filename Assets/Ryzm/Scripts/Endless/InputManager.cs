using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class InputManager : MonoBehaviour
    {
        private static InputManager _instance;
        public RunnerController runner;
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
                runner = FindObjectOfType<RunnerController>();
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
    }
}
