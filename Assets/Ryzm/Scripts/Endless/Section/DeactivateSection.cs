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
        public float deactivationTime = 2.5f;

        public bool dScheduled = false;
        bool stopDeactivation;

        public void Deactivate()
        {
            if(!dScheduled)
            {
                stopDeactivation = false;
                Invoke("SetInactive", deactivationTime);
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
