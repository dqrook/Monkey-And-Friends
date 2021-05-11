using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessPathDragon : EndlessDragon
    {
        public Transform middlePosition;
        public Transform finalPosition;
        protected override IEnumerator FlyToPosition()
        {
            animator.SetBool("fly", true);
            startedCoroutine = true;
            bool openedMouth = false;
            bool startedFire = false;
            float diff = Vector3.Distance(childTransform.localPosition, middlePosition.localPosition);
            float fireTime = 0;
            while(diff > 0.01f)
            {
                if(diff < 1)
                {
                    if(!openedMouth)
                    {
                        openedMouth = true;
                        // animator.SetBool("fly", false);
                        animator.SetBool("fireBreath", true);
                    }
                    fireTime += Time.deltaTime;
                    if(fireTime > 0.2f && !startedFire)
                    {
                        startedFire = true;
                        fire.Play();
                    }
                }
                childTransform.localPosition = Vector3.Lerp(childTransform.localPosition, middlePosition.localPosition, Time.deltaTime * 12);
                childTransform.localEulerAngles = Vector3.Lerp(childTransform.localEulerAngles, middlePosition.localEulerAngles, Time.deltaTime * 12);
                diff = Vector3.Distance(childTransform.localPosition, middlePosition.localPosition);
                yield return null;
            }
            childTransform.localPosition = middlePosition.localPosition;
            childTransform.localEulerAngles = middlePosition.localEulerAngles;
            
            if(!openedMouth)
            {
                openedMouth = true;
                // animator.SetBool("fly", false);
                animator.SetBool("fireBreath", true);
            }

            diff = Vector3.Distance(childTransform.localPosition, finalPosition.localPosition);
            while(diff > 0.01f)
            {
                if(!startedFire)
                {
                    fireTime += Time.deltaTime;
                    if(fireTime > 0.2f)
                    {
                        fire.Play();
                        startedFire = true;
                    }
                }

                childTransform.localPosition = Vector3.Lerp(childTransform.localPosition, finalPosition.localPosition, Time.deltaTime * 3);
                childTransform.localEulerAngles = Vector3.Lerp(childTransform.localEulerAngles, finalPosition.localEulerAngles, Time.deltaTime * 3);
                diff = Vector3.Distance(childTransform.localPosition, finalPosition.localPosition);
                yield return null;
            }
            childTransform.localPosition = finalPosition.localPosition;
            childTransform.localEulerAngles = finalPosition.localEulerAngles;
            yield break;
        }
    }
}
