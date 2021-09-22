using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.Test
{
    public class TestVectors : MonoBehaviour
    {
        public Transform transform1;
        public Transform transform2;

        void Update()
        {
            Vector3 a = transform1.InverseTransformPoint(transform2.position);
            Vector3 b = transform2.InverseTransformPoint(transform1.position);
            Debug.Log(a + " " + b);
        }
    }
}
