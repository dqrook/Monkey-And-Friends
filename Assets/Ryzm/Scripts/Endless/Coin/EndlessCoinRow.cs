using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessCoinRow : EndlessBarrier
    {
        public List<EndlessCoin> coins = new List<EndlessCoin>();

        public override void Initialize(Transform parentTransform, int position)
        {
            base.Initialize(parentTransform, position);
            EndlessCoinRowSpawn spawn = parentTransform.GetComponent<EndlessCoinRowSpawn>();
            if(spawn != null)
            {
                int numSpawns = spawn.coinSpawns.Count;
                int dex = 0;
                foreach(EndlessCoin coin in coins)
                {
                    if(dex < numSpawns)
                    {
                        coin.transform.position = new Vector3(coin.transform.position.x, spawn.coinSpawns[dex].position.y, coin.transform.position.z);
                    }
                    else
                    {
                        break;
                    }
                    dex++;
                }
            }
        }
    }
}
