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
        public RunnerController runner;
        private static GameManager _instance;
        public static GameManager Instance { get { return _instance; } }
        EndlessSection _currentSection;
        EndlessTSection _currentTSection;

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            } 
            else 
            {
                _instance = this;
            }
            if(runner == null)
            {
                runner = FindObjectOfType<RunnerController>();
            }
            Message.AddListener<CurrentSectionChange>(OnCurrentSectionChange);
            Message.AddListener<GameStatusRequest>(OnGameStatusRequest);
            Message.AddListener<RunnerDie>(OnRunnerDie);
            Message.AddListener<ChangeGameSpeed>(OnChangeGameSpeed);
        }

        void Start()
        {
            Message.Send(new CreateSection());
        }

        void OnDestroy()
        {
            Message.RemoveListener<CurrentSectionChange>(OnCurrentSectionChange);
            Message.RemoveListener<GameStatusRequest>(OnGameStatusRequest);
            Message.RemoveListener<RunnerDie>(OnRunnerDie);
            Message.RemoveListener<ChangeGameSpeed>(OnChangeGameSpeed);
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

        void OnChangeGameSpeed(ChangeGameSpeed changeGameSpeed)
        {
            if(changeGameSpeed.lerpTime <= 0)
            {
                speed = changeGameSpeed.speed;
            }
            else
            {
                lerpGameSpeed = LerpGameSpeed(changeGameSpeed.speed, changeGameSpeed.lerpTime);
                StartCoroutine(lerpGameSpeed);
            }
        }

        IEnumerator lerpGameSpeed;
        IEnumerator LerpGameSpeed(float targetSpeed, float lerpTime)
        {
            float _time = 0;
            while(_time <= lerpTime)
            {
                speed = Mathf.Lerp(speed, targetSpeed, _time / lerpTime);
                _time += Time.deltaTime;
            }
            speed = targetSpeed;
            yield break;
        }
    }

    public enum GameStatus
    {
        Active,
        Paused,
        Ended
    }
}
