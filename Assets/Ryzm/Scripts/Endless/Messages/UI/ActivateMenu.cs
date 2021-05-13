using Ryzm.EndlessRunner.UI;
using CodeControl;
using System.Collections.Generic;

namespace Ryzm.EndlessRunner.Messages
{
    public class ActivateMenu : Message
    {
        public MenuType type;
        public List<MenuType> deactivatedTypes = new List<MenuType>();
        // can use this variable if you wish to deactivate all menus except the ones you specified here
        public List<MenuType> activatedTypes = new List<MenuType>();
        public bool useActivated;

        public ActivateMenu() {}

        public ActivateMenu(MenuType type)
        {
            this.type = type;
            this.useActivated = false;
        }

        public ActivateMenu(List<MenuType> deactivatedTypes)
        {
            this.type = MenuType.None;
            AddDeactivated(deactivatedTypes);
        }

        public ActivateMenu(List<MenuType> activatedTypes, bool useActivated = true)
        {
            this.type = MenuType.None;
            if(useActivated)
            {
                this.activatedTypes = activatedTypes;
                this.useActivated = true;
            }
            else
            {
                AddDeactivated(activatedTypes);
            }
        }

        void AddDeactivated(List<MenuType> deactivatedTypes)
        {
            this.deactivatedTypes = deactivatedTypes;
            this.useActivated = false;
        }
    }
}
