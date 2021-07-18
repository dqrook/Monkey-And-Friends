using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class EnvironmentManager : MonoBehaviour
    {
        public List<Environment> environments = new List<Environment>();

        void Awake()
        {
            Message.AddListener<GameStatusResponse>(OnGameStatusResponse);
        }
        
        void OnDestroy()
        {
            Message.RemoveListener<GameStatusResponse>(OnGameStatusResponse);
        }

        void OnGameStatusResponse(GameStatusResponse response)
        {
            foreach(Environment environment in environments)
            {
                environment.UpdateStatus(response.status);
            }
        }
    }

    [System.Serializable]
    public class Environment
    {
        public List<GameStatus> activeStatuses = new List<GameStatus>();
        public List<GameObject> gameObjects = new List<GameObject>();
        public List<GameStatus> inactiveStatuses = new List<GameStatus>();

        GameStatus status;
        bool initialized;
        bool isActive = false;

        public void UpdateStatus(GameStatus status)
        {
            if(status != this.status || !initialized)
            {
                if(activeStatuses.Contains(status))
                {
                    if(!isActive || !initialized)
                    {
                        foreach(GameObject go in gameObjects)
                        {
                            go.SetActive(true);
                        }
                    }
                    isActive = true;
                }
                else if(inactiveStatuses.Contains(status))
                {
                    if(isActive || !initialized)
                    {
                        foreach(GameObject go in gameObjects)
                        {
                            go.SetActive(false);
                        }
                    }
                    isActive = false;
                }
                this.status = status;
                initialized = true;
            }
        }
    }
}
