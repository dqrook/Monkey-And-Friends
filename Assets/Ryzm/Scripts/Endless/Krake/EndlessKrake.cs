using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class EndlessKrake : EndlessDiveDragon
    {
        #region Public Variables
        public bool simulate;

        [Header("Style")]
        public Renderer krakeBody;
        public List<Material> krakeMaterials = new List<Material>();
        #endregion

        #region Private Variables
        float riseSpeed = 8;
        Vector3 move;
        #endregion

        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            move = Vector3.zero;
        }

        // void Update()
        // {
        //     if(simulate)
        //     {
        //         Debug.Log("simulate it");
        //         simulate = false;
        //         Reset();
        //         childGO.SetActive(true);
        //         _flyToPosition = FlyToPosition();
        //         StartCoroutine(_flyToPosition);
        //     }
        // }

        protected override void OnDisable()
        {
            Reset();
        }

        protected override void OnCollisionEnter(Collision other)
        {
            if(other.gameObject.GetComponent<EndlessController>())
            {
                Message.Send(new RunnerDie());
            }
        }
        #endregion

        public override void Initialize(Transform parentTransform, int position)
        {
            base.Initialize(parentTransform, position);
            if(krakeBody != null)
            {
                int materialIndex = -1;
                if(krakeMaterials.Count > 0)
                {
                    materialIndex = Random.Range(0, krakeMaterials.Count);
                    krakeBody.material = krakeMaterials[materialIndex];
                }
            }
        }

        #region Private Functions
        void Reset()
        {
            // animator.SetBool("flyUp", false);
            animator.SetBool("attack", false);
            // animator.SetBool("resetAttack", false);
            startedCoroutine = false;
            StopAllCoroutines();
            childTransform.localPosition = initialPosition;
            childTransform.localEulerAngles = initialEulerAngles;
            childGO.SetActive(false);
            move = Vector3.zero;
            ResetSpawns();
        }
        #endregion

        #region Coroutines
        protected override IEnumerator FlyToPosition()
        {
            // animator.SetBool("attack", true);
            // animator.SetBool("resetAttack", false);
            // animator.SetBool("flyUp", true);
            // animator.SetTrigger("fly");
            startedCoroutine = true;
            bool attackInProgress = false;
            float diff = childTransform.localPosition.sqrMagnitude;
            // Debug.Log("starting rize " + animator.GetBool("flyUp"));
            // Debug.Log("diff " + diff);
            while(!animator.GetBool("attack"))
            {
                animator.SetBool("attack", true);
                yield return null;
            }
            attackInProgress = true;
            while(diff > 0.01f)
            {
                // if(diff < differenceStart && !attackInProgress)
                // {
                //     // animator.SetBool("flyUp", false);
                //     animator.SetBool("attack", true);
                //     attackInProgress = true;
                // }
                childTransform.localPosition = Vector3.Lerp(childTransform.localPosition, Vector3.zero, Time.deltaTime * riseSpeed);
                childTransform.localEulerAngles = Vector3.Lerp(childTransform.localEulerAngles, Vector3.zero, Time.deltaTime * riseSpeed);
                diff = childTransform.localPosition.sqrMagnitude;
                yield return null;
            }
            childTransform.localPosition = Vector3.zero;
            childTransform.localEulerAngles = Vector3.zero;
            
            // simulate gravity 
            diff = Vector3.Distance(childTransform.localPosition, initialPosition);
            float ySpeed = 0;
            float gravModifier = 0.5f;
            while(diff > 0.1f)
            {
                if(diff < 0.3f && attackInProgress)
                {
                    attackInProgress = false;
                    animator.SetBool("attack", false);
                }
                gravModifier = Mathf.Lerp(gravModifier, 1, Time.deltaTime);
                ySpeed -= Time.deltaTime * 9.81f * gravModifier;
                move.y = ySpeed * Time.deltaTime;
                childTransform.Translate(move);
                diff = Vector3.Distance(childTransform.localPosition, initialPosition);
                yield return null;
            }
            animator.SetBool("attack", false);
        }
        #endregion
    }
}
