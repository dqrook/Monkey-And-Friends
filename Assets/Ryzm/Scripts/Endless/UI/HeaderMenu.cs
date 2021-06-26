using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.UI.Messages;
using CodeControl;

namespace Ryzm.UI
{
    public class HeaderMenu : RyzmMenu
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
