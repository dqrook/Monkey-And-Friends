using System.Collections;
using System.Collections.Generic;
using Ryzm.EndlessRunner.Messages;
using UnityEngine;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class EndlessEnvironment : EndlessItem
    {
        public EndlessSection parentSection;
        public EnvironmentType type;
        public List<GameObject> randomSections = new List<GameObject>();

        protected override void Awake()
        {
            base.Awake();
            Message.AddListener<SectionDeactivated>(OnSectionDeactivated);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Message.RemoveListener<SectionDeactivated>(OnSectionDeactivated);
        }

        public void Initialize(EndlessSection section)
        {
            parentSection = section;
            int numRandSections = randomSections.Count;
            if(numRandSections > 0)
            {
                int enabledIndex = Random.Range(0, numRandSections);
                for(int i = 0; i < numRandSections; i++)
                {
                    randomSections[i].SetActive(i == enabledIndex);
                }
            }
        }

        protected override void OnGameStatusResponse(GameStatusResponse gameStatusResponse)
        {
            base.OnGameStatusResponse(gameStatusResponse);
            if(gameStatusResponse.status == GameStatus.Restart)
            {
                gameObject.SetActive(false);
            }
        }

        void OnSectionDeactivated(SectionDeactivated sectionDeactivated)
        {
            if(sectionDeactivated.section == parentSection)
            {
                gameObject.SetActive(false);
            }
        }

    }

    public enum EnvironmentType
    {
        FloatingRockBoth,
        FloatingRockRight,
        FloatingRockLeft
    }
}
