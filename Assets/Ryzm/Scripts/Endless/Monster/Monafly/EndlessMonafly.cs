using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;
using Ryzm.Dragon;

namespace Ryzm.EndlessRunner
{
    public class EndlessMonafly : EndlessWaitingMonster
    {
        #region Public Variables
        public ParticlesContainer particlesContainer;
        #endregion

        #region Private Variables
        float forwardSpeed = 7;
        IEnumerator forwardAttack;
        IEnumerator stationarySpecial;
        float firePauseRate = 1;
        WaitForSeconds firePause;
        IEnumerator waitAndSend;
        #endregion

        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            move = Vector3.zero;
            firePause = new WaitForSeconds(firePauseRate);
        }
        #endregion

        #region Public Functions
        public override void Reset()
        {
            base.Reset();
            StopAllCoroutines();
            forwardAttack = null;
            SetIsAttacking(false);
            SetIsMoving(false);
            trans.localPosition = startPosition;
            if(type == MonsterType.SpecialMonafly)
            {
                particlesContainer.DisableParticles();
            }
        }

        public override void TakeDamage()
        {
            base.TakeDamage();
            SetIsAttacking(false);
            SetIsMoving(false);
            StopAllCoroutines();
            forwardAttack = null;
        }
        #endregion

        #region Protected Functions
        protected override void Attack()
        {
            if(!startedAttack)
            {
                startedAttack = true;
                if(type == MonsterType.PhysicalMonafly)
                {
                    forwardAttack = ForwardAttack();
                    StartCoroutine(forwardAttack);
                }
                else if(type == MonsterType.SpecialMonafly && particlesContainer != null)
                {
                    stationarySpecial = StationarySpecial();
                    StartCoroutine(stationarySpecial);
                }
            }
        }
        #endregion

        #region Private Functions
        void SetIsAttacking(bool isAttacking)
        {
            animator.SetBool("attack", isAttacking);
        }

        void SetIsMoving(bool isMoving)
        {
            animator.SetBool("move", isMoving);
        }

        void SetSpecial()
        {
            animator.SetTrigger("special");
        }
        #endregion

        #region Coroutines
        IEnumerator WaitAndSend()
        {
            yield return new WaitForSeconds(0.5f);
            Message.Send(new ControllerTransformRequest());
        }

        IEnumerator ForwardAttack()
        {
            Vector3 attackStartPosition = trans.position;
            float distanceTraveled = Mathf.Abs(trans.InverseTransformPoint(attackStartPosition).z);
            SetIsMoving(true);
            float zMove = Time.deltaTime * forwardSpeed;
            while(currentDistance > -10 && distanceTraveled < 25)
            {
                currentDistance = trans.InverseTransformPoint(currentDragonPosition).z;
                move.z = zMove;
                trans.Translate(move);
                distanceTraveled = Mathf.Abs(trans.InverseTransformPoint(attackStartPosition).z);
                yield return null;
            }
            SetIsMoving(false);
            SetIsAttacking(false);
        }

        IEnumerator StationarySpecial()
        {
            while(true)
            {
                float t = 0;
                bool hasParticle = particlesContainer.EnableParticle();
                while(t < particlesContainer.ExpansionTime)
                {
                    t += Time.deltaTime;
                    yield return null;
                }
                if(hasParticle)
                {
                    SetSpecial();
                }
                yield return firePause;
            }
        }
        #endregion
    }
}
