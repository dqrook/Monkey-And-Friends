using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessDragon : EndlessBarrier
    {
        public Animator animator;
        public Transform childTransform;
        public DragonFire fire;
        protected bool startedCoroutine;
        protected IEnumerator _flyToPosition;
        protected Vector3 initialPosition;
        protected Vector3 initialEulerAngles;
        protected GameObject childGO;

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
        }

        protected override void FixedUpdate()
        {
            if(!CanMove())
            {
                return;
            }
            MoveInY();

            if(!startedCoroutine)
            {
                childGO.SetActive(parentSection == _currentSection);
                if(parentSection != _currentSection)
                {
                    return;
                }
                else
                {
                    Transform location = parentSection.GetSpawnTransformForBarrierByPosition(type, runnerPosition);
                    if(location != null)
                    {
                        Vector3 init = gameObject.transform.position;
                        gameObject.transform.position = location.position;
                        gameObject.transform.rotation = location.rotation;
                    }
                    _flyToPosition = FlyToPosition();
                    StartCoroutine(_flyToPosition);
                }
            }
        }

        protected virtual IEnumerator FlyToPosition()
        {
            yield break;
        }

        protected override void OnDisable()
        {
            animator.SetBool("fireBreath", false);
            startedCoroutine = false;
            StopAllCoroutines();
            childTransform.localPosition = initialPosition;
            childTransform.localEulerAngles = initialEulerAngles;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}
