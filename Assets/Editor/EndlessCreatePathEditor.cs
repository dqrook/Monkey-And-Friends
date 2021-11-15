using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Ryzm.EndlessRunner
{
    [CustomEditor(typeof(EndlessCreatePath))]
    public class EndlessCreatePathEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            EndlessCreatePath createPath = (EndlessCreatePath)target;

            if(GUILayout.Button("Create Path"))
            {
                createPath.CreatePath();
            }

            if(GUILayout.Button("Fill Waypoints"))
            {
                createPath.FillWaypoints();
            }
        }
    }
}