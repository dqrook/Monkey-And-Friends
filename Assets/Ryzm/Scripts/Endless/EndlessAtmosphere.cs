using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ryzm.EndlessRunner.Messages;
using CodeControl;

namespace Ryzm.EndlessRunner
{
    public class EndlessAtmosphere : MonoBehaviour
    {
        public float rotationRate = 0.01f;
        Transform trans;
        Vector3 currentDragonPosition;
        bool startedCoroutine;

        void Awake()
        {
            trans = transform;
        }

        void OnEnable()
        {
            Message.AddListener<ControllerTransformResponse>(OnControllerTransformResponse);
            Message.Send(new ControllerTransformRequest());
        }

        void Update()
        {
            if(startedCoroutine)
            {
                trans.position = Vector3.Lerp(trans.position, currentDragonPosition, 5 * Time.deltaTime);
                trans.Rotate(0, rotationRate, 0);
            }
        }

        void OnDisable()
        {
            startedCoroutine = false;
            Message.RemoveListener<ControllerTransformResponse>(OnControllerTransformResponse);
        }

        void OnControllerTransformResponse(ControllerTransformResponse response)
        {
            currentDragonPosition = response.position;
            if(!startedCoroutine)
            {
                startedCoroutine = true;
                trans.position = currentDragonPosition;
            }
        }
    }
}
