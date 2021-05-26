using System;
using UnityEngine;

namespace GalacticScale
{
    public class GSAmbientSettings
    {
		public Color Color1 = Color.black;
		public Color Color2 = Color.black;
		public Color Color3 = Color.black;
		public Color WaterColor1 = Color.black;
		public Color WaterColor2 = Color.black;
		public Color WaterColor3 = Color.black;
		public Color BiomeColor1 = new Color(1f, 1f, 1f, 0f);
		public Color BiomeColor2 = new Color(1f, 1f, 1f, 0f);
		public Color BiomeColor3 = new Color(1f, 1f, 1f, 0f);
		public Color DustColor1 = new Color(1f, 1f, 1f, 0f);
		public Color DustColor2 = new Color(1f, 1f, 1f, 0f);
		public Color DustColor3 = new Color(1f, 1f, 1f, 0f);
		public float DustStrength1 = 1f;
		public float DustStrength2 = 1f;
		public float DustStrength3 = 1f;
		public int BiomeSound1;
		public int BiomeSound2;
		public int BiomeSound3;
		public string CubeMap = "Vanilla";
		[NonSerialized]
		public float LutContribution;
		[NonSerialized]
		public Cubemap ReflectionMap;
		[NonSerialized]
		public Texture2D LutTexture;
		
		public GSAmbientSettings () { }
		
		public void FromTheme(GSTheme theme)
        {
			Color1 = theme.ambientDesc.ambientColor0;
			Color2 = theme.ambientDesc.ambientColor1;
			Color3 = theme.ambientDesc.ambientColor2;
			WaterColor1 = theme.ambientDesc.waterAmbientColor0;
			WaterColor2 = theme.ambientDesc.waterAmbientColor1;
			WaterColor3 = theme.ambientDesc.waterAmbientColor2;
			BiomeColor1 = theme.ambientDesc.biomoColor0;
			BiomeColor2 = theme.ambientDesc.biomoColor1;
			BiomeColor3 = theme.ambientDesc.biomoColor2;
			DustColor1 = theme.ambientDesc.biomoDustColor0;
			DustColor2 = theme.ambientDesc.biomoDustColor1;
			DustColor3 = theme.ambientDesc.biomoDustColor2;
			DustStrength1 = theme.ambientDesc.biomoDustStrength0;
			DustStrength2 = theme.ambientDesc.biomoDustStrength1;
			DustStrength3 = theme.ambientDesc.biomoDustStrength2;
			BiomeSound1 = theme.ambientDesc.biomoSound0;
			BiomeSound2 = theme.ambientDesc.biomoSound1;
			BiomeSound3 = theme.ambientDesc.biomoSound2;
			LutContribution = theme.ambientDesc.lutContribution;
			ReflectionMap = theme.ambientDesc.reflectionMap;
			LutTexture = theme.ambientDesc.lutTexture;
			if (ReflectionMap.name.Split('_')[0] == "def") CubeMap = "Vanilla";
			else CubeMap = ReflectionMap.name;
		}
		public void ToTheme(GSTheme theme)
        {
			// This should already been defaulted by the base theme if that exists
			if (CubeMap == "Vanilla" || CubeMap == null)
			{
				//do nothing
			}//////////// FIX RARE SPAWNING IN THEME
			else
			{
				//load cubemap from asset bundle or something
			}
			theme.ambientDesc.ambientColor0 = Color1;
			theme.ambientDesc.ambientColor1 = Color2;
			theme.ambientDesc.ambientColor2 = Color3;
			theme.ambientDesc.waterAmbientColor0 = WaterColor1;
			theme.ambientDesc.waterAmbientColor1 =WaterColor2 ;
			theme.ambientDesc.waterAmbientColor2 =WaterColor3  ;
			theme.ambientDesc.biomoColor0= BiomeColor1  ;
			theme.ambientDesc.biomoColor1 =BiomeColor2  ;
			theme.ambientDesc.biomoColor2= BiomeColor3  ;
			theme.ambientDesc.biomoDustColor0 =DustColor1  ;
			theme.ambientDesc.biomoDustColor1= DustColor2  ;
			theme.ambientDesc.biomoDustColor2 =DustColor3  ;
			theme.ambientDesc.biomoDustStrength0 =DustStrength1  ;
			theme.ambientDesc.biomoDustStrength1 =DustStrength2  ;
			theme.ambientDesc.biomoDustStrength2 =DustStrength3  ;
			theme.ambientDesc.biomoSound0= BiomeSound1  ;
			theme.ambientDesc.biomoSound1 =BiomeSound2  ;
			theme.ambientDesc.biomoSound2 =BiomeSound3  ;
			theme.ambientDesc.lutContribution =LutContribution  ;
			if (ReflectionMap != null) theme.ambientDesc.reflectionMap =ReflectionMap  ;
			if (LutTexture != null) theme.ambientDesc.lutTexture =LutTexture ;

		}
	}
	
}