using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.Dragon
{
    public class DragonMaterials : MonoBehaviour
    {
        public List<DragonMaterial> materials = new List<DragonMaterial>();

        public bool Initialized
        {
            get
            {
                foreach(DragonMaterial material in materials)
                {
                    if(material.type == DragonMaterialType.Teeth)
                    {
                        continue;
                    }
                    if(!material.initialized)
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        public void SetTexture(DragonMaterialType type, Texture texture)
        {
            foreach(DragonMaterial material in materials)
            {
                if(material.type == type)
                {
                    material.SetTexture(texture);
                }
            }
        }
    }

    [System.Serializable]
    public class DragonMaterial
    {
        public GameObject go;
        public DragonMaterialType type;
        public bool initialized;
        Renderer renderer;

        public Material Material
        {
            get
            {
                if(renderer == null)
                {
                    renderer = go.GetComponent<Renderer>();
                }
                return renderer.material;
            }
        }

        public void SetTexture(Texture texture)
        {
            initialized = true;
            Material.SetTexture("_MainTex", texture);
        }
    }

    public enum DragonMaterialType
    {
        Body,
        Wing,
        Horn,
        Back,
        Teeth
    }
}
