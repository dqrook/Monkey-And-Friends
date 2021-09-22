using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

namespace Ryzm.Test
{
    public class TestMoveAlongPath : MonoBehaviour
    {
        [SerializeField] private PathCreator Path;
        [SerializeField] private float speed;
        [SerializeField] private Transform playerObj;

        private Transform pathHelper;
        private Vector3 targetPosition;
        private Quaternion targetRotation;

        private void Awake()
        {
            GameObject _pathHelper = new GameObject();
            _pathHelper.name = "Path Helper";
            pathHelper = _pathHelper.transform;
            UpdatePathPosition();
        }
        private void FixedUpdate()
        {
            targetPosition = Path.path.GetClosestPointOnPath(playerObj.position);
            pathHelper.position = targetPosition;
            UpdatePathPosition();
            PushPlayer();
        }
        private void UpdatePathPosition()
        {
            float dist = Path.path.GetClosestDistanceAlongPath(playerObj.position);
            targetRotation = Quaternion.LookRotation(Path.path.GetDirectionAtDistance(dist, EndOfPathInstruction.Stop));
            // targetRotation.z *= 0;
            // targetRotation.x *= 0;
            pathHelper.rotation = Quaternion.Lerp(pathHelper.rotation, targetRotation, Time.deltaTime * 16.0f);
            // targetPosition.y = WaterSurface.transform.position.y;
        }
        private void PushPlayer()
        {
            playerObj.Translate(pathHelper.forward * speed * Time.deltaTime);
        }
    }
}
