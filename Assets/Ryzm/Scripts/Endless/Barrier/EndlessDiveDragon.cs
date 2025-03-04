﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.EndlessRunner
{
    public class EndlessDiveDragon : EndlessAIDragon
    {
        #region Protected Variables
        protected float dropSpeed = 10;
        #endregion

        #region Coroutines
        protected override IEnumerator FlyToPosition()
        {
            animator.SetBool("fly", true);
            startedCoroutine = true;
            bool openedMouth = false;
            bool startedFire = false;
            float diff = childTransform.localPosition.sqrMagnitude;
            float fireTime = 0;
            while(diff > 0.01f)
            {
                if(diff < 1)
                {
                    if(!openedMouth)
                    {
                        openedMouth = true;
                        animator.SetBool("fireBreath", true);
                        // animator.SetBool("fly", false);
                    }
                    fireTime += Time.deltaTime;
                    if(fireTime > 0.2f && !startedFire)
                    {
                        startedFire = true;
                        fire.Play();
                    }
                }
                childTransform.localPosition = Vector3.Lerp(childTransform.localPosition, Vector3.zero, Time.deltaTime * dropSpeed);
                childTransform.localEulerAngles = Vector3.Lerp(childTransform.localEulerAngles, Vector3.zero, Time.deltaTime * dropSpeed);
                diff = childTransform.localPosition.sqrMagnitude;
                yield return null;
            }
            childTransform.localPosition = Vector3.zero;
            childTransform.localEulerAngles = Vector3.zero;
            
            if(!openedMouth)
            {
                animator.SetBool("fireBreath", true);
                // animator.SetBool("fly", false);
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
            yield break;
        }
        #endregion
    }
}
