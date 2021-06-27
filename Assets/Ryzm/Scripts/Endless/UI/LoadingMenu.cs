using Ryzm.UI.Messages;
using CodeControl;
using System.Collections;
using UnityEngine;

namespace Ryzm.UI
{
    public class LoadingMenu : RyzmMenu
    {
        IEnumerator timedDeactivate;
        bool timedLoading;

        protected override void Awake()
        {
            base.Awake();
            Message.AddListener<ActivateTimedLoadingMenu>(OnActivateTimedLoadingMenu);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Message.RemoveListener<ActivateTimedLoadingMenu>(OnActivateTimedLoadingMenu);
        }

        protected override void OnActivateMenu(ActivateMenu activate)
        {
            if(!timedLoading)
            {
                base.OnActivateMenu(activate);
            }
        }

        void OnActivateTimedLoadingMenu(ActivateTimedLoadingMenu activate)
        {
            Debug.Log("u kno i b active lol");
            IsActive = true;
            timedDeactivate = null;
            timedDeactivate = TimedDeactivate(activate.timeoutTime);
            StartCoroutine(timedDeactivate);
            timedLoading = true;
        }

        IEnumerator TimedDeactivate(float timeoutTime)
        {
            float _time = 0;
            while(_time < timeoutTime)
            {
                _time += Time.deltaTime;
                yield return null;
            }
            IsActive = false;
            timedLoading = false;
            yield break;
        }
    }
}
