using Ryzm.EndlessRunner.UI;
using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class ActivateMenuSet : Message
    {
        public MenuSet set;

        public ActivateMenuSet(MenuSet set)
        {
            this.set = set;
        }
    }
}
