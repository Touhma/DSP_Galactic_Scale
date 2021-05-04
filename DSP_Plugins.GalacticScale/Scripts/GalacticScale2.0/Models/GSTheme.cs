using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
	public class GSTheme
	{
		public string name;
		public EPlanetType type = EPlanetType.Ocean;
		public int LDBThemeId = 1;
		[NonSerialized]
		public bool added = false;
		[NonSerialized]
		public bool initialized = false;
		public int algo = 0;
		public string DisplayName = "Default Theme";
		public GSTheme baseTheme
		{
			get
			{
				GS2.Log("Attempting to get baseTheme for " + name + " it should be " + baseName);
				return (baseName != "" && baseName != null) ? GS2.ThemeLibrary[this.baseName] : null;
			}
			set
			{
				GS2.Log("Setting baseName for " + name + " to " + value);
				baseName = value.name;
			}
		}
		public string baseName;
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
		public GSTheme (string baseName)
        {
			GS2.Log("Creating new Theme based on "+baseName);
			//this.baseName = baseName;
			if (GS2.ThemeLibrary.ContainsKey(baseName)) this.baseName = baseName;
			
			if (baseTheme != null)
			{
				
				CopyFrom(baseTheme);
			} else
            {
				GS2.Log("Error: Base Theme " + baseName + " not found in library");
            }
		}
		public void Process()
        {
			GS2.Log("GSTheme " + name + " Process()");
			if (DisplayName == "Default Theme") DisplayName = name;
			InitMaterials();
			ProcessTints();
			AddToLibrary();
        }
		public GSTheme() { }
		public void AddToLibrary()
        {
			GS2.ThemeLibrary[name] = this;
        }
		public static int[] Clone(int[] source)
        {
			int[] destination = new int[source.Length];
			Array.Copy(source, destination, source.Length);
			return destination;
        }
		public void CopyFrom(GSTheme baseTheme) {
			GS2.Log("GSTheme CopyFrom " + name + " copying from " + baseTheme.name);
			LDBThemeId = baseTheme.LDBThemeId;
			MaterialPath = baseTheme.MaterialPath;
			Temperature = baseTheme.Temperature;
			Distribute = baseTheme.Distribute;
			ModX = new Vector2(baseTheme.ModX.x, baseTheme.ModX.y);
			ModY = new Vector2(baseTheme.ModY.x, baseTheme.ModY.y);
			Vegetables0 = (int[])baseTheme.Vegetables0.Clone();
			Vegetables1 = (int[])baseTheme.Vegetables1.Clone();
			Vegetables2 = (int[])baseTheme.Vegetables2.Clone();
			Vegetables3 = (int[])baseTheme.Vegetables3.Clone();
			Vegetables4 = (int[])baseTheme.Vegetables4.Clone();
			Vegetables5 = (int[])baseTheme.Vegetables5.Clone();
			VeinSpot = (int[])baseTheme.VeinSpot.Clone(); 
			VeinCount = (float[])baseTheme.VeinCount.Clone(); 
			VeinOpacity = (float[])baseTheme.VeinOpacity.Clone();
			RareVeins = (int[])baseTheme.RareVeins.Clone();
			RareSettings = (float[])baseTheme.RareSettings.Clone();
			GasItems = (int[])baseTheme.GasItems.Clone();
			GasSpeeds = (float[])baseTheme.GasSpeeds.Clone();
			UseHeightForBuild = baseTheme.UseHeightForBuild;
			Wind = baseTheme.Wind;
			IonHeight = baseTheme.IonHeight;
			WaterHeight = baseTheme.WaterHeight;
			WaterItemId = baseTheme.WaterItemId;
			Musics = (int[])baseTheme.Musics.Clone();
			SFXPath = baseTheme.SFXPath;
			SFXVolume = baseTheme.SFXVolume;
			CullingRadius = baseTheme.CullingRadius;
			terrainMat = baseTheme.terrainMat != null?UnityEngine.Object.Instantiate(baseTheme.terrainMat):null;
			oceanMat = baseTheme.oceanMat != null ? UnityEngine.Object.Instantiate(baseTheme.oceanMat) : null;
			atmosMat = baseTheme.atmosMat != null ? UnityEngine.Object.Instantiate(baseTheme.atmosMat) : null;
			lowMat = baseTheme.lowMat != null ? UnityEngine.Object.Instantiate(baseTheme.lowMat) : null;
			thumbMat = baseTheme.thumbMat != null ? UnityEngine.Object.Instantiate(baseTheme.thumbMat) : null;
			minimapMat = baseTheme.minimapMat != null ? UnityEngine.Object.Instantiate(baseTheme.minimapMat) : null;
			ambientDesc = baseTheme.ambientDesc != null ? UnityEngine.Object.Instantiate(baseTheme.ambientDesc) : null;
			ambientSfx = baseTheme.ambientSfx != null ? UnityEngine.Object.Instantiate(baseTheme.ambientSfx) : null;
		}
		public ThemeProto ToProto()
        {
			return new ThemeProto()
			{
				name = name,
				Name = name,
				sid = "",
				SID = "",
				PlanetType = type,
				DisplayName = DisplayName,
				displayName = DisplayName,
				Algos = new[] { algo },
				MaterialPath = MaterialPath,
				Temperature = Temperature,
				Distribute = Distribute,
				ModX = ModX,
				ModY = ModY,
				Vegetables0 = Vegetables0,
				Vegetables1 = Vegetables1,
				Vegetables2 = Vegetables2,
				Vegetables3 = Vegetables3,
				Vegetables4 = Vegetables4,
				Vegetables5 = Vegetables5,
				VeinSpot = VeinSpot,
				VeinCount = VeinCount,
				VeinOpacity = VeinOpacity,
				RareVeins = RareVeins,
				RareSettings = RareSettings,
				GasItems = GasItems,
				GasSpeeds = GasSpeeds,
				UseHeightForBuild = UseHeightForBuild,
				Wind = Wind,
				IonHeight = IonHeight,
				WaterHeight = WaterHeight,
				WaterItemId = WaterItemId,
				Musics = Musics,
				SFXPath = SFXPath,
				SFXVolume = SFXVolume,
				CullingRadius = CullingRadius,
				terrainMat = terrainMat,
				oceanMat = oceanMat,
				atmosMat = atmosMat,
				lowMat = lowMat,
				thumbMat = thumbMat,
				minimapMat = minimapMat,
				ambientDesc = ambientDesc,
				ambientSfx = ambientSfx,
				ID = LDBThemeId
			};
        }
		public int AddToThemeProtoSet()
        {
			GS2.Log("AddToThemeProtoSet() " + name);
			if (added) return LDBThemeId;
			if (terrainMat == null) InitMaterials();
			int newIndex = LDB._themes.dataArray.Length; //16 items, so length is 16. ID should be 17, and index should be 16. 
			Array.Resize(ref LDB._themes.dataArray, newIndex + 1); //make it 17 long
			int newId = LDB._themes.dataArray.Length; //id should be 17
			LDBThemeId = newId; //17
			LDB._themes.dataArray[newIndex] = ToProto(); //index shoudl be 16
			LDB._themes.dataIndices[newId] = newIndex; //17
			GS2.Log("Added Theme: " + name + " id " + newId + " index = " + newId + " length of array = " + LDB._themes.dataArray.Length + " and -1.id is " + LDB._themes.dataArray[LDB._themes.dataArray.Length - 1].ID); ;
			added = true;
			return newId;
        }
		public int UpdateThemeProtoSet()
        {
			if (!added) return AddToThemeProtoSet();
			else
            {
				GS2.Log("Updating Themeprotoset");
				LDB._themes.dataArray[LDBThemeId] = ToProto();
				return LDBThemeId;
            }
		}
		public void InitMaterials ()
        {
			GS2.Log("Theme InitMaterials: " + name + " " + DisplayName);
			if (terrainMaterial == null)
				terrainMat = Resources.Load<Material>(MaterialPath + "terrain");
			else terrainMat = GS2.ThemeLibrary[terrainMaterial].terrainMat;
			if (oceanMaterial == null) oceanMat = Resources.Load<Material>(MaterialPath + "ocean");
			else oceanMat = GS2.ThemeLibrary[oceanMaterial].oceanMat;
			if (atmosphereMaterial == null) atmosMat = Resources.Load<Material>(MaterialPath + "atmosphere");
			else atmosMat = GS2.ThemeLibrary[atmosphereMaterial].atmosMat; 
			if (lowMaterial == null) lowMat = Resources.Load<Material>(MaterialPath + "low");
			else lowMat = GS2.ThemeLibrary[lowMaterial].lowMat; 
			if (thumbMaterial == null) thumbMat = Resources.Load<Material>(MaterialPath + "thumb");
			else thumbMat = GS2.ThemeLibrary[thumbMaterial].thumbMat; 
			if (minimapMaterial == null) minimapMat = Resources.Load<Material>(MaterialPath + "minimap");
			else minimapMat = GS2.ThemeLibrary[minimapMaterial].minimapMat;
			if (ambient == null) ambientDesc = Resources.Load<AmbientDesc>(MaterialPath + "ambient");
			else ambientDesc = GS2.ThemeLibrary[ambient].ambientDesc;
			ambientSfx = Resources.Load<AudioClip>(SFXPath);
			ProcessTints();
			GS2.Log("Theme InitMaterials Finished");
		}
		public void SetMaterial(string material, string materialBase)
        {
			GSTheme donorTheme = GS2.ThemeLibrary[materialBase];
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
			if (material == null)
			{
				GS2.Log("TintMaterial Failed. Material = null");
				return null;
			}
			GS2.Log("TintMaterial " + material.name + " - " +color.ToString() );
			Material newMaterial = UnityEngine.Object.Instantiate(material);
			newMaterial.color = color;
			return newMaterial;
        }
		public void ProcessTints()
        {
			GS2.Log("ProcessTints "+name);
			if (terrainTint != new Color()) terrainMat = TintMaterial(terrainMat, terrainTint);
			if (oceanTint != new Color()) oceanMat = TintMaterial(oceanMat, oceanTint);
			if (atmosphereTint != new Color()) atmosMat = TintMaterial(atmosMat, atmosphereTint);
			if (lowTint != new Color()) lowMat = TintMaterial(lowMat, lowTint);
			if (thumbTint != new Color()) thumbMat = TintMaterial(thumbMat, thumbTint);
			if (minimapTint != new Color()) minimapMat = TintMaterial(minimapMat, minimapTint);
			GS2.Log("Finished Processing Tints");
		}
	}
}