using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

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
        public string walletUrl = "https://wallet.testnet.near.org/login/";

        string reservedCharacters = "!*'();:@&=+$,/?%#[]";

        public string UrlEncode(string value)
        {
            if (String.IsNullOrEmpty(value))
                return String.Empty;

            var sb = new StringBuilder();

            foreach (char @char in value)
            {
                if (reservedCharacters.IndexOf(@char) == -1)
                    sb.Append(@char);
                else
                    sb.AppendFormat("%{0:X2}", (int)@char);
            }
            return sb.ToString();
        }

        public string LoginUrl(string publicKey)
        {
            string url = walletUrl + "?app_url=" + UrlEncode(appUrl) + "&title=" + UrlEncode(title) + "&success_url=" + UrlEncode(successUrl) + "&failure_url=" + UrlEncode(failureUrl) + "&contract_id=" + UrlEncode(contractId) + "&public_key=" + UrlEncode("ed25519:" + publicKey);
            return url;
        }
    }
}
