using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Linq;
using UnityEngine;

namespace Ryzm {

  /// <summary>
  /// UnityEngine.PlayerPrefs wrapper for WebGL LocalStorage
  /// </summary>
  public static class RyzmPlayerPrefs {

    public static void DeleteKey(string key) {
      Debug.Log(string.Format("Ryzm.PlayerPrefs.DeleteKey(key: {0})", key));

      #if UNITY_WEBGL
        removeFromLocalStorage(key: key);
      #else
        UnityEngine.PlayerPrefs.DeleteKey(key: key);
      #endif
    }

    public static bool HasKey(string key) {
      Debug.Log(string.Format("Ryzm.PlayerPrefs.HasKey(key: {0})", key));
      #if UNITY_EDITOR || !UNITY_WEBGL
        return (UnityEngine.PlayerPrefs.HasKey(key: key));
      #else
        return (hasKey(key) == 1);
      #endif
    }

    public static string GetString(string key) {
      Debug.Log(string.Format("Ryzm.PlayerPrefs.GetString(key: {0})", key));

      #if UNITY_WEBGL
        return loadFromLocalStorage(key: key);
      #else
        return (UnityEngine.PlayerPrefs.GetString(key: key));
      #endif
    }

    public static void SetString(string key, string value) {
      Debug.Log(string.Format("Ryzm.PlayerPrefs.SetString(key: {0}, value: {1})", key, value));

      #if UNITY_WEBGL
        saveToLocalStorage(key: key, value: value);
      #else
        UnityEngine.PlayerPrefs.SetString(key: key, value: value);
      #endif

    }

    public static void Save() {
      Debug.Log(string.Format("Ryzm.PlayerPrefs.Save()"));

      #if !UNITY_WEBGL
        UnityEngine.PlayerPrefs.Save();
      #endif
    }

    #if UNITY_WEBGL
      [DllImport("__Internal")]
      private static extern void saveToLocalStorage(string key, string value);

      [DllImport("__Internal")]
      private static extern string loadFromLocalStorage(string key);

      [DllImport("__Internal")]
      private static extern void removeFromLocalStorage(string key);

      [DllImport("__Internal")]
      private static extern int hasKey(string key);
    #endif
  }
}
