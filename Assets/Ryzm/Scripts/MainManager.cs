﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.Messages;
using CodeControl;

namespace Ryzm
{
    public class MainManager : MonoBehaviour
    {
        public GameType type;

        void Awake()
        {
            Message.AddListener<GameTypeRequest>(OnGameTypeRequest);
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
