using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Ryzm.Dragon;

namespace Ryzm.UI
{
    public class DragonGenePanel : MonoBehaviour
    {
        #region Public Variables
        public GeneType type;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI genesText;
        public List<Image> stars = new List<Image>();
        public Image halfStar;
        #endregion

        #region Public Functions
        public void Initialize(DragonGene gene)
        {
            nameText.text = gene.name;
            nameText.gameObject.SetActive(gene.name.Length > 0);
            genesText.text = gene.rawSequence;
            int stars2Activate = gene.rarity;
            foreach(Image star in stars)
            {
                star.gameObject.SetActive(stars2Activate > 0);
                if(stars2Activate > 0)
                {
                    stars2Activate--;
                }
            }
            halfStar.gameObject.SetActive(gene.hasHalfStar);
        }
        public void Disable()
        {
            nameText.text = "";
            genesText.text = "";
            foreach(Image star in stars)
            {
                star.gameObject.SetActive(false);
            }
            halfStar.gameObject.SetActive(false);
        }
        #endregion
    }
}
