using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class BreedDragonsRequest : Message
    {
        public int dragon1Id;
        public int dragon2Id;

        public BreedDragonsRequest(int dragon1Id, int dragon2Id)
        {
            this.dragon1Id = dragon1Id;
            this.dragon2Id = dragon2Id;
        }
    }
}
