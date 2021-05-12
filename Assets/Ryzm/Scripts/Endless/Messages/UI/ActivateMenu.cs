using Ryzm.EndlessRunner.UI;
using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class ActivateMenu : Message
    {
        public MenuType type;

        public ActivateMenu() {}

        public ActivateMenu(MenuType type)
        {
            this.type = type;
        }
    }
}
