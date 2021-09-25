#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;

namespace Neon2
{
	public static class EditorUtils
	{
		public static void Space(int numSpaces)
		{
			for (int i = 0; i < numSpaces; i++)
			{
				EditorGUILayout.Space();
			}
		}

		public static List<T> SearchAssets<T>() where T : UnityEngine.Object
		{
			string[] results = AssetDatabase.FindAssets("t:" + typeof(T).Name);
			List<T> res = new List<T>();
			for (int i = 0; i < results.Length; i++)
			{
				string assetPath = AssetDatabase.GUIDToAssetPath(results[i]);
				T element = AssetDatabase.LoadAssetAtPath<T>(assetPath);
				res.Add(element);
			}

			return res;
		}

		public static T SearchAsset<T>() where T : UnityEngine.Object
		{
			string[] results = AssetDatabase.FindAssets("t:" + typeof(T).Name);
			string assetPath = AssetDatabase.GUIDToAssetPath(results[0]);

			T res = AssetDatabase.LoadAssetAtPath<T>(assetPath);
			return res;
		}

		public static T SearchAsset<T>(string name) where T : UnityEngine.Object
		{
			string[] results = AssetDatabase.FindAssets("t:" + typeof(T).Name + " " + name);
			string assetPath = AssetDatabase.GUIDToAssetPath(results[0]);

			T res = AssetDatabase.LoadAssetAtPath<T>(assetPath);
			return res;
		}
	}
}
#endif