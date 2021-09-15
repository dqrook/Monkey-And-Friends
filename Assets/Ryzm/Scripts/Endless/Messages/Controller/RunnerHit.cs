using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class RunnerHit : Message
    {
        public int damage;
        public MonsterType monsterType;
        public AttackType attackType;

        public RunnerHit()
        {
            this.damage = 1000;
        }

        public RunnerHit(int damage)
        {
            this.damage = damage;
        }

        public RunnerHit(MonsterType monsterType, AttackType attackType)
        {
            this.monsterType = monsterType;
            this.attackType = attackType;
            this.damage = -1;
        }
    }
}
