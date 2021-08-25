using UnityEngine;
using System.Collections.Generic;

namespace Ryzm.Dragon
{
    [CreateAssetMenu(fileName = "DragonGenes", menuName = "ScriptableObjects/DragonGenes", order = 2)]
    public class DragonGenes : ScriptableObject
    {
        #region Public Variables
        public DragonGene[] bodyGenes;
        public DragonGene[] dragonGenes;
        public DragonGene[] hornGenes;
        public DragonColor[] colors;
        #endregion

        #region Public Functions
        public DragonGene GetGeneBySequence(int[] genes, GeneType type)
        {
            if(type == GeneType.Moves)
            {
                DragonGene moveGene = new DragonGene();
                int rarity = 1;
                foreach(int gene in genes)
                {
                    if(gene == 0)
                    {
                        rarity += 1;
                    }
                }
                moveGene.rarity = rarity;
                moveGene.name = "";
                return moveGene;
            }
            else if(type == GeneType.Body)
            {
                return GetGene(bodyGenes, genes);
            }
            return GetGene(dragonGenes, genes);
        }
        public DragonGene GetGeneBySequence(string sequence, GeneType type)
        {
            if(type == GeneType.Body)
            {
                return GetGene(bodyGenes, sequence);
            }
            return GetGene(dragonGenes, sequence);
        }

        public DragonGene GetGeneBySequence(int[] genes, int[] hornTypeGenes)
        {
            DragonGene gene = GetGene(dragonGenes, ConvertGenes(genes));
            DragonGene hornTypeGene = GetGene(hornGenes, ConvertGenes(hornTypeGenes));
            
            DragonGene combinedGene = new DragonGene();
            combinedGene.name = gene.name + " " + hornTypeGene.name;
            float combinedRarity = (gene.rarity + hornTypeGene.rarity) / 2;
            int combinedRarityFloor = Mathf.FloorToInt(combinedRarity);
            combinedGene.rarity = combinedRarityFloor;
            if(combinedRarity > combinedRarityFloor)
            {
                combinedGene.hasHalfStar = true;
            }
            return combinedGene;
        }

        public DragonColor GetDragonColor(string color)
        {
            foreach(DragonColor dragonColor in colors)
            {
                if(dragonColor.value == color)
                {
                    return dragonColor;
                }
            }
            return colors[0];
        }

        public List<GeneProbability> GetGeneProbablities(int[] gene1, int[] gene2)
        {
            List<GeneProbability> geneProbabilities = new List<GeneProbability>();
            int numGenes = gene1.Length;
            for(int i = 0; i < numGenes; i++)
            {
                int curGene1 = gene1[i];
                int curGene2 = gene2[i];
                float domProbablity = 0;
                float recProbablity = 0;
                if(curGene1 == 2 || curGene2 == 2)
                {
                    domProbablity = 1;
                }
                else if(curGene1 == 0 && curGene2 == 0)
                {
                    recProbablity = 1;
                }
                else if(curGene1 == 1 && curGene2 == 1)
                {
                    domProbablity = 0.75f;
                    recProbablity = 0.25f;
                }
                else
                {
                    domProbablity = 0.5f;
                    recProbablity = 0.5f;
                }
                Debug.Log(domProbablity + " " + recProbablity + " " + curGene1 + " " + curGene2);
                if(i > 0)
                {
                    List<GeneProbability> probs = new List<GeneProbability>();
                    if(domProbablity > 0)
                    {
                        foreach(GeneProbability probability in geneProbabilities)
                        {
                            GeneProbability newProb = new GeneProbability();
                            newProb.value = probability.value + "1";
                            newProb.probablity = probability.probablity * domProbablity;
                            probs.Add(newProb);
                        }
                    }
                    if(recProbablity > 0)
                    {
                        foreach(GeneProbability probability in geneProbabilities)
                        {
                            GeneProbability newProb = new GeneProbability();
                            newProb.value = probability.value + "0";
                            newProb.probablity = probability.probablity * recProbablity;
                            probs.Add(newProb);
                        }
                    }
                    geneProbabilities = probs;
                    // geneProbabilities = probs;
                }
                else
                {
                    if(domProbablity > 0)
                    {
                        GeneProbability domProb = new GeneProbability();
                        domProb.value = "1";
                        domProb.probablity = domProbablity;
                        geneProbabilities.Add(domProb);
                    }
                    if(recProbablity > 0)
                    {
                        GeneProbability recProb = new GeneProbability();
                        recProb.value = "0";
                        recProb.probablity = recProbablity;
                        geneProbabilities.Add(recProb);
                    }
                }
            }

            return geneProbabilities;
        }

        public List<GeneProbability> GetColorProbablities(string color1, string color2)
        {
            List<GeneProbability> probabilities = new List<GeneProbability>();
            for(int i = 0; i < 2; i++)
            {
                GeneProbability colorProb = new GeneProbability();
                if(i == 0)
                {
                    colorProb.value = color1;
                }
                else
                {
                    colorProb.value = color2;
                }
                colorProb.probablity = 0.5f;
                probabilities.Add(colorProb);
            }

            return probabilities;
        }
        #endregion

        #region Private Functions
        DragonGene GetGene(DragonGene[] targetGenes, int[] genes)
        {
            DragonGene gene = GetGene(targetGenes, ConvertGenes(genes));
            return gene;
        }

        DragonGene GetGene(DragonGene[] genes, string sequence)
        {
            foreach(DragonGene gene in genes)
            {
                if(gene.sequence == sequence)
                {
                    return gene;
                }
            }
            return new DragonGene();
        }

        string ConvertGenes(int[] genes)
        {
            string sequence = "";
            foreach(int gene in genes)
            {
                int convertedGene = gene > 1 ? 1 : gene;
                sequence += convertedGene.ToString();
            }
            return sequence;
        }
        #endregion
        
    }

    [System.Serializable]
    public struct DragonGene
    {
        public Sprite image;
        public string name;
        public string sequence;
        [Range(1, 5)]
        public int rarity;
        [HideInInspector]
        public string rawSequence;
        [HideInInspector]
        public bool hasHalfStar;
        public string hornType;
    }

    [System.Serializable]
    public struct DragonColor
    {
        public Sprite image;
        public string name;
        public string value;
    }

    [System.Serializable]
    public struct GeneProbability
    {
        public float probablity;
        public string value;
    }

    public enum GeneType
    {
        Body,
        Wing,
        Horn,
        HornType,
        Moves
    }

}