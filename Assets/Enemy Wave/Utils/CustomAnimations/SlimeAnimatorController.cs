using System;
using UnityEngine;

namespace Neon2.SlimeSystem
{
	public class SlimeAnimatorController : MonoBehaviour
	{
		private Vector3 initialRotation = new Vector3(0f, -83.65701f, 0f);

		private const string TAG = "SlimeAnimatorController";

		private CustomAnimationRunner animationRunner;

		public SlimeAudioController slimeAudioController;
		public Transform trSlimeMeshParent;


		private CustomAnimationBinding currentAnimation;

		[Header("Animations")]
		public CustomAnimationBinding jumpAnimation;
		public CustomAnimationBinding idleAnimation;
		public CustomAnimationBinding fallingAnimation;


		public void Init(SlimeAudioController slimeAudioController)
		{
			this.slimeAudioController = slimeAudioController;

			animationRunner = new CustomAnimationRunner();

			jumpAnimation.Init(SlimeJumpCallback, animationRunner);
			idleAnimation.Init(null, animationRunner);
			fallingAnimation.Init(null, animationRunner);
		}

		public void GoToIdle()
		{
			bool _waitForEnd = currentAnimation != fallingAnimation;
			animationRunner.PrepareNextAnimation(idleAnimation, _waitForEnd);
			currentAnimation = idleAnimation;

			transform.localEulerAngles = initialRotation;
			trSlimeMeshParent.localRotation = Quaternion.identity;
		}

		public void GoToJump(bool waitForEnd)
		{
			bool _waitForEnd = waitForEnd && (currentAnimation != fallingAnimation);
			animationRunner.PrepareNextAnimation(jumpAnimation, _waitForEnd);
			currentAnimation = jumpAnimation;

			transform.localEulerAngles = initialRotation;
			trSlimeMeshParent.localRotation = Quaternion.identity;
		}

		public void GoToFalling()
		{
			animationRunner.PrepareNextAnimation(fallingAnimation, false);
			currentAnimation = fallingAnimation;

			transform.localEulerAngles = initialRotation;
			trSlimeMeshParent.localRotation = Quaternion.identity;
		}

		public void StopCurrentAnimation()
		{
			currentAnimation.StopAnimation();
		}

		private void Update()
		{
			if (animationRunner != null)
			{
				float deltaTime = Time.deltaTime;
				animationRunner._Update(deltaTime);
			}
		}

		#region CALLBACKS
		public void SlimeJumpCallback(CustomAnimation.CallbackMethod callbackMethod)
		{
			switch (callbackMethod)
			{
				case CustomAnimation.CallbackMethod.SLIME_JUMP:
					slimeAudioController.PlayJumpSound();
					break;
			}

			//CustomLogger.LogWarning(TAG, "SlimeAnimatorControllers()");
		}
		#endregion
	}
}