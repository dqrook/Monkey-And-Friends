using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;
using AtmosphericHeightFog;

namespace Ryzm.EndlessRunner
{
    public class TestMove : MonoBehaviour
    {
        public Transform mainCam;
        public Transform camPos;
        public HeightFogGlobal fog;
        public Transform fogBottom;
        public Transform target;
        public bool reset;
        public Transform finalTarget;
        public float dropSpeed = 18;

        EndlessDragon dragon;
        Transform dragonTrans;
        IEnumerator moveDragon;
        IEnumerator diveAndFadeOutFog;
        Vector3 move;
        Vector3 initialPos;
        Vector3 initialEul;
        bool initialized;
        float forwardSpeed;

        void Awake()
        {
            Message.AddListener<ControllersResponse>(OnControllersResponse);
        }

        void Start()
        {
            Message.Send(new ControllersRequest());
        }

        void Update()
        {
            if(reset)
            {
                reset = false;
                StopAllCoroutines();
                if(moveDragon != null)
                {
                    moveDragon = null;
                }
                if(diveAndFadeOutFog != null)
                {
                    diveAndFadeOutFog = null;
                }
                fog.fogIntensity = 1;
                dragonTrans.position = initialPos;
                dragonTrans.eulerAngles = initialEul;
                moveDragon = MoveDragon();
                StartCoroutine(moveDragon);
            }
        }

        void OnDestroy()
        {
            Message.RemoveListener<ControllersResponse>(OnControllersResponse);
        }

        void OnControllersResponse(ControllersResponse response)
        {
            dragon = response.dragon;
            forwardSpeed = dragon.forwardSpeed;
            dragonTrans = dragon.transform;
            initialPos = dragonTrans.position;
            initialEul = dragonTrans.eulerAngles;
            if(dragonTrans != null && moveDragon == null)
            {
                moveDragon = MoveDragon();
                StartCoroutine(moveDragon);
            }
        }

        IEnumerator MoveDragon()
        {
            yield return new WaitForSeconds(2);
            if(!initialized)
            {
                fog.gameObject.SetActive(true);
                mainCam.position = camPos.position;
                mainCam.rotation = camPos.rotation;
                mainCam.parent = dragonTrans;
                initialized = true;
            }
            yield return new WaitForSeconds(1);
            dragon.animator.SetBool("fly", true);
            float angleDiff = Mathf.Abs(dragonTrans.eulerAngles.x - target.eulerAngles.x);
            float angleSign = Mathf.Sign(dragonTrans.eulerAngles.x - target.eulerAngles.x);
            float initAngleSign = angleSign;
            bool flyingDown = false;
            while(angleDiff > 0.5f || angleSign != initAngleSign)
            {
                float zMove = Time.deltaTime * dropSpeed;
                move.z = zMove;
                move.y = 0;
                move.x = 0;
                dragonTrans.Translate(move);
                Vector3 l = Vector3.Lerp(dragonTrans.eulerAngles, target.eulerAngles, Time.deltaTime * 8);
                Vector3 rot = l - dragonTrans.eulerAngles;
                rot.y = 0;
                rot.z = 0;
                dragonTrans.Rotate(rot);
                angleSign = Mathf.Sign(dragonTrans.eulerAngles.x - target.eulerAngles.x);
                angleDiff = Mathf.Abs(dragonTrans.eulerAngles.x - target.eulerAngles.x);
                if(angleDiff < 60 && !flyingDown)
                {
                    dragon.animator.SetBool("flyDown", true);
                    flyingDown = true;
                }
                if(angleDiff < 30)
                {
                    fog.fogIntensity = 1 - (30 - angleDiff) / 120;
                }
                yield return null;
            }
            
            float yDiff = dragonTrans.position.y - fogBottom.position.y;
            Debug.Log("yDiff: " + yDiff);
            float minY = 15;
            if(yDiff < minY)
            {
                float dropDistance = minY - yDiff;
                Vector3 pos = fog.transform.position;
                pos.y -= dropDistance;
                fog.transform.position = pos;
            }
            float initialYDiff = yDiff;
            float initialIntensity = fog.fogIntensity;
            float fracDiff = 1 - (initialYDiff - yDiff) / initialYDiff;
            while(fracDiff > 0.05f)
            {
                float zMove = Time.deltaTime * dropSpeed;
                move.z = zMove;
                move.y = 0;
                move.x = 0;
                dragonTrans.Translate(move);
                if(yDiff < minY)
                {
                    fog.fogIntensity = initialIntensity * Mathf.Abs(yDiff / initialYDiff);
                }
                yDiff = dragonTrans.position.y - fogBottom.position.y;
                fracDiff = 1 - (initialYDiff - yDiff) / initialYDiff;
                yield return null;
            }

            float initialXAng = dragonTrans.eulerAngles.x;
            float finXAng = finalTarget.eulerAngles.x;
            angleDiff = Mathf.Abs(dragonTrans.eulerAngles.x - finalTarget.eulerAngles.x);
            angleSign = Mathf.Sign(dragonTrans.eulerAngles.x - finalTarget.eulerAngles.x);
            initAngleSign = angleSign;
            yDiff = dragonTrans.position.y - finalTarget.position.y;
            initialYDiff = yDiff;
            float speedTransitionTime = 1;
            float speedTime = 0;
            while(angleDiff > 0.1f || angleSign != initAngleSign)
            {
                speedTime += Time.deltaTime;
                speedTime = speedTime > speedTransitionTime ? 1 : speedTime / speedTransitionTime;
                float curSpeed = (1 - speedTime) * dropSpeed + speedTime * forwardSpeed;
                Vector3 eul = dragonTrans.eulerAngles;
                float frac = (initialYDiff - yDiff) / initialYDiff;
                eul.x = finXAng * frac + initialXAng * (1 - frac);
                dragonTrans.eulerAngles = eul;
                float zMove = Time.deltaTime * curSpeed;
                move.z = zMove;
                move.y = 0;
                move.x = 0;
                dragonTrans.Translate(move);
                angleSign = Mathf.Sign(dragonTrans.eulerAngles.x - finalTarget.eulerAngles.x);
                angleDiff = Mathf.Abs(dragonTrans.eulerAngles.x - finalTarget.eulerAngles.x);
                if(angleDiff < 15 && flyingDown)
                {
                    dragon.animator.SetBool("flyDown", false);
                    flyingDown = false;
                }
                yDiff = dragonTrans.position.y - finalTarget.position.y;
                yield return null;
            }
            Vector3 dragEul = dragonTrans.eulerAngles;
            dragEul.x = finalTarget.eulerAngles.x;
            dragonTrans.eulerAngles = dragEul;
            Vector3 dragPos = dragonTrans.position;
            dragPos.y = finalTarget.position.y;
            dragonTrans.position = dragPos;
        }
    }
}
