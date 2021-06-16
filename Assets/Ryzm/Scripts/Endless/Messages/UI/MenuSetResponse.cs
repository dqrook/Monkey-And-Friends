using Ryzm.EndlessRunner.UI;
using CodeControl;
using System.Collections.Generic;

namespace Ryzm.EndlessRunner.Messages
{
    public class MenuSetResponse : Message
    {
        public List<MenuType> menus;
        public MenuSet set;

        public MenuSetResponse(List<MenuType> menus, MenuSet set)
        {
            this.menus = menus;
            this.set = set;
        }
    }
}
