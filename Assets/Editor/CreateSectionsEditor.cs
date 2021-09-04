using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Ryzm.EndlessRunner
{
    [CustomEditor(typeof(CreateSectionCombinations))]
    public class CreateSectionsEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            
            CreateSectionCombinations createSections = (CreateSectionCombinations)target;
            if(GUILayout.Button("Bad bitch"))
            {
                createSections.GetCombos();
            }

        }
    }
}

