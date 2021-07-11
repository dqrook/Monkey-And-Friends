using CodeControl;

namespace Ryzm.UI.Messages
{
    public class PreviousMenusUpdate : Message
    {
        public MenuSet set;

        public PreviousMenusUpdate(MenuSet set)
        {
            this.set = set;
        }
    }
}
