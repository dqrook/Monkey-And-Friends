using UnityEngine;

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

    public enum GeneType
    {
        Body,
        Wing,
        Horn,
        HornType,
        Moves
    }

}