using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.Dragon
{
    public abstract class MarketDragon : MonoBehaviour
    {
        #region Public Variables
        public DragonPrefabs prefabs;
        #endregion

        #region Protected Variables
        protected Dictionary<string, BaseDragon> hornToDragons = new Dictionary<string, BaseDragon>();
        protected BaseDragon activeDragon;
        protected Transform trans;
        public string bodyPath = "Dragon/Plain/default";
        public string wingPath = "Dragon/Plain/default";
        public string hornPath = "Dragon/Plain/default";
        protected string hornType = "1";
        #endregion

        #region Event Functions
        protected virtual void Awake()
        {
            trans = transform;
        }

        protected virtual void OnDestroy() {}
        #endregion

        #region Public Functions
        public void UpdateData(MarketDragonData data)
        {
            hornType = data.data.hornType;
            SetActiveDragon();
            activeDragon.data = data.data;
            
            SetTexture(DragonMaterialType.Body, data.bodyTexture);
            SetTexture(DragonMaterialType.Wing, data.wingTexture);
            SetTexture(DragonMaterialType.Horn, data.hornTexture);
            SetTexture(DragonMaterialType.Back, data.backTexture);
        }

        public virtual void DisableMaterials()
        {
            foreach(BaseDragon dragon in hornToDragons.Values)
            {
                dragon.DisableMaterials();
            }
        }

        public virtual void EnableMaterials()
        {
            activeDragon.EnableMaterials();
        }
        #endregion

        #region Protected Functions
        protected void UpdateDragons()
        {
            SetActiveDragon();

            Texture bodyTexture = Resources.Load<Texture>(bodyPath);
            Texture wingTexture = Resources.Load<Texture>(wingPath);
            Texture hornTexture = Resources.Load<Texture>(hornPath);

            SetTexture(DragonMaterialType.Body, bodyTexture);
            SetTexture(DragonMaterialType.Wing, wingTexture);
            SetTexture(DragonMaterialType.Horn, hornTexture);
            SetTexture(DragonMaterialType.Back, hornTexture);
        }

        protected void SetActiveDragon()
        {
            if(!hornToDragons.ContainsKey(hornType))
            {
                BaseDragon newDragon = Instantiate(prefabs.GetPrefabByHornType(hornType).dragon).GetComponent<BaseDragon>();
                newDragon.transform.parent = this.trans;
                newDragon.transform.localPosition = Vector3.zero;
                newDragon.transform.localEulerAngles = Vector3.zero;
                hornToDragons.Add(hornType, newDragon);
            }
            activeDragon = hornToDragons[hornType];

            foreach(BaseDragon dragon in hornToDragons.Values)
            {
                if(dragon != activeDragon)
                {
                    dragon.gameObject.SetActive(false);
                }
                else
                {
                    dragon.gameObject.SetActive(true);
                }
            }
        }

        protected void SetTexture(DragonMaterialType type, Texture texture)
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
        #endregion
    }
}
