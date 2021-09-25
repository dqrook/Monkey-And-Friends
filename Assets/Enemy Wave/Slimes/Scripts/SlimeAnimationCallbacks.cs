using UnityEngine;

namespace Neon2.SlimeSystem
{
	public class SlimeAnimationCallbacks : MonoBehaviour
	{
		public SlimeCore slimeController;
		public SlimeAudioController slimeAudioController;

		public void Jump()
		{
			if (slimeAudioController != null)
			{
				slimeAudioController.PlayJumpSound();
			}
		}
	}
}