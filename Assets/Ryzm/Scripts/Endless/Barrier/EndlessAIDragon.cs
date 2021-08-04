using System.Collections;
using System.Collections.Generic;
using Ryzm.EndlessRunner.Messages;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessAIDragon : EndlessBarrier
    {
        #region Public Variables
        public Animator animator;
        public Transform childTransform;
        public DragonFire fire;
        #endregion
        
        #region Protected Variables
        protected bool startedCoroutine;
        protected IEnumerator _flyToPosition;
        protected Vector3 initialPosition;
        protected Vector3 initialEulerAngles;
        protected GameObject childGO;
        WaitForSeconds wait2Seconds = new WaitForSeconds(2);
        IEnumerator waitThenDisable;
        Transform spawnTrans;
        Vector3 spawnOrigPos;
        #endregion

        #region Event Functions
        protected override void Awake()
        {
            base.Awake();
            if(childTransform == null)
            {
                childTransform = GetComponentInChildren<Transform>();
            }
            childGO = childTransform.gameObject;
            initialPosition = childTransform.localPosition;
            initialEulerAngles = childTransform.localEulerAngles;
            if(animator == null)
            {
                animator = GetComponent<Animator>();
            }
            childGO.SetActive(false);
        }

        // protected virtual void FixedUpdate()
        // {
        //     if(!startedCoroutine)
        //     {
        //         if(gameStatus == GameStatus.Active && parentSection == _currentSection)
        //         {
        //             Transform location = parentSection.GetSpawnTransformForBarrier(type, runnerPosition);
        //             if(location != null)
        //             {
        //                 gameObject.transform.position = location.position;
        //                 gameObject.transform.rotation = location.rotation;
        //             }
        //             _flyToPosition = FlyToPosition();
        //             StartCoroutine(_flyToPosition);
        //         }
        //     }
        // }

        protected override void OnDisable()
        {
            _currentSection = null;
            animator.SetBool("fireBreath", false);
            startedCoroutine = false;
            StopAllCoroutines();
            childTransform.localPosition = initialPosition;
            childTransform.localEulerAngles = initialEulerAngles;
            childGO.SetActive(false);
            ResetSpawns();
        }
        #endregion

        #region Listener Functions
        protected override void OnCurrentSectionChange(CurrentSectionChange sectionChange)
        {
            base.OnCurrentSectionChange(sectionChange);
            if(!startedCoroutine)
            {
                CheckStartFly();
                childGO.SetActive(parentSection == _currentSection);
            }
            else if(parentSection != _currentSection)
            {
                // just left the dragon's parent section therefore need to disable the child object
                waitThenDisable = null;
                waitThenDisable = WaitThenDisable();
                StartCoroutine(waitThenDisable);
            }
        }

        protected override void OnGameStatusResponse(GameStatusResponse gameStatusResponse)
        {
            base.OnGameStatusResponse(gameStatusResponse);
            if(gameStatusResponse.status == GameStatus.Ended)
            {
                if(spawnTrans != null)
                {
                    spawnTrans.parent = this.transform;
                    spawnTrans.localPosition = spawnOrigPos;
                    spawnTrans = null;
                }
            }
        }
        #endregion

        #region Public Functions
        public override void Initialize(Transform parentTransform, int position)
        {
            if(CanPlaceRow(rowLikelihood))
            {
                foreach(RowSpawn spawn in spawns)
                {
                    if(spawn.position == position)
                    {
                        GameObject spawnGO = spawn.EnableRandomRow();
                        if(spawnGO != null)
                        {
                            spawnTrans = spawnGO.transform;
                            spawnOrigPos = spawnTrans.localPosition;
                            spawnTrans.parent = this.transform.parent;
                        }
                        break;
                    }
                }
            }
            else
            {
                ResetSpawns();
            }
        }
        #endregion

        #region Protected Functions
        protected void CheckStartFly()
        {
            if(!startedCoroutine && gameStatus == GameStatus.Active && _currentSection == parentSection)
            {
                Transform location = parentSection.GetSpawnTransformForBarrier(type, runnerPosition);
                if(location != null)
                {
                    gameObject.transform.position = location.position;
                    gameObject.transform.rotation = location.rotation;
                }
                gameObject.SetActive(true);
                _flyToPosition = FlyToPosition();
                StartCoroutine(_flyToPosition);
            }
        }

        protected override void ResetSpawns()
        {
            base.ResetSpawns();
            if(spawnTrans != null)
            {
                spawnTrans.parent = this.transform;
                spawnTrans.localPosition = spawnOrigPos;
                spawnTrans = null;
            }
        }
        #endregion


        #region Coroutines
        protected virtual IEnumerator FlyToPosition()
        {
            yield break;
        }

        IEnumerator WaitThenDisable()
        {
            yield return wait2Seconds;
            ResetSpawns();
            childGO.SetActive(parentSection == _currentSection);
        }
        #endregion
    }
}
