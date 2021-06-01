using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class EndlessCameraManager : MonoBehaviour
    {
        public EndlessCamera endlessCamera;
        public Transform startTransform;
        public Transform endTransform;
        GameStatus gameStatus;
        IEnumerator rotateCamera;
        Transform cameraTrans;

        void Awake()
        {
            Message.AddListener<GameStatusResponse>(OnGameStatusResponse);
            Message.AddListener<MadeWorld>(OnMadeWorld);
            cameraTrans = endlessCamera.gameObject.transform;
        }

        void OnDestroy()
        {
            Message.RemoveListener<GameStatusResponse>(OnGameStatusResponse);
            Message.RemoveListener<MadeWorld>(OnMadeWorld);
        }

        void OnGameStatusResponse(GameStatusResponse response)
        {
            gameStatus = response.status;
            if(gameStatus == GameStatus.MainMenu)
            {
                cameraTrans.position = startTransform.position;
                cameraTrans.rotation = startTransform.rotation;
            }
            else if(gameStatus == GameStatus.Starting)
            {
                rotateCamera = null;
                rotateCamera = RotateCamera(endTransform);
                StartCoroutine(rotateCamera);
            }
            else if(gameStatus == GameStatus.Restart)
            {
                cameraTrans.position = startTransform.position;
                cameraTrans.rotation = startTransform.rotation;
            }
        }

        void OnMadeWorld(MadeWorld madeWorld)
        {
            // rotateCamera = null;
            // rotateCamera = RotateCamera(endTransform);
            // StartCoroutine(rotateCamera);
            // Message.Send(new StartingGame());
        }

        float GetTotalDifference(Transform target)
        {
            float posDiff = Vector3.Distance(cameraTrans.position, target.position);
            float rotDiff = Vector3.Distance(cameraTrans.eulerAngles, target.eulerAngles);
            return posDiff + rotDiff;
        }

        IEnumerator RotateCamera(Transform target)
        {
            float diff = GetTotalDifference(target);
            while(diff > 0.1f)
            {
                cameraTrans.position = Vector3.Lerp(cameraTrans.position, target.position, Time.deltaTime * 2f);
                cameraTrans.rotation = Quaternion.Lerp(cameraTrans.rotation, target.rotation, Time.deltaTime * 2f);
                diff = GetTotalDifference(target);
                yield return null;
            }
            cameraTrans.position = target.position;
            cameraTrans.rotation = target.rotation;
            Message.Send(new StartGame());
            yield break;
        }

    }
}
