using System.Collections.Generic;
using UnityEngine;
 
namespace GalacticScale
{
  public static partial class Themes
  {
      public static GSTheme AridDesert2 = new GSTheme
      {
         Name = "AridDesert",
         Base = true,
         DisplayName = "Arid Desert".Translate(),
         PlanetType = EPlanetType.Desert,
         ThemeType = EThemeType.Telluric,
 
         LDBThemeId = 6,
         Algo = 2,
         MinRadius = 5,
         MaxRadius = 510,
         MaterialPath = "Universe/Materials/Planets/Desert 1/",
         Temperature = 2f,
         Distribute = EThemeDistribute.Default,
         Habitable = false,
         ModX = new Vector2(1f, 1f),
         ModY = new Vector2(0f, 0f),
         CustomGeneration = false,
          TerrainSettings = new GSTerrainSettings
         {
             Algorithm = "Vanilla"
         },
         VeinSettings = new GSVeinSettings
         {
              Algorithm = "Vanilla",
             VeinTypes = new GSVeinTypes()
  },
//AmbientSettings 1
AmbientSettings = new GSAmbientSettings
{
    Color1 = new Color(0f, 0f, 0f, 1f),
    Color2 = new Color(0f, 0f, 0f, 1f),
    Color3 = new Color(0f, 0f, 0f, 1f),
    WaterColor1 = new Color(0f, 0f, 0f, 1f),
    WaterColor2 = new Color(0f, 0f, 0f, 1f),
    WaterColor3 = new Color(0f, 0f, 0f, 1f),
    BiomeColor1 = new Color(1f, 1f, 1f, 0f),
    BiomeColor2 = new Color(1f, 1f, 1f, 0f),
    BiomeColor3 = new Color(1f, 1f, 1f, 0f),
    DustColor1 = new Color(1f, 1f, 1f, 0f),
    DustColor2 = new Color(1f, 1f, 1f, 0f),
    DustColor3 = new Color(1f, 1f, 1f, 0f),
    DustStrength1 = 1f,
    DustStrength2 = 1f,
    DustStrength3 = 1f,
    BiomeSound1 = 0,
    BiomeSound2 = 0,
    BiomeSound3 = 0,
    CubeMap = "Vanilla",
    Reflections = new Color(),
    LutContribution = 0f,
},
Vegetables0 = new int[]
{
617,
619,
616,
618,
1044,
615,
618,
615,
614,
613,
612,
611,
},
Vegetables1 = new int[]
{
616,
618,
619,
617,
619,
615,
1044,
613,
614,
612,
},
Vegetables2 = new int[]
{
1041,
1042,
1043,
1044,
},
Vegetables3 = new int[]
{
},
Vegetables4 = new int[]
{
},
Vegetables5 = new int[]
{
},
VeinSpot = new int[]
{
3,
10,
0,
6,
10,
1,
0,
},
VeinCount = new float[]
{
0.5f,
1f,
0f,
1f,
1f,
0.3f,
0f,
},
VeinOpacity = new float[]
{
0.6f,
0.6f,
0f,
1f,
1f,
0.3f,
0f,
},
RareVeins = new int[]
{
9,
},
RareSettings = new float[]
{
0f,
0.18f,
0.2f,
0.3f,
},
GasItems = new int[]
{
},
GasSpeeds = new float[]
{
},
UseHeightForBuild = false,
Wind = 1.5f,
IonHeight = 70f,
WaterHeight = 0f,
WaterItemId = 0,
Musics = new[]
{
11,
4,
},
SFXPath = "SFX/sfx-amb-desert-3",
SFXVolume = 0.47f,
CullingRadius = 0f,
atmosphereMaterial = new GSMaterialSettings
{
Colors = new Dictionary<string, Color>{
["_Color"] = new Color(0.3443396f, 0.734796f, 1f,1f),
["_Color0"] = new Color(0.5896226f, 0.6571189f, 1f,1f),
["_Color1"] = new Color(0.6604218f, 0.6981642f, 0.9150943f,1f),
["_Color2"] = new Color(0.7656973f, 0.7087932f, 0.8301887f,1f),
["_Color3"] = new Color(0.95f, 0.7631148f, 0.7436476f,1f),
["_Color4"] = new Color(0.9899679f, 0.7604129f, 0.426034f,1f),
["_Color5"] = new Color(0.3378015f, 0.2965468f, 0.5283019f,1f),
["_Color6"] = new Color(0.3261352f, 0.2783019f, 1f,1f),
["_Color7"] = new Color(0.3753907f, 0.1137253f, 0.6117647f,1f),
["_Color8"] = new Color(1f, 1f, 1f,1f),
["_ColorF"] = new Color(0.75f, 0.75f, 0.75f,1f),
["_EmissionColor"] = new Color(0f, 0f, 0f,1f),
["_LocalPos"] = new Color(0f, 0f, 0f,0f),
["_PlanetPos"] = new Color(0f, 0f, 0f,0f),
["_PlanetRadius"] = new Color(200f, 100f, 264f,0f),
["_Sky0"] = new Color(0.9603034f, 0.768868f, 1f,0.1607843f),
["_Sky1"] = new Color(0.5518868f, 0.5608285f, 1f,0.09803922f),
["_Sky2"] = new Color(0.972549f, 0.731376f, 0.6078432f,0.7490196f),
["_Sky3"] = new Color(0.6462264f, 0.7619467f, 1f,0.8392157f),
["_Sky4"] = new Color(0.9962379f, 0.7743359f, 0.3747257f,1f),
},
Params = new Dictionary<string, float>{
["_AtmoDensity"] = 1f,
["_AtmoThickness"] = 70f,
["_BumpScale"] = 1f,
["_Cutoff"] = 0.5f,
["_Density"] = 0.005f,
["_DetailNormalMapScale"] = 1f,
["_DstBlend"] = 0f,
["_FarFogDensity"] = 0.4f,
["_FogDensity"] = 0.4f,
["_FogSaturate"] = 1.2f,
["_GlossMapScale"] = 1f,
["_Glossiness"] = 0.5f,
["_GlossyReflections"] = 1f,
["_GroundAtmosPower"] = 3f,
["_Intensity"] = 1.2f,
["_IntensityControl"] = 1f,
["_Metallic"] = 0f,
["_Mode"] = 0f,
["_OcclusionStrength"] = 1f,
["_Parallax"] = 0.02f,
["_RimFogExp"] = 1.3f,
["_RimFogPower"] = 3f,
["_SkyAtmosPower"] = 7f,
["_SmoothnessTextureChannel"] = 0f,
["_SpecularHighlights"] = 1f,
["_SrcBlend"] = 1f,
["_SunColorAdd"] = 0f,
["_SunColorSkyUse"] = 1f,
["_SunColorUse"] = 1f,
["_SunRiseScatterPower"] = 60f,
["_UVSec"] = 0f,
["_ZWrite"] = 1f,
}
},

        };
    }
}
