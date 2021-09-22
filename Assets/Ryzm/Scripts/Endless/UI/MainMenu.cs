using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.UI.Messages;
using CodeControl;
using Ryzm.Blockchain.Messages;
using TMPro;
using Ryzm.Blockchain;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.UI
{
    public class MainMenu : RyzmMenu
    {
        #region Public Variables
        public TextMeshProUGUI loginText;
        #endregion

        #region Private Variables
        List<MenuType> loginMenus = new List<MenuType>();
        List<MenuType> settingsMenus = new List<MenuType>();
        #endregion

        #region Properties
        public override bool IsActive 
        { 
            get
            { 
                return base.IsActive;
            }
            set 
            {
                if(ShouldUpdate(value))
                {
                    if(value)
                    {
                        Message.AddListener<LoginResponse>(OnLoginResponse);
                        Message.AddListener<MenuSetResponse>(OnMenuSetResponse);
                        Message.Send(new LoginRequest());
                        Message.Send(new MenuSetRequest(MenuSet.LoginMenu));
                        Message.Send(new MenuSetRequest(MenuSet.SettingsMenu));
                    }
                    else
                    {
                        Message.RemoveListener<LoginResponse>(OnLoginResponse);
                        Message.RemoveListener<MenuSetResponse>(OnMenuSetResponse);
                    }
                    base.IsActive = value;
                }
            }
        }
        #endregion

        #region Listener Functions
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
            if(response.set == MenuSet.SettingsMenu)
            {
                settingsMenus = response.menus;
            }
            else if(response.set == MenuSet.LoginMenu)
            {
                loginMenus = response.menus;
            }
        }
        #endregion
        
        #region Public Functions
        public void OnClickStart()
        {
            Message.Send(new StartingGame());
        }

        public void OnClickLogin()
        {
            if(IsActive)
            {
                Message.Send(new ActivateMenu(loginMenus));
            }
        }

        public void OnClickSettings()
        {
            if(IsActive)
            {
                Message.Send(new ActivateMenu(settingsMenus));
            }
        }
        #endregion
    }
}
