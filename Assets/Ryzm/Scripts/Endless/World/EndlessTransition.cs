using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;
namespace Ryzm.EndlessRunner
{
    public class EndlessTransition : EndlessBasePath
    {
        #region Public Variables
        public Transform nextSpawn;
        public float deactivationTime;
        #endregion

        #region Private Variables
        IEnumerator deactivate;
        WaitForSeconds deactivationWait;
        #endregion

        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            deactivationWait = new WaitForSeconds(deactivationTime);
        }
        #endregion
        
        #region Public Functions
        public void Initialize(Transform startTransform)
        {

        }

        public override void Enter()
        {
            Message.Send(new EnterTransition(nextSpawn));
        }

        public override void Exit()
        {
            Message.Send(new ExitTransition());
            CancelDeactivation();
            deactivate = Deactivate();
            StartCoroutine(deactivate);
        }

        public override void CancelDeactivation()
        {
            if(deactivate != null)
            {
                StopCoroutine(deactivate);
                deactivate = null;
            }
        }
        #endregion

        #region Coroutines
        IEnumerator Deactivate()
        {
            yield return deactivationWait;
            gameObject.SetActive(false);
        }
        #endregion
    }
}
