using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    public class EndlessCrystalRock : MonoBehaviour
    {
        #region Public Variables
        public List<Renderer> crystals = new List<Renderer>();
        public List<Material> crystalMaterials = new List<Material>();
        #endregion

        #region Event Functions
        void OnEnable()
        {
            Activate();
        }
        #endregion

        #region Private Functions
        void Activate()
        {
            if(crystalMaterials.Count > 1)
            {
                Material crystalMaterial = crystalMaterials[Random.Range(0, crystalMaterials.Count)];
                foreach(Renderer crystal in crystals)
                {
                    crystal.sharedMaterial = crystalMaterial;
                }
            }
        }
        #endregion
    }
}
