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
                Invoke("SetInactive", 2.5f);
                dScheduled = true;
            }
        }

        public void CancelDeactivation()
        {
            if(dScheduled)
            {
                stopDeactivation = true;
                dScheduled = false;
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
