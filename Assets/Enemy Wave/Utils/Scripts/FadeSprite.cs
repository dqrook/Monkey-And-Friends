using UnityEngine;

public class FadeSprite : MonoBehaviour
{
	public SpriteRenderer spriteRenderer;

	private Color srcColor;
	private Color dstColor;
	private float fadeTime;

	private float currentTimer;
	private float t;

	private bool fading;
	private bool looped;

	public void PrepareToFadeIn(float fadeTime)
	{
		PrepareToFade(0, 1, fadeTime, false);
	}

	public void PrepareToFadeOut(float fadeTime)
	{
		PrepareToFade(1, 0, fadeTime, false);
	}

	public void PrepareToFade(Color srcColor, Color dstColor, float fadeTime, bool looped)
	{
		this.srcColor = srcColor;
		this.dstColor = dstColor;
		this.fadeTime = fadeTime;
		this.looped = looped;

		Init();
	}

	public void PrepareToFade(float srcAlpha, float dstAlpha, float fadeTime, bool looped)
	{
		this.srcColor = Color.white;
		this.srcColor.a = srcAlpha;

		this.dstColor = Color.white;
		this.dstColor.a = dstAlpha;
		this.fadeTime = fadeTime;
		this.looped = looped;
		Init();
	}

	private void Init()
	{
		currentTimer = 0f;
		t = 0f;

		//if (dstColor.a != spriteRenderer.color.a)
		//{
			fading = true;
		//}

		spriteRenderer.color = srcColor;
	}

	public bool IsFading()
	{
		return fading;
	}

	public void UpdateFade(float deltaTime)
	{
		if (fading)
		{
			t = currentTimer / fadeTime;
			currentTimer += deltaTime;

			Color newColor = Color.Lerp(srcColor, dstColor, t);
			spriteRenderer.color = newColor;

			if (t > 1)
			{
				if (looped)
				{
					currentTimer = 0;
					Color aux = srcColor;
					srcColor = dstColor;
					dstColor = aux;
				}
				else
				{
					fading = false;
				}
			}
		}
	}
}