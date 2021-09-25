using UnityEngine;
using UnityEngine.UI;

public class FPSCounter : MonoBehaviour
{
	public float samplingTime;
	public Text txtFps;

	public int currentFPS;

	private float timer;

	private void Start()
	{
		timer = 0f;
		ShowFPS(Time.deltaTime);
	}

	private void Update()
	{
		float deltaTime = Time.deltaTime;
		timer += deltaTime;

		if(timer >= samplingTime)
		{
			timer = 0f;
			ShowFPS(deltaTime);
		}
	}

	private void ShowFPS(float deltaTime)
	{
		float fps = 1 / Time.unscaledDeltaTime;
		currentFPS = (int)fps;
		//txtFps.text = ((int)fps).ToString();
	}
}
