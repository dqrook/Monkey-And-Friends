using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class GameManager : MonoBehaviour
    {
        public RunnerController runner;
        private static GameManager _instance;
        public static GameManager Instance { get { return _instance; } }
        GameObject _currentPlatform;
        EndlessSection _currentSection;
        EndlessTSection _currentTSection;

        public GameObject CurrentPlatform
        {
            get
            {
                return _currentPlatform;
            }
            set
            {
                _currentPlatform = value;
                _currentTSection = _currentPlatform.GetComponent<EndlessTSection>();
                _currentSection = _currentPlatform.GetComponent<EndlessSection>();
            }
        }

        public EndlessSection CurrentSection
        {
            get
            {
                return _currentTSection != null ? _currentTSection : _currentSection;
            }
        }

        public EndlessTSection CurrentTSection
        {
            get
            {
                return _currentTSection;
            }
        }

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(this.gameObject);
            } 
            else 
            {
                _instance = this;
            }
            if(runner == null)
            {
                runner = FindObjectOfType<RunnerController>();
            }
            Message.AddListener<CurrentPlatformChange>(OnCurrentPlatformChange);
        }

        void OnDestroy()
        {
            Message.RemoveListener<CurrentPlatformChange>(OnCurrentPlatformChange);
        }

        void OnCurrentPlatformChange(CurrentPlatformChange change)
        {
            CurrentPlatform = change.platform;
        }
    }
}
