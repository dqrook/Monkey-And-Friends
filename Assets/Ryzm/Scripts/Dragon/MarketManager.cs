using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.Dragon.Messages;
using CodeControl;
using Ryzm.EndlessRunner;
using Ryzm.Utils;
using UnityEngine.Networking;

namespace Ryzm.Dragon
{
    public class MarketManager : MonoBehaviour
    {
        public List<MarketDragon> marketDragons = new List<MarketDragon>();
        public Envs envs;
        public Dictionary<int, MarketDragonData> allDragonsForSale = new Dictionary<int, MarketDragonData>();
        public Dictionary<int, EndlessDragon> userDragonsForSale = new Dictionary<int, EndlessDragon>();
        
        [Header("Camera")]
        public Transform mainMenuSpawn;
        public Transform marketSpawn;

        // current queried page
        int currentPage;
        EndlessDragon[] dragons;
        IEnumerator getMarketDragons;
        int numberOfDragonsOnMarket;
        bool gotAllDragons;
        bool continueQuery;
        bool gettingDragons;
        bool initialized;
        IEnumerator getTextures;
        bool isLoading;
        int loadingStartDex;
        int loadingFinDex;
        int loadingNumberOfDragons;

        void Awake()
        {
            Message.AddListener<DragonsResponse>(OnDragonsResponse);
            Message.AddListener<DragonMarketRequest>(OnDragonMarketRequest);
        }

        void Start()
        {
            getMarketDragons = null;
            getMarketDragons = GetMarketDragons(true);
            StartCoroutine(getMarketDragons);
        }

        void OnDestroy()
        {
            Message.RemoveListener<DragonsResponse>(OnDragonsResponse);
            Message.RemoveListener<DragonMarketRequest>(OnDragonMarketRequest);
        }

        void OnDragonsResponse(DragonsResponse response)
        {
            dragons = response.dragons;
            userDragonsForSale.Clear();
            foreach(EndlessDragon dragon in dragons)
            {
                if(dragon.data.price > 0) 
                {
                    userDragonsForSale.Add(dragon.data.id, dragon);
                }
            }
        }

        void OnDragonMarketRequest(DragonMarketRequest request)
        {
            if(request.status == MarketStatus.Start)
            {
                if(!gotAllDragons)
                {
                    continueQuery = true;
                    if(!gettingDragons)
                    {
                        getMarketDragons = null;
                        getMarketDragons = GetMarketDragons(!initialized);
                        StartCoroutine(getMarketDragons);
                    }
                }
            }
            else if(request.status == MarketStatus.Update)
            {
                int startDex = request.startIndex;
                int finDex = startDex + request.numberOfDragons - 1;
                int numberOfDragons = request.numberOfDragons;
                finDex = finDex < numberOfDragonsOnMarket ? finDex : numberOfDragonsOnMarket - 1;
                if(!gotAllDragons)
                {
                    if(startDex < allDragonsForSale.Count)
                    {
                        if(finDex < allDragonsForSale.Count)
                        {
                            UpdateMarketDragonData(numberOfDragons, startDex, finDex);
                        }
                        else
                        {
                            Message.Send(new DragonMarketResponse(MarketStatus.Loading));
                            isLoading = true;
                            loadingStartDex = startDex;
                            loadingFinDex = finDex;
                            loadingNumberOfDragons = numberOfDragons;
                        }
                    }
                    else
                    {
                        Message.Send(new DragonMarketResponse(MarketStatus.Loading));
                        isLoading = true;
                        loadingStartDex = startDex;
                        loadingFinDex = finDex;
                        loadingNumberOfDragons = numberOfDragons;
                    }
                }
                else
                {
                    UpdateMarketDragonData(numberOfDragons, startDex, finDex);
                }
            }
            else if(request.status == MarketStatus.Exit)
            {
                foreach(MarketDragon dragon in marketDragons)
                {
                    dragon.gameObject.SetActive(false);
                }
                continueQuery = false;
                isLoading = false;
                loadingStartDex = 0;
                loadingFinDex = 0;
                loadingNumberOfDragons = 0;
            }
        }

        void UpdateMarketDragonData(int numberOfDragons, int startDex, int finDex)
        {
            MarketDragonData[] allData = GetMarketDragonData();
            MarketDragonData[] data = new MarketDragonData[numberOfDragons];
            for(int i = startDex; i < finDex; i++)
            {
                data[i - startDex] = allData[i];
            }
            Message.Send(new DragonMarketResponse(MarketStatus.Update, data));
            isLoading = false;
            int max = numberOfDragons < marketDragons.Count ? numberOfDragons : marketDragons.Count;
            for(int i = 0; i < max; i++)
            {
                marketDragons[i].gameObject.SetActive(true);
            }
        }

        MarketDragonData[] GetMarketDragonData()
        {
            MarketDragonData[] data = new MarketDragonData[0];
            allDragonsForSale.Values.CopyTo(data, 0);
            return data;
        }

        void UpdateMarketDragonTextures()
        {
            
        }

        IEnumerator GetMarketDragons(bool getNumberOfDragons)
        {
            gettingDragons = true;
            string url = getNumberOfDragons ? envs.GetMarketDragonsApiUrl : envs.GetMarketDragonsApiUrl + "?page=" + currentPage.ToString();
            UnityWebRequest request = RyzmUtils.GetRequest(url);
            yield return request.SendWebRequest();
            if(request.isNetworkError || request.isHttpError)
            {
                Debug.LogError("ERROR");
                // todo: handle this case
            }
            else
            {
                string res = request.downloadHandler.text;
                Debug.Log("GET SUCCESS " + res);
                if(getNumberOfDragons)
                {
                    NumberOfDragonsGetResponse response = NumberOfDragonsGetResponse.FromJson(res);
                    numberOfDragonsOnMarket = response.numberOfDragons;
                    initialized = true;
                }
                else
                {
                    DragonsOnMarketGetResponse response = DragonsOnMarketGetResponse.FromJson(res);
                    if(response.dragons.Count > 0)
                    {
                        foreach(DragonResponse dragon in response.dragons)
                        {
                            if(!allDragonsForSale.ContainsKey(dragon.id))
                            {
                                MarketDragonData marketDragon = new MarketDragonData(dragon, userDragonsForSale.ContainsKey(dragon.id));
                                getTextures = GetTextures(marketDragon);
                                StartCoroutine(getTextures);
                                allDragonsForSale.Add(dragon.id, marketDragon);
                            }
                        }
                        currentPage++;
                    }
                    else
                    {
                        gotAllDragons = true;
                        continueQuery = false;
                    }
                    if(isLoading)
                    {
                        if(loadingStartDex < allDragonsForSale.Count && loadingFinDex < allDragonsForSale.Count)
                        {
                            UpdateMarketDragonData(loadingNumberOfDragons, loadingStartDex, loadingFinDex);
                        }
                    }
                }
            }
            gettingDragons = false;
            if(continueQuery)
            {
                getMarketDragons = null;
                getMarketDragons = GetMarketDragons(false);
                StartCoroutine(getMarketDragons);
            }
        }

        IEnumerator GetTextures(MarketDragonData dragon)
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
                    Texture _texture = DownloadHandlerTexture.GetContent(request);
                    dragon.SetTexture(type, _texture);

                }
                index++;
                yield return null;
            }
            allDragonsForSale.Add(dragon.data.id, dragon);
        }
    }

    [System.Serializable]
    public class NumberOfDragonsGetResponse
    {
        public int numberOfDragons;

        public static NumberOfDragonsGetResponse FromJson(string jsonString)
        {
            return JsonUtility.FromJson<NumberOfDragonsGetResponse>(jsonString);
        }
    }

    [System.Serializable]
    public class DragonsOnMarketGetResponse
    {
        public List<DragonResponse> dragons;
        
        public static DragonsOnMarketGetResponse FromJson(string jsonString)
        {
            return JsonUtility.FromJson<DragonsOnMarketGetResponse>(jsonString);
        }
    }

    [System.Serializable]
    public class MarketDragonData
    {
        public DragonResponse data;
        public Texture bodyTexture;
        public Texture wingTexture;
        public Texture backTexture;
        public Texture hornTexture;
        public bool isUser;

        public bool Initialized
        {
            get
            {
                if(bodyTexture == null)
                {
                    return false;
                }
                if(wingTexture == null)
                {
                    return false;
                }
                if(backTexture == null)
                {
                    return false;
                }
                if(hornTexture == null)
                {
                    return false;
                }
                return true;
            }
        }

        public MarketDragonData(DragonResponse data, bool isUser)
        {
            this.data = data;
            this.isUser = isUser;
        }

        public void SetTexture(DragonMaterialType type, Texture texture)
        {
            switch(type)
            {
                case DragonMaterialType.Body:
                    bodyTexture = texture;
                    break;
                case DragonMaterialType.Wing:
                    wingTexture = texture;
                    break;
                case DragonMaterialType.Back:
                    backTexture = texture;
                    break;
                case DragonMaterialType.Horn:
                    hornTexture = texture;
                    break;
            }
        }
    }

    public enum MarketStatus
    {
        Start,
        Loading,
        Update,
        Exit
    }
}
