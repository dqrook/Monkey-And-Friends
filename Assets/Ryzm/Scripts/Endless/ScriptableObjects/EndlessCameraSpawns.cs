using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ryzm.EndlessRunner
{
    [CreateAssetMenu(fileName = "EndlessCameraSpawns", menuName = "ScriptableObjects/EndlessCameraSpawns", order = 2)]
    public class EndlessCameraSpawns : ScriptableObject
    {
        #region Public Variables
        public List<CameraSpawn> cameraSpawns = new List<CameraSpawn>();
        public int currentCameraSpawn;
        #endregion

        #region Properties
        public CameraSpawn CurrentCameraSpawn
        {
            get
            {
                if(currentCameraSpawn >= 0 && currentCameraSpawn < cameraSpawns.Count)
                {
                    return cameraSpawns[currentCameraSpawn];
                }
                return cameraSpawns[0];
            }
        }
        #endregion
    }

    [System.Serializable]
    public struct CameraSpawn
    {
        public Vector3 localPosition;
        public Vector3 localEulerAngles;
    }
}
