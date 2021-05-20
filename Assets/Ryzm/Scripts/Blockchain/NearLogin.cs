using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.Blockchain.Messages;
using CodeControl;
using UnityEngine.Networking;
using System.Text;

namespace Ryzm.Blockchain
{
    public class NearLogin : MonoBehaviour
    {
        private static NearLogin _instance;
        public static NearLogin Instance { get { return _instance; } }

        public NearEnvs envs;
        AccessKeyResponse accessKeyResponse;
        IEnumerator getAccessKeys;
        bool fetchingKeys;

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
            if(_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                _instance = this;
            }

            Message.AddListener<LoginRequest>(OnLoginRequest);
            Message.AddListener<CreateCredentialsRequest>(OnCreateCredentialsRequest);
            Message.AddListener<AttemptLogin>(OnAttemptLogin);
            Initialize();

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
        }

        void Initialize()
        {
            // if there is an accountName, publicKey and secretKey
            if(HasCredentials())
            {
                if(accessKeyResponse == null)
                {
                    GetAccessKeys(AccountName);
                }
                else if(CanAccessContract())
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
            Initialize();
            // bool needKeys = !CanAccessContract() && !fetchingKeys;
            // if(!HasCredentials() || needKeys)
            // {
            //     KeyPair kp = CreateKeyPair();
            //     bool hasPublicKey = PublicKey != "";
            //     if(PublicKey == "") 
            //     {
            //         PublicKey = kp.publicKey;
            //     }
            //     if(SecretKey == "")
            //     {
            //         SecretKey = kp.secretKey;
            //     }
            //     Debug.Log(envs.LoginUrl(PublicKey));
                
            //     bool fetchingKeys = needKeys && HasCredentials();
            //     string _accountName = AccountName != "" ? AccountName : null;
            //     Message.Send(new LoginResponse(_accountName, envs.LoginUrl(PublicKey), false, hasPublicKey, fetchingKeys));
            //     if(fetchingKeys)
            //     {
            //         // if you have all the necessary credentials but you havent gotten the access keys to check if they are ok 
            //         getAccessKeys = null;
            //         string[] p = { "access_key/" + _accountName, "" };
            //         getAccessKeys = _GetAccessKeys(envs.walletUrl, new NearJson("query", p).ToJson());
            //         StartCoroutine(getAccessKeys);
            //     }
            // }
            // else
            // {
            //     Login();
            // }
        }

        void OnAttemptLogin(AttemptLogin attemptLogin)
        {
            GetAccessKeys(attemptLogin.accountName);
        }

        void GetAccessKeys(string accountName)
        {
            if(!fetchingKeys)
            {
                getAccessKeys = null;
                string[] p = { "access_key/" + accountName, "" };
                getAccessKeys = _GetAccessKeys(envs.walletUrl, new NearJson("query", p).ToJson(), accountName);
                StartCoroutine(getAccessKeys);
            }
            Message.Send(new LoginResponse(accountName, envs.LoginUrl(TempPublicKey), LoginStatus.FetchingKeys));
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
            Message.Send(new LoginResponse(AccountName, envs.LoginUrl(PublicKey), LoginStatus.LoggedIn));
        }

        bool CanAccessContract()
        {
            if(accessKeyResponse != null)
            {
                string _publicKey = "ed25519:" + PublicKey;
                foreach(FullKey fullKey in accessKeyResponse.result.keys)
                {
                    if(_publicKey == fullKey.public_key && fullKey.access_key.permission.FunctionCall.receiver_id == envs.contractId)
                    {
                        return true;
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

        IEnumerator _GetAccessKeys(string url, string bodyJsonString, string _accountName)
        {
            fetchingKeys = true;
            UnityWebRequest request = PostRequest(url, bodyJsonString);
            yield return request.SendWebRequest();
            if(request.isNetworkError || request.isHttpError)
            {
                Debug.LogError("ERROR");
                RejectLogin(_accountName);
            }
            else
            {
                Debug.Log("POST SUCCESS");
                string res = request.downloadHandler.text;
                accessKeyResponse = AccessKeyResponse.FromJson(res);
                Debug.Log(accessKeyResponse.result.keys[0].access_key.permission.FunctionCall.receiver_id);
                if(CanAccessContract())
                {
                    Login(_accountName);
                }
                else
                {
                    RejectLogin(_accountName);
                }
            }
            fetchingKeys = false;
        }

        UnityWebRequest PostRequest(string url, string bodyJsonString)
        {
            UnityWebRequest request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
            request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json; charset=utf-8");
            return request;
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
            return TweetNaCl.CryptoBoxKeypair();
        }

        void Logout()
        {
            Reset();
            accessKeyResponse = null;
            Message.Send(new LoginResponse(null, null, LoginStatus.LoggedOut));
        }

        void Reset()
        {
            AccountName = "";
            PublicKey = "";
            SecretKey = "";
            TempSecretKey = "";
            TempPublicKey = "";
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
}
