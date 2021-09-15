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
        public List<Collider> colliders = new List<Collider>();
        
        [Header("Style")]
        public List<Renderer> renderers = new List<Renderer>();
        public List<Material> materials = new List<Material>();
        public int targetMaterialIndex;
        #endregion

        #region Protected Variables
        protected MonsterMetadata monsterMetadata;
        protected Animator animator;
        protected Vector3 startPosition;
        protected Transform trans;
        protected bool _isActive;
        protected GameStatus gameStatus;
        protected bool gotMetadata;
        protected bool hasHit;
        protected int deadAnimHash;
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
            Message.AddListener<MonsterMetadataResponse>(OnMonsterMetadataResponse);
            Message.Send(new MonsterMetadataRequest());
            animator = GetComponent<Animator>();
            trans = transform;
            startPosition = trans.localPosition;
            deadAnimHash = Animator.StringToHash("dead");
        }

        protected virtual void OnEnable()
        {
            Message.Send(new GameStatusRequest());
            if(materials.Count > 0)
            {
                int materialIndex = Random.Range(0, materials.Count);
                foreach(Renderer ren in renderers)
                {
                    if(targetMaterialIndex == 0)
                    {
                        ren.sharedMaterial = materials[materialIndex];
                    }
                    else
                    {
                        ren.sharedMaterials[targetMaterialIndex] = materials[materialIndex];
                    }
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
            Message.RemoveListener<MonsterMetadataResponse>(OnMonsterMetadataResponse);
        }

        protected virtual void OnTriggerEnter(Collider other) {}
        protected virtual void OnTriggerStay(Collider other) {}

        protected virtual void OnCollisionEnter(Collision other)
        {
            OnCollide(other.GetContact(0).otherCollider.gameObject);
        }

        protected virtual void OnCollisionStay(Collision other)
        {
            OnCollide(other.GetContact(0).otherCollider.gameObject);
        }

        #endregion

        #region Listener Functions
        protected virtual void OnGameStatusResponse(GameStatusResponse gameStatusResponse)
        {
            gameStatus = gameStatusResponse.status;
        }

        protected virtual void OnMonsterMetadataResponse(MonsterMetadataResponse response)
        {
            if(!gotMetadata)
            {
                monsterMetadata = response.monsterMetadata.GetMonsterMetadata(type);
                gotMetadata = true;
            }
        }
        #endregion

        #region Public Functions
        public virtual void Reset()
        {
            animator.SetBool(deadAnimHash, false);
            EnableCollider(true);
            trans.localPosition = startPosition;
            hasHit = false;
        }

        public override void TakeDamage()
        {
            Die();
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
            if(!hasHit && LayerMask.LayerToName(other.layer) == "PlayerBody")
            {
                hasHit = true;
                Message.Send(new RunnerHit(monsterMetadata.monsterType, AttackType.Physical));
            }
        }

        protected virtual void Die()
        {
            animator.SetBool(deadAnimHash, true);
            EnableCollider(false);
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
        Psyken,
        Draze,
        DiveDraze,
        SideDraze,
        PhysicalMonafly,
        None,
        Reyflora,
        StonePillar,
        StoneRow,
        Deyon,
        Azel,
        MovingFawks,
        Pegasus
    }
}