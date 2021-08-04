using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessStonePillar : EndlessBarrier
    {
        protected override void OnCollisionEnter(Collision other)
        {
            Debug.Log("collided ya nerd LOL");
        }
    }
}
