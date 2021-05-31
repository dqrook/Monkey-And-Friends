using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessPathDragon : EndlessAIDragon
    {
        public Transform middlePosition;
        public Transform finalPosition;
        public GameObject instantDeath;

        void StartFire()
        {
            ToggleInstantDeath(true);
            fire.Play();
        }

        void ToggleInstantDeath(bool active)
        {
            if(instantDeath != null)
            {
                instantDeath.SetActive(active);
            }
        }

        protected override IEnumerator FlyToPosition()
        {
            animator.SetBool("fly", true);
            startedCoroutine = true;
            bool openedMouth = false;
            bool startedFire = false;
            float diff = Vector3.Distance(childTransform.localPosition, middlePosition.localPosition);
            float fireTime = 0;
            while(diff > 0.1f)
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
                        StartFire();
                    }
                }
                childTransform.localPosition = Vector3.Lerp(childTransform.localPosition, middlePosition.localPosition, Time.deltaTime * 11);
                childTransform.localEulerAngles = Vector3.Lerp(childTransform.localEulerAngles, middlePosition.localEulerAngles, Time.deltaTime * 11);
                diff = Vector3.Distance(childTransform.localPosition, middlePosition.localPosition);
                yield return null;
            }
            
            if(!openedMouth)
            {
                openedMouth = true;
                animator.SetBool("fireBreath", true);
            }

            diff = Vector3.Distance(childTransform.localPosition, finalPosition.localPosition);
            while(diff > 0.01f && !startedFire)
            {
                if(!startedFire)
                {
                    fireTime += Time.deltaTime;
                    if(fireTime > 0.2f)
                    {
                        startedFire = true;
                        StartFire();
                    }
                }

                childTransform.localPosition = Vector3.Lerp(childTransform.localPosition, finalPosition.localPosition, Time.deltaTime * 4f);
                childTransform.localEulerAngles = Vector3.Lerp(childTransform.localEulerAngles, finalPosition.localEulerAngles, Time.deltaTime * 4f);
                diff = Vector3.Distance(childTransform.localPosition, finalPosition.localPosition);
                yield return null;
            }
            childTransform.localPosition = finalPosition.localPosition;
            childTransform.localEulerAngles = finalPosition.localEulerAngles;
            yield break;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            ToggleInstantDeath(false);
        }
    }
}
