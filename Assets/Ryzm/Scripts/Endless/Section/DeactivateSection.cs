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
        bool stopDeactivation;

        public void Deactivate()
        {
            if(!dScheduled)
            {
                stopDeactivation = false;
                Invoke("SetInactive", 3.0f);
                dScheduled = true;
            }
        }

        public void CancelDeactivation()
        {
            if(dScheduled)
            {
                stopDeactivation = true;
            }
        }

        void SetInactive()
        {
            if(!stopDeactivation)
            {
                Message.Send(new SectionDeactivated(section));
                section.gameObject.SetActive(false);
            }
            dScheduled = false;
            stopDeactivation = false;
        }
    }
}
