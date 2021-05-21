using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner.UI
{
    public class MenuManager : MonoBehaviour
    {
        public EndlessMenuSets menuSets;

        List<MenuType> mainMenus = new List<MenuType>
        {
            MenuType.Main,
            MenuType.Header
        };

        List<MenuType> activeMenus = new List<MenuType>
        {
            MenuType.Score,
            MenuType.Distance,
            MenuType.SwipeZone
        };

        List<MenuType> pauseMenus = new List<MenuType>
        {
            MenuType.Pause
        };

        List<MenuType> endMenus = new List<MenuType>
        {
            MenuType.EndGame
        };

        List<MenuType> noMenus = new List<MenuType> {};

        void Awake()
        {
            Message.AddListener<GameStatusResponse>(OnGameStatusResponse);
            Message.AddListener<RunnerDie>(OnRunnerDie);
            Message.Send(new GameStatusRequest());
        }

        void OnDestroy()
        {
            Message.RemoveListener<GameStatusResponse>(OnGameStatusResponse);
            Message.RemoveListener<RunnerDie>(OnRunnerDie);
        }

        void OnGameStatusResponse(GameStatusResponse response)
        {
            if(response.status == GameStatus.MainMenu)
            {
                // ActivateMenus(mainMenus);
                ActivateMenus(menuSets.GetMenuTypes(MenuSet.MainMenu));
            }
            else if(response.status == GameStatus.Starting)
            {
                ActivateMenus(noMenus);
            }
            else if(response.status == GameStatus.Active)
            {
                // ActivateMenus(activeMenus);
                ActivateMenus(menuSets.GetMenuTypes(MenuSet.ActiveMenu));
            }
            else if(response.status == GameStatus.Paused)
            {
                // ActivateMenus(pauseMenus);
                ActivateMenus(menuSets.GetMenuTypes(MenuSet.PauseMenu));
            }
            else if(response.status == GameStatus.Ended)
            {
                // ActivateMenus(endMenus);
                ActivateMenus(menuSets.GetMenuTypes(MenuSet.EndMenu));
            }
        }

        void OnRunnerDie(RunnerDie runnerDie)
        {
            ActivateMenus(menuSets.GetMenuTypes(MenuSet.EndMenu));
        }

        void ActivateMenus(List<MenuType> menus)
        {
            Message.Send(new ActivateMenu(activatedTypes: menus));
            Message.Send(new DeactivateMenu(activatedTypes: menus));
        }
    }

    public enum MenuSet
    {
        MainMenu,
        ActiveMenu,
        PauseMenu,
        EndMenu
    }

    public enum MenuType
    {
        None,
        Score,
        Distance,
        SwipeZone,
        Pause,
        EndGame,
        Main,
        Login,
        Header,
        Entry
    }
}
