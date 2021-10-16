using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.Platform
{
    public class PlatformRyz : MonoBehaviour
    {
        public PlatformPlayerController playerController;
        Transform trans;
        Vector2 inputs = new Vector2();

        void Awake()
        {
            trans = GetComponent<Transform>();
        }

        void Update()
        {
            Vector2 _inputs = new Vector2();

            if(Input.GetKey(KeyCode.RightArrow))
            {
                _inputs.x = 1;
            }
            else if(Input.GetKey(KeyCode.LeftArrow))
            {
                _inputs.x = -1;
            }
            else
            {
                _inputs.x = 0;
            }

            if(Input.GetKey(KeyCode.UpArrow))
            {
                _inputs.y = 1;
            }
            else if(Input.GetKey(KeyCode.DownArrow))
            {
                _inputs.y = -1;
            }
            else
            {
                _inputs.y = 0;
            }

            SetInputs(_inputs);
            UpdateCharacterInputs();
        }

        public void Jump()
        {
            UpdateCharacterInputs(true);
        }

        public void SetInputs(Vector2 inputs)
        {
            this.inputs = inputs;
        }

        void UpdateCharacterInputs(bool requestJump = false)
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();
            characterInputs.zInput = this.inputs.y;
            characterInputs.xInput = this.inputs.x;
            characterInputs.requestJump = requestJump;
            Vector3 newForward = playerController.motor.CharacterForward;
            characterInputs.forwardAxis = playerController.motor.CharacterForward;
            playerController.SetInputs(ref characterInputs);
        }
    }
}
