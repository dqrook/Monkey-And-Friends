using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner.UI
{
    public class HeaderMenu : BaseMenu
    {
        public GameObject backButton;
        List<MenuType> previousMenus = new List<MenuType>();

        public override bool IsActive 
        { 
            get
            {
                return base.IsActive;
            }
            set 
            {
                previousMenus.Clear();
                base.IsActive = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            Message.AddListener<EnableHeaderBackButton>(OnEnableHeaderBackButton);
            Message.AddListener<DisableHeaderBackButton>(OnDisableHeaderBackButton);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Message.RemoveListener<EnableHeaderBackButton>(OnEnableHeaderBackButton);
            Message.RemoveListener<DisableHeaderBackButton>(OnDisableHeaderBackButton);
        }

        public void OnClickBackButton()
        {
            MenuType[] menuTypes = new MenuType[previousMenus.Count];
            int i = 0;
            foreach(MenuType type in previousMenus)
            {
                menuTypes[i] = type;
                i++;
            }
            Message.Send(new ActivateMenu(activatedTypes: previousMenus));
            foreach(MenuType menu in menuTypes)
            {
                Debug.Log(menu + " OnClickBackButton");
            }
            Message.Send(new DeactivateMenu(activatedTypes: previousMenus));
        }

        void OnEnableHeaderBackButton(EnableHeaderBackButton enableButton)
        {
            backButton.SetActive(true);
            if(enableButton.previousMenus.Count > 0)
            {
                previousMenus = enableButton.previousMenus;
            }
        }

        void OnDisableHeaderBackButton(DisableHeaderBackButton disableButton)
        {
            backButton.SetActive(false);
        }
    }
}
