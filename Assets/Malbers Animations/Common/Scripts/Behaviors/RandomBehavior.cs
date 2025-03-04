﻿using UnityEngine;
using MalbersAnimations.Scriptables;
using UnityEngine.Animations;

namespace MalbersAnimations
{
    /// <summary>Is Used to execute random animations in a State Machine</summary>
    public class RandomBehavior : StateMachineBehaviour
    {
        public IntReference Range = new IntReference(15);
        public IntReference Priority = new IntReference(0);

        IRandomizer randomizer;

        override public void OnStateMachineEnter(Animator animator, int stateMachinePathHash)
        {
            if (randomizer == null) 
                randomizer = animator.FindInterface<IRandomizer>();

            randomizer?.SetRandom(Random.Range(1, Range + 1), Priority.Value);
        }

        public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
            randomizer?.ResetRandomPriority( Priority);
        }
    }

    public interface IRandomizer
    {
        void SetRandom(int value, int priority);
        void ResetRandomPriority(int priority);
    }
}