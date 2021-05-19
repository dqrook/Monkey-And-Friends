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

            Message.AddListener<NearInfoRequest>(OnNearInfoRequest);
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
            Message.RemoveListener<NearInfoRequest>(OnNearInfoRequest);
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
                        Message.Send(new NearInfoResponse(_accountName));
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
                        getAccessKeys = _GetAccessKeys(envs.nodeUrl, new NearJson("query", p).ToJson());
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
            if(!HasCredentials() || (!CanAccessContract() && !fetchingKeys))
            {
                Reset();
                KeyPair kp = CreateKeyPair();
                PublicKey = kp.publicKey;
                SecretKey = kp.secretKey;
                Message.Send(new LoginResponse(envs.LoginUrl()));
            }
        }

        bool CanAccessContract()
        {
            if(accessKeyResponse != null)
            {
                string _publicKey = PublicKey;
                foreach(ParentKey parentKey in accessKeyResponse.result.keys)
                {
                    if(_publicKey == parentKey.public_key && parentKey.access_key.permission.FunctionCall.receiver_id == envs.contractId)
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
                    Message.Send(new NearInfoResponse(AccountName));
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
            Message.Send(new NearInfoResponse(null));
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
        public List<ParentKey> keys;
    }

    [System.Serializable]
    public class ParentKey
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
