using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.Dragon.Messages;
using Ryzm.EndlessRunner;
using CodeControl;
using Ryzm.EndlessRunner.Messages;

namespace Ryzm.Dragon
{
    public class FloatingHomeIsland : MonoBehaviour
    {
        #region Public Variables
        public Transform endlessDragonSpawn;
        public List<DragonHomeSpawn> homeSpawns;
        #endregion
        
        #region Private Variables
        BaseDragon[] dragons = new BaseDragon[0];
        EndlessDragon endlessDragon;
        #endregion

        #region Event Functions
        void Awake()
        {
            Message.AddListener<DragonsResponse>(OnDragonsResponse);
            Message.AddListener<ResetDragons>(OnResetDragons);
            Message.AddListener<GameStatusResponse>(OnGameStatusResponse);
            Message.AddListener<ControllersResponse>(OnControllersResponse);
        }

        void Start()
        {
            Message.Send(new ControllersRequest());
        }

        void OnDestroy()
        {
            Message.RemoveListener<DragonsResponse>(OnDragonsResponse);
            Message.RemoveListener<ResetDragons>(OnResetDragons);
            Message.RemoveListener<GameStatusResponse>(OnGameStatusResponse);
            Message.RemoveListener<ControllersResponse>(OnControllersResponse);
        }
        #endregion
        
        #region Listener Functions
        void OnDragonsResponse(DragonsResponse response)
        {
            if(response.sender == "all")
            {
                dragons = response.dragons;
                Debug.Log("moving dragons");
                MoveDragons();
            }
            else if(response.sender == "newDragon" || response.sender == "marketUpdate")
            {
                dragons = response.dragons;
            }
        }

        void OnGameStatusResponse(GameStatusResponse response)
        {
            if(response.status == GameStatus.Restart || response.status == GameStatus.Exit || response.status == GameStatus.PreStarting)
            {
                endlessDragon.transform.position = endlessDragonSpawn.position;
                endlessDragon.transform.rotation = endlessDragonSpawn.rotation;
            }
        }

        void OnControllersResponse(ControllersResponse response)
        {
            endlessDragon = response.dragon;
        }

        void OnResetDragons(ResetDragons reset)
        {
            MoveDragons();
        }
        #endregion

        #region Private Functions
        void MoveDragons()
        {
            int index = 0;
            foreach(BaseDragon dragon in dragons)
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
        #endregion
    }

    [System.Serializable]
    public class DragonHomeSpawn
    {
        public int order;
        public Transform spawn;
    }
}
