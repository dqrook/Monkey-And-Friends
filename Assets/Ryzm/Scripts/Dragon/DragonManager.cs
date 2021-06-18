using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.Blockchain.Messages;
using CodeControl;
using Ryzm.Blockchain;
using Ryzm.Utils;
using UnityEngine.Networking;
using Ryzm.Dragon.Messages;
using Ryzm.EndlessRunner;

namespace Ryzm.Dragon
{
    public class DragonManager : MonoBehaviour
    {
        public Envs envs;
        public DragonPrefabs prefabs;
        public List<EndlessDragon> dragons;
        public List<DragonResponse> dragonResponses;

        [Header("Spawns")]
        public Transform newDragonSpawn;

        IEnumerator getDragons;
        bool gettingDragons;
        IEnumerator getDragonTexture;
        string accountName;
        bool breedingDragons;
        IEnumerator breedDragons;
        int dragon1Id;
        int dragon2Id;
        bool initialized;

        void Awake()
        {
            Message.AddListener<LoginResponse>(OnLoginResponse);
            Message.AddListener<SignMessageResponse>(OnSignMessageResponse);
            Message.AddListener<BreedDragonsRequest>(OnBreedDragonsRequest);
            Message.AddListener<DragonsRequest>(OnDragonsRequest);
        }

        void Start()
        {
            Message.Send(new LoginRequest());
        }

        void OnDestroy()
        {
            Message.RemoveListener<LoginResponse>(OnLoginResponse);
            Message.RemoveListener<SignMessageResponse>(OnSignMessageResponse);
            Message.RemoveListener<BreedDragonsRequest>(OnBreedDragonsRequest);
            Message.RemoveListener<DragonsRequest>(OnDragonsRequest);
            initialized = false;
        }
        
        void OnLoginResponse(LoginResponse response)
        {
            if(response.status == LoginStatus.LoggedIn)
            {
                accountName = response.accountName;
                if(!initialized)
                {
                    initialized = true;
                    // if logged in then let's get the dragon data
                    // first ya gotta sign a message
                    Message.Send(new SignMessageRequest("getDragons", "Hello World"));
                }
            }
            else if(response.status == LoginStatus.LoggedOut)
            {
                accountName = "";
            }
        }

        void OnSignMessageResponse(SignMessageResponse response)
        {
            if(response.action == "getDragons")
            {
                if(response.isSuccess)
                {
                    if(!gettingDragons)
                    {
                        string url = envs.GetDragonsApiUrl;
                        string bodyJsonString = new GetDragonsPostRequest(response.message, response.signedMessageBytes, response.publicKey, response.accountId).ToJson();
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
            else if (response.action == "breedDragons")
            {
                if(response.isSuccess)
                {
                    if(!breedingDragons)
                    {
                        string url = envs.BreedDragonsApiUrl;
                        string bodyJsonString = new BreedDragonsPostRequest(response.message, response.signedMessageBytes, response.publicKey, response.accountId, dragon1Id, dragon2Id).ToJson();
                        breedDragons = null;
                        breedDragons = BreedDragons(url, bodyJsonString);
                        StartCoroutine(breedDragons);
                        Message.Send(new BreedDragonsResponse(BreedingStatus.Breeding));
                    }
                }
                else
                {
                    Message.Send(new BreedDragonsResponse(BreedingStatus.Failed));
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
                GetDragonsPostResponse response = GetDragonsPostResponse.FromJson(res);
                dragonResponses = response.dragons;
                foreach(DragonResponse dragonRes in dragonResponses)
                {
                    GameObject go = GameObject.Instantiate(prefabs.GetPrefabByHornType(dragonRes.hornType).dragon);
                    EndlessDragon dragon = go.GetComponent<EndlessDragon>();
                    dragon.data = dragonRes;
                    dragons.Add(dragon);
                    List<MaterialTypeToUrlMap> map = new List<MaterialTypeToUrlMap>
                    {
                        new MaterialTypeToUrlMap(DragonMaterialType.Body, dragon.data.bodyTexture),
                        new MaterialTypeToUrlMap(DragonMaterialType.Wing, dragon.data.wingTexture),
                        new MaterialTypeToUrlMap(DragonMaterialType.Horn, dragon.data.hornTexture),
                        new MaterialTypeToUrlMap(DragonMaterialType.Back, dragon.data.backTexture)
                    };
                    getDragonTexture = null;
                    getDragonTexture = GetDragonTexture(dragon, map);
                    StartCoroutine(getDragonTexture);
                }
                Message.Send(new DragonsResponse(dragons, "all"));
            }
            gettingDragons = false;
        }

        IEnumerator GetDragonTexture(EndlessDragon dragon, List<MaterialTypeToUrlMap> map)
        {
            int numMaterials = map.Count;
            int index = 0;
            while(index < numMaterials)
            {
                string url = map[index].url;
                DragonMaterialType type = map[index].type;
                UnityWebRequest request = RyzmUtils.TextureRequest(url);
                yield return request.SendWebRequest();
                if(request.isNetworkError || request.isHttpError)
                {
                    Debug.LogError("ERROR");
                    // todo: handle this case
                }
                else
                {
                    if(dragon != null && dragon.materials != null)
                    {
                        Texture _texture = DownloadHandlerTexture.GetContent(request);
                        dragon.SetTexture(type, _texture);
                    }

                }
                index++;
                yield return null;
            }
        }

        void OnBreedDragonsRequest(BreedDragonsRequest request)
        {
            if(accountName.Length > 0)
            {
                dragon1Id = request.dragon1Id;
                dragon2Id = request.dragon2Id;
                Message.Send(new SignMessageRequest("breedDragons", "Hello World"));
            }
        }

        IEnumerator BreedDragons(string url, string bodyJsonString)
        {
            breedingDragons = true;
            UnityWebRequest request = RyzmUtils.PostRequest(url, bodyJsonString);
            yield return request.SendWebRequest();
            if(request.isNetworkError || request.isHttpError)
            {
                Debug.LogError("ERROR");
                Message.Send(new BreedDragonsResponse(BreedingStatus.Failed));
                // todo: handle this case
            }
            else
            {
                string res = request.downloadHandler.text;
                Debug.Log("POST SUCCESS " + res);
                BreedDragonsPostResponse response = BreedDragonsPostResponse.FromJson(res);
                DragonResponse dragonRes = response.dragon;
                GameObject go = GameObject.Instantiate(prefabs.GetPrefabByHornType(dragonRes.hornType).dragon);
                EndlessDragon dragon = go.GetComponent<EndlessDragon>();
                dragon.data = dragonRes;
                dragons.Add(dragon);
                Message.Send(new DragonsResponse(dragons, "newDragon"));
                List<MaterialTypeToUrlMap> map = new List<MaterialTypeToUrlMap>
                {
                    new MaterialTypeToUrlMap(DragonMaterialType.Body, dragon.data.bodyTexture),
                    new MaterialTypeToUrlMap(DragonMaterialType.Wing, dragon.data.wingTexture),
                    new MaterialTypeToUrlMap(DragonMaterialType.Horn, dragon.data.hornTexture),
                    new MaterialTypeToUrlMap(DragonMaterialType.Back, dragon.data.backTexture)
                };
                getDragonTexture = null;
                getDragonTexture = GetDragonTexture(dragon, map);
                Message.Send(new BreedDragonsResponse(BreedingStatus.Success, dragonRes.id));
                StartCoroutine(getDragonTexture);
            }
            breedingDragons = false;
        }

        void OnDragonsRequest(DragonsRequest request)
        {
            Message.Send(new DragonsResponse(dragons, request.sender));
        }
    }

    [System.Serializable]
    public class GetDragonsPostRequest
    {
        public string message;
        public byte[] signedMessage;
        public string publicKey;
        public string accountId;

        public GetDragonsPostRequest(string message, byte[] signedMessage, string publicKey, string accountId)
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
    public class GetDragonsPostResponse
    {
        public List<DragonResponse> dragons;

        public static GetDragonsPostResponse FromJson(string jsonString)
        {
            return JsonUtility.FromJson<GetDragonsPostResponse>(jsonString);
        }
    }

    [System.Serializable]
    public class DragonResponse
    {
        public int id;
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

    [System.Serializable]
    public class BreedDragonsPostRequest
    {
        public string message;
        public byte[] signedMessage;
        public string publicKey;
        public string accountId;
        public int dragon1Id;
        public int dragon2Id;

        public BreedDragonsPostRequest(string message, byte[] signedMessage, string publicKey, string accountId, int dragon1Id, int dragon2Id)
        {
            this.message = message;
            this.signedMessage = signedMessage;
            this.publicKey = publicKey;
            this.accountId = accountId;
            this.dragon1Id = dragon1Id;
            this.dragon2Id = dragon2Id;
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        } 
    }

    [System.Serializable]
    public class BreedDragonsPostResponse
    {
        public DragonResponse dragon;

        public static BreedDragonsPostResponse FromJson(string jsonString)
        {
            return JsonUtility.FromJson<BreedDragonsPostResponse>(jsonString);
        }
    }

    public class MaterialTypeToUrlMap
    {
        public DragonMaterialType type;
        public string url;

        public MaterialTypeToUrlMap(DragonMaterialType type, string url)
        {
            this.type = type;
            this.url = url;
        }
    }

    public enum BreedingStatus 
    {
        Breeding,
        Failed,
        Success
    }
}
