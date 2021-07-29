using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;
using Ryzm.EndlessRunner;
using Ryzm.UI.Messages;

namespace Ryzm.UI
{
    public class MenuManager : MonoBehaviour
    {
        #region Public Variables
        public MenuSets menuSets;
        #endregion

        #region Private Variables
        List<MenuType> noMenus = new List<MenuType> {};
        bool initializedGame;
        GameStatus status;
        #endregion

        #region Event Functions
        void Awake()
        {
            Message.AddListener<GameStatusResponse>(OnGameStatusResponse);
            Message.AddListener<MenuSetRequest>(OnMenuSetRequest);
            // Message.AddListener<RunnerDie>(OnRunnerDie);
        }

        void Start()
        {
            StartCoroutine(GetGameStatus());
        }
        
        void OnDestroy()
        {
            Message.RemoveListener<GameStatusResponse>(OnGameStatusResponse);
            Message.RemoveListener<MenuSetRequest>(OnMenuSetRequest);
            // Message.RemoveListener<RunnerDie>(OnRunnerDie);
        }
        #endregion

        #region Listener Functions
        void OnGameStatusResponse(GameStatusResponse response)
        {
            if(initializedGame && response.status == status)
            {
                return;
            }
            status = response.status;
            initializedGame = true;
            
            if(response.status == GameStatus.MainMenu)
            {
                ActivateMenus(menuSets.GetMenuTypes(MenuSet.MainMenu));
            }
            else if(response.status == GameStatus.Starting)
            {
                ActivateMenus(menuSets.GetMenuTypes(MenuSet.StartingMenu));
            }
            else if(response.status == GameStatus.Active)
            {
                ActivateMenus(menuSets.GetMenuTypes(MenuSet.ActiveMenu));
            }
            else if(response.status == GameStatus.Paused)
            {
                ActivateMenus(menuSets.GetMenuTypes(MenuSet.PauseMenu));
            }
            else if(response.status == GameStatus.Ended)
            {
                ActivateMenus(menuSets.GetMenuTypes(MenuSet.EndMenu));
            }
            else if(response.status == GameStatus.Restart)
            {
                ActivateMenus(menuSets.GetMenuTypes(MenuSet.RestartMenu));
            }
            else if(response.status == GameStatus.Exit)
            {
                ActivateMenus(menuSets.GetMenuTypes(MenuSet.ExitMenu));
            }
        }

        void ActivateMenus(List<MenuType> menus)
        {
            Message.Send(new ActivateMenu(activatedTypes: menus));
        }

        void OnMenuSetRequest(MenuSetRequest request)
        {
            Message.Send(new MenuSetResponse(menuSets.GetMenuTypes(request.set), request.set));
        }
        #endregion

        #region Coroutines
        IEnumerator GetGameStatus()
        {
            yield return new WaitForSeconds(Time.deltaTime * 5);
            Message.Send(new GameStatusRequest());
        }
        #endregion
    }

    public enum MenuSet
    {
        MainMenu,
        ActiveMenu,
        PauseMenu,
        EndMenu,
        RestartMenu,
        LoginMenu,
        BreedingMenu,
        MarketMenu,
        SingleDragonMenu,
        MyDragonsMenu,
        LoadingMenu,
        ExitMenu,
        StartingMenu
    }
}
