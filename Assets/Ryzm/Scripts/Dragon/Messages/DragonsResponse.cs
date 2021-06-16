using CodeControl;
using System.Collections.Generic;
using Ryzm.EndlessRunner;

namespace Ryzm.Dragon.Messages
{
    public class DragonsResponse : Message
    {
        public List<EndlessDragon> dragons;
        public string sender;

        public DragonsResponse(List<EndlessDragon> dragons)
        {
            this.dragons = dragons;
        }

        public DragonsResponse(List<EndlessDragon> dragons, string sender)
        {
            this.dragons = dragons;
            this.sender = sender;
        }
    }
}

