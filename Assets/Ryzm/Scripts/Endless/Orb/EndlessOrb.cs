using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public class EndlessOrb : EndlessItem
    {
        #region Private Variables
        Animator animator;
        bool orbCollected;
        #endregion

        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            animator = GetComponent<Animator>();
        }

        protected override void Start() {}

        protected void OnEnable()
        {
            orbCollected = false;
            animator.SetBool("shrink", false);
        }

        protected void OnDisable()
        {
            animator.SetBool("shrink", false);
        }

        void OnTriggerEnter(Collider other)
        {
            OnTrigger(other);
        }

        void OnTriggerStay(Collider other)
        {
            OnTrigger(other);
        }
        #endregion

        #region Listener Functions
        protected override void OnGameStatusResponse(GameStatusResponse gameStatusResponse)
        {
            base.OnGameStatusResponse(gameStatusResponse);
            if(gameStatusResponse.status == GameStatus.Restart || gameStatusResponse.status == GameStatus.Exit)
            {
                animator.SetBool("shrink", false);
            }
        }
        #endregion

        #region Private Functions
        void OnTrigger(Collider other)
        {
            if(!orbCollected)
            {
                animator.SetBool("shrink", true);
                orbCollected = true;
            }
        }
        #endregion
    }
}