using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.Dragon.Messages;

namespace Ryzm.Dragon
{
    public class DragonCameraManager : MonoBehaviour
    {
        public GameObject dragonCamera;
        public List<CameraTransform> cameraTransforms = new List<CameraTransform>();

        Transform camTrans;
        IEnumerator moveCamera;

        void Awake()
        {
            camTrans = dragonCamera.transform;
            Message.AddListener<DragonCameraRequest>(OnDragonCameraRequest);
            Message.AddListener<MoveCameraRequest>(OnMoveCameraRequest);
        }

        void OnDestroy()
        {
            Message.RemoveListener<DragonCameraRequest>(OnDragonCameraRequest);
            Message.RemoveListener<MoveCameraRequest>(OnMoveCameraRequest);
        }

        void OnDragonCameraRequest(DragonCameraRequest request)
        {
            Message.Send(new DragonCameraResponse(dragonCamera));
        }

        void OnMoveCameraRequest(MoveCameraRequest request)
        {
            Debug.Log(request.type);
            foreach(CameraTransform xform in cameraTransforms)
            {
                if(xform.type == request.type)
                {
                    camTrans.parent = xform.pivot != null ? xform.pivot : null;
                    MoveCamera(xform.camTrans);
                    break;
                }
            }
        }

        void MoveCamera(Transform target)
        {
            if(moveCamera != null)
            {
                StopCoroutine(moveCamera);
            }
            moveCamera = null;
            moveCamera = _MoveCamera(target);
            StartCoroutine(moveCamera);
        }

        IEnumerator _MoveCamera(Transform target)
        {
            Debug.Log("moving camera");
            Vector3 endPos = target.position;
            Quaternion endRot = target.rotation;
            float distance = Vector3.Distance(camTrans.position, endPos);
            float rotDiff = Mathf.Abs(Quaternion.Angle(camTrans.rotation, endRot));
            while(distance > 0.1f || rotDiff > 0.1f)
            {
                camTrans.position = Vector3.Lerp(camTrans.position, endPos, Time.deltaTime * 2);
                camTrans.rotation = Quaternion.Lerp(camTrans.rotation, endRot, Time.deltaTime * 2);
                distance = Vector3.Distance(camTrans.position, endPos);
                rotDiff = Mathf.Abs(Quaternion.Angle(camTrans.rotation, endRot));
                yield return null;
            }
            // camTrans.position = target.position;
            // camTrans.rotation = target.rotation;
            yield break;
        }
    }

    public enum CameraTransformType
    {
        MainMenu,
        BreedingMenu,
        Dragon1,
        Dragon2,
        SingleDragon,
        Market,
        MarketDragon1,
        MarketDragon2,
        MarketDragon3,
        MarketDragon4,
        MarketDragon5
    }

    [System.Serializable]
    public struct CameraTransform
    {
        public CameraTransformType type;
        public Transform camTrans;
        public Transform pivot;
    }
}
