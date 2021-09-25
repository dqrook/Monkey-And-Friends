using System;
using UnityEngine;

public class FadeUI : MonoBehaviour
{
	public CanvasGroup canvasGroup;

	private float srcAlpha;
	private float dstAlpha;
	private float fadeTime;

	private float currentTimer;
	private float t;

	private bool fading;
	private bool looped;

	private Action actionOnFinish;

	public void PrepareToFadeIn(float fadeTime, bool looped, Action actionOnFinish)
	{
		PrepareToFade(canvasGroup.alpha, 1, fadeTime, looped, actionOnFinish);
	}

	public void PrepareToFadeOut(float fadeTime, bool looped, Action actionOnFinish)
	{
		PrepareToFade(canvasGroup.alpha, 0, fadeTime, looped, actionOnFinish);
	}

	public void HideFromCurrentState(float fadeTime)
	{
		PrepareToFade(canvasGroup.alpha, 0, fadeTime, false, null);
	}

	public void PrepareToFade(float srcAlpha, float dstAlpha, float fadeTime, bool looped, Action actionOnFinish)
	{
		this.actionOnFinish = actionOnFinish;
		this.srcAlpha = srcAlpha;
		this.dstAlpha = dstAlpha;
		this.fadeTime = fadeTime;
		this.looped = looped;
		canvasGroup.alpha = srcAlpha;
		Init();
	}

	private void Init()
	{
		currentTimer = 0f;
		t = 0f;

		if (dstAlpha != canvasGroup.alpha)
		{
			fading = true;
		}
	}

	public bool IsRunning()
	{
		return fading;
	}

	public void FinishInmediate()
	{
		fading = false;
		canvasGroup.alpha = dstAlpha;
		if (actionOnFinish != null)
		{
			actionOnFinish();
		}
	}

	public void UpdateFade(float deltaTime)
	{
		if (fading)
		{
			t = currentTimer / fadeTime;
			currentTimer += deltaTime;

			canvasGroup.alpha = Mathf.Lerp(srcAlpha, dstAlpha, t);

			if(t > 1)
			{
				if (looped)
				{
					currentTimer = 0;
					float aux = srcAlpha;
					srcAlpha = dstAlpha;
					dstAlpha = aux;
				}
				else
				{
					fading = false;
					if(actionOnFinish != null)
					{
						actionOnFinish();
					}
				}
			}
		}
	}
}