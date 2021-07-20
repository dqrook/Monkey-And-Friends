using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.Dragon
{
    public class MarketDragon : MonoBehaviour
    {
        public DragonMaterials materials;

        public void UpdateData(MarketDragonData data)
        {
            materials.SetTexture(DragonMaterialType.Body, data.bodyTexture);
            materials.SetTexture(DragonMaterialType.Wing, data.wingTexture);
            materials.SetTexture(DragonMaterialType.Horn, data.hornTexture);
            materials.SetTexture(DragonMaterialType.Back, data.backTexture);
        }

        public void EnableMaterials()
        {
            materials.Enable();
        }

        public void DisableMaterials()
        {
            materials.Disable();
        }
    }
}
