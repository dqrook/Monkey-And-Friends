using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class GameDifficultyResponse : Message
    {
        public GameDifficulty difficulty;

        public GameDifficultyResponse(GameDifficulty difficulty)
        {
            this.difficulty = difficulty;
        }
    }
}
