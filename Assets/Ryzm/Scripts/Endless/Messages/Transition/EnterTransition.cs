using UnityEngine;
using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class EnterTransition : Message
    {
        public Transform nextSpawn;

        public EnterTransition(Transform nextSpawn)
        {
            this.nextSpawn = nextSpawn;
        }
    }
}
