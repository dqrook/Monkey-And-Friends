using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public class EndlessPsyken : EndlessHidingMonster
    {
        #region Private Variables
        float riseSpeed = 8;
        #endregion

        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            barrierType = BarrierType.Krake;
        }
        
        protected override void OnCollisionEnter(Collision other)
        {
            if(other.gameObject.GetComponent<EndlessController>())
            {
                Message.Send(new RunnerHit());
            }
        }
        #endregion

        #region Coroutines
        protected override IEnumerator MoveThenAttack()
        {
            startedCoroutine = true;
            bool attackInProgress = false;
            float diff = childTransform.localPosition.sqrMagnitude;
            while(!animator.GetBool("attack"))
            {
                animator.SetBool("attack", true);
                yield return null;
            }
            attackInProgress = true;
            while(diff > 0.01f)
            {
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
