using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class CurrentSectionChange : Message
    {
        public GameObject section;
        public EndlessSection endlessSection;
        public EndlessTSection endlessTSection;

        public CurrentSectionChange() {}

        public CurrentSectionChange(GameObject section)
        {
            this.section = section;
            endlessSection = this.section.GetComponent<EndlessSection>();
            endlessTSection = this.section.GetComponent<EndlessTSection>();
        }
    }
}
