using System;
using UnityEditor;
using UnityEngine;

public class SHSphereSlimeCustomEditor : ShaderGUI
{
	private const string KEYWORD_COLOR_TEXTURE = "COLOR_TEXTURE";
	private const string KEYWORD_BICOLOR = "BICOLOR";
	private const string KEYWORD_LIGHTED = "LIGHTED";

	private enum SlimeType
	{
		SIMPLE_COLOR = 0,
		TEXTURED = 1,
		BICOLOR = 2,
	}

	private SlimeType slimeType;
	
	public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
	{
		Material targetMat = materialEditor.target as Material;
		InitProperties(targetMat);
		
		EditorGUI.BeginChangeCheck();
		slimeType = (SlimeType)EditorGUILayout.EnumPopup(slimeType);
		bool areThereAnyChanges = EditorGUI.EndChangeCheck();


		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.Space();


		MaterialProperty bodyTextureProperty = ShaderGUI.FindProperty("_BodyTexture", properties);
		MaterialProperty slimeColorProperty = ShaderGUI.FindProperty("_TopColor", properties);
		MaterialProperty secondColorProperty = ShaderGUI.FindProperty("_SecondColor", properties);
		MaterialProperty faceProperty = ShaderGUI.FindProperty("_Face", properties);

		for (int i = 0; i < properties.Length; i++)
		{
			if(properties[i] == bodyTextureProperty  || 
				properties[i] == slimeColorProperty  || 
				properties[i] == secondColorProperty ||
				properties[i] == faceProperty)
			{
				continue;
			}

			materialEditor.ShaderProperty(properties[i], properties[i].displayName);
		}

		if(slimeType == SlimeType.TEXTURED)
		{
			materialEditor.ShaderProperty(bodyTextureProperty, bodyTextureProperty.displayName);
		}
		

		if (areThereAnyChanges)
		{
			if (slimeType == SlimeType.TEXTURED)
			{
				targetMat.EnableKeyword(KEYWORD_COLOR_TEXTURE);
				targetMat.DisableKeyword(KEYWORD_BICOLOR);
			}
			else if (slimeType == SlimeType.BICOLOR)
			{
				targetMat.EnableKeyword(KEYWORD_BICOLOR);
				targetMat.DisableKeyword(KEYWORD_COLOR_TEXTURE);
			}
			else
			{
				targetMat.DisableKeyword(KEYWORD_COLOR_TEXTURE);
				targetMat.DisableKeyword(KEYWORD_BICOLOR);
			}
		}
	}

	private void InitProperties(Material targetMat)
	{
		bool colorTextureEnabled = Array.IndexOf(targetMat.shaderKeywords, KEYWORD_COLOR_TEXTURE) != -1;
		bool bicolorEnabled = Array.IndexOf(targetMat.shaderKeywords, KEYWORD_BICOLOR) != -1;


		slimeType = SlimeType.SIMPLE_COLOR;

		if(Array.IndexOf(targetMat.shaderKeywords, KEYWORD_COLOR_TEXTURE) != -1)
		{
			slimeType = SlimeType.TEXTURED;
		}
		if(Array.IndexOf(targetMat.shaderKeywords, KEYWORD_BICOLOR) != -1)
		{
			slimeType = SlimeType.BICOLOR;
		}
	}
}