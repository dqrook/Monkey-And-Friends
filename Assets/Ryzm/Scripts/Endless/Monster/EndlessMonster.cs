using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public abstract class EndlessMonster : MonsterBase
    {
        #region Public Variables
        public MonsterType type;
        public List<Renderer> renderers = new List<Renderer>();
        public List<Collider> colliders = new List<Collider>();
        public List<Material> materials = new List<Material>();
        #endregion

        #region Protected Variables
        protected Animator animator;
        protected Vector3 startPosition;
        protected Transform trans;
        protected bool _isActive;
        protected GameStatus gameStatus;
        protected Material currentMaterial;
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
        protected virtual void Awake()
        {
            Message.AddListener<GameStatusResponse>(OnGameStatusResponse);
            animator = GetComponent<Animator>();
            trans = transform;
            startPosition = trans.localPosition;
        }

        protected virtual void OnEnable()
        {
            Message.Send(new GameStatusRequest());
            if(materials.Count > 0)
            {
                int materialIndex = Random.Range(0, materials.Count);
                foreach(Renderer ren in renderers)
                {
                    ren.material = materials[materialIndex];
                }
            }
        }

        protected virtual void OnDisable()
        {
            Reset();
        }

        protected virtual void OnDestroy()
        {
            Message.RemoveListener<GameStatusResponse>(OnGameStatusResponse);
        }

        protected virtual void OnCollisionEnter(Collision other)
        {
            // if(LayerMask.LayerToName(other.GetContact(0).otherCollider.gameObject.layer) == "PlayerBody")
            // {
            //     Message.Send(new RunnerDie());
            // }
            OnCollide(other.GetContact(0).otherCollider.gameObject);
        }

        // protected virtual void OnTriggerEnter(Collider other)
        // {
        //     // if(LayerMask.LayerToName(other.gameObject.layer) == "PlayerBody")
        //     // {
        //     //     Message.Send(new RunnerDie());
        //     // }
        //     OnCollide(other.gameObject);
        // }

        protected virtual void OnGameStatusResponse(GameStatusResponse gameStatusResponse)
        {
            gameStatus = gameStatusResponse.status;
        }
        #endregion

        #region Public Functions
        public virtual void Reset()
        {
            animator.SetBool("dead", false);
            EnableCollider(true);
            trans.localPosition = startPosition;
        }

        public override void TakeDamage()
        {
            animator.SetBool("dead", true);
            EnableCollider(false);
        }
        #endregion

        #region Protected Functions
        protected virtual void EnableCollider(bool shouldEnable)
        {
            foreach(Collider col in colliders)
            {
                col.enabled = shouldEnable;
            }
        }

        protected virtual void OnCollide(GameObject other)
        {
            if(LayerMask.LayerToName(other.layer) == "PlayerBody")
            {
                Message.Send(new RunnerDie());
            }
        }
        #endregion
    }

    public enum MonsterType
    {
        SpecialMonafly,
        Tregon,
        Rabby,
        Bombee,
        Fawks,
        Krake,
        Draze,
        DiveDraze,
        SideDraze,
        PhysicalMonafly,
        None,
        Reyflora
    }
}