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
        bool startedCoroutine;
        IEnumerator _flyToPosition;
        Vector3 initialPosition;
        Vector3 initialEulerAngles;

        protected override void Awake()
        {
            base.Awake();
            if(childTransform == null)
            {
                childTransform = GetComponentInChildren<Transform>();
            }
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
            GetRunner();
            float dragonSpeed = GameManager.Instance.speed;
            MoveForward(dragonSpeed);
            MoveInY();
            if(!startedCoroutine)
            {
                _flyToPosition = FlyToPosition();
                StartCoroutine(_flyToPosition);
            }
        }

        IEnumerator FlyToPosition()
        {
            startedCoroutine = true;
            float diff = childTransform.localPosition.sqrMagnitude;
            while(diff > 0.01f)
            {
                childTransform.localPosition = Vector3.Lerp(childTransform.localPosition, Vector3.zero, Time.deltaTime * 8);
                childTransform.localEulerAngles = Vector3.Lerp(childTransform.localEulerAngles, Vector3.zero, Time.deltaTime * 8);
                diff = childTransform.localPosition.sqrMagnitude;
                yield return null;
            }
            childTransform.localPosition = Vector3.zero;
            childTransform.localEulerAngles = Vector3.zero;
            animator.SetBool("fireBreath", true);
            yield return new WaitForSeconds(0.2f);
            fire.Play();
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
