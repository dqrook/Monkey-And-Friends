using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class DeactivateSection : MonoBehaviour
    {
        public EndlessSection section;

        bool dScheduled = false;
        
        // void OnCollisionExit(Collision player)
        // {
        //     if(player.gameObject.tag == "Player")
        //     {
        //         Deactivate();
        //     }
        // }

        public void Deactivate()
        {
            if(!dScheduled)
            {
                Invoke("SetInactive", 5.0f);
                dScheduled = true;
            }
        }

        void SetInactive()
        {
            section.gameObject.SetActive(false);
            dScheduled = false;
        }
    }
}
