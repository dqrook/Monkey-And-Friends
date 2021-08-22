using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.Dragon.Messages;
using CodeControl;
using UnityEngine.Networking;
using Ryzm.Utils;
using Ryzm.UI.Messages;

namespace Ryzm.Dragon
{
    public class MarketController : MonoBehaviour
    {
        #region Public Variables
        public int dragonsPerPage = 10;
        public Envs envs;
        public MarketFilterMaps marketFilterMaps;
        public DragonGenes genes;
        public List<DisplayDragon> displayDragons = new List<DisplayDragon>();
        public List<CameraTransform> marketCameraTransforms = new List<CameraTransform>();
        #endregion

        #region Private Variables
        int[] currentDragonIds;
        BaseDragon[] userDragons;
        IEnumerator queryMarket;
        DragonCardMetadata[] dragonCards;
        IEnumerator getMediaImage;
        IEnumerator waitForInitialization;
        WaitForEndOfFrame waitForEndOfFrame;
        IEnumerator getDragonById;
        #endregion

        #region Event Functions
        void Awake()
        {
            waitForEndOfFrame = new WaitForEndOfFrame();
            Message.AddListener<DragonsResponse>(OnDragonsResponse);
            Message.AddListener<QueryMarketRequest>(OnQueryMarketRequest);
            Message.AddListener<DisplayDragonZoomRequest>(OnDisplayDragonZoomRequest);
            Message.AddListener<ReturnToMarket>(OnReturnToMarket);
            Message.AddListener<MarketCameraTransformsRequest>(OnMarketCameraTransformsRequest);
            Message.AddListener<FilterDragonZoomRequest>(OnFilterDragonZoomRequest);
            Message.AddListener<MarketFilterMapsRequest>(OnMarketFilterMapsRequest);
            Message.AddListener<DragonGenesRequest>(OnDragonGenesRequest);
            foreach(DisplayDragon dragon in displayDragons)
            {
                dragon.Disable();
            }
        }

        void OnDestroy()
        {
            Message.RemoveListener<DragonsResponse>(OnDragonsResponse);
            Message.RemoveListener<QueryMarketRequest>(OnQueryMarketRequest);
            Message.RemoveListener<DisplayDragonZoomRequest>(OnDisplayDragonZoomRequest);
            Message.RemoveListener<ReturnToMarket>(OnReturnToMarket);
            Message.RemoveListener<MarketCameraTransformsRequest>(OnMarketCameraTransformsRequest);
            Message.RemoveListener<FilterDragonZoomRequest>(OnFilterDragonZoomRequest);
            Message.RemoveListener<MarketFilterMapsRequest>(OnMarketFilterMapsRequest);
            Message.RemoveListener<DragonGenesRequest>(OnDragonGenesRequest);
        }
        #endregion

        #region Listener Functions
        void OnDragonsResponse(DragonsResponse response)
        {
            userDragons = response.dragons;
        }

        void OnQueryMarketRequest(QueryMarketRequest request)
        {
            if(request.type == MarketQueryType.Exit)
            {
                if(queryMarket != null)
                {
                    StopCoroutine(queryMarket);
                    queryMarket = null;
                }
            }
            else
            {
                string url = envs.MarketQueryUrl();
                bool updateDragonIds = true;
                int page = 0;
                if(request.type == MarketQueryType.UpdateFilters)
                {
                    string filterString = "";
                    int numFilters = request.filters.Count;
                    for(int i = 0; i < numFilters; i++)
                    {
                        MarketFilter filter = request.filters[0];
                        if(i == 0)
                        {
                            filterString = filter.GetQueryString();
                        }
                        else
                        {
                            filterString += "&" + filter.GetQueryString();
                        }
                    }
                    url = envs.MarketQueryUrl(filterString);
                }
                else if(request.type == MarketQueryType.UpdatePage)
                {
                    page = request.page;
                    updateDragonIds = false;
                    int startIndex = request.page * dragonsPerPage;
                    int endIndex = startIndex + 9;
                    endIndex = endIndex < currentDragonIds.Length ? endIndex : currentDragonIds.Length - 1;
                    int numDragons = endIndex - startIndex;
                    string filterString = "";
                    for(int i = 0; i < numDragons; i++)
                    {
                        string id = currentDragonIds[i + startIndex].ToString();
                        if(i == 0)
                        {
                            filterString = "dragons=" + id;
                        }
                        else
                        {
                            filterString += "," + id;
                        }
                    }
                    url = envs.MarketQueryUrl(filterString);
                }
                queryMarket = null;
                queryMarket = QueryMarket(url, updateDragonIds, page);
                StartCoroutine(queryMarket);
            }
        }

        void OnDisplayDragonZoomRequest(DisplayDragonZoomRequest request)
        {
            int numDisplayDragons = displayDragons.Count;
            for(int i = 0; i < numDisplayDragons; i++)
            {
                if(request.displayDragonIndex == i)
                {
                    displayDragons[i].DisableCanvas();
                }
                else
                {
                    displayDragons[i].DisableMaterials();
                }
            }
            if(getDragonById != null)
            {
                StopCoroutine(getDragonById);
                getDragonById = null;
            }
            string url = envs.DragonByIdApiUrl(request.dragonId);
            getDragonById = GetDragonById(url);
            StartCoroutine(getDragonById);
        }

        void OnFilterDragonZoomRequest(FilterDragonZoomRequest request)
        {
            foreach(DisplayDragon dragon in displayDragons)
            {
                dragon.DisableMaterials();
            }
        }

        void OnReturnToMarket(ReturnToMarket returnToMarket)
        {
            ReturnToMarketReset();
        }

        void OnMarketCameraTransformsRequest(MarketCameraTransformsRequest request)
        {
            Message.Send(new MarketCameraTransformsResponse(marketCameraTransforms));
        }

        void OnMarketFilterMapsRequest(MarketFilterMapsRequest request)
        {
            Message.Send(new MarketFilterMapsResponse(marketFilterMaps, request.sender));
        }

        void OnDragonGenesRequest(DragonGenesRequest request)
        {
            Message.Send(new DragonGenesResponse(request.sender, genes));
        }
        #endregion

        #region Private Functions
        void ReturnToMarketReset()
        {
            foreach(DisplayDragon dragon in displayDragons)
            {
                dragon.ReEnable();
            }
            Message.Send(new MoveCameraRequest(CameraTransformType.Market));
        }
        #endregion

        #region Coroutines
        IEnumerator QueryMarket(string url, bool updateDragonIds, int page)
        {
            UnityWebRequest request = RyzmUtils.GetRequest(url);
            Debug.Log("query market dragons url: " + url);
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
                MarketQueryGetResponse response = MarketQueryGetResponse.FromJson(res);
                if(updateDragonIds)
                {
                    currentDragonIds = response.dragonIds.ToArray();
                }
                
                int totalNumDragons = response.dragonIds.Count;
                int numDisplayDragons = displayDragons.Count;
                int numNewDragons = response.dragons.Count;
                for(int i = 0; i < numDisplayDragons; i++)
                {
                    DisplayDragon dragon = displayDragons[i];
                    if(i < numNewDragons)
                    {
                        dragon.Enable(response.dragons[i]);
                    }
                    else
                    {
                        dragon.Disable();
                    }
                }
                Message.Send(new QueryMarketResponse(numNewDragons, page, totalNumDragons));
                
                // dragonCards = new DragonCardMetadata[totalNumDragons];
                // for(int i = 0; i < totalNumDragons; i++)
                // {
                //     MarketDragonMetadata metadata = response.dragons[i];
                //     DragonCardMetadata card = new DragonCardMetadata();
                //     card.owner = metadata.owner;
                //     card.id = metadata.id;
                //     card.price = metadata.price;
                //     card.media = metadata.media;
                //     dragonCards[i] = card;
                //     getMediaImage = GetMediaImage(card.media, i);
                //     StartCoroutine(getMediaImage);
                // }

                // waitForInitialization = WaitForInitialization(page, totalNumDragons);
                // StartCoroutine(waitForInitialization);
            }
        }

        IEnumerator GetDragonById(string url)
        {
            UnityWebRequest request = RyzmUtils.GetRequest(url);
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
                ReturnToMarketReset();
                Message.Send(new DisplayDragonZoomResponse(true));
            }
            else
            {
                string res = request.downloadHandler.text;
                Debug.Log("GET SUCCESS " + res);
                DragonByIdGetResponse response = DragonByIdGetResponse.FromJson(res);
                Message.Send(new EnableDragonInfoPanel(response.dragon));
                Message.Send(new DisplayDragonZoomResponse());
            }
        }

        IEnumerator GetMediaImage(string url, int index)
        {
            UnityWebRequest request = RyzmUtils.TextureRequest(url);
            int numFails = 0;
            bool failed = true;
            while(numFails < 3)
            {
                yield return request.SendWebRequest();
                if(request.isNetworkError || request.isHttpError)
                {
                    request = RyzmUtils.TextureRequest(url);
                    numFails++;
                    Debug.LogError("Failed getting media image " + numFails + " times");
                }
                else
                {
                    failed = false;
                    break;
                }
            }
            if(failed)
            {
                // todo: how to handle this
            }
            else
            {
                dragonCards[index].image = DownloadHandlerTexture.GetContent(request);
            }
        }

        IEnumerator WaitForInitialization(int page, int totalNumDragons)
        {
            bool initialized = false;
            while(!initialized)
            {
                bool isInitialized = true;
                foreach(DragonCardMetadata card in dragonCards)
                {
                    if(!card.Initialized)
                    {
                        isInitialized = false;
                        break;
                    }
                }
                initialized = isInitialized;
                yield return waitForEndOfFrame;
            }
            Message.Send(new QueryMarketResponse(dragonCards, page, totalNumDragons));
            yield break;
        }
        #endregion
    }

    public enum MarketQueryType
    {
        Start,
        UpdatePage,
        UpdateFilters,
        Exit
    }
}
