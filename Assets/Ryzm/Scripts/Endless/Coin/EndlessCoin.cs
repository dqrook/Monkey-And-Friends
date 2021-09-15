using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public class EndlessCoin : EndlessItem
    {
        #region Public Variables
        public ParticleSystem sparkle;
        public GameObject coin;
        #endregion

        #region Private Variables
        Animator animator;
        bool coinCollected;
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
            coinCollected = false;
            Reset();
        }

        protected void OnDisable()
        {
            Reset();
        }

        void OnTriggerEnter(Collider other)
        {
            if(!coinCollected)
            {
                sparkle.Play();
                coinCollected = true;
                coin.SetActive(false);
                Message.Send(new CollectCoin());
            }
        }
        #endregion

        #region Listener Functions
        protected override void OnGameStatusResponse(GameStatusResponse gameStatusResponse)
        {
            base.OnGameStatusResponse(gameStatusResponse);
            if(gameStatusResponse.status == GameStatus.Restart || gameStatusResponse.status == GameStatus.Exit)
            {
                Reset();
            }
        }
        #endregion

        #region Private Functions
        void Reset()
        {
            coinCollected = false;
            coin.SetActive(true);
            sparkle.Stop();
        }
        #endregion
    }
}
