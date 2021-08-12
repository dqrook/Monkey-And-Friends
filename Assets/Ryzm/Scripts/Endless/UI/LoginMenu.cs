using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.Blockchain.Messages;
using CodeControl;
using Ryzm.Blockchain;
using TMPro;
using Ryzm.UI.Messages;

namespace Ryzm.UI
{
    public class LoginMenu : RyzmMenu
    {
        #region Public Variables
        [Header("Loading")]
        public GameObject loadingPanel;
        public TextMeshProUGUI loadingText;
        
        [Header("Logged In")]
        public GameObject loggedInPanel;
        public TextMeshProUGUI accountName;
        
        [Header("Logged Out")]
        public GameObject loggedOutPanel;

        [Header("Temp Credentials")]
        public GameObject tempCredentialsPanel;
        public TMP_InputField accountNameInput;
        public TextMeshProUGUI rejectedText;

        [Header("Near Url")]
        public GameObject nearUrlPanel;
        public TMP_InputField accountNameInput2;
        public TextMeshProUGUI urlCopied;
        #endregion
        
        #region Private Variables
        string _nearUrl;
        List<MenuType> mainMenus = new List<MenuType>();
        bool gettingCredentials;
        bool initializedMenus;
        #endregion

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
                    if(!value && _isActive)
                    {
                        gettingCredentials = false;
                        loadingText.text = "Loading...";
                        loadingPanel.SetActive(true);
                        loggedInPanel.SetActive(false);
                        loggedOutPanel.SetActive(false);
                        tempCredentialsPanel.SetActive(false);
                        rejectedText.gameObject.SetActive(false);
                        nearUrlPanel.SetActive(false);
                    }
                    if(value)
                    {
                        Message.AddListener<LoginResponse>(OnLoginResponse);
                        Message.AddListener<CreateCredentialsResponse>(OnCreateCredentialsResponse);
                        Message.AddListener<MenuSetResponse>(OnMenuSetResponse);
                        Message.Send(new LoginRequest());
                        if(!initializedMenus)
                        {
                            initializedMenus = true;
                            Message.Send(new MenuSetRequest(MenuSet.MainMenu));
                        }
                    }
                    else
                    {
                        Message.RemoveListener<LoginResponse>(OnLoginResponse);
                        Message.RemoveListener<CreateCredentialsResponse>(OnCreateCredentialsResponse);
                        Message.RemoveListener<MenuSetResponse>(OnMenuSetResponse);
                    }
                    base.IsActive = value;
                }
            }
        }

        #region Listener Functions
        void OnLoginResponse(LoginResponse response)
        {
            nearUrlPanel.SetActive(false);
            if(!gettingCredentials)
            {
                loadingPanel.SetActive(false);
            }
            if(response.status != LoginStatus.Rejected)
            {
                rejectedText.gameObject.SetActive(false);
            }
            Debug.Log("response.status: " + response.status);
            switch(response.status)
            {
                case LoginStatus.LoggedOut:
                    loggedInPanel.SetActive(false);
                    tempCredentialsPanel.SetActive(false);
                    loggedOutPanel.SetActive(true);
                    break;
                case LoginStatus.LoggedIn:
                    loadingText.gameObject.SetActive(false);
                    loggedOutPanel.SetActive(false);
                    tempCredentialsPanel.SetActive(false);
                    accountName.text = "Welcome " + response.accountName + "!";
                    loggedInPanel.SetActive(true);
                    break;
                case LoginStatus.TempCredentials:
                    loggedOutPanel.SetActive(false);
                    loggedInPanel.SetActive(false);
                    tempCredentialsPanel.SetActive(true);
                    break;
                case LoginStatus.FetchingKeys:
                    loggedOutPanel.SetActive(false);
                    loggedInPanel.SetActive(false);
                    tempCredentialsPanel.SetActive(false);
                    loadingPanel.SetActive(true);
                    loadingText.text = "Checking Credentials...";
                    break;
                case LoginStatus.Rejected:
                    tempCredentialsPanel.SetActive(true);
                    rejectedText.gameObject.SetActive(true);
                    rejectedText.text = "Error logging in for " + response.accountName;
                    break;
                default:
                    break;
            }
        }

        void OnCreateCredentialsResponse(CreateCredentialsResponse response)
        {
            urlCopied.gameObject.SetActive(false);
            nearUrlPanel.SetActive(true);
            loadingPanel.SetActive(false);
            loggedOutPanel.SetActive(false);
            gettingCredentials = false;
            _nearUrl = response.nearUrl;
            Debug.Log(_nearUrl);
            Application.OpenURL(_nearUrl);
            // RyzmUtils.OpenUrl(_nearUrl);
        }

        void OnMenuSetResponse(MenuSetResponse response)
        {
            if(response.set == MenuSet.MainMenu)
            {
                mainMenus = response.menus;
            }
        }
        #endregion

        #region Public Functions
        public void SubmitAccountName()
        {
            Message.Send(new AttemptLogin(accountNameInput.text));
        }

        public void SubmitAccountNameFromUrlPanel()
        {
            Message.Send(new AttemptLogin(accountNameInput2.text));
        }

        public void LoginWithNear()
        {
            loadingPanel.SetActive(true);
            loadingText.text = "Creating Credentials...";
            Message.Send(new CreateCredentialsRequest());
            gettingCredentials = true;
        }

        public void Logout()
        {
            Message.Send(new LogoutRequest());
        }

        public void CopyUrlToClipboard()
        {
            GUIUtility.systemCopyBuffer = _nearUrl;
            urlCopied.gameObject.SetActive(true);
        }

        public void Exit()
        {
            Message.Send(new ActivateMenu(mainMenus));
        }
        #endregion
    }
}
