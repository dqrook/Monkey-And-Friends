using System.Collections;
using System.Collections.Generic;
using Ryzm.EndlessRunner.Messages;
using UnityEngine;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class EndlessDiveDraze : EndlessHidingMonster
    {
        #region Public Variables
        public DragonFire fire;
        #endregion

        #region Protected Variables
        protected float dropSpeed = 5;
        protected bool hasSpecialHit;
        protected float collisionTime;
        #endregion

        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            barrierType = BarrierType.DiveDragon;
            attackDistance = 40;
        }

        protected override void OnTriggerEnter(Collider other)
        {
            CheckCollision(other);
        }

        protected override void OnTriggerStay(Collider other)
        {
            CheckCollision(other);
        }
        #endregion

        #region Listener Functions
        protected override void OnMonsterMetadataResponse(MonsterMetadataResponse response)
        {
            if(!gotMetadata)
            {
                base.OnMonsterMetadataResponse(response);
                fire.monsterMetadata = monsterMetadata;
            }
        }
        #endregion

        #region Public Functions
        public override void Reset()
        {
            base.Reset();
            hasSpecialHit = false;
            collisionTime = 0;
        }
        #endregion

        #region Private Functions
        void CheckCollision(Collider other)
        {
            if(!hasSpecialHit && other.GetComponent<EndlessController>())
            {
                collisionTime += Time.deltaTime;
                if(collisionTime > 3 * Time.deltaTime)
                {
                    hasSpecialHit = true;
                    Message.Send(new RunnerHit(monsterMetadata.monsterType, AttackType.Special));
                }
            }
        }
        #endregion

        #region Coroutines
        protected override IEnumerator MoveThenAttack()
        {
            animator.SetBool("fly", true);
            startedCoroutine = true;
            bool openedMouth = false;
            bool startedFire = false;
            float diff = childTransform.localPosition.sqrMagnitude;
            Vector3 targetEuler = new Vector3(trans.eulerAngles.x, childTransform.eulerAngles.y, trans.eulerAngles.y);
            float fireTime = 0;
            while(diff > 0.01f)
            {
                if(diff < 1)
                {
                    if(!openedMouth)
                    {
                        openedMouth = true;
                        animator.SetBool("fireBreath", true);
                    }
                    fireTime += Time.deltaTime;
                    if(fireTime > 0.2f && !startedFire)
                    {
                        startedFire = true;
                        fire.Play();
                    }
                }
                Vector3 worldPos = Vector3.Lerp(childTransform.position, trans.position, Time.deltaTime * dropSpeed);
                Quaternion worldQ = Quaternion.Euler(Vector3.Lerp(childTransform.eulerAngles, targetEuler, Time.deltaTime * dropSpeed));
                
                childTransform.SetPositionAndRotation(worldPos, worldQ);
                // childTransform.localPosition = Vector3.Lerp(childTransform.localPosition, Vector3.zero, Time.deltaTime * dropSpeed);
                // childTransform.localEulerAngles = Vector3.Lerp(childTransform.localEulerAngles, targetEuler, Time.deltaTime * dropSpeed);
                diff = childTransform.localPosition.sqrMagnitude;
                yield return null;
            }
            childTransform.localPosition = Vector3.zero;
            childTransform.localEulerAngles = targetEuler;
            
            if(!openedMouth)
            {
                animator.SetBool("fireBreath", true);
            }

            if(!startedFire)
            {
                while(fireTime < 0.2f)
                {
                    fireTime += Time.deltaTime;
                    yield return null;
                }
                fire.Play();
            }
        }
        #endregion
    }
}
