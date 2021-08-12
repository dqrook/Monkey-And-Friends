using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;
using Ryzm.Messages;
using Ryzm.UI.Messages;

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
        IEnumerator exit2Main;
        IEnumerator fadeMenu;
        WaitForSeconds wait2Seconds;
        WaitForSeconds wait4Seconds;
        bool isStartingGame;
        bool madeWorld;
        MapType currentMap;
        bool gotCurrentMap;
        bool gotRunwayResponse;
        bool hasRunway;
        #endregion

        #region Event Functions
        void Awake()
        {
            Message.AddListener<GameStatusRequest>(OnGameStatusRequest);
            Message.AddListener<RunnerDie>(OnRunnerDie);
            Message.AddListener<RequestGameSpeedChange>(OnRequestGameSpeedChange);
            Message.AddListener<GameSpeedRequest>(OnGameSpeedRequest);
            Message.AddListener<MadeWorld>(OnMadeWorld);
            Message.AddListener<CreateMapResponse>(OnCreateMapResponse);
            Message.AddListener<StartRunwayResponse>(OnStartRunwayResponse);
            Message.AddListener<StartingGame>(OnStartingGame);
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
            Message.RemoveListener<CreateMapResponse>(OnCreateMapResponse);
            Message.RemoveListener<StartRunwayResponse>(OnStartRunwayResponse);
            Message.RemoveListener<StartingGame>(OnStartingGame);
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
            // _OnStartingGame();
            this.madeWorld = true;
        }

        void _OnStartingGame()
        {
            UpdateGameStatus(GameStatus.CreatingMap); // need this game status update b4 creating the map b/c when all errything is activated they will request game status and if the game status is restart then we screwed
            Message.Send(new CreateMap());
            UpdateGameStatus(GameStatus.Starting);
        }

        void OnStartingGame(StartingGame starting)
        {
            FadeMenu();
            // if(!isStartingGame)
            // {
            //     isStartingGame = true;
            //     if(fadeMenu != null)
            //     {
            //         StopCoroutine(fadeMenu);
            //         fadeMenu = null;
            //     }
            //     fadeMenu = _FadeMenu();
            //     StartCoroutine(fadeMenu);
            // }
        }

        void OnStartGame(StartGame start)
        {
            Debug.Log("start game");
            UpdateGameStatus(GameStatus.Active);
            // deactivateHomeIsland = null;
            // deactivateHomeIsland = DeactivateHomeIsland();
            // StartCoroutine(deactivateHomeIsland);
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
            if(status == GameStatus.Active)
            {
                StopAllCoroutines();
                UpdateGameStatus(GameStatus.Ended);
            }
        }

        void OnRestartGame(RestartGame restartGame)
        {
            Reset();
            // UpdateGameStatus(GameStatus.Restart);
            FadeMenu(true);
        }

        void OnExitGame(ExitGame exitGame)
        {
            Reset();
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

        void OnCreateMapResponse(CreateMapResponse response)
        {
            currentMap = response.type;
            gotCurrentMap = true;
        }

        void OnStartRunwayResponse(StartRunwayResponse response)
        {
            if(response.type == currentMap)
            {
                gotRunwayResponse = true;
                hasRunway = response.hasRunway;
            }
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

        void FadeMenu(bool restarting = false)
        {
            if(!isStartingGame)
            {
                isStartingGame = true;
                if(fadeMenu != null)
                {
                    StopCoroutine(fadeMenu);
                    fadeMenu = null;
                }
                fadeMenu = _FadeMenu(restarting);
                StartCoroutine(fadeMenu);
            }
        }

        void Reset()
        {
            madeWorld = false;
            isStartingGame = false;
            gotCurrentMap = false;
            gotRunwayResponse = false;
            hasRunway = false;
        }
        #endregion

        #region Coroutines
        IEnumerator _FadeMenu(bool restarting)
        {
            float timer = 0;
            float fadeTime = 1;
            while(timer <= fadeTime)
            {
                timer += Time.deltaTime;
                Message.Send(new UpdateLoadingFadeMenu(timer / fadeTime));
                yield return null;
            }
            
            if(restarting)
            {
                UpdateGameStatus(GameStatus.Restart);
                timer = 0;
                while(timer <= 0.5f)
                {
                    timer += Time.deltaTime;
                    yield return null;
                }
            }

            Message.Send(new MakeWorld());
            while(!madeWorld)
            {
                yield return null;
            }
            
            UpdateGameStatus(GameStatus.CreatingMap); // need this game status update b4 creating the map b/c when all errything is activated they will request game status and if the game status is restart then we screwed
            Message.Send(new CreateMap());
            while(!gotCurrentMap)
            {
                yield return null;
            }

            UpdateGameStatus(GameStatus.Starting);
            Message.Send(new StartRunway(currentMap));
            while(!gotRunwayResponse)
            {
                yield return null;
            }

            timer = 0;
            float fadeOutTime = 0.5f;
            while(timer <= fadeOutTime)
            {
                timer += Time.deltaTime;
                Message.Send(new UpdateLoadingFadeMenu(1 - timer / fadeOutTime));
                yield return null;
            }

            if(!hasRunway)
            {
                Message.Send(new StartGame());
            }
            isStartingGame = false;
            yield break;
        }
        IEnumerator DeactivateHomeIsland()
        {
            Debug.Log("deactivating home");
            yield return wait4Seconds;
            Message.Send(new DeactivateHome());
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
        CreatingMap,
        Exit,
        Temp
    }
}
