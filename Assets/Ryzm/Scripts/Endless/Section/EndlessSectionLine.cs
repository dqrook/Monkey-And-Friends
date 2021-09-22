using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public enum LineType
    {
        Start,
        End
    }

    public class EndlessSectionLine : MonoBehaviour
    {
        public EndlessBasePath section;
        public LineType type;
        
        void OnTriggerEnter(Collider other)
        {
            if(type == LineType.Start)
            {
                section.Enter();
            }
            else
            {
                section.Exit();
            }
            // EndlessController runner = other.GetComponent<EndlessController>();
            // if(runner != null)
            // {
            // }
        }
    }
}
