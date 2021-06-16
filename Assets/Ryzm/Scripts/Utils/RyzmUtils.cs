using UnityEngine.Networking;
using System.Text;

namespace Ryzm.Utils
{
    public static class RyzmUtils
    {
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
    }
}
