using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Ryzm.EndlessRunner
{
    [CustomEditor(typeof(CreateSections))]
    public class CreateSectionsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            CreateSections createSections = (CreateSections)target;
            if(GUILayout.Button("Bad bitch"))
            {
                createSections.GetCombos();
            }

        }
    }
}

