using System;
using UnityEditor;
using UnityEngine;

public class SHSlimeCustomEditor : ShaderGUI
{
	private const string KEYWORD_COLOR_TEXTURE = "COLOR_TEXTURE";
	private const string KEYWORD_BICOLOR = "BICOLOR";

	public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
	{
		Material targetMat = materialEditor.target as Material;
		bool colorTextureEnabled = Array.IndexOf(targetMat.shaderKeywords, KEYWORD_COLOR_TEXTURE) != -1;
		bool bicolorEnabled = Array.IndexOf(targetMat.shaderKeywords, KEYWORD_BICOLOR) != -1;

		EditorGUI.BeginChangeCheck();
		colorTextureEnabled = EditorGUILayout.Toggle("Color Texture", colorTextureEnabled);
		bicolorEnabled = EditorGUILayout.Toggle("Bicolor", bicolorEnabled);




		if (EditorGUI.EndChangeCheck())
		{
			if (colorTextureEnabled)
			{
				targetMat.EnableKeyword(KEYWORD_COLOR_TEXTURE);
				targetMat.DisableKeyword(KEYWORD_BICOLOR);
			}
			else if (bicolorEnabled)
			{
				targetMat.EnableKeyword(KEYWORD_BICOLOR);
				targetMat.DisableKeyword(KEYWORD_COLOR_TEXTURE);
			}
			else
			{
				targetMat.DisableKeyword(KEYWORD_BICOLOR);
				targetMat.DisableKeyword(KEYWORD_COLOR_TEXTURE);
			}
		}

		base.OnGUI(materialEditor, properties);
	}
}