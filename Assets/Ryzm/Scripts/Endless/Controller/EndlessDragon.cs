using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;
using Ryzm.Dragon;
using Ryzm.Dragon.Messages;

namespace Ryzm.EndlessRunner
{
    public class EndlessDragon : EndlessController
    {
        public DragonMaterials materials;
        public Transform monkeyPos;
        public DragonFire fire;
        public DragonResponse data;

        [HideInInspector]
        public Vector3 monkeyOffset;

        IEnumerator flyToPosition;
        IEnumerator fireBreath;
        bool isAttacking;

        protected override void Awake()
        {
            base.Awake();
            monkeyOffset = monkeyPos.position - trans.position;
        }

        public void SetTexture(DragonMaterialType type, Texture texture)
        {
            if(materials != null)
            {
                materials.SetTexture(type, texture);
                if(materials.Initialized)
                {
                    Debug.Log("dragon is initialized " + data.id);
                    Message.Send(new DragonInitialized(data.id));
                }
            }
        }

        void Update()
        {
            if(mode == ControllerMode.Dragon && gameStatus == GameStatus.Active)
            {
                EndlessRun();
            }
        }

        void EndlessRun()
        {
            animator.SetBool("fly", true);
            float zMove = Time.deltaTime * forwardSpeed;
            move.z = zMove;
            move.y = 0;
            move.x = shiftSpeed * zMove * 0.75f;
            trans.Translate(move);
            distanceTraveled += zMove;
            Message.Send(new RunnerDistanceResponse(distanceTraveled));

            if(IsShifting(Direction.Left))
            {
                Shift(Direction.Left);
            }
            else if(IsShifting(Direction.Right))
            {
                Shift(Direction.Right);
            }
            if(IsAttacking())
            {
                Attack();
            }
        }

        public void FlyToPosition(Transform t)
        {
            animator.SetBool("fly", true);
            flyToPosition = _FlyToPosition(t, forwardSpeed * 2.5f);
            StartCoroutine(flyToPosition);
        }

        public void FlyToPosition(Transform t, float speed)
        {
            animator.SetBool("fly", true);
            flyToPosition = _FlyToPosition(t, speed);
            StartCoroutine(flyToPosition);
        }

        public override void Attack()
        {
            if(fire != null && !isAttacking)
            {
                fireBreath = FireBreath();
                StartCoroutine(fireBreath);
            }
        }

        IEnumerator FireBreath()
        {
            isAttacking = true;
            animator.SetBool("fireBreath", true);
            fire.Play();
            float fbTime = 2f;
            float time = 0;
            while(time < fbTime)
            {
                time += Time.deltaTime;
                yield return null;
            }
            isAttacking = false;
            animator.SetBool("fireBreath", false);
            fire.Stop();
            yield break;
        }

        IEnumerator _FlyToPosition(Transform target, float speed)
        {
            float distance = Vector3.Distance(trans.position, target.position);
            while(distance > 0.1f)
            {
                distance = Vector3.Distance(trans.position, target.position);
                move.z = Time.deltaTime * speed;
                move.y = 0;
                move.x = 0;
                trans.Translate(move);
                yield return null;
            }
            trans.position = target.position;
            trans.rotation = target.rotation;
            yield break;
        }
    }
}
