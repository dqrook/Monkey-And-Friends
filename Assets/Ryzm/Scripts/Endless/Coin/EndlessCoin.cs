using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public class EndlessCoin : EndlessScroller
    {
        Animator animator;
        protected override void Awake()
        {
            base.Awake();
            animator = GetComponent<Animator>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            animator.SetBool("shrink", false);
        }

        void OnTriggerEnter(Collider other)
        {
            animator.SetBool("shrink", true);
            Message.Send(new CollectCoin());
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
