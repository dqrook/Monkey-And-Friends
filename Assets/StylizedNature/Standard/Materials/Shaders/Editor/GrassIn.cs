using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GrassIn : ShaderGUI
{
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
   
        MaterialProperty _Color = ShaderGUI.FindProperty("_Color", properties);
  

        MaterialProperty _Texture = ShaderGUI.FindProperty("_Texture", properties);




        MaterialProperty _Cutoff = ShaderGUI.FindProperty("_Cutoff", properties);
        MaterialProperty _Smoothness = ShaderGUI.FindProperty("_Smoothness", properties);
        MaterialProperty _Translucent = ShaderGUI.FindProperty("_Translucent", properties);
        MaterialProperty _Translucency = ShaderGUI.FindProperty("_Translucency", properties);
        MaterialProperty _TransNormalDistortion = ShaderGUI.FindProperty("_TransNormalDistortion", properties);
        MaterialProperty _TransScattering = ShaderGUI.FindProperty("_TransScattering", properties);
        MaterialProperty _TransDirect = ShaderGUI.FindProperty("_TransDirect", properties);
        MaterialProperty _Scale = ShaderGUI.FindProperty("_Scale", properties);
        MaterialProperty _TransAmbient = ShaderGUI.FindProperty("_TransAmbient", properties);
        MaterialProperty _TransShadow = ShaderGUI.FindProperty("_TransShadow", properties);
        MaterialProperty _Strength = ShaderGUI.FindProperty("_Strength", properties);
        MaterialProperty _TimeScale = ShaderGUI.FindProperty("_TimeScale", properties);

        EditorGUIUtility.wideMode = false;

        Color gray = new Color(0.3f, 0.3f, 0.3f);

        EditorGUI.DrawRect(new Rect(5, 5, EditorGUIUtility.currentViewWidth, 155), gray);
        EditorGUI.DrawRect(new Rect(5, 165, EditorGUIUtility.currentViewWidth, 190), gray);
        EditorGUI.DrawRect(new Rect(5, 360, EditorGUIUtility.currentViewWidth, 90), gray);

        GUIStyle style = new GUIStyle();
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleCenter;


        EditorGUILayout.LabelField("Basic Properties", style);
        materialEditor.ShaderProperty(_Texture, "BaseTexture");
        materialEditor.ShaderProperty(_Color, "Base Color");
        

        materialEditor.ShaderProperty(_Cutoff, "Cut Off");
        materialEditor.ShaderProperty(_Smoothness, "Smoothness");

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Transluncency Properties", style);
        materialEditor.ShaderProperty(_Translucent, "Overall Translucency");
        materialEditor.ShaderProperty(_Translucency, _Translucency.displayName);
        materialEditor.ShaderProperty(_TransNormalDistortion, _TransNormalDistortion.displayName);
        materialEditor.ShaderProperty(_TransScattering, _TransScattering.displayName);
        materialEditor.ShaderProperty(_TransDirect, _TransDirect.displayName);
        materialEditor.ShaderProperty(_TransAmbient, _TransAmbient.displayName);
        materialEditor.ShaderProperty(_TransShadow, _TransShadow.displayName);

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Wind Properties", style);
        materialEditor.ShaderProperty(_Scale, "Scale");
        materialEditor.ShaderProperty(_Strength, "Intensity");
        materialEditor.ShaderProperty(_TimeScale, "Time");

    }

    }
