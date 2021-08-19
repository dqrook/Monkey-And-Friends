using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.UI
{
    public class FlexRowContainer : MonoBehaviour
    {
       public List<FlexRow> rows = new List<FlexRow>();

       [Header("Constraints")]
        public float xOffset;
        public float yOffset;
        [Range(0, 100)]
        public float maxWidthPercent = 40;
        [Range(0, 100)]
        public float minWidthPercent = 20;
        [Range(0, 1000)]
        public float minWidthPixels = 100;
        [Range(0, 100)]
        public float maxHeightPercent = 90;
    }
}
