using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class DragonGenesRequest : Message
    {
        public string sender;

        public DragonGenesRequest(string sender = "")
        {
            this.sender = sender;
        }
    }
}
