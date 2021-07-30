using CodeControl;
using System.Collections.Generic;

namespace Ryzm.UI.Messages
{
    public class ActivateMenu : Message
    {
        public MenuType type;
        public MenuType[] deactivatedTypes;
        // can use this variable if you wish to deactivate all menus except the ones you specified here
        public bool useActivated;
        public bool useDeactivated;
        public MenuType[] activatedTypes;

        public ActivateMenu() {}

        public ActivateMenu(MenuType type)
        {
            List<MenuType> activatedTypes = new List<MenuType>()
            {
                type
            };
            ActivateMenus(activatedTypes);
        }

        public ActivateMenu(List<MenuType> activatedTypes)
        {
            ActivateMenus(activatedTypes);

            // this.activatedTypes = new MenuType[activatedTypes.Count];
            // int i = 0;
            // foreach(MenuType menuType in activatedTypes)
            // {
            //     this.activatedTypes[i] = menuType;
            //     i++;
            // }
            // this.useActivated = true;
            // this.useDeactivated = false;

            // if(useActivated)
            // {
            // }
            // else
            // {
            //     AddDeactivated(activatedTypes);
            // }
        }

        void ActivateMenus(List<MenuType> activatedTypes)
        {
            this.activatedTypes = new MenuType[activatedTypes.Count];
            int i = 0;
            foreach(MenuType menuType in activatedTypes)
            {
                this.activatedTypes[i] = menuType;
                i++;
            }
            this.useActivated = true;
            this.useDeactivated = false;
        }

        void AddDeactivated(List<MenuType> deactivatedTypes)
        {
            this.deactivatedTypes = new MenuType[deactivatedTypes.Count];
            int i = 0;
            foreach(MenuType menuType in activatedTypes)
            {
                this.deactivatedTypes[i] = menuType;
                i++;
            }
            this.useActivated = false;
            this.useDeactivated = true;
        }
    }
}
