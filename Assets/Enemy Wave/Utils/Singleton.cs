using UnityEngine;

namespace Neon2.SlimeSystem
{
	public class Singleton<T> : MonoBehaviour where T : Component
	{
		private static T _instance;
		public static T instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = FindObjectOfType<T>();
				}

				return _instance;
			}
		}
	}
}