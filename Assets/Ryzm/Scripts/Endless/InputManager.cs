using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class InputManager : MonoBehaviour
    {
        private static InputManager _instance;
        public RunnerController runner;
        public KeyCode moveRight = KeyCode.D;
        public KeyCode moveLeft = KeyCode.A;
        public KeyCode spinRight = KeyCode.RightArrow;
        public KeyCode spinLeft = KeyCode.LeftArrow;
        public KeyCode attack = KeyCode.Mouse0;
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

        void Update()
        {
            if(Input.GetKeyDown(moveRight))
            {
                Shift(Direction.Right);
            }
            else if(Input.GetKeyDown(moveLeft))
            {
                Shift(Direction.Left);
            }

            if(Input.GetKeyDown(spinRight))
            {
                Spin(Direction.Right);
            }
            else if(Input.GetKeyDown(spinLeft))
            {
                Spin(Direction.Left);
            }

            if(Input.GetKeyDown(attack))
            {
                Attack();
            }
        }

        public void Shift(Direction direction)
        {
            runner.Shift(direction);
        }

        public void Spin(Direction direction)
        {
            runner.Spin(direction);
        }

        public void Attack()
        {
            runner.Attack();
        }
    }
}
