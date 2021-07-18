using System.Collections;
using System.Collections.Generic;
using Ryzm.EndlessRunner.Messages;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessAIDragon : EndlessBarrier
    {
        #region Public Variables
        public Animator animator;
        public Transform childTransform;
        public DragonFire fire;
        #endregion
        
        #region Protected Variables
        protected bool startedCoroutine;
        protected IEnumerator _flyToPosition;
        protected Vector3 initialPosition;
        protected Vector3 initialEulerAngles;
        protected GameObject childGO;
        #endregion

        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            if(childTransform == null)
            {
                childTransform = GetComponentInChildren<Transform>();
            }
            childGO = childTransform.gameObject;
            initialPosition = childTransform.localPosition;
            initialEulerAngles = childTransform.localEulerAngles;
            if(animator == null)
            {
                animator = GetComponent<Animator>();
            }
            childGO.SetActive(false);
        }

        protected virtual void FixedUpdate()
        {
            if(!startedCoroutine)
            {
                if(CanMove() && parentSection == _currentSection)
                {
                    Transform location = parentSection.GetSpawnTransformForBarrier(type, runnerPosition);
                    if(location != null)
                    {
                        gameObject.transform.position = location.position;
                        gameObject.transform.rotation = location.rotation;
                    }
                    _flyToPosition = FlyToPosition();
                    StartCoroutine(_flyToPosition);
                }
            }
        }

        protected override void OnDisable()
        {
            animator.SetBool("fireBreath", false);
            startedCoroutine = false;
            StopAllCoroutines();
            childTransform.localPosition = initialPosition;
            childTransform.localEulerAngles = initialEulerAngles;
            childGO.SetActive(false);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
        #endregion

        protected override void OnCurrentSectionChange(CurrentSectionChange sectionChange)
        {
            base.OnCurrentSectionChange(sectionChange);
            childGO.SetActive(parentSection == _currentSection);
        }


        #region Coroutines
        protected virtual IEnumerator FlyToPosition()
        {
            yield break;
        }
        #endregion
    }
}
