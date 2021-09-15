using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class MonsterBase : MonoBehaviour
    {
        public virtual void TakeDamage() {}

        public virtual void TakeSpecialDamage()
        {
            TakeDamage();
        }
    }
}
