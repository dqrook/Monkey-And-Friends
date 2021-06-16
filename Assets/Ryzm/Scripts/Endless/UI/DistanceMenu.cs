using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner.UI
{
    public class DistanceMenu : BaseMenu
    {
        public TextMeshProUGUI distance;
        int currentDistance;

        public override bool IsActive
        {
            get
            {
                return base.IsActive;
            }
            set
            {
                if(!disable)
                {
                    if(value)
                    {
                        Message.AddListener<RunnerDistanceResponse>(OnRunnerDistanceResponse);
                        Message.Send(new RunnerDistanceRequest());
                    }
                    else
                    {
                        Message.RemoveListener<RunnerDistanceResponse>(OnRunnerDistanceResponse);
                        currentDistance = 0;
                    }
                }
                base.IsActive = value;
            }
        }

        void OnRunnerDistanceResponse(RunnerDistanceResponse response)
        {
            int _distance = Mathf.RoundToInt(response.distance);
            if(_distance != currentDistance)
            {
                currentDistance = _distance;
                distance.text = currentDistance.ToString() + " m";
            }
        }
    }
}
