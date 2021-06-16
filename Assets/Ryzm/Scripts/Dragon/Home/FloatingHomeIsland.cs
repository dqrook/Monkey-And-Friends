using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.Dragon.Messages;
using Ryzm.EndlessRunner;
using CodeControl;

namespace Ryzm.Dragon
{
    public class FloatingHomeIsland : MonoBehaviour
    {
        public List<DragonHomeSpawn> homeSpawns;
        List<EndlessDragon> dragons;

        void Awake()
        {
            Message.AddListener<DragonsResponse>(OnDragonsResponse);
            Message.AddListener<ResetDragons>(OnResetDragons);
        }

        void OnDestroy()
        {
            Message.RemoveListener<DragonsResponse>(OnDragonsResponse);
            Message.RemoveListener<ResetDragons>(OnResetDragons);
        }
        
        void OnDragonsResponse(DragonsResponse response)
        {
            if(response.sender == "floatingHomeIsland" || response.sender == "all")
            {
                dragons = response.dragons;
                MoveDragons();
            }
        }

        void OnResetDragons(ResetDragons reset)
        {
            MoveDragons();
        }

        void MoveDragons()
        {
            int index = 0;
            foreach(EndlessDragon dragon in dragons)
            {
                DragonHomeSpawn spawn = GetSpawn(index);
                if(spawn != null)
                {
                    dragon.transform.position = spawn.spawn.position;
                    dragon.transform.rotation = spawn.spawn.rotation;
                    dragon.gameObject.SetActive(true);
                }
                else
                {
                    dragon.gameObject.SetActive(false);
                }
                index++;
            }
        }

        DragonHomeSpawn GetSpawn(int order)
        {
            foreach(DragonHomeSpawn spawn in homeSpawns)
            {
                if(spawn.order == order)
                {
                    return spawn;
                }
            }
            return null;
        }
    }

    [System.Serializable]
    public class DragonHomeSpawn
    {
        public int order;
        public Transform spawn;
    }
}
