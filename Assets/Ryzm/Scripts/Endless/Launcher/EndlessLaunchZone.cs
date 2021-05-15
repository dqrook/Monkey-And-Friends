using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public class EndlessLaunchZone : MonoBehaviour
    {
        public EndlessLauncher launcher;
        bool inJump;
        bool sentLaunchUpdate;

        void OnEnable()
        {
            Message.AddListener<JumpStatusResponse>(OnJumpStatusResponse);
        }

        void OnDisable()
        {
            Message.RemoveListener<JumpStatusResponse>(OnJumpStatusResponse);
            inJump = false;
            sentLaunchUpdate = false;
        }

        void OnJumpStatusResponse(JumpStatusResponse response)
        {
            inJump = response.inJump;
        }

        void OnTriggerEnter(Collider other)
        {
            _OnTrigger(other);
        }

        void OnTriggerStay(Collider other)
        {
            _OnTrigger(other);
        }

        void _OnTrigger(Collider other)
        {
            if(inJump && !sentLaunchUpdate)
            {
                sentLaunchUpdate = true;
                launcher.Launch(other.gameObject.transform.position);
            }
        }
    }
}
