using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public abstract class EndlessMonster : EndlessScroller
    {
        #region Public Variables
        public Renderer bodyRenderer;
        public Collider bodyCollider;
        public List<Material> materials = new List<Material>();
        #endregion

        #region Protected Variables
        protected Animator anim;
        protected Vector3 startPosition;
        #endregion

        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            anim = GetComponent<Animator>();
            startPosition = transform.localPosition;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if(bodyRenderer != null)
            {
                int materialIndex = -1;
                if(materials.Count > 0)
                {
                    materialIndex = Random.Range(0, materials.Count);
                    bodyRenderer.material = materials[materialIndex];
                }
            }
        }

        protected override void Start() {}

        protected override void OnDisable()
        {
            base.OnDisable();
            Reset();
        }

        void OnCollisionEnter(Collision other)
        {
            if(other.gameObject.GetComponent<EndlessController>())
            {
                Message.Send(new RunnerDie());
            }
        }
        #endregion

        #region Public Functions
        public void Reset()
        {
            anim.SetBool("dead", false);
            EnableCollider(true);
            transform.localPosition = startPosition;
        }

        public void Die()
        {
            anim.SetBool("dead", true);
            EnableCollider(false);
        }
        #endregion

        #region Protected Functions
        protected virtual void EnableCollider(bool shouldEnable)
        {
            bodyCollider.enabled = shouldEnable;
        }
        #endregion
    }
}