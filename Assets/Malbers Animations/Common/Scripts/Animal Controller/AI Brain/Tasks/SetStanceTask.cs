﻿using UnityEngine;
namespace MalbersAnimations.Controller.AI
{
    [CreateAssetMenu(menuName = "Malbers Animations/Pluggable AI/Tasks/Set Stance")]
    public class SetStanceTask : MTask
    {
        public override string DisplayName => "Animal/Set Stance";


        [Space, Tooltip("Apply the Task to the Animal(Self) or the Target(Target)")]
        public Affected affect = Affected.Self;
        public StanceID stance;
        
        public override void StartTask(MAnimalBrain brain, int index)
        {
            switch (affect)
            {
                case Affected.Self:
                    Stance_Set(brain.Animal);
                    break;
                case Affected.Target:
                    Stance_Set(brain.TargetAnimal);
                    break;
            }

            brain.TaskDone(index);
        }

        public void Stance_Set(MAnimal animal) => animal?.Stance_Set(stance);

        void Reset()   { Description = "Set a stance on the Animal"; }
    }
}
