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


            // accountName = PlayerPrefs.GetString("accountName", "");
            // publicKey = PlayerPrefs.GetString("publicKey", "");
            // secretKey = PlayerPrefs.GetString("secretKey", "");
            
            // string[] p = new string[2];
            // p[0] = "access_key/ryzm.near";
            // p[1] = "";
            // string nj = new NearJson("query", p).ToJson();
            // StartCoroutine(_GetAccessKeys("https://rpc.mainnet.near.org", nj));
        }

        void OnDestroy()
        {
            Message.RemoveListener<LoginRequest>(OnLoginRequest);
        }

        void OnNearInfoRequest(NearInfoRequest request)
        {
            string _accountName = AccountName;
            if(HasCredentials())
            {
                if(accessKeyResponse != null)
                {
                    if(CanAccessContract())
                    {
                        Login();
                    }
                    else
                    {
                        Logout();
                    }
                }
                else
                {
                    if(!fetchingKeys)
                    {
                        getAccessKeys = null;
                        string[] p = { "access_key/" + _accountName, "" };
                        getAccessKeys = _GetAccessKeys(envs.walletUrl, new NearJson("query", p).ToJson());
                        StartCoroutine(getAccessKeys);
                    }
                }
            }
            else
            {
                Logout();
            }
        }

        void OnLoginRequest(LoginRequest request)
        {
            bool needKeys = !CanAccessContract() && !fetchingKeys;
            if(!HasCredentials() || needKeys)
            {
                // Reset();
                KeyPair kp = CreateKeyPair();
                if(PublicKey == "") 
                {
                    PublicKey = kp.publicKey;
                }
                if(SecretKey == "")
                {
                    SecretKey = kp.secretKey;
                }
                Debug.Log(envs.LoginUrl(PublicKey));
                
                bool fetchingKeys = needKeys && HasCredentials();
                string _accountName = AccountName != "" ? AccountName : null;
                if(needKeys && HasCredentials())
                {
                    // if you have all the necessary credentials but you havent gotten the access keys to check if they are ok 
                    getAccessKeys = null;
                    string[] p = { "access_key/" + _accountName, "" };
                    getAccessKeys = _GetAccessKeys(envs.walletUrl, new NearJson("query", p).ToJson());
                    StartCoroutine(getAccessKeys);
                }
                Message.Send(new LoginResponse(_accountName, envs.LoginUrl(PublicKey), false, fetchingKeys));
            }
            else
            {
                Login();
            }
        }

        void Login()
        {
            Message.Send(new LoginResponse(AccountName, envs.LoginUrl(PublicKey), true, false));
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

        IEnumerator _GetAccessKeys(string url, string bodyJsonString)
        {
            fetchingKeys = true;
            UnityWebRequest request = PostRequest(url, bodyJsonString);
            yield return request.SendWebRequest();
            if(request.isNetworkError || request.isHttpError)
            {
                Debug.LogError("ERROR");
            }
            else
            {
                Debug.Log("POST SUCCESS");
                string res = request.downloadHandler.text;
                accessKeyResponse = AccessKeyResponse.FromJson(res);
                Debug.Log(accessKeyResponse.result.keys[0].access_key.permission.FunctionCall.receiver_id);
                if(CanAccessContract())
                {
                    Login();
                }
                else
                {
                    Logout();
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

        public bool HasCredentials()
        {
            return AccountName.Length > 0 && PublicKey.Length > 0 && SecretKey.Length > 0;
        }

        public KeyPair CreateKeyPair()
        {
            return TweetNaCl.CryptoBoxKeypair();
        }

        public void Logout()
        {
            Reset();
            accessKeyResponse = null;
            Message.Send(new LoginResponse(null, null, false, false));
        }

        void Reset()
        {
            AccountName = "";
            PublicKey = "";
            SecretKey = "";
        }
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
