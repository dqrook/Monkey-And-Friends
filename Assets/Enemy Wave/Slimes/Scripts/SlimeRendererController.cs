using UnityEngine;

namespace Neon2.SlimeSystem
{
	public class SlimeRendererController : MonoBehaviour
	{
		private const string TAG = "SlimeRendererController";

		public SlimeCore slimeController;

		private void OnBecameVisible()
		{
			slimeController.SlimeVisible();
			//CustomLogger.Log(TAG, "OnBecameVisible()");
		}

		private void OnBecameInvisible()
		{
			slimeController.SlimeInvisible();
			//CustomLogger.Log(TAG, "OnBecameInvisible()");
		}
	}
}