using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.Blockchain.Messages;
using CodeControl;
using Ryzm.Blockchain;
using TMPro;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner.UI
{
    public class LoginMenu : EndlessMenu
    {
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
        
        string _nearUrl;

        bool gettingCredentials;

        public override bool IsActive 
        { 
            get
            { 
                return base.IsActive;
            }
            set 
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
                    Message.Send(new LoginRequest());
                }
                else
                {
                    Message.RemoveListener<LoginResponse>(OnLoginResponse);
                    Message.RemoveListener<CreateCredentialsResponse>(OnCreateCredentialsResponse);
                }
                base.IsActive = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }

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

        public void Logout() {}

        public void CopyUrlToClipboard()
        {
            GUIUtility.systemCopyBuffer = _nearUrl;
            urlCopied.gameObject.SetActive(true);
        }

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
            switch(response.status)
            {
                case LoginStatus.LoggedOut:
                    loggedOutPanel.SetActive(true);
                    break;
                case LoginStatus.LoggedIn:
                    accountName.text = "Welcome " + response.accountName + "!";
                    loggedInPanel.SetActive(true);
                    break;
                case LoginStatus.TempCredentials:
                    tempCredentialsPanel.SetActive(true);
                    break;
                case LoginStatus.FetchingKeys:
                    loadingPanel.SetActive(true);
                    loadingText.text = "Checking Credentials...";
                    break;
                case LoginStatus.Rejected:
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
            gettingCredentials = false;
            _nearUrl = response.nearUrl;
            Application.OpenURL(_nearUrl);
        }

    }
}
