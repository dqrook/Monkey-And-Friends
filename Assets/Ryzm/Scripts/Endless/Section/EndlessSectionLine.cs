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
        public EndlessSection section;
        public LineType type;
        
        void OnTriggerEnter(Collider other)
        {
            RunnerController runner = other.GetComponent<RunnerController>();
            if(runner != null)
            {
                if(type == LineType.Start)
                {
                    Message.Send(new CurrentPlatformChange(section.gameObject));
                    // GameManager.Instance.CurrentPlatform = other.gameObject;
                }
                else
                {
                    if(GenerateWorld.lastSpawnedPlatform.tag != "platformTSection")
                    {
                        GenerateWorld.RunDummy();
                    }
                    section.deactivate.Deactivate();
                }
            }
        }
    }
}
