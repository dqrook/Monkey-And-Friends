using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class CurrentPlatformChange : Message
    {
        public GameObject platform;

        public CurrentPlatformChange() {}

        public CurrentPlatformChange(GameObject platform)
        {
            this.platform = platform;
        }
    }
}
