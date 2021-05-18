using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class GameManager : MonoBehaviour
    {
        public GameStatus status = GameStatus.Ended;
        public float speed = 0.15f;
        IEnumerator lerpGameSpeed;

        void Awake()
        {
            Message.AddListener<GameStatusRequest>(OnGameStatusRequest);
            Message.AddListener<RunnerDie>(OnRunnerDie);
            Message.AddListener<RequestGameSpeedChange>(OnRequestGameSpeedChange);
            Message.AddListener<GameSpeedRequest>(OnGameSpeedRequest);
            Message.AddListener<StartingGame>(OnStartingGame);
            Message.AddListener<StartGame>(OnStartGame);
            Message.AddListener<PauseGame>(OnPauseGame);
            Message.AddListener<ResumeGame>(OnResumeGame);
            UpdateGameStatus(GameStatus.MainMenu);
        }

        void OnDestroy()
        {
            Message.RemoveListener<GameStatusRequest>(OnGameStatusRequest);
            Message.RemoveListener<RunnerDie>(OnRunnerDie);
            Message.RemoveListener<RequestGameSpeedChange>(OnRequestGameSpeedChange);
            Message.RemoveListener<GameSpeedRequest>(OnGameSpeedRequest);
            Message.RemoveListener<StartingGame>(OnStartingGame);
            Message.RemoveListener<StartGame>(OnStartGame);
            Message.RemoveListener<PauseGame>(OnPauseGame);
            Message.RemoveListener<ResumeGame>(OnResumeGame);
        }

        void OnStartingGame(StartingGame starting)
        {
            UpdateGameStatus(GameStatus.Starting);
        }

        void OnStartGame(StartGame start)
        {
            Message.Send(new CreateSectionRow());
            Debug.Log("CreateSectionRow");
            UpdateGameStatus(GameStatus.Active);
        }

        void OnPauseGame(PauseGame pause)
        {
            UpdateGameStatus(GameStatus.Paused);
        }

        void OnResumeGame(ResumeGame resume)
        {
            UpdateGameStatus(GameStatus.Active);
        }

        void OnRunnerDie(RunnerDie runnerDie)
        {
            UpdateGameStatus(GameStatus.Ended);
        }

        void OnGameStatusRequest(GameStatusRequest statusRequest)
        {
            UpdateGameStatus(status);
        }

        void UpdateGameStatus(GameStatus status)
        {
            this.status = status;
            Message.Send(new GameStatusResponse(status));
        }

        void OnRequestGameSpeedChange(RequestGameSpeedChange requestChangeSpeed)
        {
            if(requestChangeSpeed.lerpTime <= 0)
            {
                UpdateSpeed(requestChangeSpeed.speed);
            }
            else
            {
                lerpGameSpeed = LerpGameSpeed(requestChangeSpeed.speed, requestChangeSpeed.lerpTime);
                StartCoroutine(lerpGameSpeed);
            }
        }

        IEnumerator LerpGameSpeed(float targetSpeed, float lerpTime)
        {
            float _time = 0;
            while(_time <= lerpTime)
            {
                UpdateSpeed(Mathf.Lerp(speed, targetSpeed, _time / lerpTime));
                _time += Time.deltaTime;
            }
            UpdateSpeed(targetSpeed);
            yield break;
        }

        void UpdateSpeed(float newSpeed)
        {
            this.speed = newSpeed;
            Message.Send(new GameSpeedResponse(newSpeed));
        }

        void OnGameSpeedRequest(GameSpeedRequest request)
        {
            UpdateSpeed(this.speed);
        }
    }

    public enum GameStatus
    {
        MainMenu,
        Starting,
        Active,
        Paused,
        Ended
    }
}
