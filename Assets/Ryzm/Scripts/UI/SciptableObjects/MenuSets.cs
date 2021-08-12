using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.UI
{
    [CreateAssetMenu(fileName = "MenuSets", menuName = "ScriptableObjects/MenuSets", order = 3)]
    public class MenuSets : ScriptableObject
    {
        #region Public Variables
        public List<MenuSetMetadata> menuSets = new List<MenuSetMetadata>();
        #endregion

        #region Private Variables
        List<MenuType> emptyMenus = new List<MenuType>();
        #endregion

        #region Public Variables
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
        #endregion
    }

    [System.Serializable]
    public class MenuSetMetadata
    {
        public MenuSet type;
        public List<MenuType> menus = new List<MenuType>();
    }
}
