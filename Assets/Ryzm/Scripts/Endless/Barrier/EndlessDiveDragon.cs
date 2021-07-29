using System.Collections;
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

        #region Private Variables
        WaitForSeconds wait2Seconds = new WaitForSeconds(2);
        IEnumerator waitThenDisable;
        Transform spawnTrans;
        Vector3 spawnOrigPos;
        #endregion

        #region Listener Functions
        protected override void OnCurrentSectionChange(CurrentSectionChange sectionChange)
        {
            _currentSection = sectionChange.endlessSection;
            if(startedCoroutine)
            {
                if(parentSection != _currentSection)
                {
                    // just left the dragon's parent section therefore need to disable the child object
                    waitThenDisable = null;
                    waitThenDisable = WaitThenDisable();
                    StartCoroutine(waitThenDisable);
                }
            }
            else
            {
                childGO.SetActive(parentSection == _currentSection);
            }
        }
        #endregion

        #region Protected Functions
        public override void Initialize(Transform parentTransform, int position)
        {
            if(CanPlaceRow(rowLikelihood))
            {
                foreach(RowSpawn spawn in spawns)
                {
                    if(spawn.position == position)
                    {
                        GameObject spawnGO = spawn.EnableRandomRow();
                        if(spawnGO != null)
                        {
                            spawnTrans = spawnGO.transform;
                            spawnOrigPos = spawnTrans.localPosition;
                            spawnTrans.parent = null;
                        }
                        break;
                    }
                }
            }
            else
            {
                foreach(RowSpawn spawn in spawns)
                {
                    spawn.Disable();
                }
            }
        }
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
        IEnumerator WaitThenDisable()
        {
            yield return wait2Seconds;
            if(spawnTrans != null)
            {
                spawnTrans.parent = this.transform;
                spawnTrans.localPosition = spawnOrigPos;
                spawnTrans = null;
            }
            foreach(RowSpawn spawn in spawns)
            {
                spawn.Disable();
            }
            childGO.SetActive(parentSection == _currentSection);
        }
        #endregion
    }
}
