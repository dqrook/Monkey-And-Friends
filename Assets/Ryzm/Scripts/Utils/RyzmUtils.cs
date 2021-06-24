using UnityEngine.Networking;
using System.Text;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Ryzm.Utils
{
    public static class RyzmUtils
    {
        #if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void openWindow(string url);
        [DllImport("__Internal")]
        private static extern void signOut();
        #endif

        static string reservedCharacters = "!*'();:@&=+$,/?%#[]";

        public static UnityWebRequest PostRequest(string url, string bodyJsonString)
        {
            UnityWebRequest request = new UnityWebRequest(url, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
            request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json; charset=utf-8");
            return request;
        }

        public static UnityWebRequest TextureRequest(string url)
        {
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
            return request;
        }

        public static UnityWebRequest GetRequest(string url)
        {
            UnityWebRequest request = UnityWebRequest.Get(url);
            return request;
        }

        public static string UrlEncode(string value)
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

        public static void OpenUrl(string url) 
        {
            #if !UNITY_WEBGL || UNITY_EDITOR
                Application.OpenURL(url);
            #else
                openWindow(url);
            #endif
        }

        public static void BrowserLogout()
        {
            #if UNITY_WEBGL && !UNITY_EDITOR
                signOut();
            #endif
        }
    }
}
