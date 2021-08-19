using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.Dragon
{
    public class MarketDragon : MonoBehaviour
    {
        public DragonPrefabs prefabs;
        public Dictionary<int, BaseDragon> hornToDragons = new Dictionary<int, BaseDragon>();
        BaseDragon activeDragon;
        Transform trans;

        void Awake()
        {
            trans = transform;
        }

        public void UpdateData(MarketDragonData data)
        {
            if(!hornToDragons.ContainsKey(data.data.hornType))
            {
                BaseDragon newDragon = Instantiate(prefabs.GetPrefabByHornType(data.data.hornType).dragon).GetComponent<BaseDragon>();
                newDragon.transform.parent = this.trans;
                newDragon.transform.localPosition = Vector3.zero;
                newDragon.transform.localEulerAngles = Vector3.zero;
                hornToDragons.Add(data.data.hornType, newDragon);
            }
            activeDragon = hornToDragons[data.data.hornType];
            activeDragon.data = data.data;
            foreach(int hornType in hornToDragons.Keys)
            {
                hornToDragons[hornType].gameObject.SetActive(data.data.hornType == hornType);
            }
            SetTexture(DragonMaterialType.Body, data.bodyTexture);
            SetTexture(DragonMaterialType.Wing, data.wingTexture);
            SetTexture(DragonMaterialType.Horn, data.hornTexture);
            SetTexture(DragonMaterialType.Back, data.backTexture);
        }

        public void SetTexture(DragonMaterialType type, Texture texture)
        {
            if(activeDragon != null)
            {
                foreach(DragonMaterial material in activeDragon.materials)
                {
                    if(material.type == type)
                    {
                        material.SetTexture(texture);
                    }
                }
            }
        }

        public void DisableMaterials()
        {
            foreach(BaseDragon dragon in hornToDragons.Values)
            {
                dragon.DisableMaterials();
            }
        }

        public void EnableMaterials()
        {
            activeDragon.EnableMaterials();
        }
    }
}
