using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class EndlessCamera : MonoBehaviour
    {
        Vector3 pos, fw, up;
        Vector3 prevPos;
        Quaternion prevRot;
        Vector3 currentPlatformPos;
        Transform _transform;
        Transform _parentTransform;
        float initY;
        EndlessSection currentSection;
        Transform monkeyTrans;
        Transform dragonTrans;
        ControllerMode mode;
        GameStatus gameStatus = GameStatus.MainMenu;
        bool initialized;

        void Awake()
        {
            Message.AddListener<CurrentSectionChange>(OnCurrentSectionChange);
            Message.AddListener<ControllersResponse>(OnControllersResponse);
            Message.AddListener<ControllerModeResponse>(OnControllerModeResponse);
            Message.AddListener<GameStatusResponse>(OnGameStatusResponse);
            Message.Send(new ControllersRequest());
            Message.Send(new GameStatusRequest());
            _transform = transform;
        }

        void OnDestroy()
        {
            Message.RemoveListener<CurrentSectionChange>(OnCurrentSectionChange);
            Message.RemoveListener<ControllersResponse>(OnControllersResponse);
            Message.RemoveListener<ControllerModeResponse>(OnControllerModeResponse);
            Message.RemoveListener<GameStatusResponse>(OnGameStatusResponse);
        }

        void OnCurrentSectionChange(CurrentSectionChange sectionChange)
        {
            currentSection = sectionChange.endlessSection;
        }

        void OnControllersResponse(ControllersResponse response)
        {
            monkeyTrans = response.monkey.transform;
            dragonTrans = response.dragon.transform;
        }

        void OnControllerModeResponse(ControllerModeResponse response)
        {
            mode = response.mode;
        }

        void OnGameStatusResponse(GameStatusResponse response)
        {
            gameStatus = response.status;
            if(gameStatus == GameStatus.Active && !initialized)
            {
                initialized = true;
                _parentTransform = monkeyTrans;
                pos = _parentTransform.InverseTransformPoint(_transform.position);
                fw = _parentTransform.InverseTransformDirection(_transform.forward);
                up = _parentTransform.InverseTransformDirection(_transform.up);
                prevPos = _transform.position;
                prevRot = _transform.rotation;
                initY = pos.y;
            }
        }

        void LateUpdate()
        {
            if(gameStatus != GameStatus.Active)
            {
                return;
            }
            _parentTransform = mode == ControllerMode.Dragon ? dragonTrans : monkeyTrans;
            var newpos = _parentTransform.TransformPoint(pos);
            var newfw = _parentTransform.TransformDirection(fw);
            if(currentSection != null)
            {
                currentPlatformPos = currentSection.GetPosition(1).position;
                // todo: how to handle going on a curve?
                // if we are going forward in the global z direction then we want to keep the global x location the same as the center of the platform
                if(Mathf.Abs(newfw.z) > Mathf.Abs(newfw.x))
                {
                    newpos.x = currentPlatformPos.x;
                    // newpos.z = Mathf.Lerp(_transform.position.z, newpos.z, 0.1f);
                }
                else
                {
                    newpos.z = currentPlatformPos.z;
                    // newpos.x = Mathf.Lerp(_transform.position.x, newpos.x, 0.1f);
                }
            }
            // float newY = newpos.y;
            // newpos = Vector3.Lerp(_transform.position, newpos, 0.01f);
            // newpos.y = newY;
            // newpos.y = initY;
            // newpos.y = Mathf.Lerp(prevPos.y, newpos.y, 0.03f);
            
            var newup = _parentTransform.TransformDirection(up);
            var newrot = Quaternion.LookRotation(newfw, newup);
            newrot = Quaternion.Lerp(prevRot, newrot, 0.05f);
            _transform.rotation = newrot;
            _transform.position = newpos;
            // abs(fw.z) > abs(fw.x) keep x the same
            // Debug.Log($"{newpos.x - prevPos.x}" + " " + $"{newpos.z - prevPos.z}" + $"{newfw} \t newpos: {newpos} \t currentPlatformpos: {currentPlatformPos}" + "");
            prevPos = newpos;
            prevRot = newrot;
        }
    }
}
