using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner.UI
{
    public class HeaderMenu : EndlessMenu
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
            Message.Send(new ActivateMenu(activatedTypes: previousMenus));
            Message.Send(new DeactivateMenu(activatedTypes: previousMenus));
        }

        void OnEnableHeaderBackButton(EnableHeaderBackButton enableButton)
        {
            backButton.SetActive(true);
            if(enableButton.previousMenus.Count > 0)
            {
                previousMenus = enableButton.previousMenus;
            }
            else
            {
                previousMenus.Add(enableButton.previousMenu);
            }
        }

        void OnDisableHeaderBackButton(DisableHeaderBackButton disableButton)
        {
            backButton.SetActive(false);
        }
    }
}
