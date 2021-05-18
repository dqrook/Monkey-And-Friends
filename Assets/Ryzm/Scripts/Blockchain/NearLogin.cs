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
        public string accountName;
        public string publicKey;
        public string secretKey;

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
            accountName = PlayerPrefs.GetString("accountName", "");
            publicKey = PlayerPrefs.GetString("publicKey", "");
            secretKey = PlayerPrefs.GetString("secretKey", "");
            Debug.Log(GetUrl());
            Message.AddListener<NearUrlRequest>(OnNearUrlRequest);
            string[] p = new string[2];
            p[0] = "access_key/ryzm.near";
            p[1] = "";
            string nj = new NearJson("query", p).ToJson();
            StartCoroutine(SaveInterval("https://rpc.mainnet.near.org", nj));
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

        IEnumerator SaveInterval(string url, string bodyJsonString)
        {
            UnityWebRequest request = PostRequest(url, bodyJsonString);
            yield return request.SendWebRequest();
            if(request.isNetworkError || request.isHttpError)
            {
                Debug.LogError("ERROR");
            }
            else
            {
                Debug.Log("POST SUCCESS");
                string response = request.downloadHandler.text;
                AccessKeyResponse akr = AccessKeyResponse.FromJson(response);
                Debug.Log(akr.result.keys[0].access_key.permission.FunctionCall.receiver_id);
                // string[] separatingStrings = { "result" };
                // string[] words = response.Split(separatingStrings, System.StringSplitOptions.RemoveEmptyEntries);
                // foreach(string word in words)
                // {
                //     Debug.Log(word);
                //     if(word.Contains("keys"))
                //     {

                //     }
                // }
            }
        }

        void OnDestroy()
        {
            Message.RemoveListener<NearUrlRequest>(OnNearUrlRequest); 
        }

        void OnNearUrlRequest(NearUrlRequest request)
        {
            object o = new object();
        }

        public string GetUrl()
        {
            return envs.GetUrl();
        }

        public bool HasCredentials()
        {
            return accountName.Length > 0 && publicKey.Length > 0 && secretKey.Length > 0;
        }

        public KeyPair CreateKeyPair()
        {
            return TweetNaCl.CryptoBoxKeypair();
        }

        public string AccountName()
        {
            if(!IsLoggedIn())
            {
                return null;
            }
            return accountName;
        }

        public bool IsLoggedIn()
        {
            if(HasCredentials())
            {

            }
            else
            {
                Reset();
                KeyPair kp = CreateKeyPair();
                PlayerPrefs.SetString("publicKey", kp.publicKey);
                PlayerPrefs.SetString("secretKey", kp.secretKey);
            }
            return false;
        }

        public void Logout()
        {
            Reset();
        }

        void Reset()
        {
            PlayerPrefs.SetString("accountName", "");
            PlayerPrefs.SetString("publicKey", "");
            PlayerPrefs.SetString("secretKey", "");
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
            return JsonUtility.ToJson(this).Replace("@", "");
        } 
    }

    [System.Serializable]
    public class AccessKeyResponse
    {
        public string jsonrpc;
        public int id;
        public AccessKeyResponseResult result;

        public static AccessKeyResponse FromJson(string jsonString)
        {
            return JsonUtility.FromJson<AccessKeyResponse>(jsonString);
        }
    }

    [System.Serializable]
    public class AccessKeyResponseResult
    {
        public int block_height;
        public string block_hash;
        public List<ResultKey> keys;
    }

    [System.Serializable]
    public class ResultKey
    {
        public string public_key;
        public ResultAccessKey access_key;
    }

    [System.Serializable]
    public class ResultAccessKey
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
