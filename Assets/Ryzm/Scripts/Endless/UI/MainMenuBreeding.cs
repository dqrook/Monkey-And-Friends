using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;
using Ryzm.Blockchain.Messages;
using TMPro;
using Ryzm.Blockchain;
using Ryzm.Dragon.Messages;

namespace Ryzm.EndlessRunner.UI
{
    public class MainMenuBreeding : BaseMenu
    {
        public Transform mainCameraXform;
        public TextMeshProUGUI loginText;

        bool initialized;
        List<MenuType> breedingMenus = new List<MenuType> {};
        List<MenuType> loginMenus = new List<MenuType> {};
        List<MenuType> mainMenus = new List<MenuType> {};
        Transform camTrans;
        IEnumerator moveCamera;
        bool movingCamera;
        bool menuSetsInitialized;

        public override bool IsActive 
        { 
            get
            { 
                return base.IsActive;
            }
            set 
            {
                if(ShouldUpdate(value))
                {
                    if(value)
                    {
                        Message.AddListener<LoginResponse>(OnLoginResponse);
                        Message.AddListener<MenuSetResponse>(OnMenuSetResponse);
                        Message.AddListener<CameraResponse>(OnCameraResponse);
                        Message.Send(new LoginRequest());
                        Message.Send(new ResetDragons());
                        if(!menuSetsInitialized)
                        {
                            menuSetsInitialized = true;
                            Message.Send(new MenuSetRequest(MenuSet.BreedingMenu));
                            Message.Send(new MenuSetRequest(MenuSet.MainMenu));
                            Message.Send(new MenuSetRequest(MenuSet.LoginMenu));
                        }
                        Debug.Log("camTrans " + camTrans);
                        if(camTrans == null)
                        {
                            Message.Send(new CameraRequest());
                        }
                        else 
                        {
                            InitializeCamera(camTrans);
                        }
                    }
                    else
                    {
                        initialized = false;
                        StopAllCoroutines();
                        Message.RemoveListener<LoginResponse>(OnLoginResponse);
                        Message.RemoveListener<MenuSetResponse>(OnMenuSetResponse);
                        Message.RemoveListener<CameraResponse>(OnCameraResponse);
                    }
                    base.IsActive = value;
                }
            }
        }

        public void OnClickLogin()
        {
            Debug.Log("on click login");
            if(IsActive)
            {
                Message.Send(new ActivateMenu(activatedTypes: loginMenus));
                Message.Send(new DeactivateMenu(activatedTypes: loginMenus));
                Message.Send(new EnableHeaderBackButton(mainMenus));
            }
        }

        public void OnClickMarket()
        {

        }

        public void OnClickBreed()
        {
            if(IsActive)
            {
                Message.Send(new ActivateMenu(activatedTypes: breedingMenus));
                Message.Send(new DeactivateMenu(activatedTypes: breedingMenus));
            }
        }

        void OnLoginResponse(LoginResponse response)
        {
            switch(response.status)
            {
                case LoginStatus.FetchingKeys:
                    loginText.text = "Loading...";
                    break;
                case LoginStatus.LoggedIn:
                    loginText.text = response.accountName;
                    break;
                default:
                    loginText.text = "Login";
                    break;
            }
        }

        void OnMenuSetResponse(MenuSetResponse response)
        {
            if(response.set == MenuSet.BreedingMenu)
            {
                breedingMenus = response.menus;
            }
            else if(response.set == MenuSet.MainMenu)
            {
                mainMenus = response.menus;
            }
            else if(response.set == MenuSet.LoginMenu)
            {
                loginMenus = response.menus;
            }
        }

        void OnCameraResponse(CameraResponse response)
        {
            Debug.Log("main menu camera response");
            InitializeCamera(response.camera.transform);
        }

        void InitializeCamera(Transform cam)
        {
            Debug.Log("initializing z camera " + initialized);
            camTrans = cam;
            if(!initialized)
            {
                initialized = true;
                MoveCamera(mainCameraXform);
            }
        }

        void MoveCamera(Transform target)
        {
            Debug.Log("main menu MoveCamera");
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
            movingCamera = true;
            Vector3 endPos = target.position;
            Quaternion endRot = target.rotation;
            float distance = Vector3.Distance(camTrans.position, endPos);
            float rotDiff = Mathf.Abs(Quaternion.Angle(camTrans.rotation, endRot));
            while(distance > 0.1f || rotDiff > 1)
            {
                camTrans.position = Vector3.Lerp(camTrans.position, endPos, Time.deltaTime);
                camTrans.rotation = Quaternion.Lerp(camTrans.rotation, endRot, Time.deltaTime);
                distance = Vector3.Distance(camTrans.position, endPos);
                rotDiff = Mathf.Abs(Quaternion.Angle(camTrans.rotation, endRot));
                yield return null;
            }
            // camTrans.position = target.position;
            // camTrans.rotation = target.rotation;
            movingCamera = false;
            yield break;
        }
    }
}
