using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner.UI
{
    [CreateAssetMenu(fileName = "MenuSets", menuName = "ScriptableObjects/EndlessMenuSets", order = 3)]
    public class EndlessMenuSets : ScriptableObject
    {
        public List<EndlessMenuSet> menuSets = new List<EndlessMenuSet>();

        List<MenuType> emptyMenus = new List<MenuType>();

        public List<MenuType> GetMenuTypes(MenuSet set)
        {
            foreach(EndlessMenuSet menuSet in menuSets)
            {
                if(menuSet.type == set)
                {
                    return menuSet.menus;
                }
            }
            return emptyMenus;
        }
    }

    [System.Serializable]
    public class EndlessMenuSet
    {
        public MenuSet type;
        public List<MenuType> menus = new List<MenuType>();
    }
}
