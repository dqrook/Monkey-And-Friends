using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class CreateSection : Message
    {
        public int numberOfSections;

        public CreateSection() 
        {
            this.numberOfSections = 1;
        }

        public CreateSection(int numberOfSections)
        {
            this.numberOfSections = numberOfSections;
        }
    }
}
