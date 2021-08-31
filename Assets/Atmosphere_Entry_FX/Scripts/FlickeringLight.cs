using UnityEngine;
using System.Collections;

public class FlickeringLight : MonoBehaviour {

public Light light;
public int lightMin;
public int lightMax;

    private int lightIntensity = 10;


void Start (){

}

void Update (){

    lightIntensity = (Random.Range (lightMin, lightMax));
    light.intensity = lightIntensity;

}
}