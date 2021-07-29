using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;
using Ryzm.Messages;

namespace Ryzm.EndlessRunner
{
    public class GameManager : MonoBehaviour
    {
        #region Public Variables
        public GameStatus status = GameStatus.MainMenu;
        public float speed = 0.15f;
        #endregion

        #region Private Variables
        IEnumerator lerpGameSpeed;
        IEnumerator deactivateHomeIsland;
        IEnumerator restart2Starting;
        IEnumerator exit2Main;
        WaitForSeconds wait2Seconds;
        WaitForSeconds wait4Seconds;
        #endregion

        #region Event Functions
        void Awake()
        {
            Message.AddListener<GameStatusRequest>(OnGameStatusRequest);
            Message.AddListener<RunnerDie>(OnRunnerDie);
            Message.AddListener<RequestGameSpeedChange>(OnRequestGameSpeedChange);
            Message.AddListener<GameSpeedRequest>(OnGameSpeedRequest);
            Message.AddListener<MadeWorld>(OnMadeWorld);
            Message.AddListener<StartGame>(OnStartGame);
            Message.AddListener<PauseGame>(OnPauseGame);
            Message.AddListener<ResumeGame>(OnResumeGame);
            Message.AddListener<RestartGame>(OnRestartGame);
            Message.AddListener<ExitGame>(OnExitGame);
            wait2Seconds = new WaitForSeconds(2);
            wait4Seconds = new WaitForSeconds(4);
        }

        void Start()
        {
            UpdateGameStatus(GameStatus.MainMenu);
            Message.Send(new GameTypeRequest());
        }

        void OnDestroy()
        {
            Message.RemoveListener<GameStatusRequest>(OnGameStatusRequest);
            Message.RemoveListener<RunnerDie>(OnRunnerDie);
            Message.RemoveListener<RequestGameSpeedChange>(OnRequestGameSpeedChange);
            Message.RemoveListener<GameSpeedRequest>(OnGameSpeedRequest);
            Message.RemoveListener<MadeWorld>(OnMadeWorld);
            Message.RemoveListener<StartGame>(OnStartGame);
            Message.RemoveListener<PauseGame>(OnPauseGame);
            Message.RemoveListener<ResumeGame>(OnResumeGame);
            Message.RemoveListener<RestartGame>(OnRestartGame);
            Message.RemoveListener<ExitGame>(OnExitGame);
        }

        void Update()
        {
            if(Input.GetKeyDown(KeyCode.P))
            {
                Time.timeScale = Time.timeScale > 0 ? 0 : 1;
            }
        }
        #endregion

        #region Listener Functions
        void OnMadeWorld(MadeWorld madeWorld)
        {
            OnStartingGame();
        }

        void OnStartingGame()
        {
            UpdateGameStatus(GameStatus.PreStarting);
            Message.Send(new CreateMap());
            UpdateGameStatus(GameStatus.Starting);
        }

        void OnStartGame(StartGame start)
        {
            Debug.Log("start game");
            UpdateGameStatus(GameStatus.Active);
            deactivateHomeIsland = null;
            deactivateHomeIsland = DeactivateHomeIsland();
            StartCoroutine(deactivateHomeIsland);
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
            StopAllCoroutines();
            UpdateGameStatus(GameStatus.Ended);
        }

        void OnRestartGame(RestartGame restartGame)
        {
            UpdateGameStatus(GameStatus.Restart);
            restart2Starting = null;
            restart2Starting = Restart2Starting();
            StartCoroutine(restart2Starting);
        }

        void OnExitGame(ExitGame exitGame)
        {
            UpdateGameStatus(GameStatus.Exit);
            exit2Main = null;
            exit2Main = ExitToMainMenu();
            StartCoroutine(exit2Main);
        }

        void OnGameStatusRequest(GameStatusRequest statusRequest)
        {
            UpdateGameStatus(status);
        }

        void OnGameSpeedRequest(GameSpeedRequest request)
        {
            UpdateSpeed(this.speed);
        }

        void OnGameTypeResponse(GameTypeResponse response)
        {

        }
        #endregion

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

        #region Private Functions
        void UpdateGameStatus(GameStatus status)
        {
            this.status = status;
            Message.Send(new GameStatusResponse(status));
        }

        void UpdateSpeed(float newSpeed)
        {
            this.speed = newSpeed;
            Message.Send(new GameSpeedResponse(newSpeed));
        }
        #endregion

        #region Coroutines
        IEnumerator DeactivateHomeIsland()
        {
            Debug.Log("deactivating home");
            yield return wait4Seconds;
            Message.Send(new DeactivateHome());
        }

        IEnumerator Restart2Starting()
        {
            yield return wait2Seconds;
            OnStartingGame();
        }

        IEnumerator ExitToMainMenu()
        {
            yield return wait2Seconds;
            UpdateGameStatus(GameStatus.MainMenu);
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
        #endregion
    }

    public enum GameStatus
    {
        MainMenu,
        Starting,
        Active,
        Paused,
        Ended,
        Restart,
        PreStarting,
        Exit
    }
}
