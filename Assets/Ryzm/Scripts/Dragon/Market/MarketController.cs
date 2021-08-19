using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.Dragon.Messages;
using CodeControl;
using UnityEngine.Networking;
using Ryzm.Utils;

namespace Ryzm.Dragon
{
    public class MarketController : MonoBehaviour
    {
        #region Public Variables
        public int dragonsPerPage = 10;
        public Envs envs;
        public int[] currentDragonIds;
        #endregion

        #region Private Variables
        BaseDragon[] userDragons;
        IEnumerator queryMarket;
        DragonCardMetadata[] dragonCards;
        IEnumerator getMediaImage;
        IEnumerator waitForInitialization;
        WaitForEndOfFrame waitForEndOfFrame;
        #endregion

        #region Event Functions
        void Awake()
        {
            waitForEndOfFrame = new WaitForEndOfFrame();
            Message.AddListener<DragonsResponse>(OnDragonsResponse);
            Message.AddListener<QueryMarketRequest>(OnQueryMarketRequest);
        }

        void OnDestroy()
        {
            Message.RemoveListener<DragonsResponse>(OnDragonsResponse);
            Message.RemoveListener<QueryMarketRequest>(OnQueryMarketRequest);
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
                
                int numNewDragons = response.dragonIds.Count;
                dragonCards = new DragonCardMetadata[numNewDragons];
                for(int i = 0; i < numNewDragons; i++)
                {
                    MarketDragonMetadata metadata = response.dragons[i];
                    DragonCardMetadata card = new DragonCardMetadata();
                    card.owner = metadata.owner;
                    card.id = metadata.id;
                    card.price = metadata.price;
                    card.media = metadata.media;
                    dragonCards[i] = card;
                    getMediaImage = GetMediaImage(card.media, i);
                    StartCoroutine(getMediaImage);
                }

                waitForInitialization = WaitForInitialization(page, currentDragonIds.Length);
                StartCoroutine(waitForInitialization);
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
