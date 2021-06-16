using CodeControl;

namespace Ryzm.Dragon.Messages
{
    public class DragonsRequest : Message 
    {
        public string sender;

        public DragonsRequest() {}

        public DragonsRequest(string sender)
        {
            this.sender = sender;
        }
    }
}
