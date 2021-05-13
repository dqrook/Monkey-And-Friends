using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner.UI
{
    public class MenuManager : MonoBehaviour
    {
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

        void Awake()
        {
            Message.AddListener<RunnerDie>(OnRunnerDie);
            Message.AddListener<StartGame>(OnStartGame);
            Message.AddListener<PauseGame>(OnPauseGame);
            Message.AddListener<ResumeGame>(OnResumeGame);
        }

        void OnDestroy()
        {
            Message.RemoveListener<RunnerDie>(OnRunnerDie);
            Message.RemoveListener<StartGame>(OnStartGame);
            Message.RemoveListener<PauseGame>(OnPauseGame);
            Message.RemoveListener<ResumeGame>(OnResumeGame);
        }

        void OnStartGame(StartGame start)
        {
            Message.Send(new ActivateMenu(activatedTypes: startMenus));
            Message.Send(new DeactivateMenu(activatedTypes: startMenus));
        }

        void OnPauseGame(PauseGame pause)
        {
            Message.Send(new ActivateMenu(activatedTypes: pauseMenus));
            Message.Send(new DeactivateMenu(activatedTypes: pauseMenus));
        }

        void OnResumeGame(ResumeGame resume)
        {
            Message.Send(new ActivateMenu(activatedTypes: startMenus));
            Message.Send(new DeactivateMenu(activatedTypes: startMenus));
        }

        void OnRunnerDie(RunnerDie runnerDie)
        {
            Message.Send(new ActivateMenu(activatedTypes: endMenus));
            Message.Send(new DeactivateMenu(activatedTypes: endMenus));
        }
    }
}
