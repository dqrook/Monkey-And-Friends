using CodeControl;
using Ryzm.EndlessRunner.UI;
using System.Collections.Generic;

namespace Ryzm.EndlessRunner.Messages
{
    public class DeactivateMenu : Message
    {
        public MenuType type;
        // can use this variable if you wish to deactivate all menus except the ones you specified here
        public List<MenuType> activatedTypes = new List<MenuType>();
        public bool useActivated;

        public DeactivateMenu() {}

        public DeactivateMenu(MenuType type)
        {
            this.type = type;
            useActivated = false;
        }

        public DeactivateMenu(List<MenuType> activatedTypes)
        {
            this.activatedTypes = activatedTypes;
            useActivated = true;
        }
    }
}
