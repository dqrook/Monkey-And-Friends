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
        public MenuSets menuSets;

        List<MenuType> noMenus = new List<MenuType> {};
        bool initializedGame;
        GameStatus status;

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
        IEnumerator GetGameStatus()
        {
            yield return new WaitForSeconds(Time.deltaTime * 5);
            Message.Send(new GameStatusRequest());
        }
        void OnDestroy()
        {
            Message.RemoveListener<GameStatusResponse>(OnGameStatusResponse);
            Message.RemoveListener<MenuSetRequest>(OnMenuSetRequest);
            // Message.RemoveListener<RunnerDie>(OnRunnerDie);
        }

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
                ActivateMenus(noMenus);
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
        }

        // void OnRunnerDie(RunnerDie runnerDie)
        // {
        //     ActivateMenus(menuSets.GetMenuTypes(MenuSet.EndMenu));
        // }

        void ActivateMenus(List<MenuType> menus)
        {
            Message.Send(new ActivateMenu(activatedTypes: menus));
        }

        void OnMenuSetRequest(MenuSetRequest request)
        {
            Message.Send(new MenuSetResponse(menuSets.GetMenuTypes(request.set), request.set));
        }
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
        MarketMenu
    }
}
