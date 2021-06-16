using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;
using Ryzm.Blockchain.Messages;
using TMPro;
using Ryzm.Blockchain;
using Ryzm.Dragon.Messages;

namespace Ryzm.EndlessRunner.UI
{
    public class MainMenuBreeding : BaseMenu
    {
        public TextMeshProUGUI loginText;

        List<MenuType> breedingMenus = new List<MenuType> {};

        public override bool IsActive 
        { 
            get
            { 
                return base.IsActive;
            }
            set 
            {
                if(!disable)
                {
                    if(value)
                    {
                        Message.AddListener<LoginResponse>(OnLoginResponse);
                        Message.AddListener<MenuSetResponse>(OnMenuSetResponse);
                        Message.Send(new LoginRequest());
                        Message.Send(new ResetDragons());
                        Message.Send(new MenuSetRequest(MenuSet.MainMenu));
                        Message.Send(new MenuSetRequest(MenuSet.LoginMenu));
                    }
                    else
                    {
                        Message.RemoveListener<LoginResponse>(OnLoginResponse);
                        Message.AddListener<MenuSetResponse>(OnMenuSetResponse);
                    }
                }
                base.IsActive = value;
            }
        }

        void OnLoginResponse(LoginResponse response)
        {
            switch(response.status)
            {
                case LoginStatus.FetchingKeys:
                    loginText.text = "Loading...";
                    break;
                case LoginStatus.LoggedIn:
                    loginText.text = response.accountName;
                    break;
                default:
                    loginText.text = "Login";
                    break;
            }
        }

        void OnMenuSetResponse(MenuSetResponse response)
        {
            if(response.set == MenuSet.BreedingMenu)
            {
                breedingMenus = response.menus;
            }
        }
    }
}
