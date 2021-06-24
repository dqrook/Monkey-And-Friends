using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using Ryzm.Utils;

namespace Ryzm.Blockchain
{
    [CreateAssetMenu(fileName = "NearEnvs", menuName = "ScriptableObjects/NearEnvs", order = 2)]
    public class NearEnvs : ScriptableObject
    {
        public string title;
        public string successUrl;
        public string failureUrl;
        public string appUrl;
        public string contractId;
        public string nodeUrl = "https://rpc.testnet.near.org";
        public string walletUrl = "https://wallet.testnet.near.org";

        public string LoginUrl(string publicKey)
        {
            string url = walletUrl + "/login?app_url=" + RyzmUtils.UrlEncode(appUrl) + "&title=" + RyzmUtils.UrlEncode(title) + "&success_url=" + RyzmUtils.UrlEncode(successUrl) + "&failure_url=" + RyzmUtils.UrlEncode(failureUrl) + "&public_key=" + RyzmUtils.UrlEncode("ed25519:" + publicKey);
            url += "&contract_id=" + RyzmUtils.UrlEncode(contractId);
            return url;
        }

        public string SignTransactionUrl(string transactionHash)
        {
            string url = walletUrl + "/sign?transactions=" + RyzmUtils.UrlEncode(transactionHash) + "&callbackUrl=" + RyzmUtils.UrlEncode(appUrl);
            return url;
        }
    }
}
