using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.Blockchain.Messages;
using CodeControl;
using Ryzm.Blockchain;
using Ryzm.Utils;
using UnityEngine.Networking;

namespace Ryzm.Dragon
{
    public class DragonManager : MonoBehaviour
    {
        public Envs envs;
        public List<DragonResponse> dragons;

        IEnumerator getDragons;
        bool gettingDragons;

        void Awake()
        {
            Message.AddListener<LoginResponse>(OnLoginResponse);
            Message.AddListener<SignMessageResponse>(OnSignMessageResponse);
        }

        void Start()
        {
            Message.Send(new LoginRequest());
        }

        void OnDestroy()
        {
            Message.RemoveListener<LoginResponse>(OnLoginResponse);
            Message.RemoveListener<SignMessageResponse>(OnSignMessageResponse);
        }
        
        void OnLoginResponse(LoginResponse response)
        {
            if(response.status == LoginStatus.LoggedIn)
            {
                // if logged in then let's get the dragon data
                // first ya gotta sign a message
                Message.Send(new SignMessageRequest("dragonManager", "Hello World"));
            }
        }

        void OnSignMessageResponse(SignMessageResponse response)
        {
            if(response.receiver == "dragonManager")
            {
                if(response.isSuccess)
                {
                    if(!gettingDragons)
                    {
                        string url = envs.GetDragonsApiUrl;
                        string bodyJsonString = new GetDragonsRequest(response.message, response.signedMessageBytes, response.publicKey, response.accountId).ToJson();
                        Debug.Log(bodyJsonString);
                        getDragons = null;
                        getDragons = GetDragons(url, bodyJsonString);
                        StartCoroutine(getDragons);
                    }
                }
                else
                {
                    // todo: handle when signing the message fails
                }
            }
        }

        IEnumerator GetDragons(string url, string bodyJsonString)
        {
            gettingDragons = true;
            UnityWebRequest request = RyzmUtils.PostRequest(url, bodyJsonString);
            yield return request.SendWebRequest();
            if(request.isNetworkError || request.isHttpError)
            {
                Debug.LogError("ERROR");
                // todo: handle this case
            }
            else
            {
                string res = request.downloadHandler.text;
                Debug.Log("POST SUCCESS " + res);
                GetDragonsResponse response = GetDragonsResponse.FromJson(res);
                dragons = response.dragons;
            }
            gettingDragons = false;
        }
    }

    [System.Serializable]
    public class GetDragonsRequest
    {
        public string message;
        public byte[] signedMessage;
        public string publicKey;
        public string accountId;

        public GetDragonsRequest(string message, byte[] signedMessage, string publicKey, string accountId)
        {
            this.message = message;
            this.signedMessage = signedMessage;
            this.publicKey = publicKey;
            this.accountId = accountId;
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        } 
    }

    [System.Serializable]
    public class GetDragonsResponse
    {
        public List<DragonResponse> dragons;

        public static GetDragonsResponse FromJson(string jsonString)
        {
            return JsonUtility.FromJson<GetDragonsResponse>(jsonString);
        }
    }

    [System.Serializable]
    public class DragonResponse
    {
        public string id;
        public string owner;
        public List<int> genes;
        public int baseSpeed;
        public int baseAttack;
        public int baseDefense;
        public int baseHealth;
        public string bodyTexture;
        public string wingTexture;
        public string backTexture;
        public string hornTexture;
        public int hornType;
    }
}
