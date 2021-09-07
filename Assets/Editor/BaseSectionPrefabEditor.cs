using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Ryzm.EndlessRunner
{
    [CustomEditor(typeof(BaseSectionPrefab))]
    public class BaseSectionPrefabEditor : Editor
    {
        // void OnEnable ()
        // {
        //     SceneView.duringSceneGui -= OnSceneGUI;
        //     SceneView.duringSceneGui += OnSceneGUI;
        // }

        // void OnDisable ()
        // {
        //     SceneView.duringSceneGui -= OnSceneGUI;
        // }

        // void OnSceneGUI(SceneView sceneView)
        // {
        //     SceneView.RepaintAll();
        //     BaseSectionPrefab bsp = (BaseSectionPrefab)target;
        //     Event e = Event.current;
        //     bool isGood = e.type == EventType.KeyDown;
        //     // Debug.Log(e.type);
        //     if(isGood)
        //     {
        //         if(Event.current.keyCode == KeyCode.UpArrow)
        //         {
        //             bsp.NextRowCombo();
        //         }

        //         if(Event.current.keyCode == KeyCode.DownArrow)
        //         {
        //             bsp.PreviousRowCombo();
        //         }

        //         if(Event.current.keyCode == KeyCode.RightArrow)
        //         {
        //             bsp.NextSubSecCombo();
        //         }

        //         if(Event.current.keyCode == KeyCode.LeftArrow)
        //         {
        //             bsp.PreviousSubSecCombo();
        //         }

        //         if(Event.current.keyCode == KeyCode.S)
        //         {
        //             bsp.SaveApprovedCombo();
        //         }

        //         if(Event.current.keyCode == KeyCode.D)
        //         {
        //             bsp.NextApprovedCombo();
        //         }

        //         if(Event.current.keyCode == KeyCode.A)
        //         {
        //             bsp.PreviousApprovedCombo();
        //         }

        //         if(Event.current.keyCode == KeyCode.I)
        //         {
        //             bsp.RemoveApprovedCombo();
        //         }
        //     }
        // }

        // public override void OnInspectorGUI()
        // {
        //     DrawDefaultInspector();
        //     BaseSectionPrefab bsp = (BaseSectionPrefab)target;

        //     if(GUILayout.Button("Fill Rows"))
        //     {
        //         bsp.FillRows();
        //     }
        // }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            BaseSectionPrefab bsp = (BaseSectionPrefab)target;
        
            GUILayout.Label("Current Sub Section Combo Dex " + bsp.SubSectionComboIndex.ToString());

            GUILayout.Label("Main");
            if(GUILayout.Button("Fill Rows"))
            {
                bsp.FillRows();
            }

            if(GUILayout.Button("Get Combinations"))
            {
                bsp.GetCombinations();
            }

            if(GUILayout.Button("Reset"))
            {
                bsp.Reset();
            }

            GUILayout.Label("Rows");
            if(GUILayout.Button("Next Row Combo"))
            {
                bsp.NextRowCombo();
            }

            if(GUILayout.Button("Previous Row Combo"))
            {
                bsp.PreviousRowCombo();
            }

            GUILayout.Label("Sub Sections");
            if(GUILayout.Button("Next Sub Section Combo"))
            {
                bsp.NextSubSecCombo();
            }

            if(GUILayout.Button("Previous Sub Section Combo"))
            {
                bsp.PreviousSubSecCombo();
            }

            GUILayout.Label("Play");
            if(GUILayout.Button("Activate"))
            {
                bsp.Activate(GameDifficulty.Easy);
            }

            if(GUILayout.Button("Deactivate"))
            {
                bsp.Deactivate();
            }

            GUILayout.Label("Approved Combos");
            if(GUILayout.Button("Save Approved Combo"))
            {
                bsp.SaveGeneratedCombo();
            }

            if(GUILayout.Button("Next Approved Combo"))
            {
                bsp.NextGeneratedCombo();
            }

            if(GUILayout.Button("Previous Approved Combo"))
            {
                bsp.PreviousGeneratedCombo();
            }

            if(GUILayout.Button("Remove Approved Combo"))
            {
                bsp.RemoveGeneratedCombo();
            }

            GUILayout.Label("Endless Section Combos");
            if(GUILayout.Button("Create Endless Section Combinations"))
            {
                bsp.CreateEndlessSectionCombinations();
            }
            if(GUILayout.Button("Generate Section Combinations"))
            {
                bsp.GenerateSectionCombinations();
            }
            if(GUILayout.Button("Clear Generated Combinations"))
            {
                bsp.ClearGeneratedCombinations();
            }
        }
    }
}
