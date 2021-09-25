#if UNITY_EDITOR
using UnityEngine;

public static class MaterialSearcher
{
	public enum SRP
	{
		Builtin = 0,
		URP = 1,
		HDRP = 2,
	}

	public enum MaterialFolder
	{
		Slime,
		Samples,
	}

	private const string PREFIX_PATH = "Assets/Enemy Wave/";

	private const string SUFIX_BUILTIN_FOLDER = "Built-in/";
	private const string SUFIX_BUILTIN_MATERIAL = "_Builtin";

	private const string SUFIX_URP_FOLDER = "URP/";
	private const string SUFIX_URP_MATERIAL = "_URP";

	private const string MAT_EXTENSION = ".mat";

	private const string SAMPLE_MATERIALS_FOLDER = "Sample Scenes/Materials/";
	private const string SLIMES_MATERIALS_FOLDER = "Slimes/Materials/";


	public static Material SearchMaterial(string materialName, MaterialFolder materialFolder)
	{
		SRP srp = Neon2.Neon2Utils.GetSelectedSRP();
		Material res = SearchMaterial(materialName, srp, materialFolder);

		return res;
	}

	private static Material SearchMaterial(string materialName, SRP srp, MaterialFolder materialFolder)
	{
		string materialPath = GetMaterialPath(materialName, srp, materialFolder);
		Material res = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(materialPath);
		return res;
	}

	private static string GetSufixFolderBySRP(SRP srp)
	{
		string res = "";

		switch (srp)
		{
			case SRP.Builtin:
				res = SUFIX_BUILTIN_FOLDER;
				break;
			case SRP.URP:
				res = SUFIX_URP_FOLDER;
				break;
			case SRP.HDRP:
				break;
		}

		return res;
	}

	private static string GetSufixMaterialBySRP(SRP srp)
	{
		string res = "";

		switch (srp)
		{
			case SRP.Builtin:
				res = SUFIX_BUILTIN_MATERIAL;
				break;
			case SRP.URP:
				res = SUFIX_URP_MATERIAL;
				break;
			case SRP.HDRP:
				break;
		}

		return res;
	}

	private static string GetMaterialFolder(MaterialFolder materialFolder)
	{
		string res = "";

		switch (materialFolder)
		{
			case MaterialFolder.Samples:
				res = SAMPLE_MATERIALS_FOLDER;
				break;
			case MaterialFolder.Slime:
				res = SLIMES_MATERIALS_FOLDER;
				break;
		}

		return res;
	}

	private static string GetMaterialPath(string materialName, SRP srp, MaterialFolder materialFolder)
	{
		string res = PREFIX_PATH + GetMaterialFolder(materialFolder) + GetSufixFolderBySRP(srp) + materialName + GetSufixMaterialBySRP(srp) + MAT_EXTENSION;
		
		return res;
	}
}
#endif