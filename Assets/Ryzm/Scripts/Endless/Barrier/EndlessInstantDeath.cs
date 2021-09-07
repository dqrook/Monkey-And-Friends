using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class EndlessInstantDeath : MonoBehaviour
    {
        protected void OnCollisionEnter(Collision other)
        {
            if(LayerMask.LayerToName(other.GetContact(0).otherCollider.gameObject.layer) == "PlayerBody")
            {
                Message.Send(new RunnerHit());
            }
        }
    }
}
