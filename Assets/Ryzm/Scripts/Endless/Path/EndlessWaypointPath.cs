using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

namespace Ryzm.EndlessRunner
{
    public class EndlessWaypointPath : MonoBehaviour
    {
        public Transform[] waypoints;
        public VerticalPosition verticalPosition;
        public HorizontalPosition horizontalPosition;
        public PathCreator pathCreator;

        VertexPath VertexPath
        {
            get
            {
                return new VertexPath(pathCreator.bezierPath, pathCreator.transform);
                // return pathCreator.path;
            }
        }

        public void CreateBezierPath()
        {
            BezierPath bezierPath = new BezierPath(waypoints, false, PathSpace.xyz);
            pathCreator.bezierPath = bezierPath;
        }
    }

    public enum VerticalPosition
    {
        Top = 1,
        Middle = 0,
        Bottom = -1
    }

    public enum HorizontalPosition
    {
        Left = -1,
        Middle = 0,
        Right = 1
    }
}
