using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.Blockchain.Messages;
using CodeControl;
using UnityEngine.Networking;
using Ryzm.Utils;
using System.Text;

namespace Ryzm.Blockchain
{
    public class NearLogin : MonoBehaviour
    {
        public Envs envs;
        AccessKeyResponse accessKeyResponse;
        IEnumerator getAccessKeys;
        bool fetchingKeys;
        string secondaryPublicKey;
        bool secondaryInitialized;
        string nearLogo = "Ⓝ";

        public string AccountName
        {
            get
            {
                return PlayerPrefs.GetString("accountName", "");
            }
            set
            {
                PlayerPrefs.SetString("accountName", value);
                PlayerPrefs.Save();
            }
        }

        public string PublicKey
        {
            get
            {
                return PlayerPrefs.GetString("publicKey", "");
            }
            set
            {
                PlayerPrefs.SetString("publicKey", value);
                PlayerPrefs.Save();
            }
        }

        public string SecretKey
        {
            get
            {
                return PlayerPrefs.GetString("secretKey", "");
            }
            set
            {
                PlayerPrefs.SetString("secretKey", value);
                PlayerPrefs.Save();
            }
        }

        string TempPublicKey
        {
            get
            {
                return PlayerPrefs.GetString("tempPublicKey", "");
            }
            set
            {
                PlayerPrefs.SetString("tempPublicKey", value);
                PlayerPrefs.Save();
            }
        }

        string TempSecretKey
        {
            get
            {
                return PlayerPrefs.GetString("tempSecretKey", "");
            }
            set
            {
                PlayerPrefs.SetString("tempSecretKey", value);
                PlayerPrefs.Save();
            }
        }

        void Awake()
        {
            Message.AddListener<LoginRequest>(OnLoginRequest);
            Message.AddListener<CreateCredentialsRequest>(OnCreateCredentialsRequest);
            Message.AddListener<AttemptLogin>(OnAttemptLogin);
            Message.AddListener<SignMessageRequest>(OnSignMessageRequest);
            Message.AddListener<LogoutRequest>(OnLogoutRequest);

            if(RyzmPlayerPrefs.HasKey("undefined_wallet_auth_key"))
            {
                string s = RyzmPlayerPrefs.GetString("undefined_wallet_auth_key");
                WalletAuthKey authKey = WalletAuthKey.FromJson(s);
                foreach(string key in authKey.allKeys)
                {
                    secondaryPublicKey = key;
                    secondaryInitialized = true;
                }
                if(AccountName.Length == 0)
                {
                    AccountName = authKey.accountId;
                    if(PublicKey == "" || SecretKey == "")
                    {
                        PublicKey = TempPublicKey;
                        SecretKey = TempSecretKey;
                    }
                }
            }
            else
            {
                Debug.Log("no auth key");
            }
            TryLogin();
            // Logout();

            // string[] p = new string[2];
            // p[0] = "access_key/ryzm.near";
            // p[1] = "";
            // string nj = new NearJson("query", p).ToJson();
            // StartCoroutine(_GetAccessKeys("https://rpc.mainnet.near.org", nj));
        }

        void OnDestroy()
        {
            Message.RemoveListener<LoginRequest>(OnLoginRequest);
            Message.RemoveListener<CreateCredentialsRequest>(OnCreateCredentialsRequest);
            Message.RemoveListener<AttemptLogin>(OnAttemptLogin);
            Message.RemoveListener<SignMessageRequest>(OnSignMessageRequest);
            Message.RemoveListener<LogoutRequest>(OnLogoutRequest);
        }

        void TryLogin()
        {
            // if there is an accountName, publicKey and secretKey
            if(HasCredentials())
            {
                if(accessKeyResponse == null)
                {
                    GetAccessKeys(AccountName, LoginStatus.LoggedOut);
                }
                else if(CanAccessContract(PublicKey))
                {
                    Login(AccountName);
                }
                else
                {
                    // means the user has all their info but they still cant access
                    // hopefully will never occur
                    Logout();
                }
            }
            else if(HasTempCredentials())
            {
                Message.Send(new LoginResponse(null, envs.LoginUrl(TempPublicKey), LoginStatus.TempCredentials));
            }
            else
            {
                Logout();
            }
        }

        void OnLoginRequest(LoginRequest request)
        {
            TryLogin();
        }

        void OnCreateCredentialsRequest(CreateCredentialsRequest request)
        {
            if(TempPublicKey == "" || TempSecretKey == "")
            {
                KeyPair kp = CreateKeyPair();
                TempPublicKey = kp.publicKey;
                TempSecretKey = kp.secretKey;
            }
            Message.Send(new CreateCredentialsResponse(envs.LoginUrl(TempPublicKey)));
        }

        void OnAttemptLogin(AttemptLogin attemptLogin)
        {
            Debug.Log(attemptLogin.accountName);
            GetAccessKeys(attemptLogin.accountName, LoginStatus.Rejected);
        }

        void OnSignMessageRequest(SignMessageRequest request)
        {
            if(IsLoggedIn())
            {
                byte[] privateKeyBytes = Base58.Decode(SecretKey);
                byte[] messageBytes = Encoding.ASCII.GetBytes(request.message);
                byte[] signedMessageBytes = TweetNaCl.CryptoSign(messageBytes, privateKeyBytes);
                // string signedMessage = Encoding.ASCII.GetString(signedMessageBytes);
                Message.Send(new SignMessageResponse(request.action, request.message, signedMessageBytes, PublicKey, AccountName));
            }
            else
            {
                Message.Send(new SignMessageResponse());
            }
        }

        void OnLogoutRequest(LogoutRequest request)
        {
            Logout();
        }

        void GetAccessKeys(string accountName, LoginStatus status)
        {
            if(!fetchingKeys)
            {
                getAccessKeys = null;
                string[] p = { "access_key/" + accountName, "" };
                getAccessKeys = _GetAccessKeys(envs.nodeUrl, new NearJson("query", p).ToJson(), accountName, status);
                StartCoroutine(getAccessKeys);
            }
            Message.Send(new LoginResponse(accountName, envs.LoginUrl(TempPublicKey), LoginStatus.FetchingKeys));
        }

        void Login(string _accountName)
        {
            if(PublicKey == "" || SecretKey == "")
            {
                PublicKey = TempPublicKey;
                SecretKey = TempSecretKey;
            }
            AccountName = _accountName;
            TempPublicKey = "";
            TempSecretKey = "";
            Debug.Log("logged in! SecretKey: " + SecretKey);
            Debug.Log("logged in! SecondaryPublicKey: " + secondaryPublicKey);
            Message.Send(new LoginResponse(AccountName, envs.LoginUrl(PublicKey), LoginStatus.LoggedIn, SecretKey, secondaryPublicKey));
        }

        bool CanAccessContract(string _publicKey)
        {
            if(accessKeyResponse != null && accessKeyResponse.result != null && accessKeyResponse.result.keys != null)
            {
                string transformedKey = "ed25519:" + _publicKey;
                foreach(FullKey fullKey in accessKeyResponse.result.keys)
                {
                    if(transformedKey == fullKey.public_key)
                    {
                        if(fullKey.access_key.permission.FunctionCall.receiver_id == envs.contractId) 
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        void RejectLogin(string _accountName)
        {
            accessKeyResponse = null;
            string _publicKey = PublicKey != "" ? PublicKey : TempPublicKey;
            Message.Send(new LoginResponse(_accountName, envs.LoginUrl(_publicKey), LoginStatus.Rejected));
        }

        IEnumerator _GetAccessKeys(string url, string bodyJsonString, string _accountName, LoginStatus status)
        {
            Debug.Log(url + " " + bodyJsonString + " " + _accountName);
            fetchingKeys = true;
            UnityWebRequest request = RyzmUtils.PostRequest(url, bodyJsonString);
            int numFails = 0;
            bool failed = true;
            while(numFails < 3)
            {
                yield return request.SendWebRequest();
                if(request.isNetworkError || request.isHttpError)
                {
                    request = RyzmUtils.PostRequest(url, bodyJsonString);
                    numFails++;
                    Debug.LogError("Failed getting access keys " + numFails + " times");
                }
                else
                {
                    failed = false;
                    break;
                }
            }
            if(failed)
            {
                Debug.LogError("ERROR");
                if(status == LoginStatus.Rejected)
                {
                    RejectLogin(_accountName);
                }
                else if(status == LoginStatus.LoggedOut)
                {
                    Logout();
                }
            }
            else
            {
                string res = request.downloadHandler.text;
                Debug.Log("POST SUCCESS " + res);
                accessKeyResponse = AccessKeyResponse.FromJson(res);
                
                string _publicKey = PublicKey != "" ? PublicKey : TempPublicKey;
                if(CanAccessContract(_publicKey))
                {
                    if(!secondaryInitialized)
                    {
                        AccessKeyResponse2 response2 = AccessKeyResponse2.FromJson(res);
                        int nonce = -1;
                        foreach(FullKey2 key in response2.result.keys)
                        {
                            if(key.access_key.permission == "FullAccess")
                            {
                                if(key.access_key.nonce > nonce)
                                {
                                    nonce = key.access_key.nonce;
                                    secondaryPublicKey = key.public_key;
                                }
                            }
                        }
                    }
                    Login(_accountName);
                }
                else
                {
                    Debug.Log("got keys but cant access contract");
                    if(status == LoginStatus.Rejected)
                    {
                        RejectLogin(_accountName);
                    }
                    else if(status == LoginStatus.LoggedOut)
                    {
                        Logout();
                    }
                }
            }
            fetchingKeys = false;
        }

        bool IsLoggedIn()
        {
            return HasCredentials() && CanAccessContract(PublicKey);
        }

        bool HasCredentials()
        {
            return AccountName.Length > 0 && PublicKey.Length > 0 && SecretKey.Length > 0;
        }

        bool HasTempCredentials()
        {
            return TempPublicKey.Length > 0 && TempSecretKey.Length > 0;
        }

        KeyPair CreateKeyPair()
        {
            return TweetNaCl.CryptoSignKeypair();
        }

        void Logout()
        {
            Reset();
            accessKeyResponse = null;
            Message.Send(new LoginResponse(null, null, LoginStatus.LoggedOut));
            RyzmUtils.BrowserLogout();
        }

        void Reset()
        {
            AccountName = "";
            PublicKey = "";
            SecretKey = "";
            TempSecretKey = "";
            TempPublicKey = "";
            secondaryPublicKey = "";
        }
    }

    [System.Serializable]
    public enum LoginStatus
    {
        LoggedOut,
        LoggedIn,
        FetchingKeys,
        TempCredentials,
        Rejected
    }

    [System.Serializable]
    public class NearJson
    {
        public string method;
        public string[] @params;
        public int id;
        public string jsonrpc = "2.0";

        public NearJson(string method, string[] p, int id = 123)
        {
            this.method = method;
            this.@params = p;
            this.id = id;
        }

        public string ToJson()
        {
            // replace the @ in @params b/c params is a reserved keyword
            return JsonUtility.ToJson(this).Replace("@", "");
        } 
    }

    [System.Serializable]
    public class AccessKeyResponse
    {
        public string jsonrpc;
        public int id;
        public ResponseResult result;

        public static AccessKeyResponse FromJson(string jsonString)
        {
            return JsonUtility.FromJson<AccessKeyResponse>(jsonString);
        }
    }

    [System.Serializable]
    public class ResponseResult
    {
        public int block_height;
        public string block_hash;
        public List<FullKey> keys;
    }

    [System.Serializable]
    public class FullKey
    {
        public string public_key;
        public AccessKey access_key;
    }

    [System.Serializable]
    public class AccessKey
    {
        public int nonce;
        public AccessKeyPermission permission;
    }

    [System.Serializable]
    public class AccessKeyPermission
    {
        public PermissionFunctionCall FunctionCall;
    }

    [System.Serializable]
    public class PermissionFunctionCall
    {
        public string allowance;
        public string receiver_id;
        public List<string> methodNames;
    }

    [System.Serializable]
    public class AccessKeyResponse2
    {
        public string jsonrpc;
        public int id;
        public ResponseResult2 result;

        public static AccessKeyResponse2 FromJson(string jsonString)
        {
            return JsonUtility.FromJson<AccessKeyResponse2>(jsonString);
        }
    }

    [System.Serializable]
    public class ResponseResult2
    {
        public int block_height;
        public string block_hash;
        public List<FullKey2> keys;
    }

    [System.Serializable]
    public class FullKey2
    {
        public string public_key;
        public AccessKey2 access_key;
    }

    [System.Serializable]
    public class AccessKey2
    {
        public int nonce;
        public string permission;
    }

    [System.Serializable]
    public class WalletAuthKey
    {
        public string accountId;
        public List<string> allKeys;

        public static WalletAuthKey FromJson(string jsonString)
        {
            return JsonUtility.FromJson<WalletAuthKey>(jsonString);
        }
    }
}

// near-api-js:keystore:pending_key ed25519:EpDiy6T8fqn6SsAytagkZ7jA2SUN6AStDqPNs4N5aYKq   :mainnet
// ed25519:2Bpde4F2eMUiTeT3G6cjQA1fC3AhEHceEbNQrnPWnhuBV8yY7PgBdp9Wzddo8vVL4zRrnhsXnm8J7FoSHh7iKJZd
// 4bceSkkMyvLp1ZNaFVcTjZhXCcauzvWpJjA28xak23Zz5Yh8qvbJLgiwfjEQLhix1nMkdaHZF5J7Kb6eFv9X5Az9