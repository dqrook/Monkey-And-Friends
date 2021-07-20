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
}
