namespace Ryzm.EndlessRunner
{
    public class EndlessSideDraze : EndlessDiveDraze
    {
        protected override void Awake()
        {
            base.Awake();
            dropSpeed = 15;
            barrierType = BarrierType.SideDragon;
        }
    }
}
