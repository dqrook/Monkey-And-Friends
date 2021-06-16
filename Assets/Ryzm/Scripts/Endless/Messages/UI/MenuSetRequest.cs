using Ryzm.EndlessRunner.UI;
using CodeControl;

namespace Ryzm.EndlessRunner.Messages
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
