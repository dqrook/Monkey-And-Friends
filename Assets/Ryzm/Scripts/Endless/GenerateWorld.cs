using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class GenerateWorld : MonoBehaviour
    {
        public GameObject[] platforms;
        GameObject dummyTraveler;
        Transform dummyTransform;


        // Start is called before the first frame update
        void Start()
        {
            dummyTraveler = new GameObject("dummy");
            dummyTransform = dummyTraveler.transform;

            for(int i = 0; i < 20; i++)
            {
                int platformNumber = Random.Range(0, platforms.Length);
                string tag = platforms[platformNumber].tag;
                Instantiate(platforms[platformNumber], dummyTransform.position, dummyTransform.rotation);

                if(tag == "stairsUp") 
                {
                    dummyTransform.Translate(0, 5, 0);
                }
                else if(tag == "stairsDown")
                {
                    dummyTransform.Translate(0, -5, 0);
                }
                else if(tag == "platformTSection")
                {
                    int angle = Random.Range(0, 2) == 0 ? 90 : -90;
                    dummyTransform.Rotate(new Vector3(0, angle, 0));
                    dummyTransform.Translate(Vector3.forward * -10);
                }
                dummyTransform.Translate(Vector3.forward * -10);
            }
        }
    }
}
