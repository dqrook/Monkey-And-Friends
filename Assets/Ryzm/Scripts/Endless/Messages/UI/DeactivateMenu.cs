using CodeControl;
using Ryzm.EndlessRunner.UI;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner.Messages
{
    public class DeactivateMenu : Message
    {
        public MenuType type;
        public List<MenuType> deactivatedTypes = new List<MenuType>();
        // can use this variable if you wish to deactivate all menus except the ones you specified here
        public List<MenuType> activatedTypes = new List<MenuType>();
        public bool useActivated;
        public bool useDeactivated;

        public DeactivateMenu() {}

        public DeactivateMenu(MenuType type)
        {
            this.type = type;
            this.useActivated = false;
            this.useDeactivated = false;
        }

        public DeactivateMenu(List<MenuType> deactivatedTypes)
        {
            this.type = MenuType.None;
            this.useActivated = true;
            this.useDeactivated = false;
            AddDeactivated(deactivatedTypes);
        }

        public DeactivateMenu(List<MenuType> activatedTypes, bool useActivated = true)
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
