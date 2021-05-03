using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
	public class GSTheme
	{
		public string name;
		public EPlanetType type = EPlanetType.Ocean;
		public int LDBThemeId = 1;
		public int algo = 0;
		public string DisplayName = "Default Theme";
		public GSTheme baseTheme;
		public string MaterialPath = "Universe/Materials/Planets/Ocean 1/";
		public float Temperature = 0.0f;
		public EThemeDistribute Distribute = EThemeDistribute.Interstellar;
		public Vector2 ModX = new Vector2(0.0f, 0.0f);
		public Vector2 ModY = new Vector2(0.0f, 0.0f);
		public int[] Vegetables0 = new[] {
				604,
				605,
				603,
				604,
				102,
				604,
				605,
				105,
				602,
				601
			};
		public int[] Vegetables1 = new[] {
			103,
				102,
				103,
				104,
				104,
				104,
				101,
				104,
				604,
				106 };
		public int[] Vegetables2 = new[] {1001,
				1002,
				1003 };
		public int[] Vegetables3 = new[] {1005,
				1006,
				1007,
				1006,
				1007 };
		public int[] Vegetables4 = new[] { 1004 };
		public int[] Vegetables5 = new int[] { };
		public int[] VeinSpot = new int[] { 
			7,
				5,
				0,
				0,
				8,
				11,
				18 
		};
		public float[] VeinCount = new float[] {0.7f,
				0.6f,
				0.0f,
				0.0f,
				1.0f,
				1.0f,
				1.0f };
		public float[] VeinOpacity = new float[] {0.6f,
				0.5f,
				0.0f,
				0.0f,
				0.7f,
				1.0f,
				1.0f };
		public int[] RareVeins = new int[] {11 };
		public float[] RareSettings = new float[] {0.0f,
				1.0f,
				0.3f,
				0.3f };
		public int[] GasItems = new int[] { };
		public float[] GasSpeeds = new float[] { };
		public bool UseHeightForBuild = false;
		public float Wind = 1f;
		public float IonHeight=60f;
		public float WaterHeight=0f;
		public int WaterItemId=1000;
		public int[] Musics = new int[] {9};
		public string SFXPath= "SFX/sfx-amb-ocean-1";
		public float SFXVolume=0.53f;
		public float CullingRadius=0f;
		[NonSerialized]
		public Material terrainMat;
		public string terrainMaterial;
		public Color terrainTint;
		[NonSerialized]
		public Material oceanMat;
		public string oceanMaterial;
		public Color oceanTint;
		[NonSerialized] 
		public Material atmosMat;
		public string atmosphereMaterial;
		public Color atmosphereTint;
		[NonSerialized]
		public Material lowMat;
		public string lowMaterial;
		public Color lowTint;
		[NonSerialized]
		public Material thumbMat;
		public string thumbMaterial;
		public Color thumbTint;
		[NonSerialized]
		public Material minimapMat;
		public string minimapMaterial;
		public Color minimapTint;
		[NonSerialized]
		public AmbientDesc ambientDesc;
		public string ambient;
		[NonSerialized]
		public AudioClip ambientSfx;
		public GSTheme (string name)
        {
			this.name = name;
			DisplayName = name;
			if (baseTheme != null) InitTheme(baseTheme);
			InitMaterials();
			ProcessTints();
			if (RareSettings.Length != RareVeins.Length * 4)
			{
				Debug.LogError("Error with RareSettings != RareVeins.Length * 4");
			}
		}
		public GSTheme() { InitMaterials(); }
		public void InitTheme(GSTheme baseTheme) {
			MaterialPath = baseTheme.MaterialPath;
			Temperature = baseTheme.Temperature;
			Distribute = baseTheme.Distribute;
			ModX = baseTheme.ModX;
			ModY = baseTheme.ModY;
			Vegetables0 = baseTheme.Vegetables0;
			Vegetables1 = baseTheme.Vegetables1;
			Vegetables2 = baseTheme.Vegetables2;
			Vegetables3 = baseTheme.Vegetables3;
			Vegetables4 = baseTheme.Vegetables4;
			Vegetables5 = baseTheme.Vegetables5;
			VeinSpot = baseTheme.VeinSpot;
			VeinCount = baseTheme.VeinCount;
			VeinOpacity = baseTheme.VeinOpacity;
			RareVeins = baseTheme.RareVeins;
			RareSettings = baseTheme.RareSettings;
			GasItems = baseTheme.GasItems;
			GasSpeeds = baseTheme.GasSpeeds;
			UseHeightForBuild = baseTheme.UseHeightForBuild;
			Wind = baseTheme.Wind;
			IonHeight = baseTheme.IonHeight;
			WaterHeight = baseTheme.WaterHeight;
			WaterItemId = baseTheme.WaterItemId;
			Musics = baseTheme.Musics;
			SFXPath = baseTheme.SFXPath;
			SFXVolume = baseTheme.SFXVolume;
			CullingRadius = baseTheme.CullingRadius;
			terrainMat = baseTheme.terrainMat;
			oceanMat = baseTheme.oceanMat;
			atmosMat = baseTheme.atmosMat;
			lowMat = baseTheme.lowMat;
			thumbMat = baseTheme.thumbMat;
			minimapMat = baseTheme.minimapMat;
			ambientDesc = baseTheme.ambientDesc;
			ambientSfx = baseTheme.ambientSfx;
	}
		public void InitMaterials ()
        {
			if (terrainMaterial == null)
				terrainMat = Resources.Load<Material>(MaterialPath + "terrain");
			else terrainMat = GS2.planetThemes[terrainMaterial].terrainMat;
			if (oceanMaterial == null) oceanMat = Resources.Load<Material>(MaterialPath + "ocean");
			else oceanMat = GS2.planetThemes[oceanMaterial].oceanMat;
			if (atmosphereMaterial == null) atmosMat = Resources.Load<Material>(MaterialPath + "atmosphere");
			else atmosMat = GS2.planetThemes[atmosphereMaterial].atmosMat; 
			if (lowMaterial == null) lowMat = Resources.Load<Material>(MaterialPath + "low");
			else lowMat = GS2.planetThemes[lowMaterial].lowMat; 
			if (thumbMaterial == null) thumbMat = Resources.Load<Material>(MaterialPath + "thumb");
			else thumbMat = GS2.planetThemes[thumbMaterial].thumbMat; 
			if (minimapMaterial == null) minimapMat = Resources.Load<Material>(MaterialPath + "minimap");
			else minimapMat = GS2.planetThemes[minimapMaterial].minimapMat;
			if (ambient == null) ambientDesc = Resources.Load<AmbientDesc>(MaterialPath + "ambient");
			else ambientDesc = GS2.planetThemes[ambient].ambientDesc;
			ambientSfx = Resources.Load<AudioClip>(SFXPath);
		}
		public void SetMaterial(string material, string materialBase)
        {
			GSTheme donorTheme = GS2.planetThemes[materialBase];
			switch(material)
            {
				case "terrain":terrainMat = donorTheme.terrainMat; break;
				case "ocean":oceanMat = donorTheme.oceanMat; break;
				case "atmosphere":atmosMat = donorTheme.atmosMat; break;
				case "low":lowMat = donorTheme.lowMat; break;
				case "thumb":thumbMat = donorTheme.thumbMat;break;
				case "minimap":minimapMat = donorTheme.minimapMat;break;
				default: GS2.Log("Error Setting Material: " + material + " does not exist"); break;
            }
        }
		public static Material TintMaterial(Material material, Color color)
        {
			Material newMaterial = UnityEngine.Object.Instantiate(material);
			newMaterial.color = color;
			return newMaterial;
        }
		public void ProcessTints()
        {
			if (terrainTint != null) terrainMat = TintMaterial(terrainMat, terrainTint);
			if (oceanTint != null) oceanMat = TintMaterial(oceanMat, oceanTint);
			if (atmosphereTint != null) atmosMat = TintMaterial(atmosMat, atmosphereTint);
			if (lowTint != null) lowMat = TintMaterial(lowMat, lowTint);
			if (thumbTint != null) thumbMat = TintMaterial(thumbMat, thumbTint);
			if (minimapTint != null) minimapMat = TintMaterial(minimapMat, minimapTint);
		}
	}
}