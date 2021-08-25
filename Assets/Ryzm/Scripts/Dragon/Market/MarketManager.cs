using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.Dragon.Messages;
using CodeControl;
using Ryzm.Utils;
using UnityEngine.Networking;

namespace Ryzm.Dragon
{
    public class MarketManager : MonoBehaviour
    {
        #region Public Variables
        public List<CustomDragon> marketDragons = new List<CustomDragon>();
        public Envs envs;
        public Dictionary<int, MarketDragonData> allDragonsForSale = new Dictionary<int, MarketDragonData>();
        public Dictionary<int, BaseDragon> userDragonsForSale = new Dictionary<int, BaseDragon>();
        #endregion

        #region Private Variables
        // current queried page
        int currentPage;
        BaseDragon[] dragons;
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
        #endregion

        #region Event Functions
        void Awake()
        {
            Message.AddListener<DragonsResponse>(OnDragonsResponse);
            Message.AddListener<DragonMarketRequest>(OnDragonMarketRequest);
            Message.AddListener<NumberOfMarketDragonsRequest>(OnNumberOfMarketDragonsRequest);
            Message.AddListener<UpdateVisibleMarketDragons>(OnUpdateVisibleMarketDragons);
        }

        void Start()
        {
            getMarketDragons = null;
            getMarketDragons = GetMarketDragons(true);
            StartCoroutine(getMarketDragons);
            foreach(CustomDragon dragon in marketDragons)
            {
                if(dragon.gameObject.activeSelf)
                {
                    dragon.gameObject.SetActive(false);
                }
            }
        }

        void OnDestroy()
        {
            Message.RemoveListener<DragonsResponse>(OnDragonsResponse);
            Message.RemoveListener<DragonMarketRequest>(OnDragonMarketRequest);
            Message.RemoveListener<NumberOfMarketDragonsRequest>(OnNumberOfMarketDragonsRequest);
            Message.RemoveListener<UpdateVisibleMarketDragons>(OnUpdateVisibleMarketDragons);
        }
        #endregion

        #region Listener Functions
        void OnDragonsResponse(DragonsResponse response)
        {
            dragons = response.dragons;
            userDragonsForSale.Clear();
            foreach(BaseDragon dragon in dragons)
            {
                if(dragon.data.price > 0) 
                {
                    userDragonsForSale.Add(dragon.data.id, dragon);
                }
                if(allDragonsForSale.ContainsKey(dragon.data.id))
                {
                    if(dragon.data.price > 0)
                    {
                        allDragonsForSale[dragon.data.id].isUser = true;
                    }
                    else
                    {
                        allDragonsForSale.Remove(dragon.data.id);
                    }
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
                    int _numberOfDragons = finDex - startDex + 1 < numberOfDragons ? finDex - startDex + 1 : numberOfDragons;
                    if(startDex < allDragonsForSale.Count)
                    {
                        if(finDex < allDragonsForSale.Count)
                        {
                            UpdateMarketDragonData(_numberOfDragons, startDex, finDex);
                        }
                        else
                        {
                            LoadMoreDragons(startDex, finDex, _numberOfDragons);
                        }
                    }
                    else
                    {
                        LoadMoreDragons(startDex, finDex, _numberOfDragons);
                    }
                }
                else
                {
                    int _numberOfDragons = finDex - startDex + 1 < numberOfDragons ? finDex - startDex + 1 : numberOfDragons;
                    UpdateMarketDragonData(_numberOfDragons, startDex, finDex);
                }
            }
            else if(request.status == MarketStatus.CancelLoading)
            {
                isLoading = false;
            }
            else if(request.status == MarketStatus.Exit)
            {
                foreach(CustomDragon dragon in marketDragons)
                {
                    dragon.EnableMaterials();
                    dragon.gameObject.SetActive(false);
                }
                continueQuery = false;
                isLoading = false;
                loadingStartDex = 0;
                loadingFinDex = 0;
                loadingNumberOfDragons = 0;
            }
        }

        void OnNumberOfMarketDragonsRequest(NumberOfMarketDragonsRequest request)
        {
            Message.Send(new NumberOfMarketDragonsResponse(numberOfDragonsOnMarket));
        }

        void OnUpdateVisibleMarketDragons(UpdateVisibleMarketDragons update)
        {
            int numDragons = marketDragons.Count;
            for(int i = 0; i < numDragons; i++)
            {
                if(update.allVisible || update.visibleIndex == i)
                {
                    marketDragons[i].EnableMaterials();
                }
                else
                {
                    marketDragons[i].DisableMaterials();
                }
            }
        }
        #endregion

        #region Private Functions
        void LoadMoreDragons(int startDex, int finDex, int numberOfDragons)
        {
            Message.Send(new DragonMarketResponse(MarketStatus.Loading));
            isLoading = true;
            loadingStartDex = startDex;
            loadingFinDex = finDex;
            loadingNumberOfDragons = numberOfDragons;

            getMarketDragons = null;
            getMarketDragons = GetMarketDragons(!initialized);
            StartCoroutine(getMarketDragons);
        }

        void UpdateMarketDragonData(int numberOfDragons, int startDex, int finDex)
        {
            MarketDragonData[] allData = GetMarketDragonData();
            MarketDragonData[] data = new MarketDragonData[numberOfDragons];
            Debug.Log(numberOfDragons + " " + startDex + " " + finDex);
            for(int i = startDex; i < finDex + 1; i++)
            {
                data[i - startDex] = allData[i];
            }
            Message.Send(new DragonMarketResponse(MarketStatus.Update, data));
            isLoading = false;
            int max = numberOfDragons < marketDragons.Count ? numberOfDragons : marketDragons.Count;
            
            for(int i = 0; i < max; i++)
            {
                marketDragons[i].gameObject.SetActive(true);
                marketDragons[i].UpdateData(data[i]);
                marketDragons[i].EnableMaterials();
            }
        }

        MarketDragonData[] GetMarketDragonData()
        {
            MarketDragonData[] data = new MarketDragonData[allDragonsForSale.Values.Count];
            allDragonsForSale.Values.CopyTo(data, 0);
            return data;
        }
        #endregion

        #region Coroutines
        IEnumerator GetMarketDragons(bool getNumberOfDragons)
        {
            gettingDragons = true;
            string url = getNumberOfDragons ? envs.GetMarketDragonsApiUrl : envs.GetMarketDragonsApiUrl + "?page=" + currentPage.ToString();
            UnityWebRequest request = RyzmUtils.GetRequest(url);
            Debug.Log("get market dragons url: " + url);
            int numFails = 0;
            bool failed = true;
            while(numFails < 3)
            {
                yield return request.SendWebRequest();
                if(request.isNetworkError || request.isHttpError)
                {
                    request = RyzmUtils.GetRequest(url);
                    numFails++;
                    Debug.LogError("Failed getting market dragons " + numFails + " times");
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
                Debug.Log("GET SUCCESS " + res);
                if(getNumberOfDragons)
                {
                    NumberOfDragonsGetResponse response = NumberOfDragonsGetResponse.FromJson(res);
                    numberOfDragonsOnMarket = response.numberOfDragons;
                    Message.Send(new NumberOfMarketDragonsResponse(numberOfDragonsOnMarket));
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
                                MarketDragonData marketDragonData = new MarketDragonData(dragon, userDragonsForSale.ContainsKey(dragon.id));
                                getTextures = GetTextures(marketDragonData);
                                StartCoroutine(getTextures);
                                allDragonsForSale.Add(dragon.id, marketDragonData);
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
            // if(continueQuery)
            // {
            //     getMarketDragons = null;
            //     getMarketDragons = GetMarketDragons(false);
            //     StartCoroutine(getMarketDragons);
            // }
        }

        IEnumerator GetTextures(MarketDragonData dragon)
        {
            List<MaterialTypeToUrlMap> maps = new List<MaterialTypeToUrlMap>
            {
                new MaterialTypeToUrlMap(DragonMaterialType.Body, dragon.data.bodyTexture),
                new MaterialTypeToUrlMap(DragonMaterialType.Wing, dragon.data.wingTexture),
                new MaterialTypeToUrlMap(DragonMaterialType.Horn, dragon.data.hornTexture),
                new MaterialTypeToUrlMap(DragonMaterialType.Back, dragon.data.backTexture)
            };
            
            int numMaterials = maps.Count;
            int index = 0;
            foreach(MaterialTypeToUrlMap map in maps)
            {
                string u = maps[index].url;
                DragonMaterialType type = maps[index].type;
                Texture _texture = Resources.Load<Texture>("Dragon/" + u);
                dragon.SetTexture(type, _texture);
                index++;
            }
            string url = dragon.data.media;
            UnityWebRequest request = RyzmUtils.TextureRequest(url);
            int numFails = 0;
            bool failed = true;
            if(dragon.Initialized)
            {
                yield break;
            }
            while(numFails < 3)
            {
                yield return request.SendWebRequest();
                if(request.isNetworkError || request.isHttpError)
                {
                    request = RyzmUtils.TextureRequest(url);
                    numFails++;
                    Debug.LogError("Failed getting market dragons " + numFails + " times");
                }
                else
                {
                    failed = false;
                    break;
                }
            }
            if(failed)
            {

            }
            yield break;
        }
        #endregion
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
        public Texture dragonImage;

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
                if(dragonImage == null)
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
        Exit,
        CancelLoading
    }
}
