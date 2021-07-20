using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class EndlessTree : MonoBehaviour
    {
        void OnCollisionEnter(Collision other)
        {
            if(other.gameObject.GetComponent<EndlessController>())
            {
                Message.Send(new RunnerDie());
            }
        }
    }
}
