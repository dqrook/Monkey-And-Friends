using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public class EndlessCoin : EndlessItem
    {
        Animator animator;
        bool coinCollected;

        protected override void Awake()
        {
            base.Awake();
            animator = GetComponent<Animator>();
        }

        protected override void Start() {}

        protected void OnEnable()
        {
            coinCollected = false;
            animator.SetBool("shrink", false);
        }

        protected void OnDisable()
        {
            animator.SetBool("shrink", false);
        }

        void OnTriggerEnter(Collider other)
        {
            animator.SetBool("shrink", true);
            if(!coinCollected)
            {
                coinCollected = true;
                Message.Send(new CollectCoin());
            }
        }

        protected override void OnGameStatusResponse(GameStatusResponse gameStatusResponse)
        {
            base.OnGameStatusResponse(gameStatusResponse);
            if(gameStatusResponse.status == GameStatus.Restart || gameStatusResponse.status == GameStatus.Exit)
            {
                animator.SetBool("shrink", false);
            }
        }
    }
}
