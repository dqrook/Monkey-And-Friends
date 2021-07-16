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
        public EndlessTurnSection endlessTurnSection;
        public EndlessTSection endlessTSection;
        public int rowId;

        public CurrentSectionChange() {}

        public CurrentSectionChange(GameObject section)
        {
            this.section = section;
            endlessSection = this.section.GetComponent<EndlessSection>();
            endlessTurnSection = this.section.GetComponent<EndlessTurnSection>();
            endlessTSection = this.section.GetComponent<EndlessTSection>();
        }

        public CurrentSectionChange(GameObject section, int rowId)
        {
            this.section = section;
            endlessSection = this.section.GetComponent<EndlessSection>();
            endlessTurnSection = this.section.GetComponent<EndlessTurnSection>();
            endlessTSection = this.section.GetComponent<EndlessTSection>();
            this.rowId = rowId;
        }
    }
}
