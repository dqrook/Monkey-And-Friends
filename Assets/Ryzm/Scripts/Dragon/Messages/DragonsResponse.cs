using CodeControl;
using System.Collections.Generic;
using Ryzm.EndlessRunner;

namespace Ryzm.Dragon.Messages
{
    public class DragonsResponse : Message
    {
        public string sender;
        public BaseDragon[] dragons;

        public DragonsResponse(List<BaseDragon> dragons)
        {
            CreateDragonsList(dragons);
        }

        public DragonsResponse(List<BaseDragon> dragons, string sender)
        {
            this.sender = sender;
            CreateDragonsList(dragons);
        }

        void CreateDragonsList(List<BaseDragon> dragons)
        {
            int dex = 0;
            this.dragons = new BaseDragon[dragons.Count];
            foreach(BaseDragon dragon in dragons)
            {
                this.dragons[dex] = dragon;
                dex++;
            }
        }
    }
}

