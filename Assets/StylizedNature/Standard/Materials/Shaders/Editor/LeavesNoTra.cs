using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class LeavesNoTra : ShaderGUI
{
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {

      
        
        MaterialProperty _Color = ShaderGUI.FindProperty("_Color", properties);
        MaterialProperty _Texture = ShaderGUI.FindProperty("_Texture", properties);
        MaterialProperty _Cutoff = ShaderGUI.FindProperty("_Cutoff", properties);
        MaterialProperty _Smoothness = ShaderGUI.FindProperty("_Smoothness", properties);
     
        MaterialProperty _Scale1 = ShaderGUI.FindProperty("_Scale1", properties);
   
        MaterialProperty _LeavesScale = ShaderGUI.FindProperty("_LeavesScale", properties);
        MaterialProperty _Strength1 = ShaderGUI.FindProperty("_Strength1", properties);
        MaterialProperty _TimeScale1 = ShaderGUI.FindProperty("_TimeScale1", properties);

        EditorGUIUtility.wideMode = false;

        Color gray = new Color(0.3f, 0.3f, 0.3f);

        EditorGUI.DrawRect(new Rect(5, 5, EditorGUIUtility.currentViewWidth, 180), gray);
        EditorGUI.DrawRect(new Rect(5, 190, EditorGUIUtility.currentViewWidth, 90), gray);

        GUIStyle style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleCenter;


        EditorGUILayout.LabelField("Basic Properties", style);
        materialEditor.ShaderProperty(_Color, "Color");
        materialEditor.ShaderProperty(_Texture, "Texture");
        materialEditor.ShaderProperty(_Cutoff, "Cut Off");
        materialEditor.ShaderProperty(_Smoothness, "Smoothness");
        materialEditor.ShaderProperty(_LeavesScale, "Leaves Scale");

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Wind Properties", style);
        materialEditor.ShaderProperty(_Scale1, "Scale");
        materialEditor.ShaderProperty(_Strength1, "Intensity");
        materialEditor.ShaderProperty(_TimeScale1, "Time");

    }

}
