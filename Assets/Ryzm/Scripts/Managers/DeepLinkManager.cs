using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm
{
    public class DeepLinkManager : MonoBehaviour
    {
        #region Event Functions
        void Awake()
        {
            ImaginationOverflow.UniversalDeepLinking.DeepLinkManager.Instance.LinkActivated += LinkActivated;
        }
        
        void OnDestroy()
        {
            ImaginationOverflow.UniversalDeepLinking.DeepLinkManager.Instance.LinkActivated -= LinkActivated;
        }
        #endregion

        #region Private Functions
        void LinkActivated(ImaginationOverflow.UniversalDeepLinking.LinkActivation linkActivation)
        {
            string url = linkActivation.Uri;
            string querystring = linkActivation.RawQueryString;
            string qParameter = linkActivation.QueryString["q"];
        }
        #endregion
    }
}
