using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.Utils
{
    public class ScreenCapture : MonoBehaviour
    {
        public int superSize = 4;

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.V))
            {
                OnClickCapture();
            }
        }

        public void OnClickCapture()
        {
            string name = "/Users/ryzm/Desktop/" + Time.frameCount.ToString() + ".png";
            UnityEngine.ScreenCapture.CaptureScreenshot(name, superSize);
        }
    }
}
