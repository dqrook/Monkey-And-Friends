using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class DragonGenesResponse : Message
    {
        public string receiver;
        public DragonGenes genes;

        public DragonGenesResponse(string receiver, DragonGenes genes)
        {
            this.receiver = receiver;
            this.genes = genes;
        }
    }
}
