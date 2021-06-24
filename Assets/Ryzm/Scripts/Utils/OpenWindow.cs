using UnityEngine;
using System.Runtime.InteropServices;

namespace Ryzm.Utils
{
    public static class OpenWindow
    {
        #if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void openWindow(string url);
        #endif

        public static void OpenUrl(string url) 
        {
            #if !UNITY_WEBGL || UNITY_EDITOR
                Application.OpenURL(url);
            #else
                openWindow(url);
            #endif
        }
    }
}
