﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.Blockchain.Messages;
using CodeControl;
using Ryzm.Blockchain;
using Ryzm.Utils;
using UnityEngine.Networking;
using Ryzm.Dragon.Messages;
using Ryzm.EndlessRunner;
using System.Linq;

namespace Ryzm.Dragon
{
    public class DragonManager : MonoBehaviour
    {
        public NearEnvs nearEnvs;
        public Envs envs;
        public DragonPrefabs prefabs;
        public List<DragonResponse> dragonResponses = new List<DragonResponse>();
        public Dictionary<int, EndlessDragon> dragons = new Dictionary<int, EndlessDragon>();

        [Header("Spawns")]
        public Transform newDragonSpawn;

        IEnumerator getDragons;
        bool gettingDragons;
        IEnumerator getDragonTexture;
        string accountName;
        string privateKey;
        string secondaryPublicKey;
        bool breedingDragons;
        IEnumerator breedDragons;
        int dragon1Id;
        int dragon2Id;
        bool initialized;
        int currentNumberOfDragons;
        IEnumerator getDragonIds;
        IEnumerator getDragonById;

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
                privateKey = response.privateKey;
                secondaryPublicKey = response.secondaryPublicKey;
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
                initialized = false;
                accountName = "";
                privateKey = "";
                secondaryPublicKey = "";
                foreach(EndlessDragon dragon in dragons.Values)
                {
                    Destroy(dragon.gameObject);
                }
                dragons.Clear();
                dragonResponses.Clear();
                Message.Send(new DragonsResponse(dragons.Values.ToList(), "all"));
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
                        string bodyJsonString = new BreedDragonsPostRequest(response.message, response.signedMessageBytes, response.publicKey, response.accountId, dragon1Id, dragon2Id, privateKey).ToJson();
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
                    dragons.Add(dragon.data.id, dragon);
                    dragon.GetTextures();
                }
                Message.Send(new DragonsResponse(dragons.Values.ToList(), "all"));
            }
            gettingDragons = false;
        }

        IEnumerator GetDragonTexture(EndlessDragon dragon)
        {
            List<MaterialTypeToUrlMap> map = new List<MaterialTypeToUrlMap>
            {
                new MaterialTypeToUrlMap(DragonMaterialType.Body, dragon.data.bodyTexture),
                new MaterialTypeToUrlMap(DragonMaterialType.Wing, dragon.data.wingTexture),
                new MaterialTypeToUrlMap(DragonMaterialType.Horn, dragon.data.hornTexture),
                new MaterialTypeToUrlMap(DragonMaterialType.Back, dragon.data.backTexture)
            };
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
            currentNumberOfDragons = dragons.Count;
            if(accountName.Length > 0)
            {
                dragon1Id = request.dragon1Id;
                dragon2Id = request.dragon2Id;
                // Message.Send(new SignMessageRequest("breedDragons", "Hello World"));

                string url = envs.BreedDragonsTxHashApiUrl;
                string bodyJsonString = new BreedDragonsTxHashRequest(accountName, dragon1Id, dragon2Id, privateKey, secondaryPublicKey).ToJson();
                breedDragons = null;
                breedDragons = BreedDragonsTxHash(url, bodyJsonString);
                StartCoroutine(breedDragons);
                Message.Send(new BreedDragonsResponse(BreedingStatus.Breeding));
            }
        }

        IEnumerator BreedDragonsTxHash(string url, string bodyJsonString)
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
                BreedDragonsTxHashResponse response = BreedDragonsTxHashResponse.FromJson(res);
                string signedUrl = nearEnvs.SignTransactionUrl(response.hash);
                getDragonIds = null;
                getDragonIds = GetDragonIds(envs.DragonIdsApiUrl(accountName));
                StartCoroutine(getDragonIds);
                // Application.OpenURL(signedUrl);
                RyzmUtils.OpenUrl(signedUrl);
            }
            breedingDragons = false;
        }

        IEnumerator GetDragonIds(string url)
        {
            UnityWebRequest request = RyzmUtils.GetRequest(url);
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
                Debug.Log("GET SUCCESS " + res);
                OwnerNumberOfDragonsResponse response = OwnerNumberOfDragonsResponse.FromJson(res);
                bool foundNew = false;
                int newId = 0;
                foreach(int id in response.dragonIds)
                {
                    if(!dragons.ContainsKey(id))
                    {
                        foundNew = true;
                        newId = id;
                        Debug.Log("new id " + newId);
                        break;
                    }
                }
                if(!foundNew)
                {
                    float t = 0;
                    while(t < 0.25f)
                    {
                        t += Time.deltaTime;
                        yield return null;
                    }
                    getDragonIds = null;
                    getDragonIds = GetDragonIds(url);
                    StartCoroutine(getDragonIds);
                }
                else
                {
                    getDragonById = null;
                    getDragonById = GetDragonById(envs.DragonByIdApiUrl(newId));
                    StartCoroutine(getDragonById);
                }
            }
        }

        IEnumerator GetDragonById(string url)
        {
            UnityWebRequest request = RyzmUtils.GetRequest(url);
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
                Debug.Log("GET SUCCESS " + res);
                DragonByIdGetResponse response = DragonByIdGetResponse.FromJson(res);

                DragonResponse dragonRes = response.dragon;
                GameObject go = GameObject.Instantiate(prefabs.GetPrefabByHornType(dragonRes.hornType).dragon);
                EndlessDragon dragon = go.GetComponent<EndlessDragon>();
                dragon.data = dragonRes;
                dragons.Add(dragon.data.id, dragon);
                Message.Send(new DragonsResponse(dragons.Values.ToList(), "newDragon"));
                dragon.GetTextures();
                Message.Send(new BreedDragonsResponse(BreedingStatus.Success, dragonRes.id));
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
                // dragons.Add(dragon);
                dragons.Add(dragon.data.id, dragon);
                Message.Send(new DragonsResponse(dragons.Values.ToList(), "newDragon"));
                dragon.GetTextures();
                Message.Send(new BreedDragonsResponse(BreedingStatus.Success, dragonRes.id));

                // List<MaterialTypeToUrlMap> map = new List<MaterialTypeToUrlMap>
                // {
                //     new MaterialTypeToUrlMap(DragonMaterialType.Body, dragon.data.bodyTexture),
                //     new MaterialTypeToUrlMap(DragonMaterialType.Wing, dragon.data.wingTexture),
                //     new MaterialTypeToUrlMap(DragonMaterialType.Horn, dragon.data.hornTexture),
                //     new MaterialTypeToUrlMap(DragonMaterialType.Back, dragon.data.backTexture)
                // };
                // getDragonTexture = null;
                // getDragonTexture = GetDragonTexture(dragon);
                // StartCoroutine(getDragonTexture);
            }
            breedingDragons = false;
        }

        void OnDragonsRequest(DragonsRequest request)
        {
            Message.Send(new DragonsResponse(dragons.Values.ToList(), request.sender));
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
        public float price;
    }

    [System.Serializable]
    public class BreedDragonsTxHashRequest
    {
        public string accountId;
        public int dragon1Id;
        public int dragon2Id;
        public string privateKey;
        public string publicKey;

        public BreedDragonsTxHashRequest(string accountId, int dragon1Id, int dragon2Id, string privateKey, string publicKey)
        {
            this.accountId = accountId;
            this.dragon1Id = dragon1Id;
            this.dragon2Id = dragon2Id;
            this.privateKey = privateKey;
            this.publicKey = publicKey;
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        } 
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
        public string privateKey;

        public BreedDragonsPostRequest(string message, byte[] signedMessage, string publicKey, string accountId, int dragon1Id, int dragon2Id, string privateKey)
        {
            this.message = message;
            this.signedMessage = signedMessage;
            this.publicKey = publicKey;
            this.accountId = accountId;
            this.dragon1Id = dragon1Id;
            this.dragon2Id = dragon2Id;
            this.privateKey = privateKey;
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        } 
    }

    [System.Serializable]
    public class OwnerNumberOfDragonsResponse
    {
        public int[] dragonIds;

        public static OwnerNumberOfDragonsResponse FromJson(string jsonString)
        {
            return JsonUtility.FromJson<OwnerNumberOfDragonsResponse>(jsonString);
        }
    }

    [System.Serializable]
    public class DragonByIdGetResponse
    {
        public DragonResponse dragon;

        public static DragonByIdGetResponse FromJson(string jsonString)
        {
            return JsonUtility.FromJson<DragonByIdGetResponse>(jsonString);
        }
    }

    [System.Serializable]
    public class BreedDragonsTxHashResponse
    {
        public string hash;

        public static BreedDragonsTxHashResponse FromJson(string jsonString)
        {
            return JsonUtility.FromJson<BreedDragonsTxHashResponse>(jsonString);
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
