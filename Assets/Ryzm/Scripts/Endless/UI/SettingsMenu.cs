using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.EndlessRunner.Messages;
using Ryzm.UI.Messages;
using Ryzm.EndlessRunner;
using UnityEngine.UI;

namespace Ryzm.UI
{
    public class SettingsMenu : RyzmMenu
    {
        #region Public Variables
        public List<ToggleIndex> toggles = new List<ToggleIndex>();
        #endregion

        #region Private Variables
        EndlessCameraSpawns cameraSpawns;
        List<MenuType> mainMenus = new List<MenuType>();
        bool initializedToggles;
        #endregion

        #region Properties
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
                        Message.AddListener<CameraSpawnsResponse>(OnCameraSpawnsResponse);
                        if(!initializedToggles)
                        {
                            Message.Send(new CameraSpawnsRequest());
                        }
                        Message.AddListener<MenuSetResponse>(OnMenuSetResponse);
                        if(mainMenus.Count == 0)
                        {
                            Message.Send(new MenuSetRequest(MenuSet.MainMenu));
                        }
                    }
                    else
                    {
                        Message.RemoveListener<CameraSpawnsResponse>(OnCameraSpawnsResponse);
                        Message.RemoveListener<MenuSetResponse>(OnMenuSetResponse);
                    }
                    base.IsActive = value;
                }
            }
        }
        #endregion

        #region Listener Functions
        void OnCameraSpawnsResponse(CameraSpawnsResponse response)
        {
            if(!initializedToggles)
            {
                initializedToggles = true;
                cameraSpawns = response.cameraSpawns;
                int numSpawns = cameraSpawns.cameraSpawns.Count;
                int numToggles = toggles.Count;
                for(int i = 0; i < numToggles; i++)
                {
                    ToggleIndex toggleIndex = toggles[i];
                    Toggle toggle = toggleIndex.toggle;
                    if(i < numSpawns)
                    {
                        toggle.isOn = cameraSpawns.currentCameraSpawn == i;
                        toggle.onValueChanged.AddListener(delegate {
                            ToggleValueChanged(toggleIndex);
                        });
                        toggle.gameObject.SetActive(true);
                    }
                    else
                    {
                        toggle.gameObject.SetActive(false);
                    }
                }
            }
        }

        void OnMenuSetResponse(MenuSetResponse response)
        {
            if(response.set == MenuSet.MainMenu)
            {
                mainMenus = response.menus;
            }
        }
        #endregion

        #region Public Functions
        public void Exit()
        {
            Message.Send(new ActivateMenu(mainMenus));
        }
        #endregion

        #region Private Functions
        void ToggleValueChanged(ToggleIndex changedToggle)
        {
            Debug.Log(changedToggle.index + " " + changedToggle.toggle.isOn);
            if(changedToggle.toggle.isOn)
            {
                Message.Send(new UpdateCurrentCameraSpawn(changedToggle.index));
            }
        }
        #endregion
    }

    [System.Serializable]
    public struct ToggleIndex
    {
        public int index;
        public Toggle toggle;
    }
}
