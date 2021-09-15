using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.Messages;
using CodeControl;

namespace Ryzm
{
    public class MainManager : MonoBehaviour
    {
        public GameType type;
        public int defaultHeight;
        public int defaultWidth;

        void Awake()
        {
            Message.AddListener<GameTypeRequest>(OnGameTypeRequest);
            
            #if !UNITY_EDITOR
            Application.targetFrameRate = 60;
            #endif

            defaultHeight = Screen.height;
            defaultWidth = Screen.width;
            // float percent = 0.75f;
            // Screen.SetResolution(Mathf.RoundToInt(percent * defaultWidth), Mathf.RoundToInt(percent * defaultHeight), true);
        }

        void OnDestroy()
        {
            Message.RemoveListener<GameTypeRequest>(OnGameTypeRequest);
        }

        void OnGameTypeRequest(GameTypeRequest request)
        {
            Message.Send(new GameTypeResponse(type));
        }
    }

    public enum GameType
    {
        Breeding,
        EndlessRunner
    }
}
