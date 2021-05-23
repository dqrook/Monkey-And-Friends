using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkScript : MonoBehaviour {

    public bool canNotBlink;

    [Range(1, 8)]
    public float blinkPause;

    bool blinking;

    GameObject character;
    SkinnedMeshRenderer characterMeshRenderer;
    
    float blinkWeight;
    float blinkSpeed = 1500f;
    float maxBlinkWeight = 100;
    float minBlinkWeight = 0;

    // Use this for initialization
    void Start ()
    {
        character = this.gameObject;
        characterMeshRenderer = character.GetComponent<SkinnedMeshRenderer>();

        if (!canNotBlink)
        {
            StartCoroutine("Blink");
        }
	}

    public void Update()
    {
        if(!canNotBlink)
        {
            if (blinking)
            {
                if (blinkWeight <= maxBlinkWeight)
                {
                    blinkWeight += Time.deltaTime * blinkSpeed;
                    characterMeshRenderer.SetBlendShapeWeight(0, blinkWeight);
                }
            }
            else
            {
                if (minBlinkWeight <= blinkWeight)
                {
                    blinkWeight -= Time.deltaTime * blinkSpeed;
                    characterMeshRenderer.SetBlendShapeWeight(0, blinkWeight);
                }
            }
        }
    }

    [ContextMenu("Blink")]
    IEnumerator Blink()
    {
        while (!canNotBlink)
        {
            yield return new WaitForSeconds(blinkPause);

            blinking = true;

            yield return new WaitForSeconds(.2f);

            blinking = false;

            yield return null;
        }
    }
    

    [ContextMenu("Blink Test")]
    public void BlinkTest()
    {
        character = this.gameObject;
        characterMeshRenderer = character.GetComponent<SkinnedMeshRenderer>();
        characterMeshRenderer.SetBlendShapeWeight(0, blinkWeight);

        if (!canNotBlink)
        {
            StartCoroutine("Blink");
        }
    }
}
