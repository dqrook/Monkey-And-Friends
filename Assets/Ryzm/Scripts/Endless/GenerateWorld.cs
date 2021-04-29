using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class GenerateWorld : MonoBehaviour
    {
        static public Transform dummyTransform;
        static public GameObject lastPlatform;

        void Awake()
        {
            dummyTransform = new GameObject("dummy").transform;
        }

        public static void RunDummy()
        {
            GameObject p = EndlessPool.Instance.GetRandom();
            if(p == null) return;
            
            if(lastPlatform != null)
            {
                int move = lastPlatform.tag == "platformTSection" ? 20 : 10;
                dummyTransform.position = lastPlatform.transform.position + RunnerController.player.transform.forward * move;
                if(lastPlatform.tag == "stairsUp")
                {
                    dummyTransform.Translate(0, 5, 0);
                }
                else if (lastPlatform.tag == "stairsDown")
                {
                    dummyTransform.Translate(0, -5, 0);
                }
            }

            lastPlatform = p;
            p.transform.position = dummyTransform.position;
            p.transform.rotation = dummyTransform.rotation;
            p.SetActive(true);

            // if(p.tag == "stairsDown")
            // {
            //     dummyTransform.Translate(0, -5, 0);
            //     p.transform.position = dummyTransform.position;
            // }
        }
    }
}
