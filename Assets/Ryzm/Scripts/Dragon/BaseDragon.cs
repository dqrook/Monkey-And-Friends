using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeControl;
using Ryzm.Dragon.Messages;
using Ryzm.Utils;
using UnityEngine.Networking;

namespace Ryzm.Dragon
{
    public class BaseDragon : MonoBehaviour
    {
        #region Public Variables
        public DragonResponse data;
        public List<DragonMaterial> materials = new List<DragonMaterial>();
        public Renderer face;
        #endregion

        #region Private Variables
        Transform trans;
        IEnumerator getDragonTexture;
        #endregion

        #region Properties
        bool MaterialsInitialized
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
        #endregion

        #region Event Functions
        void Awake()
        {
            trans = transform;
        }
        #endregion

        #region Public Functions
        public void UpdateData(DragonResponse newData, List<DragonMaterial> newMaterials)
        {
            data = newData;
            foreach(DragonMaterial newMaterial in newMaterials)
            {
                foreach(DragonMaterial material in materials)
                {
                    if(material.type == newMaterial.type)
                    {
                        material.SetTexture(newMaterial.texture);
                        break;
                    }
                }
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
            if(MaterialsInitialized)
            {
                Debug.Log("dragon is initialized " + data.id);
                Message.Send(new DragonInitialized(data.id));
            }
        }

        public void GetTextures()
        {
            getDragonTexture = null;
            getDragonTexture = _GetTextures();
            StartCoroutine(getDragonTexture);
        }

        public void DisableMaterials()
        {
            face.enabled = false;
            foreach(DragonMaterial material in materials)
            {
                material.Disable();
            }
        }

        public void EnableMaterials()
        {
            face.enabled = true;
            foreach(DragonMaterial material in materials)
            {
                material.Enable();
            }
        }
        #endregion

        #region Coroutines
        IEnumerator _GetTextures()
        {
            List<MaterialTypeToUrlMap> map = new List<MaterialTypeToUrlMap>
            {
                new MaterialTypeToUrlMap(DragonMaterialType.Body, data.bodyTexture),
                new MaterialTypeToUrlMap(DragonMaterialType.Wing, data.wingTexture),
                new MaterialTypeToUrlMap(DragonMaterialType.Horn, data.hornTexture),
                new MaterialTypeToUrlMap(DragonMaterialType.Back, data.backTexture)
            };
            
            int numMaterials = map.Count;
            int index = 0;
            while(index < numMaterials)
            {
                string url = map[index].url;
                DragonMaterialType type = map[index].type;
                UnityWebRequest request = RyzmUtils.TextureRequest(url);
                yield return request.SendWebRequest();
                if(request.isNetworkError || request.isHttpError)
                {
                    Debug.LogError("ERROR");
                    // todo: handle this case
                }
                else
                {
                    if(materials != null)
                    {
                        Texture _texture = DownloadHandlerTexture.GetContent(request);
                        SetTexture(type, _texture);
                    }

                }
                index++;
                yield return null;
            }
        }
        #endregion
    }

    [System.Serializable]
    public class DragonMaterial
    {
        public GameObject go;
        public DragonMaterialType type;
        public bool initialized;
        public Texture texture;
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
            this.texture = texture;
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
