using UnityEngine;
using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class EndlessItemSpawn : Message
    {
        public GameObject item;

        public EndlessItemSpawn(GameObject item)
        {
            this.item = item;
        }
    }
}
