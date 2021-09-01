using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public class EndlessOrb : EndlessItem
    {
        Animator animator;
        bool orbCollected;

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
            animator.SetBool("shrink", true);
            if(!orbCollected)
            {
                orbCollected = true;
                // Message.Send(new CollectCoin());
            }
        }

        protected override void OnGameStatusResponse(GameStatusResponse gameStatusResponse)
        {
            base.OnGameStatusResponse(gameStatusResponse);
            if(gameStatusResponse.status == GameStatus.Restart)
            {
                animator.SetBool("shrink", false);
            }
        }
    }
}