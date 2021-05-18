using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner.UI
{
    public class MenuManager : MonoBehaviour
    {
        List<MenuType> mainMenus = new List<MenuType>
        {
            MenuType.Main
        };

        List<MenuType> startMenus = new List<MenuType>
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
            }
            else if(response.status == GameStatus.Starting)
            {
                ActivateMenus(noMenus);
            }
            else if(response.status == GameStatus.Active)
            {
                ActivateMenus(startMenus);
            }
            else if(response.status == GameStatus.Paused)
            {
                ActivateMenus(pauseMenus);
            }
            else if(response.status == GameStatus.Active)
            {
                ActivateMenus(endMenus);
            }
        }

        void OnRunnerDie(RunnerDie runnerDie)
        {
            ActivateMenus(endMenus);
        }

        void ActivateMenus(List<MenuType> menus)
        {
            Message.Send(new ActivateMenu(activatedTypes: menus));
            Message.Send(new DeactivateMenu(activatedTypes: menus));
        }
    }
}
