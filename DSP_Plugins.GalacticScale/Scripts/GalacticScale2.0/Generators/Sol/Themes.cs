using UnityEngine;

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

            GSTheme venus = new GSTheme("AcidDeath", "Acid Deathworld", "VolcanicAsh");
            venus.atmosphereMaterial.Tint =new Color(0.674f, 0.556f, 0.207f, 1);
            venus.oceanMaterial.Tint = new Color(0.9f, 0.9f, 0.1f);
            venus.terrainMaterial.Tint =new Color(0.901f, 0.686f, 0.098f, 1);
            venus.thumbMaterial.Tint = new Color(0.901f, 0.686f, 0.098f, 1);
            venus.minimapMaterial.Tint = new Color(0.901f, 0.686f, 0.098f, 1);
            venus.terrainMaterial.CopyFrom = "AshenGelisol.terrainMat";
            venus.AmbientSettings.DustStrength1 = 10;
            venus.AmbientSettings.DustStrength2 = 10;
            venus.AmbientSettings.DustStrength3 = 10;
            venus.AmbientSettings.DustColor1 = Color.red;
            venus.Process();
        }
    }
}