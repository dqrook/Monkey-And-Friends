using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.UI.Messages;
using CodeControl;
using Ryzm.Blockchain.Messages;
using TMPro;
using Ryzm.Blockchain;
using Ryzm.Dragon.Messages;
using Ryzm.Dragon;

namespace Ryzm.UI
{
    public class MainMenuBreeding : RyzmMenu
    {
        public TextMeshProUGUI loginText;

        bool initialized;
        List<MenuType> breedingMenus = new List<MenuType> {};
        List<MenuType> loginMenus = new List<MenuType> {};
        List<MenuType> mainMenus = new List<MenuType> {};
        bool menuSetsInitialized;

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
                        Message.Send(new ResetDragons());
                        if(!menuSetsInitialized)
                        {
                            menuSetsInitialized = true;
                            Message.Send(new MenuSetRequest(MenuSet.BreedingMenu));
                            Message.Send(new MenuSetRequest(MenuSet.MainMenu));
                            Message.Send(new MenuSetRequest(MenuSet.LoginMenu));
                        }
                        InitializeCamera();
                    }
                    else
                    {
                        initialized = false;
                        StopAllCoroutines();
                        Message.RemoveListener<LoginResponse>(OnLoginResponse);
                        Message.RemoveListener<MenuSetResponse>(OnMenuSetResponse);
                    }
                    base.IsActive = value;
                }
            }
        }

        public void OnClickLogin()
        {
            Debug.Log("on click login");
            if(IsActive)
            {
                Message.Send(new ActivateMenu(activatedTypes: loginMenus));
                Message.Send(new EnableHeaderBackButton(mainMenus));
            }
        }

        public void OnClickMarket()
        {

        }

        public void OnClickBreed()
        {
            if(IsActive)
            {
                Message.Send(new ActivateMenu(activatedTypes: breedingMenus));
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
            else if(response.set == MenuSet.MainMenu)
            {
                mainMenus = response.menus;
            }
            else if(response.set == MenuSet.LoginMenu)
            {
                loginMenus = response.menus;
            }
        }

        void InitializeCamera()
        {
            Debug.Log("initializing z camera " + initialized);
            if(!initialized)
            {
                initialized = true;
                Message.Send(new MoveCameraRequest(TransformType.MainMenu));
            }
        }
    }
}
