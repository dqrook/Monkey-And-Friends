using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class GameManager : MonoBehaviour
    {
        public GameStatus status = GameStatus.Active;
        public float speed = 0.15f;
        EndlessSection _currentSection;
        EndlessTSection _currentTSection;
        IEnumerator lerpGameSpeed;

        void Awake()
        {
            Message.AddListener<CurrentSectionChange>(OnCurrentSectionChange);
            Message.AddListener<GameStatusRequest>(OnGameStatusRequest);
            Message.AddListener<RunnerDie>(OnRunnerDie);
            Message.AddListener<RequestGameSpeedChange>(OnRequestGameSpeedChange);
            Message.AddListener<GameSpeedRequest>(OnGameSpeedRequest);
        }

        void Start()
        {
            Message.Send(new CreateSectionRow());
        }

        void OnDestroy()
        {
            Message.RemoveListener<CurrentSectionChange>(OnCurrentSectionChange);
            Message.RemoveListener<GameStatusRequest>(OnGameStatusRequest);
            Message.RemoveListener<RunnerDie>(OnRunnerDie);
            Message.RemoveListener<RequestGameSpeedChange>(OnRequestGameSpeedChange);
            Message.RemoveListener<GameSpeedRequest>(OnGameSpeedRequest);
        }

        void OnCurrentSectionChange(CurrentSectionChange change)
        {
            _currentTSection = change.endlessTSection;
            _currentSection = change.endlessSection;
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
        Active,
        Paused,
        Ended
    }
}
