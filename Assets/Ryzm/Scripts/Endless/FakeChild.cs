using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Ryzm.EndlessRunner
{
    public class FakeChild : MonoBehaviour
    {
        public RunnerController runner;
        public Transform Parent;//Remember to assign the parent transform 
        private Vector3 pos, fw, up;
        Vector3 prevPos;
        Vector3 currentPlatformPos;
        Transform _transform;
        Transform _parentTransform;

        void Start()
        {
            if(runner != null)
            {
                Parent = runner.transform;
            }
            _transform = transform;
            _parentTransform = Parent.transform;
            pos = _parentTransform.InverseTransformPoint(_transform.position);
            fw = _parentTransform.InverseTransformDirection(_transform.forward);
            up = _parentTransform.InverseTransformDirection(_transform.up);
            prevPos = _transform.position;
        }
        void Update()
        {
            currentPlatformPos = RunnerController.CurrentSection.GetPosition(1).position;
            var newpos = _parentTransform.TransformPoint(pos);
            var newfw = _parentTransform.TransformDirection(fw);
            if(Mathf.Abs(newfw.z) > Mathf.Abs(newfw.x))
            {
                newpos.x = currentPlatformPos.x;
            }
            else
            {
                newpos.z = currentPlatformPos.z;
            }
            
            var newup = Parent.transform.TransformDirection(up);
            var newrot = Quaternion.LookRotation(newfw, newup);
            newrot = Quaternion.Lerp(_transform.rotation, newrot, 0.1f);
            float newY = newpos.y;
            newpos = Vector3.Lerp(_transform.position, newpos, 0.1f);
            newpos.y = newY;
            _transform.rotation = newrot;
            _transform.position = newpos;
            // abs(fw.z) > abs(fw.x) keep x the same
            // Debug.Log($"{newpos.x - prevPos.x}" + " " + $"{newpos.z - prevPos.z}" + $"{newfw} \t newpos: {newpos} \t currentPlatformpos: {currentPlatformPos}" + "");
            prevPos = newpos;
        }
    }
}
