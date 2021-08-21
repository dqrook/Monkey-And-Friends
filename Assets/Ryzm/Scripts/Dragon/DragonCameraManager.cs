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
        public List<CameraTransform> marketCameraTransforms = new List<CameraTransform>();

        Transform camTrans;
        IEnumerator moveCamera;

        #region Event Functions
        void Awake()
        {
            camTrans = dragonCamera.transform;
            Message.AddListener<DragonCameraRequest>(OnDragonCameraRequest);
            Message.AddListener<MoveCameraRequest>(OnMoveCameraRequest);
            Message.AddListener<DisplayDragonZoomRequest>(OnDisplayDragonZoomRequest);
            Message.AddListener<MarketCameraTransformsResponse>(OnMarketCameraTransformsResponse);
            Message.AddListener<FilterDragonZoomRequest>(OnFilterDragonZoomRequest);
        }

        void Start()
        {
            Message.Send(new MarketCameraTransformsRequest());
        }

        void OnDestroy()
        {
            Message.RemoveListener<DragonCameraRequest>(OnDragonCameraRequest);
            Message.RemoveListener<MoveCameraRequest>(OnMoveCameraRequest);
            Message.RemoveListener<DisplayDragonZoomRequest>(OnDisplayDragonZoomRequest);
            Message.RemoveListener<MarketCameraTransformsResponse>(OnMarketCameraTransformsResponse);
            Message.RemoveListener<FilterDragonZoomRequest>(OnFilterDragonZoomRequest);
        }
        #endregion

        #region Listener Functions
        void OnDragonCameraRequest(DragonCameraRequest request)
        {
            Message.Send(new DragonCameraResponse(dragonCamera));
        }

        void OnMoveCameraRequest(MoveCameraRequest request)
        {
            Debug.Log(request.type);
            bool foundIt = false;
            foreach(CameraTransform xform in cameraTransforms)
            {
                if(xform.type == request.type)
                {
                    camTrans.parent = xform.pivot != null ? xform.pivot : null;
                    MoveCamera(xform.camTrans, request.type);
                    foundIt = true;
                    break;
                }
            }
            if(!foundIt)
            {
                foreach(CameraTransform xform in marketCameraTransforms)
                {
                    if(xform.type == request.type)
                    {
                        camTrans.parent = xform.pivot != null ? xform.pivot : null;
                        MoveCamera(xform.camTrans, request.type);
                        foundIt = true;
                        break;
                    }
                }
            }
        }

        void OnDisplayDragonZoomRequest(DisplayDragonZoomRequest request)
        {
            int index = request.displayDragonIndex;
            CameraTransformType type = CameraTransformType.MarketDragon1;
            switch(index)
            {
                case 0:
                    type = CameraTransformType.MarketDragon1;
                    break;
                case 1:
                    type = CameraTransformType.MarketDragon2;
                    break;
                case 2:
                    type = CameraTransformType.MarketDragon3;
                    break;
                case 3:
                    type = CameraTransformType.MarketDragon4;
                    break;
                case 4:
                    type = CameraTransformType.MarketDragon5;
                    break;
                case 5:
                    type = CameraTransformType.MarketDragon6;
                    break;
                case 6:
                    type = CameraTransformType.MarketDragon7;
                    break;
                case 7:
                    type = CameraTransformType.MarketDragon8;
                    break;
                case 8:
                    type = CameraTransformType.MarketDragon9;
                    break;
                case 9:
                    type = CameraTransformType.MarketDragon10;
                    break;
            }
            OnMoveCameraRequest(new MoveCameraRequest(type));
        }

        void OnFilterDragonZoomRequest(FilterDragonZoomRequest request)
        {
            OnMoveCameraRequest(new MoveCameraRequest(CameraTransformType.FilterDragon));
        }

        void OnMarketCameraTransformsResponse(MarketCameraTransformsResponse response)
        {
            marketCameraTransforms = response.transforms;
        }
        #endregion

        #region Private Functions
        void MoveCamera(Transform target, CameraTransformType type)
        {
            if(moveCamera != null)
            {
                StopCoroutine(moveCamera);
            }
            moveCamera = null;
            moveCamera = _MoveCamera(target, type);
            StartCoroutine(moveCamera);
        }
        #endregion

        #region Coroutines
        IEnumerator _MoveCamera(Transform target, CameraTransformType type)
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
        #endregion
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
        MarketDragon5,
        MarketDragon6,
        MarketDragon7,
        MarketDragon8,
        MarketDragon9,
        MarketDragon10,
        FilterDragon
    }

    [System.Serializable]
    public struct CameraTransform
    {
        public CameraTransformType type;
        public Transform camTrans;
        public Transform pivot;
    }
}
