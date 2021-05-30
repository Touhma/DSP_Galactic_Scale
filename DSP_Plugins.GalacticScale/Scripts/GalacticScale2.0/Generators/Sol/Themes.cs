using UnityEngine;
using System.Collections.Generic;
namespace GalacticScale.Generators
{
    public partial class Sol : iConfigurableGenerator
    {
        public void InitThemes()
        {
            GS2.Log("Creating Themes for Sol");
            GSTheme oiler = new GSTheme("OilGiant", "SpaceWhale Excrement", "IceGiant");
            oiler.terrainMaterial.Tint = new Color(0.3f, 0.3f, 0.3f, 1f);
            oiler.atmosphereMaterial.Tint = new Color(0f, 0f, 0f, 1);
            oiler.thumbMaterial.Tint = new Color(0.01f, 0.005f, 0f, 0.001f);
            oiler.PlanetType = EPlanetType.Gas;
            oiler.TerrainSettings.Algorithm = "GSTA1";
            oiler.TerrainSettings.HeightMulti = 1.4;
            oiler.CustomGeneration = true;
            oiler.GasItems[0] = 1114;
            oiler.GasItems[1] = 1120;
            oiler.GasSpeeds[0] = 0.1f;
            oiler.GasSpeeds[1] = 10f;
            oiler.Process();
            GS2.Log("Oiler Processed");

            GSTheme obsidian = new GSTheme("Obsidian", "Obsidian", "IceGelisol");
            obsidian.terrainMaterial.Tint = new Color(0.05f, 0.05f, 0.05f, 1);
            obsidian.oceanMaterial.Tint = new Color(0.0f, 0.533f, 0.501f, 0.5f);
            obsidian.atmosphereMaterial.Tint = Color.black;
            obsidian.TerrainSettings.Algorithm = "GSTA5";
            obsidian.TerrainSettings.BiomeHeightMulti = -10f;
            obsidian.CustomGeneration = true;
            obsidian.AmbientSettings.CubeMap = "GS2";
            obsidian.terrainMat.SetFloat("_HeightEmissionRadius", 0f);
            obsidian.terrainMat.SetFloat("_EmissionStrength", 0f);
            obsidian.terrainMat.SetFloat("_NormalStrength", 0.3f);
            obsidian.terrainMat.SetFloat("_Distance", 0f);
            obsidian.Process();
            
            GS2.Log("About to Process redIce");
            GSTheme redIce = new GSTheme("IceMalusol", "Ice Malusol", "IceGelisol");
            redIce.terrainMaterial.Textures.Add("_BioTex0A", "GS2|red-ice");
            redIce.terrainMaterial.Textures.Add("_BioTex1A", "GS2|grey-rock");
            redIce.terrainMaterial.Textures.Add("_BioTex2A", "GS2|grey-snow");
            redIce.AmbientSettings.Reflections = new Color(1, 0, 0, 1);
            redIce.AmbientSettings.CubeMap = "GS2";
            redIce.Process();

            GSTheme venus = new GSTheme("AcidGreenhouse", "Acid Greenhouse", "VolcanicAsh");
            venus.atmosphereMaterial.Tint =new Color(0.5f, 0.4f, 0.0f, 0.8f);
            venus.atmosphereMaterial.Params = new Dictionary<string, float>()
            {
                ["_AtmoDensity"] = 1,
                ["_Cutoff"] = 0.1f,
                ["_FarFogDensity"] = 1f,
                ["_FogDensity"] = 1f,
                ["_FogSaturate"] = 1.8f,
                ["_Intensity"] = 1.8f,
                ["_Parallax"] = 0.02f,
                ["_RimFogExp"] = 1.3f,
                ["_RimFogPower"] = 3f,
                ["_SkyAtmosPower"] = 17f,
                ["_SunColorAdd"] = 40f,
                ["_SunColorSkyUse"] = 0.4f,
                ["_SunColorUse"] = 0.2f,
                ["_SunRiseScatterPower"] = 160f
            };
            venus.oceanMaterial.Tint = new Color(0.7f, 0.7f, 0.07f);
            venus.terrainMaterial.Tint =new Color(0.901f, 0.686f, 0.098f, 1);
            venus.thumbMaterial.Tint = new Color(0.901f, 0.686f, 0.098f, 1);
            venus.minimapMaterial.Tint = new Color(0.901f, 0.686f, 0.098f, 1);
            venus.terrainMaterial.CopyFrom = "AshenGelisol.terrainMat";
            venus.terrainMaterial.Colors = new Dictionary<string, Color>()
            {
                ["_AmbientColor0"] = new Color(0.9f, 0.9f, 0.1f, 1f),
                ["_AmbientColor0"] = new Color(0.8f, 0.8f, 0.7f, 1f),
                ["_AmbientColor0"] = new Color(0.5f, 0.5f, 0.0f, 1f),
                ["_Color"] = new Color(0.65f, 0.5f, 0.15f, 1f),
            };
            venus.terrainMaterial.Params = new Dictionary<string, float>()
            {
                ["_AmbientInc"] = 0.5f
            };
            venus.AmbientSettings.DustStrength1 = 10;
            venus.AmbientSettings.DustStrength2 = 10;
            venus.AmbientSettings.DustStrength3 = 10;
            venus.AmbientSettings.DustColor1 = new Color(0.38f,0.38f, 0f, 1f);
            venus.AmbientSettings.DustColor2 = new Color(0.95f, 0.75f, 0.25f, 1f);
            venus.AmbientSettings.DustColor3 = new Color(0.9f, 0.7f, 0.2f, 1f);
            venus.AmbientSettings.LutContribution = 0.8f;
            venus.CustomGeneration = true;
            venus.TerrainSettings.Algorithm = "GSTA3";
            venus.TerrainSettings.LandModifier = 1;
            venus.TerrainSettings.HeightMulti = 2;
            venus.Process();
        }
    }
}