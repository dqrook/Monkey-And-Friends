using UnityEngine;
using Ryzm.Utils;

namespace Ryzm
{
    [CreateAssetMenu(fileName = "Envs", menuName = "ScriptableObjects/Envs", order = 2)]
    public class Envs : ScriptableObject
    {
        #region Public Variables
        [Header("Near")]
        public string title;
        public string contractId;
        public string nodeUrl = "https://rpc.testnet.near.org";
        public string walletUrl = "https://wallet.testnet.near.org";

        [Header("Frontend")]
        public string frontendUrl;
        public string loginSuccessPath;
        public string loginFailPath;
        public string transactionsPath;

        [Header("API")]
        public string apiUrl;
        public string getDragonsPath;
        public string breedDragonsPath;
        public string getMarketDragonsPath;
        public string breedDragonsTxHashPath;
        public string dragonIdsPath;
        public string dragonByIdPath;
        public string buyDragonTxHashPath;
        public string addDragonToMarketPath;
        public string removeDragonFromMarketPath;
        public string marketQueryPath;
        #endregion

        #region Properties
        public string GetDragonsApiUrl
        {
            get
            {
                return apiUrl + getDragonsPath;
            }
        }

        public string BreedDragonsApiUrl
        {
            get
            {
                return apiUrl + breedDragonsPath;
            }
        }

        public string GetMarketDragonsApiUrl
        {
            get
            {
                return apiUrl + getMarketDragonsPath;
            }
        }

        public string BreedDragonsTxHashApiUrl
        {
            get
            {
                return apiUrl + breedDragonsTxHashPath;
            }
        }

        public string BuyDragonTxHashApiUrl
        {
            get
            {
                return apiUrl + buyDragonTxHashPath;
            }
        }

        public string AddDragonToMarketApiUrl
        {
            get
            {
                return apiUrl + addDragonToMarketPath;
            }
        }

        public string RemoveDragonFromMarketApiUrl
        {
            get
            {
                return apiUrl + removeDragonFromMarketPath;
            }
        }

        string SuccessUrl
        {
            get
            {
                return frontendUrl + loginSuccessPath;
            }
        }

        string FailureUrl
        {
            get
            {
                return frontendUrl + loginFailPath;
            }
        }

        string TransactionUrl
        {
            get
            {
                return frontendUrl + transactionsPath;
            }
        }
        #endregion

        #region Public Functions
        public string DragonIdsApiUrl(string account)
        {
            return apiUrl + dragonIdsPath + "?owner=" + account;
        }

        public string DragonByIdApiUrl(int id)
        {
            return apiUrl + dragonByIdPath + "?dragon_id=" + id.ToString();
        }

        public string LoginUrl(string publicKey)
        {
            string url = walletUrl + "/login?app_url=" + RyzmUtils.UrlEncode(frontendUrl) + "&title=" + RyzmUtils.UrlEncode(title) + "&success_url=" + RyzmUtils.UrlEncode(SuccessUrl) + "&failure_url=" + RyzmUtils.UrlEncode(FailureUrl) + "&public_key=" + RyzmUtils.UrlEncode("ed25519:" + publicKey);
            url += "&contract_id=" + RyzmUtils.UrlEncode(contractId);
            return url;
        }

        public string SignTransactionUrl(string transactionHash)
        {
            string url = walletUrl + "/sign?transactions=" + RyzmUtils.UrlEncode(transactionHash) + "&callbackUrl=" + RyzmUtils.UrlEncode(TransactionUrl);
            return url;
        }

        public string MarketQueryUrl(string queryString = "")
        {
            string url = apiUrl + marketQueryPath;
            if(queryString.Length > 0)
            {
                url += "?" + queryString;
            }
            return url;
        }
        #endregion
    }
}
