using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner.UI
{
    public class DistanceMenu : EndlessMenu
    {
        public TextMeshProUGUI distance;
        int currentDistance;

        protected override void Awake()
        {
            base.Awake();
            Message.AddListener<RunnerDistanceResponse>(OnRunnerDistanceResponse);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Message.RemoveListener<RunnerDistanceResponse>(OnRunnerDistanceResponse);
        }

        protected override void OnActivateMenu(ActivateMenu activate)
        {
            Message.Send(new RunnerDistanceRequest());
            base.OnActivateMenu(activate);
        }

        protected override void OnDeactivateMenu(DeactivateMenu deactivate)
        {
            base.OnDeactivateMenu(deactivate);
            currentDistance = 0;
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
