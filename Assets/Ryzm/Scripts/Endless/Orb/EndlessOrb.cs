using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public class EndlessOrb : EndlessItem
    {

        #region Public Variables
        public ParticleSystem sparkle;
        public GameObject gem;
        #endregion

        #region Private Variables
        Animator animator;
        bool gemCollected;
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
            gemCollected = false;
            Reset();
        }

        protected void OnDisable()
        {
            Reset();
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
                Reset();
            }
        }
        #endregion

        #region Private Functions
        void OnTrigger(Collider other)
        {
            if(!gemCollected)
            {
                sparkle.Play();
                gemCollected = true;
                gem.SetActive(false);
                Message.Send(new CollectGem());
            }
        }
        #endregion

        #region Private Functions
        void Reset()
        {
            gemCollected = false;
            gem.SetActive(true);
            sparkle.Stop();
        }
        #endregion
    }
}