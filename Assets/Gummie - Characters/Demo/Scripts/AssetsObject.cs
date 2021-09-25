using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Animmal.Slimes
{
    [System.Serializable]
    public class Customizations
    {
        [HideInInspector]
        public Transform Basemesh;
        [HideInInspector]
        public Renderer FaceMesh;
        [HideInInspector]
        public Transform AccessoryParent;

        public string DefaultAnimationName = "Idle";
        public List<Transform> Accessories = new List<Transform>();
        public List<Material> FaceMaterials;

        public void SetAccessory(int _AccessoryID)
        {
            Basemesh.gameObject.SetActive(true);
            for (int i = 0; i < Accessories.Count; i++)
            {
                if (i == _AccessoryID)
                    Accessories[i].gameObject.SetActive(true);
                else
                    Accessories[i].gameObject.SetActive(false);
            }

        }

        public void SetFaceMaterial(int _FaceMaterialID)
        {
            if (_FaceMaterialID < FaceMaterials.Count)
                FaceMesh.material = FaceMaterials[_FaceMaterialID];
            else
                FaceMesh.material = FaceMaterials[FaceMaterials.Count - 1];
        }
        public void HideAll()
        {
            Basemesh.gameObject.SetActive(false);
            for (int i = 0; i < Accessories.Count; i++)
            {
                Accessories[i].gameObject.SetActive(false);
            }
        }
    }
    public class AssetsObject : MonoBehaviour
    {
        
        public List<Customizations> Customizations = new List<Customizations>();

        int CurrentID = 0;
        int CurrentAccessory = 0;
        int CurrentFace = 0;
        public void Init(List<AssetDB> _AssetDB)
        {
            for (int i = 0; i < _AssetDB.Count; i++)
            {
         
                Customizations _Accessory = new Customizations();

                _Accessory.Basemesh = Instantiate(_AssetDB[i].Prefab, transform) as Transform;
                _Accessory.FaceMesh = _Accessory.Basemesh.Find(_AssetDB[i].FaceMesh.name).GetComponent<Renderer>();
                _Accessory.AccessoryParent = (new GameObject(_AssetDB[i].Name + "AccessoryParent")).GetComponent<Transform>();
                _Accessory.AccessoryParent.SetParent(transform);
                _Accessory.FaceMaterials = _AssetDB[i].Customizations.FaceMaterials;
                for (int j = 0; j < _AssetDB[i].Customizations.Accessories.Count; j++)
                {
                    Transform _AccessoryItem = Instantiate(_AssetDB[i].Customizations.Accessories[j], _Accessory.AccessoryParent) as Transform;
                    _Accessory.Accessories.Add(_AccessoryItem);
                }
                Customizations.Add(_Accessory);
                if (i == 0)
                    Customizations[i].SetAccessory(0);
                else
                {
                    Customizations[i].HideAll();
                }
            }           
        }

        public void SetObject(int _ObjectID)
        {
            Customizations[CurrentID].HideAll();
            CurrentID = _ObjectID;
            SetCurrentAccessory(CurrentAccessory);
        }

        public int GetObjectTriangleCount()
        {
            int _Count = 0;


            Component[] _SkinnedMeshRenderers;


            _SkinnedMeshRenderers = Customizations[CurrentID].Basemesh.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (SkinnedMeshRenderer _SkinnedMesh in _SkinnedMeshRenderers)
                _Count += _SkinnedMesh.sharedMesh.triangles.Length / 3;

            _SkinnedMeshRenderers = Customizations[CurrentID].Accessories[CurrentAccessory].gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach (SkinnedMeshRenderer _SkinnedMesh in _SkinnedMeshRenderers)
                _Count += _SkinnedMesh.sharedMesh.triangles.Length / 3;

            return _Count;

        }

        public void SetAnimation(string _AnimTrigger, bool _ForceFromStart = false)
        {
            if (_ForceFromStart == false)
            {
                Customizations[CurrentID].Basemesh.gameObject.GetComponent<Animator>().SetTrigger(_AnimTrigger);
                Customizations[CurrentID].Accessories[CurrentAccessory].gameObject.GetComponent<Animator>().SetTrigger(_AnimTrigger);
            }
            else
            {
                Customizations[CurrentID].Basemesh.gameObject.GetComponent<Animator>().Play(_AnimTrigger, -1, 0);
                Customizations[CurrentID].Accessories[CurrentAccessory].gameObject.GetComponent<Animator>().Play(_AnimTrigger, -1, 0);
            }
        }


        public void SetCurrentAccessory(int _Accessory)
        {
            CurrentAccessory = _Accessory;
            Customizations[CurrentID].SetAccessory(_Accessory);
            SetAnimation(Customizations[CurrentID].DefaultAnimationName, true);
        }

        public void SetCurrentFace(int _FaceID)
        {
            CurrentFace = _FaceID;
            Customizations[CurrentID].SetFaceMaterial(_FaceID);
        }
    }
}