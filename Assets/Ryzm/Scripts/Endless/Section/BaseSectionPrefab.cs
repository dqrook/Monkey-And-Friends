using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class BaseSectionPrefab : MonoBehaviour
    {
        #region Public Variables
        public EndlessSection section;
        public List<AddOnMap> addOnMaps = new List<AddOnMap>();
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
                Deactivate();
                SectionCombination combination = section.combinations.GetSectionCombinationByDifficulty(difficulty);
                int numRows = rows.Count;
                int numCoinAddOns = 0;
                int numOrbAddOns = 0;
                AddOnSpawn previousSpawn = new AddOnSpawn();
                for(int i = 0; i < numRows; i++)
                {
                    AddOnSpawn spawn = new AddOnSpawn();
                    if(previousSpawn.spawn == null || previousSpawn.type != AddOnSpawnType.Jump)
                    {
                        spawn = GetAddOnSpawn(rows[i].Activate(combination.subSectionCombinations[i]));
                    }
                    else
                    {
                        AddOnSpawnType spawnType = previousSpawn.type == AddOnSpawnType.Jump ? AddOnSpawnType.Straight : AddOnSpawnType.Jump;
                        spawn = GetAddOnSpawn(rows[i].Activate(combination.subSectionCombinations[i]), spawnType);
                    }
                    previousSpawn = spawn;
                    if(spawn.spawn != null)
                    {
                        foreach(AddOnMap map in addOnMaps)
                        {
                            if(map.type == spawn.type)
                            {
                                AddOn addOn = new AddOn();
                                if(numCoinAddOns == 2)
                                {
                                    addOn = map.GetAddOn(i, AddOnType.Orb);
                                }
                                else if(numOrbAddOns == 2)
                                {
                                    addOn = map.GetAddOn(i, AddOnType.Coin);
                                }
                                else
                                {
                                    addOn = map.GetAddOn(i);
                                }
                                if(addOn.transform != null)
                                {
                                    if(addOn.type == AddOnType.Coin)
                                    {
                                        numCoinAddOns++;
                                    }
                                    else
                                    {
                                        numOrbAddOns++;
                                    }
                                    addOn.transform.gameObject.SetActive(true);
                                    addOn.transform.position = spawn.spawn.position;
                                    addOn.transform.rotation = spawn.spawn.rotation;
                                }
                                break;
                            }
                        }
                    }
                }
            }
        }

        public void Deactivate()
        {
            foreach(SectionRow row in rows)
            {
                row.Deactivate();
            }
            foreach(AddOnMap map in addOnMaps)
            {
                map.Deactivate();
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
            int numCombos = approvedSectionCombinations.Count;
            for(int i = 0; i < numCombos; i++)
            {
                approvedSectionCombinations[i].index = i;
            }
            if(section?.combinations != null)
            {
                section.combinations.CreateCombinationGroups(approvedSectionCombinations);
            }
        }

        public void FillSectionCombinations()
        {
            approvedSectionCombinations.Clear();
            if(section?.combinations != null)
            {
                foreach(SectionCombinationsGroup group in section.combinations.combinationsGroups)
                {
                    foreach(SectionCombination combination in group.sectionCombinations)
                    {
                        approvedSectionCombinations.Add(combination);
                    }
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

        void UpdateRowCombinations()
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

        AddOnSpawn GetAddOnSpawn(List<EndlessSectionSpawn> spawns)
        {
            int numSpawns = spawns.Count;
            if(numSpawns == 0)
            {
                return new AddOnSpawn();
            }
            int rand = Random.Range(0, numSpawns);
            return spawns[rand].GetAddOnSpawn();
        }

        AddOnSpawn GetAddOnSpawn(List<EndlessSectionSpawn> spawns, AddOnSpawnType spawnType)
        {
            int numSpawns = spawns.Count;
            if(numSpawns == 0)
            {
                return new AddOnSpawn();
            }
            int rand = Random.Range(0, numSpawns);
            return spawns[rand].GetAddOnSpawn(spawnType);
        }
        #endregion
    }

    [System.Serializable]
    public class SectionRow
    {
        public List<SubSection> subSections = new List<SubSection>();

        public List<EndlessSectionSpawn> Activate(SubSectionCombination combination)
        {
            int numIndices = combination.activatedIndices.Count;
            List<EndlessSectionSpawn> spawns = new List<EndlessSectionSpawn>();
            for(int i = 0; i < numIndices; i++)
            {
                int index = combination.activatedIndices[i];
                spawns.Add(subSections[i].Activate(index));
            }
            return spawns;
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
        public EndlessSectionSpawn Activate(int index)
        {
            int numSelect = Spawns.Count;
            EndlessSectionSpawn spawn = null;
            for(int i = 0; i < numSelect; i++)
            {
                if(index == i)
                {
                    spawn = Spawns[i];
                }
                Spawns[i]?.gameObject?.SetActive(index == i);
            }
            return spawn;
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
                currentSubSecComboDex = dex;
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

        public SubSectionCombination(int rowIndex, List<int> activatedIndices, int totalDifficulty, float multipliedDifficulty)
        {
            this.rowIndex = rowIndex;
            foreach(int index in activatedIndices)
            {
                this.activatedIndices.Add(index);
            }
            this.totalDifficulty = totalDifficulty;
            this.multipliedDifficulty = multipliedDifficulty;
        }
    }

    [System.Serializable]
    public class SectionCombination
    {
        public int index;
        public int totalDifficulty;
        public float multipliedDifficulty;
        public List<SubSectionCombination> subSectionCombinations = new List<SubSectionCombination>();

        public SectionCombination() {}

        public SectionCombination(int index, int totalDifficulty, float multipliedDifficulty, List<SubSectionCombination> subSectionCombinations)
        {
            this.index = index;
            this.totalDifficulty = totalDifficulty;
            this.multipliedDifficulty = multipliedDifficulty;
            foreach(SubSectionCombination combination in subSectionCombinations)
            {
                this.subSectionCombinations.Add(new SubSectionCombination(combination.rowIndex, combination.activatedIndices, combination.totalDifficulty, combination.multipliedDifficulty));
            }
        }

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

    [System.Serializable]
    public struct AddOnMap
    {
        public AddOnSpawnType type;
        public Transform[] coinAddOns;
        public Transform[] orbAddOns;

        public AddOn GetAddOn(int index)
        {
            AddOn addOn = new AddOn();
            int numCoins = coinAddOns.Length;
            int numOrbs = orbAddOns.Length;
            bool skipCoins = index > numCoins - 1;
            bool skipOrbs = index > numOrbs - 1;

            if(!skipCoins && !skipOrbs)
            {
                int rand = Random.Range(0, 2);
                if(rand == 0)
                {
                    addOn.type = AddOnType.Coin;
                    addOn.transform = coinAddOns[index];
                }
                else
                {
                    addOn.type = AddOnType.Orb;
                    addOn.transform = orbAddOns[index];
                }
            }
            else if(!skipOrbs)
            {
                addOn.type = AddOnType.Orb;
                addOn.transform = orbAddOns[index];
            }
            else if(!skipCoins)
            {
                addOn.type = AddOnType.Coin;
                addOn.transform = coinAddOns[index];
            }

            return addOn;
        }

        public AddOn GetAddOn(int index, AddOnType type)
        {
            AddOn addOn = new AddOn();
            addOn.type = type;
            int numCoins = coinAddOns.Length;
            int numOrbs = orbAddOns.Length;
            bool skipCoins = index > numCoins - 1;
            bool skipOrbs = index > numOrbs - 1;

            if(type == AddOnType.Coin)
            {
                if(!skipCoins)
                {
                    addOn.transform = coinAddOns[index];
                }
            }
            else
            {
                if(!skipOrbs)
                {
                    addOn.transform = orbAddOns[index];
                }
            }
            return addOn;
        }

        // public Transform GetAddOn(int index)
        // {
        //     AddOn addOn = new AddOn();
        //     int numCoins = coinAddOns.Length;
        //     int numOrbs = orbAddOns.Length;
        //     bool skipCoins = index > numCoins - 1;
        //     bool skipOrbs = index > numOrbs - 1;

        //     if(skipCoins && skipOrbs)
        //     {
        //         return null;
        //     }
        //     else if(skipCoins)
        //     {
        //         return orbAddOns[index];
        //     }
        //     else if(skipOrbs)
        //     {
        //         return coinAddOns[index];
        //     }
        //     else
        //     {
        //         int rand = Random.Range(0, 2);
        //         if(rand == 0)
        //         {
        //             return coinAddOns[index];
        //         }
        //         return orbAddOns[index];
        //     }
        // }

        public void Deactivate()
        {
            foreach(Transform addOn in coinAddOns)
            {
                addOn.gameObject.SetActive(false);
            }
            foreach(Transform addOn in orbAddOns)
            {
                addOn.gameObject.SetActive(false);
            }
        }
    }

    [System.Serializable]
    public struct AddOn
    {
        public AddOnType type;
        public Transform transform;
    }

    public enum AddOnType
    {
        Coin,
        Orb
    }

    public enum AddOnSpawnType
    {
        Straight,
        Jump
    }

    [System.Serializable]
    public struct AddOnSpawn
    {
        public AddOnSpawnType type;
        public Transform spawn;
    }
}
