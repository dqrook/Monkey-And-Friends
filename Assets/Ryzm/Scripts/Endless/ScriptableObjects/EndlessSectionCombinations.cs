using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Ryzm.EndlessRunner
{
    [CreateAssetMenu(fileName = "SectionCombinations", menuName = "ScriptableObjects/EndlessSectionCombinations", order = 2)]
    public class EndlessSectionCombinations : ScriptableObject
    {
        public List<SectionCombinationsGroup> combinationsGroups = new List<SectionCombinationsGroup>();
        List<SectionCombinationsGroup> currentGroups = new List<SectionCombinationsGroup>();

        public void CreateCombinationGroups(List<SectionCombination> approvedCombinations)
        {
            combinationsGroups.Clear();
            foreach(SectionCombination combination in approvedCombinations)
            {
                SectionCombinationsGroup newGroup = new SectionCombinationsGroup();
                newGroup.difficulty = combination.multipliedDifficulty;
                bool foundIt = false;
                foreach(SectionCombinationsGroup group in combinationsGroups)
                {
                    if(group.difficulty == combination.multipliedDifficulty)
                    {
                        newGroup = group;
                        foundIt = true;
                        break;
                    }
                }
                newGroup.sectionCombinations.Add(combination);
                if(!foundIt)
                {
                    combinationsGroups.Add(newGroup);
                }
            }
            combinationsGroups = combinationsGroups.OrderBy(x => x.difficulty).ToList();
        }

        public SectionCombination GetSectionCombinationByDifficulty(GameDifficulty difficulty)
        {
            int minValue = 16;
            int maxValue = 1000;
            if(difficulty == GameDifficulty.Easy)
            {
                minValue = 0;
                maxValue = 7;
            }
            else if(difficulty == GameDifficulty.Medium)
            {
                minValue = 8;
                maxValue = 15;
            }
            currentGroups.Clear();
            foreach(SectionCombinationsGroup group in combinationsGroups)
            {
                if(group.difficulty >= minValue && group.difficulty <= maxValue)
                {
                    currentGroups.Add(group);
                }
            }
            int randGroup = Random.Range(0, currentGroups.Count);
            int randCombo = Random.Range(0, currentGroups[randGroup].sectionCombinations.Count);
            
            return currentGroups[randGroup].sectionCombinations[randCombo];
        }
    }

    [System.Serializable]
    public class SectionCombinationsGroup
    {
        public float difficulty;
        public List<SectionCombination> sectionCombinations = new List<SectionCombination>();
    }
}
