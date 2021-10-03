using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class EndlessCamera : MonoBehaviour
    {
        #region Public Variables
        public Camera cam;
        #endregion

        #region Private Variables
        Vector3 pos, fw, up;
        Vector3 prevPos;
        Quaternion prevRot;
        Vector3 currentPlatformPos;
        Transform _transform;
        Transform _parentTransform;
        EndlessSection currentSection;
        Transform ryzTrans;
        Transform dragonTrans;
        EndlessController dragon;
        EndlessController ryz;
        ControllerMode mode;
        GameStatus gameStatus = GameStatus.MainMenu;
        bool initialized;
        bool isRestart;
        #endregion

        #region Properties
        Transform CurrentTransform
        {
            get
            {
                return mode == ControllerMode.Monkey ? ryzTrans : dragonTrans;
            }
        }

        EndlessController CurrentController
        {
            get
            {
                return mode == ControllerMode.Monkey ? ryz : dragon;
            }
        }
        #endregion

        #region Event Functions
        void Awake()
        {
            Message.AddListener<CurrentSectionChange>(OnCurrentSectionChange);
            Message.AddListener<ControllersResponse>(OnControllersResponse);
            Message.AddListener<ControllerModeResponse>(OnControllerModeResponse);
            Message.AddListener<GameStatusResponse>(OnGameStatusResponse);
            Message.Send(new ControllersRequest());
            // Message.Send(new GameStatusRequest());
            _transform = transform;
            if(cam == null)
            {
                cam = GetComponent<Camera>();
            }
        }

        void LateUpdate()
        {
            if(gameStatus != GameStatus.Active)
            {
                return;
            }
            
            _parentTransform = mode == ControllerMode.Monkey ? ryzTrans : dragonTrans;
            var newpos = _parentTransform.TransformPoint(pos);
            var newfw = _parentTransform.TransformDirection(fw);
            // if(currentSection != null)
            // {
            //     currentPlatformPos = currentSection.GetPosition(1).position;
            //     // todo: how to handle going on a curve?
            //     // if we are going forward in the global z direction then we want to keep the global x location the same as the center of the platform
            //     if(Mathf.Abs(newfw.z) > Mathf.Abs(newfw.x))
            //     {
            //         newpos.x = currentPlatformPos.x;
            //         // newpos.z = Mathf.Lerp(_transform.position.z, newpos.z, 0.1f);
            //     }
            //     else
            //     {
            //         newpos.z = currentPlatformPos.z;
            //         // newpos.x = Mathf.Lerp(_transform.position.x, newpos.x, 0.1f);
            //     }
            // }
            
            var newup = _parentTransform.TransformDirection(up);
            var newrot = Quaternion.LookRotation(newfw, newup);
            newrot = Quaternion.Lerp(prevRot, newrot, 0.05f);
            _transform.rotation = newrot;
            Vector3 finPos = newpos;
            // finPos.x = Mathf.Lerp(_transform.position.x, newpos.x, 5 * Time.deltaTime);
            _transform.position = finPos;
            
            prevRot = newrot;
        }

        void OnDestroy()
        {
            Message.RemoveListener<CurrentSectionChange>(OnCurrentSectionChange);
            Message.RemoveListener<ControllersResponse>(OnControllersResponse);
            Message.RemoveListener<ControllerModeResponse>(OnControllerModeResponse);
            Message.RemoveListener<GameStatusResponse>(OnGameStatusResponse);
        }
        #endregion

        #region Listener Functions
        void OnCurrentSectionChange(CurrentSectionChange sectionChange)
        {
            currentSection = sectionChange.endlessSection;
        }

        void OnControllersResponse(ControllersResponse response)
        {
            dragon = response.dragon;
            ryz = response.ryz;
            ryzTrans = response.ryz.transform;
            dragonTrans = response.dragon.transform;
        }

        void OnControllerModeResponse(ControllerModeResponse response)
        {
            mode = response.mode;
        }

        void OnGameStatusResponse(GameStatusResponse response)
        {
            gameStatus = response.status;
            if(gameStatus == GameStatus.Active)
            {
                if(!initialized)
                {
                    initialized = true;
                    _parentTransform = CurrentTransform;
                    pos = _parentTransform.InverseTransformPoint(_transform.position);
                    fw = _parentTransform.InverseTransformDirection(_transform.forward);
                    up = _parentTransform.InverseTransformDirection(_transform.up);
                    prevRot = _transform.rotation;
                }
                if(isRestart)
                {
                    isRestart = false;
                    _transform.parent = CurrentTransform;
                    _transform.localPosition = CurrentController.localCameraSpawn.localPosition;
                    _transform.localRotation = CurrentController.localCameraSpawn.localRotation;
                    prevRot = _transform.rotation;
                    _transform.parent = null;
                }
            }
            else if(gameStatus == GameStatus.Restart)
            {
                currentSection = null;
                isRestart = true;
            }
            else if(gameStatus == GameStatus.Exit)
            {
                currentSection = null;
            }
        }
        #endregion
    }
}
