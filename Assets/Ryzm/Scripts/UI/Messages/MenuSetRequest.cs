using CodeControl;

namespace Ryzm.UI.Messages
{
    public class MenuSetRequest : Message
    {
        public MenuSet set;

        public MenuSetRequest(MenuSet set)
        {
            this.set = set;
        }
    }
}