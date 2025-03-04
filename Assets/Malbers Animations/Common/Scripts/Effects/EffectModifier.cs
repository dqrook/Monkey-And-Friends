﻿using UnityEngine;

namespace MalbersAnimations.Utilities
{
    /// <summary>Scriptable object to modify the effect parameters before, during or after the effect plays. Used on With the Effect Manager</summary>
    public abstract class EffectModifier : ScriptableObject
    {
        [TextArea]
        public string Description = string.Empty;

        public virtual void AwakeEffect(Effect effect) { }

        public virtual void StartEffect(Effect effect) { }

        //public virtual void UpdateEffect(Effect effect) { }

        //public virtual void LateUpdateEffect(Effect effect) { }

        public virtual void StopEffect(Effect effect) { }
    }
}