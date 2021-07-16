using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;
using Ryzm.Dragon;
using Ryzm.Dragon.Messages;
using Ryzm.Utils;
using UnityEngine.Networking;

namespace Ryzm.EndlessRunner
{
    public class EndlessDragon : EndlessController
    {
        public DragonMaterials materials;
        public Transform monkeyPos;
        public DragonFire fire;
        public DragonResponse data;
        public float flyUpSpeed = 10;
        public float flyDownSpeed = 5;
        public bool forceJump;

        [HideInInspector]
        public Vector3 monkeyOffset;

        IEnumerator flyToPosition;
        IEnumerator fireBreath;
        IEnumerator getDragonTexture;
        bool isAttacking;
        bool _isFlying;
        bool flyingInitialized;

        #region Properties
        public bool ForSale
        {
            get
            {
                if(data == null)
                {
                    return false;
                }
                return data.price > 0;
            }
        }

        public float Price
        {
            get
            {
                if(data == null)
                {
                    return 0;
                }
                return data.price;
            }
        }

        bool IsFlying
        {
            get
            {
                return _isFlying;
            }
            set
            {
                if(value != _isFlying || !flyingInitialized)
                {
                    _isFlying = value;
                    animator.SetBool("fly", value);
                    flyingInitialized = true;
                }
            }
        }
        #endregion

        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            monkeyOffset = monkeyPos.position - trans.position;
        }

        void Update()
        {
            if((mode == ControllerMode.MonkeyDragon || mode == ControllerMode.Dragon) && gameStatus == GameStatus.Active)
            {
                EndlessRun();
            }
            if(!InJump && forceJump)
            {
                IsFlying = true;
                Jump();
            }
            else
            {
                forceJump = false;
            }
        }
        #endregion

        #region Functions
        void EndlessRun()
        {
            IsFlying = true;
            if(!InJump)
            {
                Move(0);
            }

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

        void Move(float yMove)
        {
            float zMove = Time.deltaTime * forwardSpeed;
            move.z = zMove;
            move.y = yMove;
            move.x = shiftSpeed * zMove * 0.75f;
            trans.Translate(move);
            distanceTraveled += zMove;
            Message.Send(new RunnerDistanceResponse(distanceTraveled));
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

        public void GetTextures()
        {
            getDragonTexture = null;
            getDragonTexture = _GetTextures();
            StartCoroutine(getDragonTexture);
        }

        public void DisableMaterials()
        {
            materials.Disable();
        }

        public void EnableMaterials()
        {
            materials.Enable();
        }

        public override void Attack()
        {
            if(fire != null && !isAttacking)
            {
                fireBreath = FireBreath();
                StartCoroutine(fireBreath);
            }
        }

        public override void Jump()
        {
            if(!InJump && !inShift)
            {
                if(IsFlying)
                {
                    flyUp = FlyUp();
                    StartCoroutine(flyUp);
                }
                else if(IsGrounded())
                {
                    // todo: handle jumping when grounded
                }
            }
        }

        float elevation = 2;
        IEnumerator flyUp;

        IEnumerator FlyUp()
        {
            animator.SetTrigger("flyUp");
            InJump = true;
            float initY = trans.position.y;
            float currentY = trans.position.y;
            float absDiff = Mathf.Abs(currentY - initY);
            float multiplier = 2 * (elevation - absDiff);
            while(multiplier > 0.1f)
            {
                currentY = trans.position.y;
                absDiff = Mathf.Abs(currentY - initY);
                multiplier = 2 * (elevation - absDiff);
                multiplier = multiplier > 1 ? 1 : multiplier;
                Move(flyUpSpeed * multiplier * Time.deltaTime);
                yield return null;
            }
            animator.SetTrigger("flyDown");
            float timeMultiplier = 0.1f;
            while(absDiff > 0.25f)
            {
                timeMultiplier += Time.deltaTime;
                timeMultiplier = timeMultiplier < 1 ? timeMultiplier : 1;
                currentY = trans.position.y;
                absDiff = Mathf.Abs(currentY - initY);
                float downMultiplier = absDiff > 1 ? 1 : absDiff > 0.5f ? absDiff : 0.5f;
                Move(-flyDownSpeed * timeMultiplier * downMultiplier * Time.deltaTime);
                yield return null;
            }

            while(absDiff > 0.01f)
            {
                currentY = trans.position.y;
                absDiff = Mathf.Abs(currentY - initY);
                float downMultiplier = absDiff > 0.05f ? absDiff : 0.05f;
                Move(-flyDownSpeed * downMultiplier * Time.deltaTime);
                yield return null;
            }
            animator.SetTrigger("finishFly");
            trans.position = new Vector3(trans.position.x, initY, trans.position.z);
            InJump = false;
            yield break;
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
        #endregion

        #region Coroutines
        IEnumerator _GetTextures()
        {
            List<MaterialTypeToUrlMap> map = new List<MaterialTypeToUrlMap>
            {
                new MaterialTypeToUrlMap(DragonMaterialType.Body, data.bodyTexture),
                new MaterialTypeToUrlMap(DragonMaterialType.Wing, data.wingTexture),
                new MaterialTypeToUrlMap(DragonMaterialType.Horn, data.hornTexture),
                new MaterialTypeToUrlMap(DragonMaterialType.Back, data.backTexture)
            };
            
            int numMaterials = map.Count;
            int index = 0;
            while(index < numMaterials)
            {
                string url = map[index].url;
                DragonMaterialType type = map[index].type;
                UnityWebRequest request = RyzmUtils.TextureRequest(url);
                yield return request.SendWebRequest();
                if(request.isNetworkError || request.isHttpError)
                {
                    Debug.LogError("ERROR");
                    // todo: handle this case
                }
                else
                {
                    if(materials != null)
                    {
                        Texture _texture = DownloadHandlerTexture.GetContent(request);
                        SetTexture(type, _texture);
                    }

                }
                index++;
                yield return null;
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
        #endregion
    }
}
