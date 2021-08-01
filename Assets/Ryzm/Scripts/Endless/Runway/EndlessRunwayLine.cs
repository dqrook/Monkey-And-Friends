using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessRunwayLine : MonoBehaviour
    {
        public EndlessRunway runway;
        public RunwayLineType type = RunwayLineType.End;
        
        void OnTriggerEnter(Collider other)
        {
            EndlessController runner = other.GetComponent<EndlessController>();
            if(runner != null)
            {
                Debug.Log("crossed da line ya nerd");
                runway.CrossedLine(type);
            }
        }
    }

    public enum RunwayLineType
    {
        Halfway,
        End
    }
}
