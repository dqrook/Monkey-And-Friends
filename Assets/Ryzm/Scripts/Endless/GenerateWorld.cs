using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class GenerateWorld : MonoBehaviour
    {
        static public Transform dummyTransform;
        // last platform added
        static public GameObject lastSpawnedPlatform;

        void Awake()
        {
            dummyTransform = new GameObject("dummy").transform;
        }

        public static void RunDummy()
        {
            GameObject p = EndlessPool.Instance.GetRandom();
            if(p == null) return;
            
            if(lastSpawnedPlatform != null)
            {
                int move = lastSpawnedPlatform.tag == "platformTSection" ? 20 : 10;
                dummyTransform.position = lastSpawnedPlatform.transform.position + RunnerController.player.transform.forward * move;
                if(lastSpawnedPlatform.tag == "stairsUp")
                {
                    dummyTransform.Translate(0, 5, 0);
                }
                else if (lastSpawnedPlatform.tag == "stairsDown")
                {
                    dummyTransform.Translate(0, -5, 0);
                }
            }

            lastSpawnedPlatform = p;
            p.transform.position = dummyTransform.position;
            p.transform.rotation = dummyTransform.rotation;
            p.SetActive(true);
        }
    }
}
