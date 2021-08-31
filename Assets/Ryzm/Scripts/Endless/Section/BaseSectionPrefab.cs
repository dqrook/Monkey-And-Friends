using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class BaseSectionPrefab : MonoBehaviour
    {
        #region Public Variables
        public EndlessSection section;
        public List<SectionRow> rows = new List<SectionRow>();
        public List<RowCombination> rowCombinations = new List<RowCombination>();
        public List<SectionCombination> approvedSectionCombinations = new List<SectionCombination>();
        public int rowCombinationIndex = 0;
        public int approvedComboIndex = -1;
        #endregion

        #region Properties
        public int SubSectionComboIndex 
        {
            get
            {
                if(rowCombinationIndex < rowCombinations.Count)
                {
                    return rowCombinations[rowCombinationIndex].currentSubSecComboDex;
                }
                return 0;
            }
            set
            {
                rowCombinations[rowCombinationIndex].currentSubSecComboDex = value;
            }
        }

        List<SubSectionCombination> CurrentSubSectionCombos 
        {
            get
            {
                return rowCombinations[rowCombinationIndex].subSectionCombinations;
            }
        }

        SectionCombination CurrentApprovedCombination
        {
            get
            {
                return approvedSectionCombinations[approvedComboIndex];
            }
        }

        SubSectionCombination CurrentSubSectionCombo
        {
            get
            {
                return rowCombinations[rowCombinationIndex].CurrentCombination;
            }
        }
        #endregion

        #region Public Functions
        public void Activate(GameDifficulty difficulty)
        {
            if(section?.combinations != null)
            {
                SectionCombination combination = section.combinations.GetCombinationGroupByDifficulty(difficulty);
                int numRows = rows.Count;
                for(int i = 0; i < numRows; i++)
                {
                    rows[i].Activate(combination.subSectionCombinations[i]);
                }
            }
        }

        public void Deactivate()
        {
            foreach(SectionRow row in rows)
            {
                row.Deactivate();
            }
        }

        public void GetCombinations()
        {
            rowCombinations.Clear();
            List<int> sectionLengths = new List<int>();
            int rowIndex = 0;
            foreach(SectionRow row in rows)
            {
                List<SubSectionCombination> subSectionCombos = new List<SubSectionCombination>();
                sectionLengths.Clear();
                foreach(SubSection subSection in row.subSections)
                {
                    sectionLengths.Add(subSection.Selections.Count);
                }
                int numInitCombos = sectionLengths[0];
                for(int i = 0; i < numInitCombos; i++)
                {
                    SubSectionCombination combo = new SubSectionCombination();
                    combo.activatedIndices.Add(i);
                    subSectionCombos.Add(combo);
                }
                int secLengths = sectionLengths.Count;
                for(int i = 1; i < secLengths; i++)
                {
                    List<SubSectionCombination> ssCombos = new List<SubSectionCombination>();
                    for(int j = 0; j < sectionLengths[i]; j++)
                    {
                        foreach(SubSectionCombination rowCombination in subSectionCombos)
                        {
                            SubSectionCombination ssc = new SubSectionCombination();
                            foreach(int shit in rowCombination.activatedIndices)
                            {
                                ssc.activatedIndices.Add(shit);
                            }
                            ssc.activatedIndices.Add(j);
                            ssCombos.Add(ssc);
                        }
                    }
                    subSectionCombos = ssCombos;
                }
                RowCombination secCombo = new RowCombination();
                secCombo.subSectionCombinations = subSectionCombos;
                rowCombinations.Add(secCombo);
                rowIndex++;
            }
            UpdateRowCombinations();
        }

        public void Reset()
        {
            foreach(RowCombination combo in rowCombinations)
            {
                combo.currentSubSecComboDex = 0;
            }
            rowCombinationIndex = 0;
            approvedComboIndex = 0;
            UpdateRows();
        }

        public void NextSubSecCombo()
        {
            SubSectionComboIndex++;
            SubSectionComboIndex = SubSectionComboIndex < CurrentSubSectionCombos.Count ? SubSectionComboIndex : 0;
            UpdateRows();
        }

        public void PreviousSubSecCombo()
        {
            SubSectionComboIndex--;
            SubSectionComboIndex = SubSectionComboIndex >= 0 ? SubSectionComboIndex : CurrentSubSectionCombos.Count - 1;
            UpdateRows();
        }

        public void NextRowCombo()
        {
            rowCombinationIndex++;
            rowCombinationIndex = rowCombinationIndex < rowCombinations.Count ? rowCombinationIndex : 0;
            UpdateRows();
        }

        public void PreviousRowCombo()
        {
            rowCombinationIndex--;
            rowCombinationIndex = rowCombinationIndex >= 0 ? rowCombinationIndex : rowCombinations.Count - 1;
            UpdateRows();
        }

        public void SaveApprovedCombo()
        {
            List<SubSectionCombination> subSectionCombinations = new List<SubSectionCombination>();
            foreach(RowCombination rowCombination in rowCombinations)
            {
                subSectionCombinations.Add(rowCombination.CurrentCombination);
            }
            SectionCombination sectionCombination = new SectionCombination();
            sectionCombination.subSectionCombinations = subSectionCombinations;
            sectionCombination.UpdateTotalDifficulty();
            approvedSectionCombinations.Add(sectionCombination);

        }

        public void NextApprovedCombo()
        {
            approvedComboIndex++;
            approvedComboIndex = approvedComboIndex < approvedSectionCombinations.Count ? approvedComboIndex : 0;
            UpdateApprovedCombination();
        }

        public void PreviousApprovedCombo()
        {
            approvedComboIndex--;
            approvedComboIndex = approvedComboIndex >= 0 ? approvedComboIndex : approvedSectionCombinations.Count - 1;
            UpdateApprovedCombination();
        }

        public void RemoveApprovedCombo()
        {
            approvedSectionCombinations.RemoveAt(approvedComboIndex);
        }

        public void CreateEndlessSectionCombinations()
        {
            if(section?.combinations != null)
            {
                section.combinations.CreateCombinationGroups(approvedSectionCombinations);
            }
        }

        public void UpdateRowCombinations()
        {
            int numSecCombos = rowCombinations.Count;
            
            for(int i = 0; i < numSecCombos; i++)
            {
                RowCombination rowCombo = rowCombinations[i];
                foreach(SubSectionCombination subSectionCombo in rowCombo.subSectionCombinations)
                {
                    subSectionCombo.rowIndex = i;
                    SectionRow row = rows[i];
                    int totalDifficulty = 0;
                    int numGreaterThan0 = 0;
                    int numSubSections = row.subSections.Count;
                    for(int j = 0; j < numSubSections; j++)
                    {
                        EndlessSectionSpawn ss = row.subSections[j].Spawns[subSectionCombo.activatedIndices[j]];
                        totalDifficulty += ss.difficultyPoints;
                        if(ss.difficultyPoints > 0)
                        {
                            numGreaterThan0++;
                        }
                    }
                    subSectionCombo.totalDifficulty = totalDifficulty;
                    float multiplier = numGreaterThan0 < 2 ? 1 : 1 + 0.5f * (numGreaterThan0 - 1);
                    subSectionCombo.multipliedDifficulty = Mathf.Ceil(multiplier * totalDifficulty);
                }
            }
        }
        #endregion

        #region Private Functions
        void UpdateRows()
        {
            int numSubSections = rows[rowCombinationIndex].subSections.Count;

            for(int i = 0; i < numSubSections; i++)
            {
                int sub2Activaate = CurrentSubSectionCombo.activatedIndices[i];
                rows[rowCombinationIndex].subSections[i].Activate(sub2Activaate);
            }
        }

        void UpdateApprovedCombination()
        {
            int numRows = rows.Count;
            for(int i = 0; i < numRows; i++)
            {
                rows[i].Deactivate();
                rows[i].Activate(CurrentApprovedCombination.subSectionCombinations[i]);
            }
            
            int numRowCombinations = rowCombinations.Count;
            for(int i = 0; i < numRowCombinations; i++)
            {
                rowCombinations[i].CurrentCombination = CurrentApprovedCombination.subSectionCombinations[i];
            }
        }
        #endregion
    }

    [System.Serializable]
    public class SectionRow
    {
        public List<SubSection> subSections = new List<SubSection>();

        public void Activate(SubSectionCombination combination)
        {
            int numIndices = combination.activatedIndices.Count;
            for(int i = 0; i < numIndices; i++)
            {
                int index = combination.activatedIndices[i];
                subSections[i].Activate(index);
            }
        }

        public void Deactivate()
        {
            foreach(SubSection subSection in subSections)
            {
                subSection.Deactivate();
            }
        }
    }

    [System.Serializable]
    public class SubSection
    {
        #region Public Variables
        public Transform parent;
        public List<GameObject> selections = new List<GameObject>();
        #endregion

        #region Private Variables
        List<EndlessSectionSpawn> _spawns = new List<EndlessSectionSpawn>();
        #endregion

        #region Properties
        public List<GameObject> Selections
        {
            get
            {
                if(parent != null)
                {
                    selections.Clear();
                    foreach(Transform t in parent)
                    {
                        if(t.parent == parent)
                        {
                            selections.Add(t.gameObject);
                        }
                    }
                }
                return selections;
            }
        }

        public List<EndlessSectionSpawn> Spawns
        {
            get
            {
                if(_spawns.Count == 0)
                {
                    foreach(GameObject sel in Selections)
                    {
                        _spawns.Add(sel.GetComponent<EndlessSectionSpawn>());
                    }
                }
                return _spawns;
            }
        }
        #endregion

        #region Public Functions
        public void Activate(int index)
        {
            int numSelect = Spawns.Count;
            for(int i = 0; i < numSelect; i++)
            {
                Spawns[i].gameObject.SetActive(index == i);
            }
        }

        public void Deactivate()
        {
            foreach(GameObject go in selections)
            {
                go.SetActive(false);
            }
        }
        #endregion
    }

    [System.Serializable]
    public class RowCombination
    {
        public int currentSubSecComboDex = -1;
        public List<SubSectionCombination> subSectionCombinations = new List<SubSectionCombination>();

        public SubSectionCombination CurrentCombination
        {
            get
            {
                int dex = currentSubSecComboDex > 0 ? currentSubSecComboDex : 0;
                return subSectionCombinations[dex];
            }
            set
            {
                int dex = currentSubSecComboDex;
                int numSubSections = subSectionCombinations.Count;
                for(int i = 0; i < subSectionCombinations.Count; i++)
                {
                    SubSectionCombination subSectionCombination = subSectionCombinations[i];
                    int numIndices = value.activatedIndices.Count;
                    bool foundIt = true;
                    for(int j = 0; j < numIndices; j++)
                    {
                        if(value.activatedIndices[j] != subSectionCombination.activatedIndices[j])
                        {
                            foundIt = false;
                            break;
                        }
                    }
                    if(foundIt)
                    {
                        dex = i;
                        break;
                    }
                }
            }
        }
    }

    [System.Serializable]
    public class SubSectionCombination
    {
        public int rowIndex;
        public List<int> activatedIndices = new List<int>();
        public int totalDifficulty;
        public float multipliedDifficulty;

        public SubSectionCombination() {}
    }

    [System.Serializable]
    public class SectionCombination
    {
        public int totalDifficulty;
        public float multipliedDifficulty;
        public List<SubSectionCombination> subSectionCombinations = new List<SubSectionCombination>();

        public SectionCombination() {}

        public void UpdateTotalDifficulty()
        {
            totalDifficulty = 0;
            multipliedDifficulty = 0;
            foreach(SubSectionCombination combination in subSectionCombinations)
            {
                totalDifficulty += combination.totalDifficulty;
                multipliedDifficulty += combination.multipliedDifficulty;
            }
        }
    }
}
