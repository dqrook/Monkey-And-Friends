using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Ryzm.EndlessRunner
{
    public class BaseSectionPrefab : MonoBehaviour
    {
        #region Public Variables
        public EndlessSection section;
        public List<AddOnMap> addOnMaps = new List<AddOnMap>();
        public List<SectionRow> rows = new List<SectionRow>();
        public List<RowCombination> rowCombinations = new List<RowCombination>();
        public List<SectionCombination> generatedSectionCombinations = new List<SectionCombination>();
        public int rowCombinationIndex = 0;
        public int generatedComboIndex = -1;
        public EndlessMonster newMonsterPrefab;
        public MonsterType targetMonsterType;
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

        SectionCombination CurrentGeneratedCombination
        {
            get
            {
                return generatedSectionCombinations[generatedComboIndex];
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

        public void ReplaceMonster()
        {
            foreach(SectionRow row in rows)
            {
                foreach(SubSection subSection in row.subSections)
                {
                    foreach(EndlessSectionSpawn spawn in subSection.spawns)
                    {
                        if(spawn.monsterType == targetMonsterType)
                        {
                            EndlessMonster[] monsters = spawn.transform.GetComponentsInChildren<EndlessMonster>();
                            EndlessMonster targetMonster = monsters[0];
                            Transform targetTrans = targetMonster.transform;
                            if(targetMonster.type == targetMonsterType)
                            {
                                EndlessMonster newMonster = Instantiate(newMonsterPrefab);
                                Transform newTrans = newMonster.transform;
                                newTrans.parent = targetTrans.parent;
                                newTrans.localPosition = targetTrans.localPosition;
                                newTrans.localEulerAngles = targetTrans.localEulerAngles;
                                string[] targetName = targetTrans.parent.gameObject.name.Split('-');
                                targetTrans.parent.gameObject.name = targetName[0] + "-" + newMonster.type.ToString().ToLower();
                                spawn.monsterType = newMonster.type;
                                DestroyImmediate(targetMonster.gameObject);
                            }
                        }
                    }
                }
            }
        }

        #region Public Functions
        public void Activate(GameDifficulty difficulty)
        {
            if(section?.combinations != null)
            {
                Deactivate();
                SectionCombination combination = section.combinations.GetSectionCombinationByDifficulty(difficulty);
                _Activate(combination);
            }
        }
        
        public void Activate(float runnerDistance)
        {
            if(section?.combinations != null)
            {
                Deactivate();
                SectionCombination combination = section.combinations.GetSectionCombinationByDistance(runnerDistance);
                _Activate(combination);
            }
        }

        void _Activate(SectionCombination combination)
        {
            int numRows = rows.Count;
            int numCoinAddOns = 0;
            int numOrbAddOns = 0;
            AddOnSpawn previousSpawn = new AddOnSpawn();
            for(int i = 0; i < numRows; i++)
            {
                AddOnSpawn spawn = new AddOnSpawn();
                if(previousSpawn.spawn == null) // || previousSpawn.type != AddOnSpawnType.Jump
                {
                    spawn = GetAddOnSpawn(rows[i].Activate(combination.subSectionCombinations[i]));
                }
                else
                {
                    spawn = GetAddOnSpawn(rows[i].Activate(combination.subSectionCombinations[i]), AddOnSpawnType.Straight, previousSpawn.subSectionPosition);
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
                    sectionLengths.Add(subSection.spawns.Count);
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
                        foreach(SubSectionCombination subSecCombination in subSectionCombos)
                        {
                            SubSectionCombination ssc = new SubSectionCombination();
                            foreach(int activatedIndex in subSecCombination.activatedIndices)
                            {
                                ssc.activatedIndices.Add(activatedIndex);
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
            generatedComboIndex = 0;
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

        public void SaveGeneratedCombo()
        {
            List<SubSectionCombination> subSectionCombinations = new List<SubSectionCombination>();
            foreach(RowCombination rowCombination in rowCombinations)
            {
                subSectionCombinations.Add(rowCombination.CurrentCombination);
            }
            SectionCombination sectionCombination = new SectionCombination();
            sectionCombination.subSectionCombinations = subSectionCombinations;
            sectionCombination.UpdateTotalDifficulty();
            generatedSectionCombinations.Add(sectionCombination);

        }

        public void NextGeneratedCombo()
        {
            generatedComboIndex++;
            generatedComboIndex = generatedComboIndex < generatedSectionCombinations.Count ? generatedComboIndex : 0;
            UpdateApprovedCombination();
        }

        public void PreviousGeneratedCombo()
        {
            generatedComboIndex--;
            generatedComboIndex = generatedComboIndex >= 0 ? generatedComboIndex : generatedSectionCombinations.Count - 1;
            UpdateApprovedCombination();
        }

        public void RemoveGeneratedCombo()
        {
            generatedSectionCombinations.RemoveAt(generatedComboIndex);
        }

        public void CreateEndlessSectionCombinations()
        {
            GenerateSectionCombinations();
            if(section?.combinations != null)
            {
                section.combinations.CreateCombinationGroups(generatedSectionCombinations);
            }
            ClearGeneratedCombinations();
        }

        public void FillSectionCombinations()
        {
            generatedSectionCombinations.Clear();
            if(section?.combinations != null)
            {
                foreach(SectionCombinationsGroup group in section.combinations.combinationsGroups)
                {
                    foreach(SectionCombination combination in group.sectionCombinations)
                    {
                        generatedSectionCombinations.Add(combination);
                    }
                }
            }
        }

        public void GenerateSectionCombinations()
        {
            this.generatedSectionCombinations.Clear();
            int numRows = rows.Count;
            int numSS = 0;
            foreach(SectionRow row in rows)
            {
                numSS += row.subSections.Count;
            }
            
            SubSection[] subSections = new SubSection[numSS];
            int index = 0;
            for(int i = 0; i < numRows; i++)
            {
                int numSubSections = rows[i].subSections.Count;
                for(int j = 0; j < numSubSections; j++)
                {
                    subSections[index] = rows[i].subSections[j];
                    index++;
                }
            }

            List<List<SelectionMetadata>> selectionMetadatas = Generate(0, subSections, new List<SelectionMetadata>());
            index = 0;
            foreach(List<SelectionMetadata> metadatas in selectionMetadatas)
            {
                List<SubSectionCombination> ssCombos = new List<SubSectionCombination>();
                ssCombos.Add(new SubSectionCombination(0));
                ssCombos.Add(new SubSectionCombination(1));
                ssCombos.Add(new SubSectionCombination(2));

                foreach(SelectionMetadata metadata in metadatas)
                {
                    ssCombos[metadata.rowIndex].activatedIndices.Add(metadata.selectionIndex);
                }
                foreach(SubSectionCombination combination in ssCombos)
                {
                    int totalDifficulty = 0;
                    int numGreaterThan0 = 0;
                    int numIndices = combination.activatedIndices.Count;
                    for(int j = 0; j < numIndices; j++)
                    {
                        int totalIndex = combination.rowIndex * 3 + j;
                        EndlessSectionSpawn ss = subSections[totalIndex].Spawns[combination.activatedIndices[j]];
                        totalDifficulty += ss.difficultyPoints;
                        if(ss.difficultyPoints > 0)
                        {
                            numGreaterThan0++;
                        }
                    }
                    combination.totalDifficulty = totalDifficulty;
                    float multiplier = numGreaterThan0 < 2 ? 1 : 1 + 0.5f * (numGreaterThan0 - 1);
                    combination.multipliedDifficulty = Mathf.Ceil(multiplier * totalDifficulty);
                    combination.CreateMonsterTypes(rows[combination.rowIndex].subSections);
                }

                generatedSectionCombinations.Add(new SectionCombination(index, ssCombos));
                index++;
            }

            Debug.Log(generatedSectionCombinations.Count);
        }

        public void FillRows()
        {
            rows.Clear();
            for(int i = 0; i < 3; i++)
            {
                rows.Add(new SectionRow());
            }
            EndlessSectionSpawn[] spawns = transform.GetComponentsInChildren<EndlessSectionSpawn>(true);
            foreach(EndlessSectionSpawn spawn in spawns)
            {
                SubSectionPosition position = spawn.subSectionPosition;
                int rowIndex = 0;
                if(spawn.name.Contains("2"))
                {
                    rowIndex = 1;
                }
                else if(spawn.name.Contains("3"))
                {
                    rowIndex = 2;
                }
                Debug.Log(spawn.name + " " + position + " " + rowIndex);
                SectionRow row = rows[rowIndex];
                bool hasSubsection = false;
                foreach(SubSection subSection in row.subSections)
                {
                    if(subSection.subSectionPosition == position)
                    {
                        hasSubsection = true;
                        subSection.spawns.Add(spawn);
                        break;
                    }
                }

                if(!hasSubsection)
                {
                    SubSection ss = new SubSection(rowIndex, position);
                    ss.spawns.Add(spawn);
                    row.subSections.Add(ss);
                }
            }
            GetCombinations();
        }

        public void ClearGeneratedCombinations()
        {
            generatedSectionCombinations.Clear();
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
                rows[i].Activate(CurrentGeneratedCombination.subSectionCombinations[i]);
            }
            
            int numRowCombinations = rowCombinations.Count;
            for(int i = 0; i < numRowCombinations; i++)
            {
                rowCombinations[i].CurrentCombination = CurrentGeneratedCombination.subSectionCombinations[i];
            }
        }

        void UpdateRowCombinations()
        {
            int numberOfRows = rowCombinations.Count;
            
            for(int i = 0; i < numberOfRows; i++)
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
            AddOnSpawn spawn = spawns[rand].GetAddOnSpawn();
            spawn.subSectionPosition = (SubSectionPosition)rand;
            return spawn;
        }

        AddOnSpawn GetAddOnSpawn(List<EndlessSectionSpawn> spawns, AddOnSpawnType spawnType)
        {
            int numSpawns = spawns.Count;
            if(numSpawns == 0)
            {
                return new AddOnSpawn();
            }
            int rand = Random.Range(0, numSpawns);
            AddOnSpawn spawn = spawns[rand].GetAddOnSpawn(spawnType);
            spawn.subSectionPosition = (SubSectionPosition)rand;
            return spawn;
        }

        AddOnSpawn GetAddOnSpawn(List<EndlessSectionSpawn> spawns, AddOnSpawnType spawnType, SubSectionPosition position)
        {
            int numSpawns = spawns.Count;
            if(numSpawns == 0)
            {
                return new AddOnSpawn();
            }
            int pos = (int)position;
            for(int i = 0; i < 3; i++)
            {
                if(i != pos)
                {
                    AddOnSpawn spawn = spawns[i].GetAddOnSpawn(spawnType);
                    if(spawn.spawn != null)
                    {
                        spawn.subSectionPosition = (SubSectionPosition)i;
                        return spawn;
                    }
                }
            }
            return new AddOnSpawn();
        }

        int GetMonsterMax(MonsterType monsterType)
        {
            if(monsterType == MonsterType.Rabby || monsterType == MonsterType.Bombee || monsterType == MonsterType.Deyon || monsterType == MonsterType.Fawks || monsterType == MonsterType.Monodon)
            {
                return 3;
            }
            else if(monsterType == MonsterType.Tregon || monsterType == MonsterType.StonePillar)
            {
                return 2;
            }
            else
            {
                return 1;
            }
        }

        bool IsSingleRowMonster(MonsterType mType)
        {
            return mType == MonsterType.SideDraze || mType == MonsterType.Reyflora || mType == MonsterType.Azel;
        }

        bool IsBarrierMonster(MonsterType mType)
        {
            return mType == MonsterType.Tregon || mType == MonsterType.StonePillar;
        }

        bool CheckIfValid(SelectionMetadata metadata, List<SelectionMetadata> subSectionMetadatas)
        {
            MonsterType mType = metadata.monsterType;
            int monsterMax = GetMonsterMax(mType);
            int totalIndex = metadata.TotalIndex;
            int previousRowIndex = totalIndex - 3;
            int previousIndex = totalIndex - 1;
            int previousIndex2 = totalIndex - 2;
            if(mType == MonsterType.None || totalIndex == 0)
            {
                return true;
            }
            else if(metadata.rowIndex == 0)
            {
                MonsterType prevMType = subSectionMetadatas[previousIndex].monsterType;
                if(mType == prevMType && IsBarrierMonster(mType))
                {
                    return false;
                }
                if(metadata.subSectionIndex == 2)
                {
                    MonsterType prevMType2 = subSectionMetadatas[previousIndex2].monsterType;
                    if(mType == prevMType && mType == prevMType2)
                    {
                        return false;
                    }
                    if(IsSingleRowMonster(prevMType) || IsSingleRowMonster(prevMType2))
                    {
                        return false;
                    }
                    if(IsSingleRowMonster(mType) && (prevMType2 != MonsterType.None || prevMType != MonsterType.None))
                    {
                        return false;
                    }
                }
                else if(metadata.subSectionIndex == 1)
                {
                    if(IsSingleRowMonster(prevMType))
                    {
                        return false;
                    }
                    if(IsSingleRowMonster(mType) && prevMType != MonsterType.None)
                    {
                        return false;
                    }
                }
            }
            else
            {
                // on row 1 or 2
                MonsterType prevRowMType = subSectionMetadatas[previousRowIndex].monsterType;
                MonsterType prevMType = subSectionMetadatas[previousIndex].monsterType;
                MonsterType prevMType2 = subSectionMetadatas[previousIndex2].monsterType;
                if(prevRowMType == mType)
                {
                    return false;
                }
                if(metadata.subSectionIndex == 2)
                {
                    // need to check if other 2 in row are same
                    if(mType == prevMType && mType == prevMType2)
                    {
                        return false;
                    }
                    if(mType == prevMType && IsBarrierMonster(mType))
                    {
                        return false;
                    }
                    if(IsSingleRowMonster(prevMType) || IsSingleRowMonster(prevMType2))
                    {
                        return false;
                    }
                    if(IsSingleRowMonster(mType) && (prevMType != MonsterType.None || prevMType2 != MonsterType.None))
                    {
                        return false;
                    }
                }
                else if(metadata.subSectionIndex == 1)
                {
                    if(mType == prevMType && IsBarrierMonster(mType))
                    {
                        return false;
                    }
                    if(IsSingleRowMonster(prevMType))
                    {
                        return false;
                    }
                    if(IsSingleRowMonster(mType) && prevMType != MonsterType.None)
                    {
                        return false;
                    }
                }
                if(mType == MonsterType.DiveDraze || mType == MonsterType.SpecialMonafly || mType == MonsterType.PhysicalMonafly || mType == MonsterType.Pegasus || mType == MonsterType.MovingMonodon)
                {
                    if(metadata.rowIndex == 2)
                    {
                        MonsterType prevRowMType2 = subSectionMetadatas[totalIndex - 6].monsterType;
                        if(prevRowMType != MonsterType.None || prevRowMType2 != MonsterType.None)
                        {
                            return false;
                        }
                    }
                    else if(prevRowMType != MonsterType.None)
                    {
                        return false;
                    }
                }
            }
            int numOfSameType = 0;
            foreach(SelectionMetadata subSectionMetadata in subSectionMetadatas)
            {
                if(subSectionMetadata.monsterType == mType)
                {
                    numOfSameType++;
                }
            }
            
            if(numOfSameType >= monsterMax)
            {
                return false;
            }

            return true;
        }

        List<List<SelectionMetadata>> Generate(int index, SubSection[] subSections, List<SelectionMetadata> selectionMetadatas)
        {
            SubSection subSection = subSections[index];
            int numSubSections = subSections.Length;
            int numSelections = subSection.spawns.Count;
            // int numSubSections = 3;
            // int numSelections = 3;
            List<List<SelectionMetadata>> responseMetadatas = new List<List<SelectionMetadata>>();
            List<List<SelectionMetadata>> tempResponses = new List<List<SelectionMetadata>>();
            for(int i = 0; i < numSelections; i++)
            {
                SelectionMetadata metadata = new SelectionMetadata();
                metadata.monsterType = subSection.Spawns[i].monsterType;
                metadata.rowIndex = subSection.rowIndex;
                metadata.selectionIndex = i;
                metadata.subSectionIndex = (int)subSection.subSectionPosition;
                bool canAdd = CheckIfValid(metadata, selectionMetadatas);
                if(canAdd)
                {
                    List<SelectionMetadata> newSelectionMetadata = selectionMetadatas.ToList();
                    // todo check did shiiiiiit
                    newSelectionMetadata.Add(metadata);
                    if(index < numSubSections - 1)
                    {
                        tempResponses = Generate(index + 1, subSections, newSelectionMetadata);
                        
                        foreach(List<SelectionMetadata> response in tempResponses)
                        {
                            responseMetadatas.Add(response);
                        }
                    }
                    else
                    {
                        responseMetadatas.Add(newSelectionMetadata);
                    }
                }
            }

            return responseMetadatas;
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
        public int rowIndex;
        public SubSectionPosition subSectionPosition;
        public List<EndlessSectionSpawn> spawns = new List<EndlessSectionSpawn>();
        #endregion

        #region Initializers
        public SubSection() {}

        public SubSection(int rowIndex, SubSectionPosition subSectionPosition)
        {
            this.rowIndex = rowIndex;
            this.subSectionPosition = subSectionPosition;
        }
        #endregion

        #region Properties
        public List<EndlessSectionSpawn> Spawns
        {
            get
            {
                return spawns;
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
                Spawns[i]?.gameObject?.SetActive(index == i);
                if(index == i)
                {
                    spawn = Spawns[i];
                    spawn.Activate();
                }
            }
            return spawn;
        }

        public void Deactivate()
        {
            foreach(EndlessSectionSpawn spawn in spawns)
            {
                spawn.gameObject.SetActive(false);
            }
            // foreach(GameObject go in selections)
            // {
            //     go.SetActive(false);
            // }
        }
        #endregion
    }
    
    public enum SubSectionPosition
    {
        Left,
        Middle,
        Right
    }

    public class SelectionMetadata
    {
        public int rowIndex;
        public int subSectionIndex;
        public MonsterType monsterType;
        public int selectionIndex;

        public int TotalIndex
        {
            get
            {
                return rowIndex * 3 + subSectionIndex;
            }
        }
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
        public List<MonsterType> monsterTypes = new List<MonsterType>();
        public int totalDifficulty;
        public float multipliedDifficulty;

        public SubSectionCombination() {}

        public SubSectionCombination(int rowIndex)
        {
            this.rowIndex = rowIndex;
        }

        public void CreateMonsterTypes(List<SubSection> subSections)
        {
            monsterTypes.Clear();
            int numSS = subSections.Count;
            for(int i = 0; i < numSS; i++)
            {
                if(subSections[i].rowIndex == rowIndex)
                {
                    monsterTypes.Add(subSections[i].Spawns[activatedIndices[i]].monsterType);
                }
            }
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

        public SectionCombination(int index, List<SubSectionCombination> subSectionCombinations)
        {
            this.index = index;
            this.subSectionCombinations = subSectionCombinations.ToList();
            UpdateTotalDifficulty();
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
        [HideInInspector]
        public SubSectionPosition subSectionPosition;
    }
}
