using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.Dragon.Messages;
using CodeControl;
using Ryzm.UI.Messages;
using Ryzm.EndlessRunner;
using Ryzm.Dragon;

namespace Ryzm.UI
{
    public class SingleDragonMenu : RyzmMenu
    {
        EndlessDragon singleDragon;
        EndlessDragon[] dragons;
        List<MenuType> mainMenus  = new List<MenuType>();

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
                        Message.AddListener<DragonsResponse>(OnDragonsResponse);
                        Message.AddListener<MenuSetResponse>(OnMenuSetResponse);
                        Message.AddListener<SingleDragonUpdate>(OnSingleDragonUpdate);
                        
                        // Message.Send(new DragonsRequest("singleDragonMenu"));
                        Message.Send(new MoveCameraRequest(CameraTransformType.SingleDragon));
                        if(mainMenus.Count == 0)
                        {
                            Message.Send(new MenuSetRequest(MenuSet.MainMenu));
                        }
                    }
                    else
                    {
                        Message.RemoveListener<DragonsResponse>(OnDragonsResponse);
                        Message.RemoveListener<MenuSetResponse>(OnMenuSetResponse);
                        Message.RemoveListener<SingleDragonUpdate>(OnSingleDragonUpdate);
                    }
                    base.IsActive = value;
                }
            }
        }

        void OnSingleDragonUpdate(SingleDragonUpdate update)
        {
            singleDragon = update.dragon;
            Message.Send(new DeactivateLoadingMenu());
        }

        void OnDragonsResponse(DragonsResponse response)
        {
            if(response.sender == "singleDragonMenu")
            {
                dragons = response.dragons;
            }
        }

        void OnMenuSetResponse(MenuSetResponse response)
        {
            if(response.set == MenuSet.MainMenu)
            {
                mainMenus = response.menus;
            }
        }

        public void Exit()
        {
            Message.Send(new ActivateMenu(activatedTypes: mainMenus));
        }
    }
}
