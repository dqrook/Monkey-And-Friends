using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class EndlessWall : EndlessItem
    {
        #region Public Variables
        public WallType type;
        public List<GameObject> environments = new List<GameObject>();
        #endregion

        #region Private Variables
        int startRowId;
        int endRowId;
        int randEnv;
        bool disableOnStart;
        #endregion

        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            Message.AddListener<RowChange>(OnRowChange);
        }
        
        protected void OnDisable()
        {
            startRowId = -1;
            endRowId = -1;
            randEnv = 0;
            disableOnStart = false;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Message.RemoveListener<RowChange>(OnRowChange);
        }
        #endregion

        #region Listener Functions
        void OnRowChange(RowChange change)
        {
            if(change.oldRowId == startRowId)
            {
                endRowId = change.newRowId;
                if(disableOnStart)
                {
                    int dex = 0;
                    foreach(GameObject env in environments)
                    {
                        env.SetActive(dex == randEnv);
                        dex++;
                    }
                }
            }
            else if(change.oldRowId == endRowId)
            {
                gameObject.SetActive(false);
            }
        }
        #endregion

        #region Public Functions
        public void Initialize(int startRowId, bool disableOnStart)
        {
            this.startRowId = startRowId;
            this.disableOnStart = disableOnStart;
            if(environments.Count > 0)
            {
                randEnv = Random.Range(0, environments.Count);
                int dex = 0;
                foreach(GameObject env in environments)
                {
                    env.SetActive(!disableOnStart && dex == randEnv);
                    dex++;
                }
            }
        }
        #endregion


    }

    public enum WallType
    {
        RockGrass1
    }

    public enum WallSide
    {
        Right,
        Left,
        Both
    }
}
