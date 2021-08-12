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
using System.Linq;
using Ryzm.Messages;

namespace Ryzm.Dragon
{
    public class DragonManager : MonoBehaviour
    {
        #region Public Variables
        public Envs envs;
        public DragonGenes genes;
        public DragonPrefabs prefabs;
        public Dictionary<int, BaseDragon> dragons = new Dictionary<int, BaseDragon>();

        [Header("Spawns")]
        public List<DragonSpawn> dragonSpawns = new List<DragonSpawn>();
        #endregion

        #region Private Variables
        IEnumerator getDragons;
        bool gettingDragons;
        string accountName;
        string privateKey;
        string secondaryPublicKey;
        bool breedingDragons;
        IEnumerator breedDragonsTxHash;
        int dragonId;
        int dragon2Id;
        bool initialized;
        int currentNumberOfDragons;
        IEnumerator getDragonIds;
        IEnumerator getDragonById;
        List<int> initializingDragonIds = new List<int>();
        List<int> initializedDragonIds = new List<int>();
        bool initializingDragons;
        IEnumerator buyDragonTxHash;
        int newDragonId;
        IEnumerator addDragonToMarket;
        IEnumerator removeDragonFromMarket;
        GameType gameType;
        #endregion

        #region Event Functions
        void Awake()
        {
            Message.AddListener<LoginResponse>(OnLoginResponse);
            Message.AddListener<SignMessageResponse>(OnSignMessageResponse);
            Message.AddListener<BreedDragonsRequest>(OnBreedDragonsRequest);
            Message.AddListener<BuyDragonRequest>(OnBuyDragonRequest);
            Message.AddListener<DragonInitialized>(OnDragonInitialized);
            Message.AddListener<DragonsRequest>(OnDragonsRequest);
            Message.AddListener<CancelTransaction>(OnCancelTransaction);
            Message.AddListener<DragonTransformUpdate>(OnDragonTransformUpdate);
            Message.AddListener<AddDragonToMarketRequest>(OnAddDragonToMarketRequest);
            Message.AddListener<RemoveDragonFromMarketRequest>(OnRemoveDragonFromMarketRequest);
            Message.AddListener<GameTypeResponse>(OnGameTypeResponse);
            Message.AddListener<DragonGenesRequest>(OnDragonGenesRequest);
            newDragonId = -1;
        }

        void Start()
        {
            Message.Send(new LoginRequest());
            Message.Send(new GameTypeRequest());
        }

        void OnDestroy()
        {
            Message.RemoveListener<LoginResponse>(OnLoginResponse);
            Message.RemoveListener<SignMessageResponse>(OnSignMessageResponse);
            Message.RemoveListener<BreedDragonsRequest>(OnBreedDragonsRequest);
            Message.RemoveListener<BuyDragonRequest>(OnBuyDragonRequest);
            Message.RemoveListener<DragonInitialized>(OnDragonInitialized);
            Message.RemoveListener<DragonsRequest>(OnDragonsRequest);
            Message.RemoveListener<CancelTransaction>(OnCancelTransaction);
            Message.RemoveListener<DragonTransformUpdate>(OnDragonTransformUpdate);
            Message.RemoveListener<AddDragonToMarketRequest>(OnAddDragonToMarketRequest);
            Message.RemoveListener<RemoveDragonFromMarketRequest>(OnRemoveDragonFromMarketRequest);
            Message.RemoveListener<GameTypeResponse>(OnGameTypeResponse);
            Message.RemoveListener<DragonGenesRequest>(OnDragonGenesRequest);
        }
        #endregion
        
        #region Listener Functions
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
                foreach(BaseDragon dragon in dragons.Values)
                {
                    Destroy(dragon.gameObject);
                }
                dragons.Clear();
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
        }

        void OnBreedDragonsRequest(BreedDragonsRequest request)
        {
            currentNumberOfDragons = dragons.Count;
            if(accountName.Length > 0)
            {
                dragonId = request.dragon1Id;
                dragon2Id = request.dragon2Id;
                // Message.Send(new SignMessageRequest("breedDragons", "Hello World"));

                string url = envs.BreedDragonsTxHashApiUrl;
                string bodyJsonString = new BreedDragonsTxHashRequest(accountName, dragonId, dragon2Id, privateKey, secondaryPublicKey).ToJson();
                breedDragonsTxHash = null;
                breedDragonsTxHash = BreedDragonsTxHash(url, bodyJsonString);
                StartCoroutine(breedDragonsTxHash);
                Message.Send(new BreedDragonsResponse(TransactionStatus.Processing));
            }
            else
            {
                Message.Send(new BreedDragonsResponse(TransactionStatus.Failed));
            }
        }

        void OnBuyDragonRequest(BuyDragonRequest request)
        {
            currentNumberOfDragons = dragons.Count;
            if(accountName.Length > 0)
            {
                string url = envs.BuyDragonTxHashApiUrl;
                string bodyJsonString = new BuyDragonTxHashRequest(accountName, request.dragonId, privateKey, secondaryPublicKey, request.price).ToJson();
                buyDragonTxHash = null;
                buyDragonTxHash = BuyDragonTxHash(url, bodyJsonString);
                StartCoroutine(buyDragonTxHash);
                Message.Send(new BuyDragonResponse(TransactionStatus.Processing));
            }
        }

        void OnDragonInitialized(DragonInitialized initialized)
        {
            if(initializingDragons)
            {
                if(initializingDragonIds.Contains(initialized.id))
                {
                    initializingDragonIds.Remove(initialized.id);
                    initializedDragonIds.Add(initialized.id);
                }
                if(initializingDragonIds.Count == 0)
                {
                    initializingDragons = false;
                    foreach(int dragonId in initializedDragonIds)
                    {
                        Debug.Log("initializing " + dragonId.ToString());
                        // dragons[dragonId].EnableMaterials();
                        if(gameType == GameType.Breeding)
                        {
                            dragons[dragonId].EnableMaterials();
                        }
                    }
                    initializingDragonIds.Clear();
                }
            }
            else if(initialized.id == newDragonId)
            {
                BaseDragon newDragon = dragons[newDragonId];
                if(newDragon != null)
                {
                    foreach(DragonSpawn spawn in dragonSpawns)
                    {
                        if(spawn.type == DragonSpawnType.SingleDragon)
                        {
                            newDragon.transform.position = spawn.spawn.position;
                            newDragon.transform.rotation = spawn.spawn.rotation;
                            break;
                        }
                    }
                    foreach(BaseDragon dragon in dragons.Values)
                    {
                        dragon.gameObject.SetActive(dragon.data.id == newDragonId);
                    }
                    Debug.Log("doing it ya know");
                    Message.Send(new SingleDragonUpdate(newDragon));
                }
                newDragonId = -1;
            }
        }

        void OnDragonsRequest(DragonsRequest request)
        {
            Message.Send(new DragonsResponse(dragons.Values.ToList(), request.sender));
        }

        void OnCancelTransaction(CancelTransaction cancel)
        {
            StopAllCoroutines();
        }

        void OnDragonTransformUpdate(DragonTransformUpdate update)
        {
            if(dragons.ContainsKey(update.dragonId))
            {
                BaseDragon newDragon = dragons[update.dragonId];
                foreach(DragonSpawn spawn in dragonSpawns)
                {
                    if(spawn.type == update.spawnType)
                    {
                        newDragon.transform.position = spawn.spawn.position;
                        newDragon.transform.rotation = spawn.spawn.rotation;
                        break;
                    }
                }
            }
        }

        void OnAddDragonToMarketRequest(AddDragonToMarketRequest request)
        {
            if(dragons.ContainsKey(request.dragonId))
            {
                string bodyJsonString = new AddDragonToMarketPostRequest(accountName, request.dragonId, privateKey, request.price).ToJson();
                string url = envs.AddDragonToMarketApiUrl;
                addDragonToMarket = null;
                Debug.Log(url + " " + bodyJsonString);
                addDragonToMarket = AddDragonToMarket(url, bodyJsonString, request.dragonId, request.price);
                StartCoroutine(addDragonToMarket);
                Message.Send(new AddDragonToMarketResponse(TransactionStatus.Processing));
            }
            else
            {
                Message.Send(new AddDragonToMarketResponse(TransactionStatus.Failed));
            }
        }

        void OnRemoveDragonFromMarketRequest(RemoveDragonFromMarketRequest request)
        {
            if(dragons.ContainsKey(request.dragonId))
            {
                string bodyJsonString = new RemoveDragonFromMarketPostRequest(accountName, request.dragonId, privateKey).ToJson();
                string url = envs.RemoveDragonFromMarketApiUrl;
                removeDragonFromMarket = null;
                removeDragonFromMarket = RemoveDragonFromMarket(url, bodyJsonString, request.dragonId);
                StartCoroutine(removeDragonFromMarket);
                Message.Send(new RemoveDragonFromMarketResponse(TransactionStatus.Processing));
            }
            else
            {
                Message.Send(new RemoveDragonFromMarketResponse(TransactionStatus.Failed));
            }
        }

        void OnGameTypeResponse(GameTypeResponse response)
        {
            gameType = response.type;
        }

        void OnDragonGenesRequest(DragonGenesRequest request)
        {
            Message.Send(new DragonGenesResponse(request.sender, genes));
        }
        #endregion

        #region Private Functions
        void SignUrlAndOpen(string res, bool isBreeding)
        {
            TxHashResponse response = TxHashResponse.FromJson(res);
            string signedUrl = envs.SignTransactionUrl(response.hash);
            getDragonIds = null;
            getDragonIds = GetDragonIds(envs.DragonIdsApiUrl(accountName), isBreeding);
            StartCoroutine(getDragonIds);
            // Application.OpenURL(signedUrl);
            RyzmUtils.OpenUrl(signedUrl);
        }
        #endregion

        #region Coroutines
        IEnumerator GetDragons(string url, string bodyJsonString)
        {
            gettingDragons = true;
            UnityWebRequest request = RyzmUtils.PostRequest(url, bodyJsonString);
            Debug.Log(url);
            int numFails = 0;
            bool failed = true;
            while(numFails < 3)
            {
                yield return request.SendWebRequest();
                if(request.isNetworkError || request.isHttpError)
                {
                    request = RyzmUtils.PostRequest(url, bodyJsonString);
                    numFails++;
                    Debug.LogError("Failed getting dragons " + numFails + " times");
                    Debug.LogError(request.error);
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
                // todo: handle this case
            }
            else
            {
                string res = request.downloadHandler.text;
                Debug.Log("POST SUCCESS " + res);
                GetDragonsPostResponse response = GetDragonsPostResponse.FromJson(res);
                initializingDragons = true;
                foreach(DragonResponse dragonRes in response.dragons)
                {
                    GameObject go = GameObject.Instantiate(prefabs.GetPrefabByHornType(dragonRes.hornType).dragon);
                    BaseDragon dragon = go.GetComponent<BaseDragon>();
                    dragon.DisableMaterials();
                    dragon.data = dragonRes;
                    dragons.Add(dragon.data.id, dragon);
                    initializingDragonIds.Add(dragon.data.id);
                }
                foreach(BaseDragon dragon in dragons.Values)
                {
                    dragon.GetTextures();
                }
                Message.Send(new DragonsResponse(dragons.Values.ToList(), "all"));
            }
            gettingDragons = false;
        }

        IEnumerator BreedDragonsTxHash(string url, string bodyJsonString)
        {
            breedingDragons = true;
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
                    Debug.LogError("Failed getting breed dragons tx hash " + numFails + " times");
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
                Message.Send(new BreedDragonsResponse(TransactionStatus.Failed));
                // todo: handle this case
            }
            else
            {
                string res = request.downloadHandler.text;
                Debug.Log("BreedDragonsTxHash POST SUCCESS " + res);
                SignUrlAndOpen(res, true);
            }
        }

        IEnumerator BuyDragonTxHash(string url, string bodyJsonString)
        {
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
                    Debug.LogError("Failed getting buy dragon tx hash " + numFails + " times");
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
                Message.Send(new BuyDragonResponse(TransactionStatus.Failed));
                // todo: handle this case
            }
            else
            {
                string res = request.downloadHandler.text;
                Debug.Log("POST SUCCESS " + res);
                SignUrlAndOpen(request.downloadHandler.text, false);
            }
        }

        IEnumerator GetDragonIds(string url, bool isBreeding)
        {
            UnityWebRequest request = RyzmUtils.GetRequest(url);
            yield return request.SendWebRequest();
            int numFails = 0;
            bool failed = true;
            while(numFails < 3)
            {
                yield return request.SendWebRequest();
                if(request.isNetworkError || request.isHttpError)
                {
                    request = RyzmUtils.GetRequest(url);
                    numFails++;
                    Debug.LogError("Failed getting dragon ids " + numFails + " times");
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
                if(isBreeding)
                {
                    Message.Send(new BreedDragonsResponse(TransactionStatus.Failed));
                }
                else
                {
                    Message.Send(new BuyDragonResponse(TransactionStatus.Failed));
                }
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
                    getDragonIds = GetDragonIds(url, isBreeding);
                    StartCoroutine(getDragonIds);
                }
                else
                {
                    getDragonById = null;
                    getDragonById = GetDragonById(envs.DragonByIdApiUrl(newId), isBreeding);
                    StartCoroutine(getDragonById);
                }
            }
        }

        IEnumerator GetDragonById(string url, bool isBreeding)
        {
            UnityWebRequest request = RyzmUtils.GetRequest(url);
            yield return request.SendWebRequest();
            int numFails = 0;
            bool failed = true;
            while(numFails < 3)
            {
                yield return request.SendWebRequest();
                if(request.isNetworkError || request.isHttpError)
                {
                    request = RyzmUtils.GetRequest(url);
                    numFails++;
                    Debug.LogError("Failed getting dragon ids " + numFails + " times");
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
                if(isBreeding)
                {
                    Message.Send(new BreedDragonsResponse(TransactionStatus.Failed));
                }
                else
                {
                    Message.Send(new BuyDragonResponse(TransactionStatus.Failed));
                }
            }
            else
            {
                string res = request.downloadHandler.text;
                Debug.Log("GET SUCCESS " + res);
                DragonByIdGetResponse response = DragonByIdGetResponse.FromJson(res);

                DragonResponse dragonRes = response.dragon;
                GameObject go = GameObject.Instantiate(prefabs.GetPrefabByHornType(dragonRes.hornType).dragon);
                BaseDragon dragon = go.GetComponent<BaseDragon>();
                dragon.data = dragonRes;
                dragons.Add(dragon.data.id, dragon);
                Message.Send(new DragonsResponse(dragons.Values.ToList(), "newDragon"));
                dragon.GetTextures();
                if(isBreeding)
                {
                    Message.Send(new BreedDragonsResponse(TransactionStatus.Success, dragonRes.id));
                }
                else
                {
                    Debug.Log("got new dragon lol");
                    newDragonId = dragonRes.id;
                    Message.Send(new BuyDragonResponse(TransactionStatus.Success, dragonRes.id));
                }
            }
            breedingDragons = false;
        }

        IEnumerator AddDragonToMarket(string url, string bodyJsonString, int dragonId, float price)
        {
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
                    Debug.LogError("Failed getting buy dragon tx hash " + numFails + " times");
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
                Message.Send(new AddDragonToMarketResponse(TransactionStatus.Failed));
            }
            else
            {
                string res = request.downloadHandler.text;
                Debug.Log("POST SUCCESS " + res);
                AddDragonToMarketPostResponse response = AddDragonToMarketPostResponse.FromJson(res);
                if(response.isSuccess)
                {
                    dragons[dragonId].data.price = price;
                    Message.Send(new DragonsResponse(dragons.Values.ToList(), "marketUpdate"));
                    Message.Send(new AddDragonToMarketResponse(TransactionStatus.Success, dragonId, price));
                }
                else
                {
                    Message.Send(new AddDragonToMarketResponse(TransactionStatus.Failed));
                }
            }
        }

        IEnumerator RemoveDragonFromMarket(string url, string bodyJsonString, int dragonId)
        {
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
                    Debug.LogError("Failed getting buy dragon tx hash " + numFails + " times");
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
                Message.Send(new RemoveDragonFromMarketResponse(TransactionStatus.Failed));
            }
            else
            {
                string res = request.downloadHandler.text;
                Debug.Log("POST SUCCESS " + res);
                RemoveDragonFromMarketPostResponse response = RemoveDragonFromMarketPostResponse.FromJson(res);
                if(response.isSuccess)
                {
                    dragons[dragonId].data.price = 0;
                    Message.Send(new DragonsResponse(dragons.Values.ToList(), "marketUpdate"));
                    Message.Send(new RemoveDragonFromMarketResponse(TransactionStatus.Success, dragonId));
                }
                else
                {
                    Message.Send(new RemoveDragonFromMarketResponse(TransactionStatus.Failed));
                }
            }
        }
        #endregion
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
        public int parent1;
        public int parent2;
        public int baseSpeed;
        public int baseAttack;
        public int baseDefense;
        public int baseHealth;
        public int primaryColor;
        public int secondaryColor;
        public List<int> bodyGenes;
        public List<int> wingGenes;
        public List<int> hornGenes;
        public List<int> hornTypeGenes;
        public List<int> moveGenes;
        public string bodyTexture;
        public string wingTexture;
        public string backTexture;
        public string hornTexture;
        public string bodyGenesSequence;
        public string wingGenesSequence;
        public string hornGenesSequence;
        public string moveGenesSequence;
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
    public class BuyDragonTxHashRequest
    {
        public string accountId;
        public int id;
        public string privateKey;
        public string publicKey;
        public float price;

        public BuyDragonTxHashRequest(string accountId, int id, string privateKey, string publicKey, float price)
        {
            this.accountId = accountId;
            this.id = id;
            this.privateKey = privateKey;
            this.publicKey = publicKey;
            this.price = price;
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
    public class TxHashResponse
    {
        public string hash;

        public static TxHashResponse FromJson(string jsonString)
        {
            return JsonUtility.FromJson<TxHashResponse>(jsonString);
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

    [System.Serializable]
    public class AddDragonToMarketPostRequest
    {
        public string accountId;
        public int id;
        public string privateKey;
        public float price;

        public AddDragonToMarketPostRequest(string accountId, int id, string privateKey, float price)
        {
            this.accountId = accountId;
            this.id = id;
            this.privateKey = privateKey;
            this.price = price;
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        } 
    }

    [System.Serializable]
    public class RemoveDragonFromMarketPostRequest
    {
        public string accountId;
        public int id;
        public string privateKey;

        public RemoveDragonFromMarketPostRequest(string accountId, int id, string privateKey)
        {
            this.accountId = accountId;
            this.id = id;
            this.privateKey = privateKey;
        }

        public string ToJson()
        {
            return JsonUtility.ToJson(this);
        } 
    }

    [System.Serializable]
    public class AddDragonToMarketPostResponse
    {
        public bool isSuccess;

        public static AddDragonToMarketPostResponse FromJson(string jsonString)
        {
            return JsonUtility.FromJson<AddDragonToMarketPostResponse>(jsonString);
        }
    } 

    [System.Serializable]
    public class RemoveDragonFromMarketPostResponse
    {
        public bool isSuccess;

        public static RemoveDragonFromMarketPostResponse FromJson(string jsonString)
        {
            return JsonUtility.FromJson<RemoveDragonFromMarketPostResponse>(jsonString);
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

    [System.Serializable]
    public struct DragonSpawn
    {
        public DragonSpawnType type;
        public Transform spawn;
    }

    public enum DragonSpawnType
    {
        SingleDragon
    }

    public enum TransactionStatus 
    {
        Processing,
        Failed,
        Success
    }
}
