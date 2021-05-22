using Ryzm.EndlessRunner.UI;
using CodeControl;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner.Messages
{
    public class ActivateMenu : Message
    {
        public MenuType type;
        public List<MenuType> deactivatedTypes;
        // can use this variable if you wish to deactivate all menus except the ones you specified here
        public bool useActivated;
        public bool useDeactivated;
        public MenuType[] activatedTypes;

        public ActivateMenu() {}

        public ActivateMenu(MenuType type)
        {
            this.type = type;
            this.useActivated = false;
            this.useDeactivated = false;
        }

        public ActivateMenu(List<MenuType> deactivatedTypes)
        {
            this.type = MenuType.None;
            AddDeactivated(deactivatedTypes);
        }

        public ActivateMenu(List<MenuType> activatedTypes, bool useActivated = true)
        {
            // this.type = MenuType.None;
            if(useActivated)
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
            else
            {
                AddDeactivated(activatedTypes);
            }
        }

        void AddDeactivated(List<MenuType> deactivatedTypes)
        {
            this.deactivatedTypes = deactivatedTypes;
            this.useActivated = false;
            this.useDeactivated = true;
        }
    }
}
