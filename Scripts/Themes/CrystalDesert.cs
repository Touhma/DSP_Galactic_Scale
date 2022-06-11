using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme CrystalDesert = new GSTheme
        {
            Name = "CrystalDesert",
            Base = true,
            DisplayName = "Crystal Desert".Translate(),
            PlanetType = EPlanetType.Desert,
            ThemeType = EThemeType.Telluric,

            LDBThemeId = 23,
            Algo = 11,
            MinRadius = 5,
            MaxRadius = 510,
            MaterialPath = "Universe/Materials/Planets/Desert 9/",
            Temperature = 0.08f,
            Distribute = EThemeDistribute.Interstellar,
            Habitable = false,
            ModX = new Vector2(1.5f, 1.5f),
            ModY = new Vector2(1f, 1f),
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
                Color1 = new Color(0.09774543f, 0.1058909f, 0.112f, 1f),
                Color2 = new Color(0.1215269f, 0.1426122f, 0.1889999f, 1f),
                Color3 = new Color(0.08354498f, 0.1124602f, 0.217f, 1f),
                WaterColor1 = new Color(0f, 0f, 0f, 1f),
                WaterColor2 = new Color(0f, 0f, 0f, 1f),
                WaterColor3 = new Color(0f, 0f, 0f, 1f),
                BiomeColor1 = new Color(0.608f, 0.2219796f, 0f, 1f),
                BiomeColor2 = new Color(0.002744f, 0.01956437f, 0.049f, 1f),
                BiomeColor3 = new Color(0f, 0.06559949f, 0.084f, 1f),
                DustColor1 = new Color(1f, 0.5858376f, 0.2117647f, 0.8f),
                DustColor2 = new Color(0.06388201f, 0.1815109f, 0.273f, 0.8f),
                DustColor3 = new Color(0.194285f, 0.3754598f, 0.455f, 0.8f),
                DustStrength1 = 4f,
                DustStrength2 = 7f,
                DustStrength3 = 7f,
                BiomeSound1 = 2,
                BiomeSound2 = 0,
                BiomeSound3 = 0,
                CubeMap = "Vanilla",
                Reflections = new Color(),
                LutContribution = 1f,
            },
            Vegetables0 = new int[]
            {
                701,
                702,
                703,
            },
            Vegetables1 = new int[]
            {
                713,
                712,
                711,
            },
            Vegetables2 = new int[]
            {
                710,
                709,
                708,
                707,
                706,
                705,
                704,
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
                13,
                2,
                0,
                2,
                0,
                2,
                0,
            },
            VeinCount = new float[]
            {
                1f,
                0.5f,
                0f,
                0.7f,
                0f,
                0.6f,
                0f,
            },
            VeinOpacity = new float[]
            {
                1.2f,
                0.8f,
                0f,
                1f,
                0f,
                0.5f,
                0f,
            },
            RareVeins = new int[]
            {
                11,
                12,
            },
            RareSettings = new float[]
            {
                0f,
                0.7f,
                0.2f,
                0.6f,
                0f,
                1f,
                1f,
                0.84f,
            },
            GasItems = new int[]
            {
            },
            GasSpeeds = new float[]
            {
            },
            UseHeightForBuild = false,
            Wind = 1.5f,
            IonHeight = 55f,
            WaterHeight = 0f,
            WaterItemId = 0,
            Musics = new int[]
            {
                26,
                11,
            },
            SFXPath = "SFX/sfx-amb-desert-5",
            SFXVolume = 0.3f,
            CullingRadius = 0f,

            atmosphereMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_Color"] = new Color(0.3443396f, 0.734796f, 1f, 1f),
                    ["_Color0"] = new Color(0f, 0.5480272f, 0.6698113f, 1f),
                    ["_Color1"] = new Color(0.3019607f, 0.5607843f, 0.6705883f, 1f),
                    ["_Color2"] = new Color(0.357022f, 0.5783352f, 0.6698113f, 1f),
                    ["_Color3"] = new Color(0.608179f, 0.7073157f, 0.745283f, 1f),
                    ["_Color4"] = new Color(1f, 0.7482267f, 0.4163144f, 1f),
                    ["_Color5"] = new Color(0.3224253f, 0.2627448f, 0.8862745f, 1f),
                    ["_Color6"] = new Color(0.2146669f, 0.6243376f, 0.6792453f, 1f),
                    ["_Color7"] = new Color(0.202919f, 0.375155f, 0.4528299f, 1f),
                    ["_Color8"] = new Color(0.5355552f, 0.7375664f, 0.7830188f, 1f),
                    ["_ColorF"] = new Color(0.517444f, 0.6661898f, 0.7169812f, 1f),
                    ["_EmissionColor"] = new Color(0f, 0f, 0f, 1f),
                    ["_LocalPos"] = new Color(135.9492f, 55.65244f, -136.3778f, 0f),
                    ["_PlanetPos"] = new Color(0f, 0f, 0f, 0f),
                    ["_PlanetRadius"] = new Color(200f, 100f, 260f, 0f),
                    ["_Sky0"] = new Color(0.1827606f, 0.3324268f, 0.3490564f, 0.3215686f),
                    ["_Sky1"] = new Color(0.1830721f, 0.2725294f, 0.3207545f, 0.1686275f),
                    ["_Sky2"] = new Color(0.6775098f, 0.8084766f, 0.8207547f, 0.4f),
                    ["_Sky3"] = new Color(0f, 0.2429768f, 0.3584904f, 1f),
                    ["_Sky4"] = new Color(1f, 0.7433999f, 0.3255919f, 1f),
                },
                Params = new Dictionary<string, float>
                {
                    ["_AtmoDensity"] = 1f,
                    ["_AtmoThickness"] = 70f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_Density"] = 0.005f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_DstBlend"] = 0f,
                    ["_FarFogDensity"] = 0.3f,
                    ["_FogDensity"] = 0.6f,
                    ["_FogSaturate"] = 0.69f,
                    ["_GlossMapScale"] = 1f,
                    ["_Glossiness"] = 0.5f,
                    ["_GlossyReflections"] = 1f,
                    ["_GroundAtmosPower"] = 3f,
                    ["_Intensity"] = 1f,
                    ["_IntensityControl"] = 1f,
                    ["_Metallic"] = 0f,
                    ["_Mode"] = 0f,
                    ["_OcclusionStrength"] = 1f,
                    ["_Parallax"] = 0.02f,
                    ["_RimFogExp"] = 1.3f,
                    ["_RimFogPower"] = 1f,
                    ["_SkyAtmosPower"] = 8f,
                    ["_SmoothnessTextureChannel"] = 0f,
                    ["_SpecularHighlights"] = 1f,
                    ["_SrcBlend"] = 1f,
                    ["_SunColorAdd"] = 1f,
                    ["_SunColorSkyUse"] = 1f,
                    ["_SunColorUse"] = 1f,
                    ["_SunRiseScatterPower"] = 60f,
                    ["_UVSec"] = 0f,
                    ["_ZWrite"] = 1f,
                }
            },
            minimapMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_Color"] = new Color(0.2211641f, 0.4795249f, 0.6603774f, 1f),
                    ["_ColorBio0"] = new Color(0.839f, 0.5464357f, 0.217301f, 1f),
                    ["_ColorBio1"] = new Color(0.168469f, 0.2388467f, 0.287f, 1f),
                    ["_ColorBio2"] = new Color(0.195888f, 0.4744096f, 0.636f, 1f),
                    ["_EmissionColor"] = new Color(0f, 0f, 0f, 1f),
                    ["_HeightSettings"] = new Color(-0.1f, 0.3f, 0.1f, 0.1f),
                    ["_RimColor"] = new Color(0f, 0.3997594f, 0.5647059f, 1f),
                    ["_ShoreLineColor"] = new Color(0f, 0f, 0f, 0f),
                },
                Params = new Dictionary<string, float>
                {
                    ["_BioStrength"] = 0.5f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_DstBlend"] = 0f,
                    ["_GlossMapScale"] = 1f,
                    ["_Glossiness"] = 0.5f,
                    ["_GlossyReflections"] = 1f,
                    ["_Metallic"] = 0f,
                    ["_Mode"] = 0f,
                    ["_OcclusionStrength"] = 1f,
                    ["_Parallax"] = 0.02f,
                    ["_ShoreHeight"] = 0f,
                    ["_ShoreInvThick"] = 13f,
                    ["_SmoothnessTextureChannel"] = 0f,
                    ["_SpecularHighlights"] = 1f,
                    ["_SrcBlend"] = 1f,
                    ["_UVSec"] = 0f,
                    ["_WireIntens"] = 0.6f,
                    ["_ZWrite"] = 1f,
                }
            },
            terrainMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_AmbientColor0"] = new Color(0.09774543f, 0.1058909f, 0.112f, 1f),
                    ["_AmbientColor1"] = new Color(0.121527f, 0.1426123f, 0.189f, 1f),
                    ["_AmbientColor2"] = new Color(0.08354498f, 0.1124602f, 0.217f, 1f),
                    ["_Color"] = new Color(0.7048772f, 0.8673259f, 0.9056604f, 1f),
                    ["_EmissionColor"] = new Color(0f, 0f, 0f, 1f),
                    ["_GICloudColor"] = new Color(1f, 1f, 1f, 1f),
                    ["_HeightEmissionColor"] = new Color(0f, 0f, 0f, 0f),
                    ["_LightColorScreen"] = new Color(1f, 0.7259278f, 0.4858491f, 1f),
                    ["_Rotation"] = new Color(0f, 0f, 0f, 1f),
                    ["_SpeclColor"] = new Color(1f, 0.8249525f, 0f, 0.6784314f),
                    ["_SunDir"] = new Color(-0.9803334f, 0.08005269f, 0.1803827f, 0f),
                },
                Params = new Dictionary<string, float>
                {
                    ["_AmbientInc"] = 0.5f,
                    ["_BioFuzzMask"] = -0.2f,
                    ["_BioFuzzStrength"] = 1f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_Distance"] = 399.5652f,
                    ["_DstBlend"] = 0f,
                    ["_EmissionStrength"] = 0f,
                    ["_GICloudStrength"] = 0f,
                    ["_GISaturate"] = 1f,
                    ["_GIStrengthDay"] = 1f,
                    ["_GIStrengthNight"] = 0.2f,
                    ["_GlossMapScale"] = 1f,
                    ["_Glossiness"] = 0.5f,
                    ["_GlossyReflections"] = 1f,
                    ["_HeightEmissionRadius"] = 50f,
                    ["_Metallic"] = 0f,
                    ["_Mode"] = 0f,
                    ["_Multiplier"] = 1.8f,
                    ["_NormalStrength"] = 0.6f,
                    ["_OcclusionStrength"] = 1f,
                    ["_Parallax"] = 0.02f,
                    ["_Radius"] = 200f,
                    ["_SmoothnessTextureChannel"] = 0f,
                    ["_SpecularHighlights"] = 1f,
                    ["_SrcBlend"] = 1f,
                    ["_StepBlend"] = 1f,
                    ["_UVSec"] = 0f,
                    ["_ZWrite"] = 1f,
                }
            },
            thumbMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_AmbientColor"] = new Color(0.2274797f, 0.001245998f, 0.2641509f, 1f),
                    ["_Color"] = new Color(0.09207059f, 0.2202352f, 0.3372549f, 1f),
                    ["_ColorBio0"] = new Color(0.867f, 0.5452422f, 0.18207f, 1f),
                    ["_ColorBio1"] = new Color(0.043904f, 0.2511972f, 0.392f, 1f),
                    ["_ColorBio2"] = new Color(0.144396f, 0.390213f, 0.573f, 1f),
                    ["_EmissionColor"] = new Color(0f, 0f, 0f, 1f),
                    ["_FlowColor"] = new Color(0.7924528f, 0.7924528f, 0.7924528f, 0.7882353f),
                    ["_HeightSettings"] = new Color(-0.1f, 0.3f, 0.1f, 0.1f),
                    ["_HoloColor"] = new Color(0.3f, 0.7f, 0.25f, 0.2f),
                    ["_NotVisibleColor"] = new Color(0f, 0.03f, 0.07499998f, 0.2f),
                    ["_RimColor"] = new Color(0.4386792f, 0.7010356f, 1f, 1f),
                    ["_Rotation"] = new Color(-0.2043909f, 0.07387411f, 0.09414349f, -0.9715472f),
                    ["_ShoreLineColor"] = new Color(0f, 0f, 0f, 0f),
                    ["_SpecularColor"] = new Color(0.5188679f, 0.3004048f, 0.1737718f, 1f),
                    ["_Spot0"] = new Color(0.6f, -0.3f, -0.5f, 1f),
                    ["_SunDir"] = new Color(-0.380532f, 0.05802305f, 0.9229457f, 0f),
                },
                Params = new Dictionary<string, float>
                {
                    ["_BioStrength"] = 0.8f,
                    ["_BodyIntensity"] = 0.27f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_Diameter"] = 0.1f,
                    ["_Distort"] = 0.01f,
                    ["_DstBlend"] = 0f,
                    ["_FarHeight"] = 0.5f,
                    ["_GlossMapScale"] = 1f,
                    ["_Glossiness"] = 0.15f,
                    ["_GlossyReflections"] = 1f,
                    ["_HoloIntensity"] = 0.8f,
                    ["_Metallic"] = 0.58f,
                    ["_Mode"] = 0f,
                    ["_Multiplier"] = 4f,
                    ["_NoiseIntensity"] = 0.15f,
                    ["_NoiseThres"] = 0f,
                    ["_OcclusionStrength"] = 1f,
                    ["_Parallax"] = 0.02f,
                    ["_PolarWhirl"] = -0.3f,
                    ["_PolarWhirlPower"] = 8f,
                    ["_ShoreHeight"] = 0f,
                    ["_ShoreInvThick"] = 13f,
                    ["_SmoothnessTextureChannel"] = 0f,
                    ["_SpecularHighlights"] = 1f,
                    ["_Speed"] = 2f,
                    ["_SrcBlend"] = 1f,
                    ["_Tile"] = 0.2f,
                    ["_TileX"] = 7f,
                    ["_TileY"] = 2.5f,
                    ["_TimeFactor"] = 0f,
                    ["_ToggleVerta"] = 0f,
                    ["_UVSec"] = 0f,
                    ["_WireIntens"] = 2f,
                    ["_ZWrite"] = 1f,
                }
            },
        };
    }
}