using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

namespace Ryzm.EndlessRunner
{
    public class EndlessCreatePath : MonoBehaviour
    {
        public List<EndlessWaypointPath> waypointPaths = new List<EndlessWaypointPath>();
        public GameObject waypointPrefab;
        public float verticalDistance = 2;
        public float horizontalDistance = 1;
        public Transform[] baseWaypoints;

        public void CreatePath()
        {
            int numPaths = waypointPaths.Count;
            int numWaypoints = baseWaypoints.Length;
            EndlessWaypointPath basePath = waypointPaths[0];
            basePath.waypoints = new Transform[baseWaypoints.Length];
            basePath.waypoints = baseWaypoints;
            basePath.CreateBezierPath();
            // for(int i = 1; i < numPaths; i+=1)
            // {
            //     EndlessWaypointPath newPath = waypointPaths[i];
            //     newPath.waypoints = new Transform[numWaypoints];
            //     Vector3 newLocalPos = new Vector3((int)newPath.horizontalPosition * horizontalDistance, (int)newPath.verticalPosition * verticalDistance, 0);
            //     string baseName = newPath.horizontalPosition.ToString() + " " + newPath.verticalPosition.ToString() + " ";
            //     for(int j = 0; j < numWaypoints; j+=1)
            //     {
            //         Transform baseWaypoint = basePath.waypoints[j];
            //         GameObject newWaypoint = GameObject.Instantiate(waypointPrefab);
            //         newWaypoint.name = baseName + " " + j;
            //         newWaypoint.transform.position = baseWaypoint.TransformPoint(newLocalPos);
            //         newWaypoint.transform.parent = newPath.transform;
            //         newPath.waypoints[j] = newWaypoint.transform;
            //     }
            //     newPath.CreateBezierPath();
            // }
        }

        public void FillWaypoints()
        {
            Transform[] xForms = GetComponentsInChildren<Transform>();
            Transform trans = transform;
            int count = 0;
            baseWaypoints = new Transform[xForms.Length - 1];
            foreach(Transform xForm in xForms)
            {
                if(xForm != trans)
                {
                    baseWaypoints[count] = xForm;
                    count++;
                }
            }
        }

        Dictionary<VerticalPosition, Dictionary<HorizontalPosition, EndlessWaypointPath>> CreateDictionaryFromWaypointPaths()
        {
            Dictionary<VerticalPosition, Dictionary<HorizontalPosition, EndlessWaypointPath>> waypointPathDictionary = new Dictionary<VerticalPosition, Dictionary<HorizontalPosition, EndlessWaypointPath>>();
            foreach(EndlessWaypointPath path in waypointPaths)
            {
                waypointPathDictionary[path.verticalPosition][path.horizontalPosition] = path;
            }
            return waypointPathDictionary;
        }
    }
}
