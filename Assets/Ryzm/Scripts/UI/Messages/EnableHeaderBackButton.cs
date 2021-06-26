using System.Collections.Generic;
using CodeControl;

namespace Ryzm.UI.Messages
{
    public class EnableHeaderBackButton : Message
    {
        public List<MenuType> previousMenus = new List<MenuType>();

        public EnableHeaderBackButton(MenuType previousMenu)
        {
            previousMenus.Add(previousMenu);
        }

        public EnableHeaderBackButton(List<MenuType> previousMenus)
        {
            this.previousMenus = previousMenus;
        }
    }
}