using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.UI
{
    [CreateAssetMenu(fileName = "MenuSets", menuName = "ScriptableObjects/MenuSets", order = 3)]
    public class MenuSets : ScriptableObject
    {
        public List<MenuSetMetadata> menuSets = new List<MenuSetMetadata>();

        List<MenuType> emptyMenus = new List<MenuType>();

        public List<MenuType> GetMenuTypes(MenuSet set)
        {
            foreach(MenuSetMetadata menuSet in menuSets)
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
    public class MenuSetMetadata
    {
        public MenuSet type;
        public List<MenuType> menus = new List<MenuType>();
    }
}
