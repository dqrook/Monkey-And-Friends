using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.Dragon
{
    public class DragonMaterials : MonoBehaviour
    {
        public List<DragonMaterial> materials = new List<DragonMaterial>();
        public Renderer face;

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

        public void Enable()
        {
            face.enabled = true;
            foreach(DragonMaterial material in materials)
            {
                material.Enable();
            }
        }

        public void Disable()
        {
            face.enabled = false;
            foreach(DragonMaterial material in materials)
            {
                material.Disable();
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
        Renderer _renderer;

        Renderer R
        {
            get
            {
                if(_renderer == null)
                {
                    _renderer = go.GetComponent<Renderer>();
                }
                return _renderer;
            }
        }

        public Material Material
        {
            get
            {
                return R.material;
            }
        }

        public void Disable()
        {
            R.enabled = false;
        }

        public void Enable()
        {
            R.enabled = true;
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
