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
            Message.Send(new ActivateMenu(MenuType.Score));
            Message.Send(new ActivateMenu(MenuType.Distance));
            Message.Send(new ActivateMenu(MenuType.SwipeZone));
            Message.Send(new DeactivateMenu(activatedTypes: startMenus));
        }

        void OnPauseGame(PauseGame pause)
        {
            
        }

        void OnResumeGame(ResumeGame resume)
        {
            
        }

        void OnRunnerDie(RunnerDie runnerDie)
        {
            
        }
    }
}
