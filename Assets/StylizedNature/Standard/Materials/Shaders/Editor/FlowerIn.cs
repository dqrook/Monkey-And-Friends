using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class FlowerIn : ShaderGUI
{
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {




        MaterialProperty _PlantColor = ShaderGUI.FindProperty("_PlantColor", properties);
        MaterialProperty _FlowerColor = ShaderGUI.FindProperty("_FlowerColor", properties);

        MaterialProperty _Texture1 = ShaderGUI.FindProperty("_Texture1", properties);
        MaterialProperty _FlowerBase = ShaderGUI.FindProperty("_FlowerBase", properties);
        MaterialProperty _FlowerTint = ShaderGUI.FindProperty("_FlowerTint", properties);



        MaterialProperty _Cutoff = ShaderGUI.FindProperty("_Cutoff", properties);
        MaterialProperty _Smoothness1 = ShaderGUI.FindProperty("_Smoothness1", properties);
        MaterialProperty _Translucent1 = ShaderGUI.FindProperty("_Translucent1", properties);
        MaterialProperty _Translucency = ShaderGUI.FindProperty("_Translucency", properties);
        MaterialProperty _TransNormalDistortion = ShaderGUI.FindProperty("_TransNormalDistortion", properties);
        MaterialProperty _TransScattering = ShaderGUI.FindProperty("_TransScattering", properties);
        MaterialProperty _TransDirect = ShaderGUI.FindProperty("_TransDirect", properties);
        MaterialProperty _Scale1 = ShaderGUI.FindProperty("_Scale1", properties);
        MaterialProperty _TransAmbient = ShaderGUI.FindProperty("_TransAmbient", properties);
        MaterialProperty _TransShadow = ShaderGUI.FindProperty("_TransShadow", properties);
        MaterialProperty _Strength1 = ShaderGUI.FindProperty("_Strength1", properties);
        MaterialProperty _TimeScale1 = ShaderGUI.FindProperty("_TimeScale1", properties);

        EditorGUIUtility.wideMode = false;

        Color gray = new Color(0.3f, 0.3f, 0.3f);

        EditorGUI.DrawRect(new Rect(5, 5, EditorGUIUtility.currentViewWidth, 320), gray);
       EditorGUI.DrawRect(new Rect(5, 330, EditorGUIUtility.currentViewWidth, 190), gray);
       EditorGUI.DrawRect(new Rect(5, 525, EditorGUIUtility.currentViewWidth, 90), gray);

        GUIStyle style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleCenter;


        EditorGUILayout.LabelField("Basic Properties", style);
        materialEditor.ShaderProperty(_Texture1, "BaseTexture");
        materialEditor.ShaderProperty(_FlowerBase, "Flower Base");
        materialEditor.ShaderProperty(_PlantColor, "Flower Base Color");
        materialEditor.ShaderProperty(_FlowerTint, "Flower Tint");
        materialEditor.ShaderProperty(_FlowerColor, "Flower Color");

        materialEditor.ShaderProperty(_Cutoff, "Cut Off");
        materialEditor.ShaderProperty(_Smoothness1, "Smoothness");

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Transluncency Properties", style);
        materialEditor.ShaderProperty(_Translucent1, "Overall Translucency");
        materialEditor.ShaderProperty(_Translucency, _Translucency.displayName);
        materialEditor.ShaderProperty(_TransNormalDistortion, _TransNormalDistortion.displayName);
        materialEditor.ShaderProperty(_TransScattering, _TransScattering.displayName);
        materialEditor.ShaderProperty(_TransDirect, _TransDirect.displayName);
        materialEditor.ShaderProperty(_TransAmbient, _TransAmbient.displayName);
        materialEditor.ShaderProperty(_TransShadow, _TransShadow.displayName);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Wind Properties", style);
        materialEditor.ShaderProperty(_Scale1, "Scale");
        materialEditor.ShaderProperty(_Strength1, "Intensity");
        materialEditor.ShaderProperty(_TimeScale1, "Time");
    }
}