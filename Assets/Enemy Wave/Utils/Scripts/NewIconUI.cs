using UnityEngine;
using UnityEngine.UI;

public class NewIconUI : MonoBehaviour
{
	public delegate bool CheckNewActionDelegate();
	private CheckNewActionDelegate checkNewAction;

	private float timer;
	private float srcAlpha;
	private float dstAlpha;



	[Header("Config")]
	public float animationLength;
	public float minAlpha;
	public float maxAlpha;

	[Header("GUI References")]
	public Image imgIcon;

	public void Init(CheckNewActionDelegate checkNewAction)
	{
		this.checkNewAction = checkNewAction;

		timer = 0f;
		srcAlpha = minAlpha;
		dstAlpha = maxAlpha;

		UpdateGUI();
	}

	public void Show()
	{
		gameObject.SetActive(true);
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}

	private void Show(bool value)
	{
		if (value)
		{
			Show();
		}
		else
		{
			Hide();
		}
	}

	public void UpdateGUI()
	{
		if (checkNewAction != null)
		{
			bool areThereNewElements = checkNewAction();
			Show(areThereNewElements);
		}
	}

	public void Update()
	{
		float deltaTime = Time.deltaTime;

		UpdateAlpha(deltaTime);
	}

	private void UpdateAlpha(float deltaTime)
	{
		float t = timer / animationLength;
		timer += deltaTime;

		float newAlpha = Mathf.Lerp(srcAlpha, dstAlpha, t);

		Color newColor = imgIcon.color;
		newColor.a = newAlpha;
		imgIcon.color = newColor;

		if(t >= 1)
		{
			float aux = srcAlpha;
			srcAlpha = dstAlpha;
			dstAlpha = aux;
			timer = 0f;
		}
	}
}