using System.Collections.Generic;
using CodeControl;
using Ryzm.EndlessRunner.UI;

namespace Ryzm.EndlessRunner.Messages
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
