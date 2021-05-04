using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

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
        float initY;
        EndlessSection currentSection;

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
            initY = pos.y;
        }

        void OnEnable()
        {
            Message.AddListener<CurrentSectionChange>(OnCurrentSectionChange);
        }

        void OnDisable()
        {
            Message.RemoveListener<CurrentSectionChange>(OnCurrentSectionChange);
        }

        void Update()
        {
            var newpos = _parentTransform.TransformPoint(pos);
            var newfw = _parentTransform.TransformDirection(fw);
            if(currentSection != null)
            {
                currentPlatformPos = currentSection.GetPosition(1).position;
                // todo: how to handle going on a curve?
                // if we are going forward in the global z direction then we want to keep the global x location the same as the center of the platform
                if(Mathf.Abs(newfw.z) > Mathf.Abs(newfw.x))
                {
                    newpos.x = currentPlatformPos.x;
                }
                else
                {
                    newpos.z = currentPlatformPos.z;
                }
            }
            // newpos.y = initY;
            
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

        void OnCurrentSectionChange(CurrentSectionChange sectionChange)
        {
            currentSection = sectionChange.endlessSection;
        }
    }
}
