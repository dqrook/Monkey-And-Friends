using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public class EndlessRabby : EndlessScroller
    {
        #region Public Variables
        public Renderer rabbyBody;
        public Collider rabbyCollider;
        #endregion

        #region Private Variables
        Animator anim;
        Vector3 startPosition;
        #endregion

        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            anim = GetComponent<Animator>();
            startPosition = transform.localPosition;
        }

        protected override void Start() {}

        void OnCollisionEnter(Collision other)
        {
            if(other.gameObject.GetComponent<EndlessController>())
            {
                Message.Send(new RunnerDie());
            }
        }
        #endregion

        #region Public Functions
        public void Initialize(Material mat = null)
        {
            if(mat != null && rabbyBody != null)
            {
                rabbyBody.material = mat;
            }
        }

        public void Reset()
        {
            anim.SetBool("dead", false);
            rabbyCollider.enabled = true;
            transform.localPosition = startPosition;
        }

        public void Die()
        {
            anim.SetBool("dead", true);
            rabbyCollider.enabled = false;
        }

        #endregion
    }
}
