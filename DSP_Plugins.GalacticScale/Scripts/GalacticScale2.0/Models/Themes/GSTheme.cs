﻿using GSFullSerializer;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace GalacticScale
{
	[fsObject(Converter = typeof(GSFSThemeConverter))]
	public class GSTheme
	{
		public string Name;
		public EPlanetType PlanetType = EPlanetType.Ocean;
		[NonSerialized]
		public bool Base = false;
		[NonSerialized]
		public int LDBThemeId = 1;
		[NonSerialized]
		public bool added = false;
		[NonSerialized]
		public bool initialized = false;
		public int Algo = 0;
		public bool CustomGeneration = false;
		public string DisplayName = "Default Theme";
		public GSTheme baseTheme
		{
			get => (BaseName != "" && BaseName != null) ? GS2.ThemeLibrary[this.BaseName] : null;
			set => BaseName = value.Name;
		}
		public string BaseName;
		public string MaterialPath = "Universe/Materials/Planets/Ocean 1/";
		public float Temperature = 0.0f;
		public EThemeDistribute Distribute = EThemeDistribute.Interstellar;
		[NonSerialized] 
		public Vector2 ModX = new Vector2(0.0f, 0.0f);
		[NonSerialized]
		public Vector2 ModY = new Vector2(0.0f, 0.0f);
		public GSTerrainSettings TerrainSettings = new GSTerrainSettings();
		public GSVeinSettings VeinSettings = new GSVeinSettings()
		{
			Algorithm = "GS2",
			VeinTypes = new List<GSVeinType>()			
		};
		public GSVegeSettings VegeSettings = new GSVegeSettings()
		{
			Algorithm = "Vanilla"
		};
		public int[] Vegetables0 = new int[] {};
		public int[] Vegetables1 = new int[] {};
		public int[] Vegetables2 = new int[] {};
		public int[] Vegetables3 = new int[] {};
		public int[] Vegetables4 = new int[] {};
		public int[] Vegetables5 = new int[] {};
		public int[] VeinSpot = new int[] { };
		public float[] VeinCount = new float[] { };
		public float[] VeinOpacity = new float[] { };
		public int[] RareVeins = new int[] { };
		public float[] RareSettings = new float[] { };
		public int[] GasItems = new int[] {};
		public float[] GasSpeeds = new float[] {};
		public bool UseHeightForBuild = false;
		public float Wind = 1f;
		public float IonHeight=60f;
		public float WaterHeight=0f;
		public int WaterItemId=1000;
		public int[] Musics = new int[] {9};
		public string SFXPath= "SFX/sfx-amb-ocean-1";
		public float SFXVolume=0.53f;
		public float CullingRadius=0f;
		public int IceFlag = 0;
		[NonSerialized]
		public Material terrainMat;
		public GSMaterialSettings terrainMaterial = new GSMaterialSettings();
		//public string terrainMaterial;
		//public Color terrainTint;
		[NonSerialized]
		public Material oceanMat;
		public GSMaterialSettings oceanMaterial = new GSMaterialSettings();
		//public string oceanMaterial;
		//public Color oceanTint;
		[NonSerialized] 
		public Material atmosMat;
		public GSMaterialSettings atmosphereMaterial = new GSMaterialSettings();
		//public string atmosphereMaterial;
		//public Color atmosphereTint;
		[NonSerialized]
		public Material thumbMat;
		public GSMaterialSettings thumbMaterial = new GSMaterialSettings();
		//public string thumbMaterial;
		//public Color thumbTint;
		[NonSerialized]
		public Material minimapMat;
		public GSMaterialSettings minimapMaterial = new GSMaterialSettings();
		//public string minimapMaterial;
		//public Color minimapTint;
		[NonSerialized]
		public AmbientDesc ambientDesc = new AmbientDesc();
		[NonSerialized] 
		public string ambient;
		[NonSerialized]
		public AudioClip ambientSfx;
		public GSAmbientSettings AmbientSettings = new GSAmbientSettings();

		// ////////////////////////////////////////
		// / Constructor
		// ////////////////////////////////////////
		public GSTheme() { }
		public GSTheme (string name, string displayName, string baseName = null)
        {
			DisplayName = displayName;
			Name = name;
			if (baseName != null && GS2.ThemeLibrary.ContainsKey(baseName)) { 
				this.BaseName = baseName;
				GS2.Log("About to Copy From");
				CopyFrom(baseTheme); 
			}
			else if (baseName != null) GS2.Error("Error creating theme '" + name + "': Base Theme '" + baseName + "' not found in theme library");
		}
		public void Process()
        {
			GS2.Log("-Start "+Name + " " + GS2.GetCaller());
			Init();
			GS2.Log("Adding to Library "+ Name + " " + DisplayName);
			AddToLibrary();
			GS2.Log("Cubemap instance = " + ambientDesc?.reflectionMap?.GetInstanceID());
			GS2.Log("End");
        }
		public void Init()
        {
			if (DisplayName == "Default Theme") DisplayName = Name;
			if (!initialized) InitMaterials();
			if (PlanetType != EPlanetType.Gas) {
				if (AmbientSettings == null)
				{
					AmbientSettings = new GSAmbientSettings();
					AmbientSettings.FromTheme(this);
				}
				else
				{
					GS2.Log("Setting CubeMap for " + Name);
					GS2.Log("Cubemap instance = " + ambientDesc?.reflectionMap.GetInstanceID());
					AmbientSettings.ToTheme(this);
					GS2.Log("Cubemap instance = " + ambientDesc?.reflectionMap.GetInstanceID());
				}
				GS2.Log("Finished Processing Ambient Settings for " + Name);
			}
			
            if (VeinSettings.RequiresConversion && !Base) ConvertVeinData();
            else PopulateVeinData();
            if (VegeSettings.Empty) PopulateVegeData();
			else ConvertVegeData();
			ProcessTints();
			if (TerrainSettings.BrightnessFix)
			{
				terrainMat.SetFloat("_HeightEmissionRadius", 5); //fix for lava
			}
		}
		public void PopulateVegeData()
        {
			VegeSettings.Group1 = GSVegeSettings.FromIDArray(Vegetables0);
			VegeSettings.Group2 = GSVegeSettings.FromIDArray(Vegetables1);
			VegeSettings.Group3 = GSVegeSettings.FromIDArray(Vegetables2);
			VegeSettings.Group4 = GSVegeSettings.FromIDArray(Vegetables3);
			VegeSettings.Group5 = GSVegeSettings.FromIDArray(Vegetables4);
			VegeSettings.Group6 = GSVegeSettings.FromIDArray(Vegetables5);
		}
		public void ConvertVegeData()
        {
			GS2.Log("Converting Vege Data for " + Name + " Group 1");
			Vegetables0 = GSVegeSettings.ToIDArray(VegeSettings.Group1);
			GS2.Log("Converting Vege Data for " + Name + " Group 2"); 
			Vegetables1 = GSVegeSettings.ToIDArray(VegeSettings.Group2);
			GS2.Log("Converting Vege Data for " + Name + " Group 3");
			Vegetables2 = GSVegeSettings.ToIDArray(VegeSettings.Group3);
			GS2.Log("Converting Vege Data for " + Name + " Group 4");
			Vegetables3 = GSVegeSettings.ToIDArray(VegeSettings.Group4);
			GS2.Log("Converting Vege Data for " + Name + " Group 5"); 
			Vegetables4 = GSVegeSettings.ToIDArray(VegeSettings.Group5);
			GS2.Log("Converting Vege Data for " + Name + " Group 6");
			Vegetables5 = GSVegeSettings.ToIDArray(VegeSettings.Group6);
		}
		public void ConvertVeinData()
		{
			GS2.Log("Start "+Name);
			//GS2.Log(Name + "-" + VeinSpot.Length.ToString());
			List<float> _rareSettings = new List<float>();
			List<int> _rareVeins = new List<int>();
			int veinArrayLength = 25;
			if (PlanetModelingManager.veinProtos != null) veinArrayLength = PlanetModelingManager.veinProtos.Length;
			VeinSpot = new int[veinArrayLength];
			VeinOpacity = new float[veinArrayLength];
			VeinCount = new float[veinArrayLength];
		
			for (var i = 0; i < VeinSettings.VeinTypes.Count; i++)
			{ // For each EVeinType
				//GS2.Log("Getting VeinType");
				GSVeinType vt = VeinSettings.VeinTypes[i];
				var type = vt.type;
				float opacity = 0;
				float count = 0;
				int veinCount = vt.veins.Count;
				//GS2.Log("Type:" + type + " veinCount:" + veinCount);
				for (var j = 0; j < veinCount; j++)
				{
					//GS2.Log("Getting Vein");
					GSVein v = vt.veins[j];
					count += v.count;
					opacity += v.richness;
				}
				//GS2.Log("Count:" + count + " Opacity:" + opacity);
                //if ((int)type < 8)
                if (vt.rare)
                {  //Special
                    //GS2.Log("Rare");
                    var specialOpacity = opacity / veinCount;
                    var specialCount = veinCount;
                    //var specialNumber = count / veinCount;
                    //var specialIndex = RareVeins.Length;
                    //var specialSettingsIndex = specialIndex * 4;
                    _rareVeins.Add((int)type);
					if (GS2.Force1RareChance) _rareSettings.Add(1);
					else _rareSettings.Add(0); //Chance to spawn on birth star planet (Should be 0, 1 for testing)
                    _rareSettings.Add(1); //Chance to spawn on non birth star planet (Could take into account the vanilla rare spawning factors)
                    _rareSettings.Add(specialCount / 25); //Chance for extra vein to spawn
                    _rareSettings.Add(specialOpacity); //Stupidly combined count and opacity
                }
                else
                {
                    //GS2.Log("Not Rare. Index:" + ((int)type - 1) + " for Type: " + type + " (int)=" + (int)type);
                    VeinOpacity[(int)type - 1] = opacity / veinCount;
                    VeinCount[(int)type - 1] = count / 25 / veinCount;
                    VeinSpot[(int)type - 1] = veinCount;
                    //GS2.Log(type.ToString() + "| Set Spot:" + veinCount + " Count to" + VeinCount[(int)type - 1] + " Opacity to " + VeinOpacity[(int)type - 1]);
                }
            }
			RareSettings = _rareSettings.ToArray();
			RareVeins = _rareVeins.ToArray();
            //GS2.Log("VeinSpot");
            //GS2.LogJson(VeinSpot);
            //GS2.Log("VeinCount");
            //GS2.LogJson(VeinCount);
            //GS2.Log("VeinOpacity");
            //GS2.LogJson(VeinOpacity);
            //GS2.Log("RareSettings");
            //GS2.LogJson(RareSettings);
            //GS2.Log("RareVeins");
            //GS2.LogJson(RareVeins);
            GS2.Log("End " + Name);
        }
		public void PopulateVeinData()
		{
			for (var vType = 0; vType < VeinSpot.Length; vType++)
			{
				if (VeinSpot[vType] == 0) continue;
				GSVeinType tempVeinGroup = new GSVeinType()
				{
						type = (EVeinType)(vType+1),
						veins = new List<GSVein>()
				};
				for (var vCount = 0; vCount < VeinSpot[vType]; vCount++)
				{
					tempVeinGroup.veins.Add(
						new GSVein()
						{
							count = (int)(VeinCount[vType] * 25),
							richness = VeinOpacity[vType]
						}
					);
				}
				VeinSettings.VeinTypes.Add(tempVeinGroup);
			}
			if (RareVeins.Length == 0) return;
			for (var i = 0; i<RareVeins.Length;i++)
            {
				float richness = RareSettings[i * 4 + 3];
				int count = (int)(richness * 25);
				GSVeinType tempVeinGroup = new GSVeinType()
				{
					type = (EVeinType)RareVeins[i],
					veins = new List<GSVein>(),
					rare = true
				};
				for (var j = 0; j < count; j++)
                {
					tempVeinGroup.veins.Add(new GSVein()
					{
						count = count,
						richness = richness
					});
                }
				VeinSettings.VeinTypes.Add(tempVeinGroup);
			}
		}

		public void AddToLibrary()
        {
			GS2.ThemeLibrary[Name] = this;
        }
		public static int[] CloneIntegerArray(int[] source)
        {
			int[] destination = new int[source.Length];
			Array.Copy(source, destination, source.Length);
			return destination;
        }

		/// <summary>
		/// Initialise this theme from another theme's data.
		/// </summary>
		/// <param name="baseTheme"></param>
		public void CopyFrom(GSTheme baseTheme) {
			GS2.Log("Copying from " + baseTheme.Name);
			if (!baseTheme.initialized) baseTheme.InitMaterials();
			Algo = baseTheme.Algo;
			PlanetType = baseTheme.PlanetType;
			LDBThemeId = 1;
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
			VeinSettings = baseTheme.VeinSettings.Clone();
			TerrainSettings = baseTheme.TerrainSettings.Clone();
			VegeSettings = baseTheme.VegeSettings.Clone();
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
			terrainMat = (baseTheme.terrainMat != null)?UnityEngine.Object.Instantiate(baseTheme.terrainMat): null;
			oceanMat = (baseTheme.oceanMat != null) ? UnityEngine.Object.Instantiate(baseTheme.oceanMat) : null;
			atmosMat = (baseTheme.atmosMat != null) ? UnityEngine.Object.Instantiate(baseTheme.atmosMat) : null;
			thumbMat = (baseTheme.thumbMat != null) ? UnityEngine.Object.Instantiate(baseTheme.thumbMat) : null;
			minimapMat = (baseTheme.minimapMat != null) ? UnityEngine.Object.Instantiate(baseTheme.minimapMat) : null;
			ambientDesc = (baseTheme.ambientDesc != null) ? UnityEngine.Object.Instantiate(baseTheme.ambientDesc) : null;
			ambientSfx = (baseTheme.ambientSfx != null) ? UnityEngine.Object.Instantiate(baseTheme.ambientSfx) : null;
			GS2.Log("Copying ambientSettings for " + Name);
			if (PlanetType != EPlanetType.Gas) {
				if (baseTheme.AmbientSettings == null) { baseTheme.AmbientSettings = new GSAmbientSettings(); baseTheme.AmbientSettings.FromTheme(baseTheme); }
				if (baseTheme.AmbientSettings != null) AmbientSettings = baseTheme.AmbientSettings.Clone();
			}
		}

		/// <summary>
		/// Convert to a ThemeProto so that the game can use the materials etc
		/// </summary>
		/// <returns></returns>
		public ThemeProto ToProto()
        {
			return new ThemeProto()
			{
				name = Name,
				Name = Name,
				sid = "",
				SID = "",
				PlanetType = PlanetType,
				DisplayName = DisplayName,
				displayName = DisplayName,
				Algos = new[] { Algo },
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
				thumbMat = thumbMat,
				minimapMat = minimapMat,
				ambientDesc = ambientDesc,
				ambientSfx = ambientSfx,
				ID = LDBThemeId
			};
        }
		public int AddToThemeProtoSet()
        {
			if (added) return LDBThemeId;
			if (!initialized) InitMaterials();
            int newIndex = LDB._themes.dataArray.Length;
			Array.Resize(ref LDB._themes.dataArray, newIndex + 1); 
			int newId = LDB._themes.dataArray.Length;
			LDBThemeId = newId;
			LDB._themes.dataArray[newIndex] = ToProto();
			LDB._themes.dataIndices[newId] = newIndex;
			added = true;
            return newId;
        }
		public int UpdateThemeProtoSet()
        {
			if (!added) return AddToThemeProtoSet();
			else
            {		
				terrainMat.SetFloat("_Radius", 100f);
                LDB._themes.dataArray[LDB._themes.dataIndices[LDBThemeId]] = ToProto();
                return LDBThemeId;
            }
		}
		private bool CreateMaterial(GSMaterialSettings settings, out Material material)
        {
            GS2.Log("Start|"+Name);
			string materialType = "terrain";
			if (settings == oceanMaterial) materialType = "ocean";
			if (settings == atmosphereMaterial) materialType = "atmosphere";
			if (settings == minimapMaterial) materialType = "minimap";
			if (settings == thumbMaterial) materialType = "thumb";
			if (settings.CopyFrom == null)
			{
				GS2.Log("Not Copying From Another Theme");
				Material tempMat;
				if (settings.Path == null)
				{
					GS2.Log("Creating Material from MaterialPath Resource @ " + MaterialPath + materialType);
					tempMat = Resources.Load<Material>(MaterialPath + materialType);
				}
				else
				{
					GS2.Log("Creating Material from Settings Defined Resource @ " + settings.Path);
					tempMat = Resources.Load<Material>(settings.Path);
				}
				if (tempMat != null)
				{
					GS2.Log("Creating Material");
					material = UnityEngine.Object.Instantiate(tempMat);
				}
				else
				{
					GS2.Log("Failed to Create Material|" + Name);
					material = Resources.Load<Material>(MaterialPath + materialType);
				}

			}
			else
			{
				GS2.Log("Copying from Theme: " + settings.CopyFrom);
				string[] copyFrom = settings.CopyFrom.Split('.');
				if (copyFrom.Length != 2 || copyFrom[0] == null || copyFrom[0] == "" || copyFrom[1] == null || copyFrom[1] == "")
				{
					GS2.Error("Copyfrom Parameter for Theme Material cannot be parsed. Please ensure it is in the format ThemeName.terrainMat etc");
					material = Resources.Load<Material>(MaterialPath + materialType);
				}
				else
				{
					GSTheme materialBaseTheme = GS2.ThemeLibrary.Find(copyFrom[0]);
					string materialName = copyFrom[1];
					material = UnityEngine.Object.Instantiate((Material)typeof(GSTheme).GetField(materialName).GetValue(materialBaseTheme));
				}
			}
			GS2.Log("Setting Textures for " + Name + " with " + settings.Textures.Count + " texture items in list");
			foreach (var kvp in settings.Textures)
			{
				
				string[] value = kvp.Value.Split('|');
				string location = value[0];
				string path = value[1];
				string name = kvp.Key;
				GS2.Log("Setting Texture " + name + " from " + location + " / " + path);
				Texture tex = null;
				if (location == "GS2") tex = Utils.GetTextureFromBundle(path);
                if (location == "FILE") tex = Utils.GetTextureFromFile(System.IO.Path.Combine(GS2.DataDir,path));
				if (location == "RESOURCE") tex = Utils.GetTextureFromResource(path);
				if (location == "BUNDLE") tex = Utils.GetTextureFromExternalBundle(path);
				if (tex == null)
				{
					GS2.Error("Texture not found");
				}
				else
				{
					GS2.Log("Assigning Texture");
					material.SetTexture(name, tex);
				}

			}
			return false;
		}

		public void InitMaterials ()
        {
			GS2.Log("Start");
			if (initialized) return;
			GS2.Log("Creating Terrain Material");
			CreateMaterial(terrainMaterial,out terrainMat);
			GS2.Log("Creating Ocean Material");
			CreateMaterial(oceanMaterial, out oceanMat);
			GS2.Log("Creating Atmosphere Material");
			CreateMaterial(atmosphereMaterial, out atmosMat);
			GS2.Log("Creating Minimap Material");
			CreateMaterial(minimapMaterial, out minimapMat);
			GS2.Log("Creating Thumb Material");
			CreateMaterial(thumbMaterial, out thumbMat);
			GS2.Log("Initializing AmbientDesc");
			if (PlanetType != EPlanetType.Gas)
			{
				if (AmbientSettings.ResourcePath != null && AmbientSettings.ResourcePath != "")
				{
					GS2.Log("Loading AmbientDesc from AmbientSettings.ResourcePath" + AmbientSettings.ResourcePath);
					Resources.Load<AmbientDesc>(AmbientSettings.ResourcePath);
				}
				else if (ambient == null)
				{
					GS2.Log("Loading AmbientDesc from MaterialPath = " + MaterialPath + "ambient");
					ambientDesc = Resources.Load<AmbientDesc>(MaterialPath + "ambient");
				}
				else {
					GS2.Log("Loading AmbientDesc from base theme = "+ambient);
					ambientDesc = GS2.ThemeLibrary[ambient].ambientDesc; 
				}
				ambientSfx = Resources.Load<AudioClip>(SFXPath);
			}
			initialized = true;
			GS2.Log("About to process tints for "+Name);
            ProcessTints();
			ProcessMaterialSettings();
		}
		public void ProcessMaterialSettings()
        {
			GS2.Log("Processing MaterialSettings for " + Name);
			foreach (var kvp in terrainMaterial?.Colors)
			{
				GS2.Log("Setting Terrain Material Color " + kvp.Key + " to " + kvp.Value.ToString() + " for " + Name);
				terrainMat.SetColor(kvp.Key, kvp.Value);
			}
			foreach (var kvp in oceanMaterial?.Colors) oceanMat.SetColor(kvp.Key, kvp.Value);
			foreach (var kvp in atmosphereMaterial?.Colors) atmosMat.SetColor(kvp.Key, kvp.Value);
			foreach (var kvp in thumbMaterial?.Colors) thumbMat.SetColor(kvp.Key, kvp.Value);
			foreach (var kvp in minimapMaterial?.Colors) minimapMat.SetColor(kvp.Key, kvp.Value);
			foreach (var kvp in terrainMaterial?.Params) terrainMat.SetFloat(kvp.Key, kvp.Value);
			foreach (var kvp in oceanMaterial?.Params) oceanMat.SetFloat(kvp.Key, kvp.Value);
			foreach (var kvp in atmosphereMaterial?.Params) atmosMat.SetFloat(kvp.Key, kvp.Value);
			foreach (var kvp in thumbMaterial?.Params) thumbMat.SetFloat(kvp.Key, kvp.Value);
			foreach (var kvp in minimapMaterial?.Params) minimapMat.SetFloat(kvp.Key, kvp.Value);
		}
		public void SetMaterial(string material, string materialBase)
        {
			GSTheme donorTheme = GS2.ThemeLibrary[materialBase];
			switch(material)
            {
				case "terrain":terrainMat = donorTheme.terrainMat; break;
				case "ocean":oceanMat = donorTheme.oceanMat; break;
				case "atmosphere":atmosMat = donorTheme.atmosMat; break;
				case "thumb":thumbMat = donorTheme.thumbMat;break;
				case "minimap":minimapMat = donorTheme.minimapMat;break;
				default: GS2.Log("Error Setting Material: " + material + " does not exist"); break;
            }
        }
		public static Material TintMaterial(Material material, Color color)
        {
			if (material == null) return null;
			
			Material newMaterial = UnityEngine.Object.Instantiate(material);
			newMaterial.color = color;
			return newMaterial;
        }
		public void ProcessTints()
        {
			GS2.Log("Processing Terrain Tint for " + Name);
			if (terrainMaterial.Tint != new Color()) TintTerrain(terrainMaterial.Tint);
			GS2.Log("Processing Ocean Tint for " + Name);
			if (oceanMaterial.Tint != new Color())
			{
				GS2.Log("Color Found");
				TintOcean(oceanMaterial.Tint);
			} else
            {
				GS2.Log("No Color = " + oceanMaterial.Tint.ToString());
				//TintOcean(new Color(.3f, .3f, 0.3f, 1));
            }
			if (atmosphereMaterial.Tint != new Color()) TintAtmosphere(atmosphereMaterial.Tint);
			//TODO
			//if (lowTint != new Color()) lowMat = TintMaterial(lowMat, lowTint); //This doesn't appear to exist in any theme?
			if (thumbMaterial.Tint != new Color()) thumbMat = TintMaterial(thumbMat, thumbMaterial.Tint);
			if (minimapMaterial.Tint != new Color()) minimapMat = TintMaterial(minimapMat, minimapMaterial.Tint);
		}
		//public void Monkey(Color c)
		//{
		//	if (oceanMat != null) TintOcean(c);
  //          //atmosMat.SetFloat("_Density", 0.5f); // no effect?
  //          //atmosMat.SetFloat("_SkyAtmosPower", 1f); //Lower makes atmosphere thicker. 1f makes it about 0.5 radius thick
  //          //atmosMat.SetFloat("_FarFogDensity", 20.0f); // Fog viewed from space 

  //          //atmosMat.SetFloat("_FogDensity", 20f); //0.9f //Fog viewed from ground
  //          //atmosMat.SetColor("_CausticsColor", Color.yellow);//
  //          //atmosMat.SetColor("_Color", Color.red);//: { r: 0.3443396, g: 0.734796, b: 1, a: 1}

  //          //atmosMat.SetColor("_Color0", Color.clear);//Outer atmosphere viewed from space : { r: 0.3899999, g: 0.488919, b: 1, a: 1}
  //          //atmosMat.SetColor("_Color1", Color.clear);//Closer... : { r: 0, g: 0.7073908, b: 1, a: 1}
  //          //atmosMat.SetColor("_Color2", Color.clear);//Closer...: { r: 0.2117646, g: 0.8043795, b: 0.9607843, a: 1}
  //          //atmosMat.SetColor("_Color3", Color.clear);//Close to planet, viewed from space : { r: 0.5727176, g: 0.9294221, b: 0.9529411, a: 1}

  //          //atmosMat.SetColor("_Color4", Color.green);//Sunny Fog Colour? : { r: 1, g: 0.7391673, b: 0.413056, a: 1}
  //          //atmosMat.SetColor("_Color5", Color.green);//Fog Colour @ Horizon: { r: 0.240566, g: 0.5836905, b: 1, a: 1}

  //          //atmosMat.SetColor("_Color6", Color.clear);//Haze seen from space towards star? : { r: 0.6941177, g: 0.3529412, b: 1, a: 1}
  //          //atmosMat.SetColor("_Color7", Color.clear);//Haze seen from space away from star? : { r: 0.2074581, g: 0.3139569, b: 0.6981132, a: 1}

  //          //atmosMat.SetColor("_Color8", Color.magenta);//Twilight Fog: { r: 1, g: 1, b: 1, a: 1}
  //          //atmosMat.SetColor("_ColorF", Color.yellow);//Fog Colour : { r: 0.4327686, g: 0.5402345, b: 0.7372549, a: 1}
  //          //atmosMat.SetColor("_EmissionColor", Color.clear);//?: { r: 0, g: 0, b: 0, a: 1}
  //          //atmosMat.SetColor("_LocalPos", Color.clear);//: { r: -76.93655, g: -113.229, b: 165.6604, a: 0}
  //          //atmosMat.SetColor("_PlanetPos", Color.clear);//: { r: 0, g: 0, b: 0, a: 0}
  //          //atmosMat.SetColor("_PlanetRadius", new Color(20,20,30,0));//: { r: 200, g: 199.98, b: 270, a: 0}

  //          //atmosMat.SetColor("_Sky0", Color.clear);//Day Horizon from ground
  //          //atmosMat.SetColor("_Sky1", Color.clear);//Day Sky from ground
  //          //atmosMat.SetColor("_Sky2", Color.clear);//Night Horizon from ground
  //          //atmosMat.SetColor("_Sky3", Color.clear);//Night Horizon from ground

  //          //atmosMat.SetColor("_Sky4", Color.clear);//Day Sky Sunset? : { r: 1, g: 0.7298433, b: 0.3081232, a: 1}

		//}
		public void TintTerrain(Color c)
        {
			if (terrainMat == null) return;
			SetColor(terrainMat,"_Color",c);
			SetColor(terrainMat, "_AmbientColor0", c);
			SetColor(terrainMat, "_AmbientColor1", c);
			SetColor(terrainMat, "_AmbientColor2", c);
			SetColor(terrainMat, "_LightColorScreen", c);
            SetColor(terrainMat, "_HeightEmissionColor", c);
            SetColor(terrainMat, "_SpeclColor", c);
        }

		public void SetColor(Material mat, string name, Color c)
        {
			
			if (mat == null || !mat.HasProperty(name)) return;
			Color origColor = mat.GetColor(name);
			float gs = origColor.grayscale;
			float a = origColor.a;
			Color origGrayScale = new Color(gs, gs, gs, a);
			float lerp = c.a;
			Color toColor = new Color(c.r, c.g, c.b, a);
            mat.SetColor(name, Color.Lerp(origGrayScale, toColor, lerp));
        }
		public void TintAtmosphere(Color c)
		{
			if (atmosMat == null) return;
			SetColor(atmosMat, "_CausticsColor", c);
			SetColor(atmosMat, "_Color", c);
			SetColor(atmosMat, "_Color0", c);
			SetColor(atmosMat, "_Color1", c);
			SetColor(atmosMat, "_Color2", c);
			SetColor(atmosMat, "_Color3", c);
			SetColor(atmosMat, "_Color4", c);
			SetColor(atmosMat, "_Color5", c);
			SetColor(atmosMat, "_Color6", c);
			SetColor(atmosMat, "_Color7", c);
			SetColor(atmosMat, "_Color8", c);
			SetColor(atmosMat, "_ColorF", c);
			SetColor(atmosMat, "_Sky0", c);
			SetColor(atmosMat, "_Sky1", c);
			SetColor(atmosMat, "_Sky2", c);
			SetColor(atmosMat, "_Sky3", c);
			SetColor(atmosMat, "_Sky4", c);
            SetColor(atmosMat, "_EmissionColor", c);
        }
		public void TintOcean(Color c)
        {
			GS2.Log("Start");
			if (oceanMat == null)
			{
				GS2.Warn("oceanMat Null for " + Name);
				return;
			}
			SetColor(oceanMat,"_CausticsColor", c); //Highlights
			SetColor(oceanMat, "_Color", c); //Shore
			SetColor(oceanMat, "_Color0", c); 
			SetColor(oceanMat, "_Color1", c); //Shalows
			SetColor(oceanMat, "_Color2", c); //Mids
			SetColor(oceanMat, "_Color3", c); //Deep
			SetColor(oceanMat, "_FoamColor", c); //Barely visible
			SetColor(oceanMat, "_FresnelColor", c); //Horizon tint
			SetColor(oceanMat, "_SpeclColor", c);
			SetColor(oceanMat, "_SpeclColor1", c);
			SetColor(oceanMat, "_ReflectionColor", c);
			GS2.Log("End");
			//oceanMat.SetColor("_DepthFactor", new Color(.4f, .5f, .4f, 0.1f));
			//Used as Vector4 in the shader
			//X 0.1alpha seems best, really just determines height of ripples. 0.9 looks terrible.
			//Y Lowering R channel from .5 to .1 makes deepest parts look shallower.
			//Z Lowering G to .1 makes the water look transparent
			//W Lowering B to 0.1 makes shallows look more opaque, foam stand out more.
			
		}
	}
}