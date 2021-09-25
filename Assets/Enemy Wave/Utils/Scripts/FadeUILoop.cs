using UnityEngine;
using UnityEngine.UI;

public class FadeUILoop
{
	private Graphic graphic;
	private Color srcColor;
	private Color dstColor;
	private float fadeTime;
	private float currentTimer;
	private bool loop;

	public void SetLoop(bool loop)
	{
		this.loop = loop;

		if (!loop)
		{
			srcColor = graphic.color;
			dstColor = Color.white;
			dstColor.a = 0;
			currentTimer = 0f;
			fadeTime = 0.25f;
		}
	}

	public void Init(float dstAlpha, float fadeTime, Graphic graphic)
	{
		this.fadeTime = fadeTime;
		this.graphic = graphic;

		srcColor = Color.white;

		dstColor = Color.white;
		dstColor.a = dstAlpha;
	}

	public void UpdateFade(float deltaTime)
	{
		float t = currentTimer / fadeTime;
		currentTimer += deltaTime;

		Color newColor = Color.Lerp(srcColor, dstColor, t);
		graphic.color = newColor;

		if (loop)
		{
			if (t >= 1)
			{
				Color aux = srcColor;
				srcColor = dstColor;
				dstColor = aux;

				t = 0f;
				currentTimer = 0f;
			}
		}
	}
}