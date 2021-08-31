using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public abstract class EndlessMonster : EndlessItem
    {
        #region Public Variables
        public MonsterType type;
        public Renderer bodyRenderer;
        public List<Renderer> renderers = new List<Renderer>();
        public Collider bodyCollider;
        public List<Collider> colliders = new List<Collider>();
        public List<Material> materials = new List<Material>();
        #endregion

        #region Protected Variables
        protected Animator animator;
        protected Vector3 startPosition;
        protected Transform trans;
        protected bool _isActive;
        #endregion

        public bool IsActive
        {
            get
            {
                return _isActive;
            }
            set
            {
                animator.enabled = value;
                EnableCollider(value);
            }
        }

        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            animator = GetComponent<Animator>();
            trans = transform;
            startPosition = trans.localPosition;
        }

        protected virtual void OnEnable()
        {
            if(materials.Count > 0)
            {
                int materialIndex = Random.Range(0, materials.Count);
                if(bodyRenderer != null)
                {
                    bodyRenderer.material = materials[materialIndex];
                }
                foreach(Renderer ren in renderers)
                {
                    ren.material = materials[materialIndex];
                }
            }
        }

        protected override void Start() {}

        protected virtual void OnDisable()
        {
            Reset();
        }

        protected virtual void OnCollisionEnter(Collision other)
        {
            if(LayerMask.LayerToName(other.GetContact(0).otherCollider.gameObject.layer) == "PlayerBody")
            {
                Message.Send(new RunnerDie());
            }
        }

        protected override void OnGameStatusResponse(GameStatusResponse gameStatusResponse)
        {
            gameStatus = gameStatusResponse.status;
        }
        #endregion

        #region Public Functions
        public virtual void Reset()
        {
            animator.SetBool("dead", false);
            EnableCollider(true);
            transform.localPosition = startPosition;
        }

        public virtual void TakeDamage()
        {
            animator.SetBool("dead", true);
            EnableCollider(false);
        }
        #endregion

        #region Protected Functions
        protected virtual void EnableCollider(bool shouldEnable)
        {
            if(bodyCollider != null)
            {
                bodyCollider.enabled = shouldEnable;
            }
            foreach(Collider col in colliders)
            {
                col.enabled = shouldEnable;
            }
        }
        #endregion
    }

    public enum MonsterType
    {
        Monafly,
        Tregon,
        Rabby,
        Bombee,
        Fawks,
        Krake,
        Draze,
        DiveDraze,
        SideDraze
    }
}