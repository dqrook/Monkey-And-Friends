﻿namespace Ryzm.EndlessRunner
{
    public class MonsterHitpoint : MonsterBase
    {
        public EndlessMonster monster;

        public override void TakeDamage()
        {
            monster.TakeDamage();
        }

        public override void TakeSpecialDamage()
        {
            monster.TakeSpecialDamage();
        }
    }
}
