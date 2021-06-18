using CodeControl;
using System.Collections.Generic;
using Ryzm.EndlessRunner;

namespace Ryzm.Dragon.Messages
{
    public class DragonsResponse : Message
    {
        public string sender;
        public EndlessDragon[] dragons;

        public DragonsResponse(List<EndlessDragon> dragons)
        {
            CreateDragonsList(dragons);
        }

        public DragonsResponse(List<EndlessDragon> dragons, string sender)
        {
            this.sender = sender;
            CreateDragonsList(dragons);
        }

        void CreateDragonsList(List<EndlessDragon> dragons)
        {
            int dex = 0;
            this.dragons = new EndlessDragon[dragons.Count];
            foreach(EndlessDragon dragon in dragons)
            {
                this.dragons[dex] = dragon;
                dex++;
            }
        }
    }
}

