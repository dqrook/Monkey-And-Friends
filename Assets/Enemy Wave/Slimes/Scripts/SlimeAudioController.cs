using UnityEngine;

namespace Neon2.SlimeSystem
{
	public class SlimeAudioController : MonoBehaviour
	{
		private const string TAG = "SlimeAudioController";

		public AudioSource audioSource;
		private AudioClip[] clipsJump;
		
		public void Init(AudioClip[] clipsJump)
		{
			this.clipsJump = clipsJump;
		}

		public void PlayJumpSound()
		{
			if (clipsJump == null) { return; }
			if (clipsJump.Length <= 0) { return; }

			
			int rndIndex = Random.Range(0, clipsJump.Length);
			AudioClip audioClip = clipsJump[rndIndex];

			if (audioClip != null)
			{
				audioSource.clip = audioClip;
				audioSource.Play();
			}
		}
	}
}