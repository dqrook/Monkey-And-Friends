using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.Dragon
{
    public class DragonMaterials : MonoBehaviour
    {
        public List<DragonMaterial> materials = new List<DragonMaterial>();

        public void SetTexture(DragonMaterialType type, Texture texture)
        {
            foreach(DragonMaterial material in materials)
            {
                if(material.type == type)
                {
                    material.Material.SetTexture("_MainTex", texture);
                }
            }
        }
    }

    [System.Serializable]
    public class DragonMaterial
    {
        public GameObject go;
        public DragonMaterialType type;
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
