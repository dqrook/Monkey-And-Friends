using CodeControl;

namespace Ryzm.EndlessRunner.Messages
{
    public class MonsterMetadataResponse : Message
    {
        public EndlessMonsterMetadata monsterMetadata;

        public MonsterMetadataResponse(EndlessMonsterMetadata monsterMetadata)
        {
            this.monsterMetadata = monsterMetadata;
        }
    }
}
