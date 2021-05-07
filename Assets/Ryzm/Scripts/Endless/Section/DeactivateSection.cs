using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class DeactivateSection : MonoBehaviour
    {
        public EndlessSection section;

        bool dScheduled = false;

        public void Deactivate()
        {
            if(!dScheduled)
            {
                Invoke("SetInactive", 4.0f);
                dScheduled = true;
            }
        }

        void SetInactive()
        {
            Message.Send(new SectionDeactivated(section));
            section.gameObject.SetActive(false);
            dScheduled = false;
        }
    }
}
