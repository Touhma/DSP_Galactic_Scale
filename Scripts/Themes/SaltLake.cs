using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme SaltLake = new()
        {
            Name = "SaltLake",
            Base = true,
            DisplayName = "Rocky Salt Lake".Translate(),
            PlanetType = EPlanetType.Desert,
            ThemeType = EThemeType.Telluric,

            LDBThemeId = 17,
            Algo = 2,
            MinRadius = 5,
            MaxRadius = 510,
            MaterialPath = "Universe/Materials/Planets/Desert 5/",
            Temperature = 1.0f,
            Distribute = EThemeDistribute.Default,
            ModX = new Vector2(0.0f, 0.0f),
            ModY = new Vector2(1.0f, 1.0f),
            VeinSettings = new GSVeinSettings { Algorithm = "Vanilla" },
            AmbientSettings = new GSAmbientSettings //NEED CHANGING on sakura, and other themes imported
            {
                Color1 = new Color(0.1098039f, 0.1067107f, 0.1059607f, 1.0f),
                Color2 = new Color(0.01777778f, 0.03657109f, 0.07999998f, 1.0f),
                Color3 = new Color(0.01944444f, 0.03333334f, 0.04999998f, 1.0f),
                WaterColor1 = new Color(0.0f, 0.0f, 0.0f, 1.0f),
                WaterColor2 = new Color(0.0f, 0.0f, 0.0f, 1.0f),
                WaterColor3 = new Color(0.0f, 0.0f, 0.0f, 1.0f),
                BiomeColor1 = new Color(0.764151f, 0.764151f, 0.764151f, 1.0f),
                BiomeColor2 = new Color(0.490566f, 0.2671033f, 0.2059452f, 1.0f),
                BiomeColor3 = new Color(0.0f, 0.0f, 0.0f, 1.0f),
                DustColor1 = new Color(1.0f, 0.9755956f, 0.8980392f, 1.0f),
                DustColor2 = new Color(0.7843137f, 0.5873853f, 0.5019608f, 1.0f),
                DustColor3 = new Color(0.245283f, 0.245283f, 0.245283f, 1.0f),
                DustStrength1 = 8f,
                DustStrength2 = 4f,
                DustStrength3 = 2f,
                BiomeSound1 = 0,
                BiomeSound2 = 0,
                BiomeSound3 = 2,
                CubeMap = "Vanilla",
                Reflections = new Color(),
                LutContribution = 0.4f
            },
            Vegetables0 = new[]
            {
                6,
                7,
                8,
                9,
                10,
                11,
                12
            },
            Vegetables1 = new[]
            {
                1,
                2,
                3,
                4,
                5
            },
            Vegetables2 = new int[]
            {
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
            VeinSpot = new[]
            {
                2,
                8,
                9,
                1,
                3,
                1,
                0
            },
            VeinCount = new[]
            {
                1.0f,
                0.8f,
                0.8f,
                1.0f,
                0.7f,
                0.3f,
                0.0f
            },
            VeinOpacity = new[]
            {
                0.6f,
                0.6f,
                1.0f,
                1.0f,
                0.5f,
                0.3f,
                0.0f
            },
            RareVeins = new[]
            {
                9,
                12
            },
            RareSettings = new[]
            {
                0.0f,
                0.7f,
                0.7f,
                0.5f,
                0.0f,
                0.1f,
                0.2f,
                0.6f
            },
            GasItems = new int[]
            {
            },
            GasSpeeds = new float[]
            {
            },
            UseHeightForBuild = false,
            Wind = 1.1f,
            IonHeight = 60f,
            WaterHeight = 0f,
            WaterItemId = 0,
            Musics = new[]
            {
                4,
                11
            },
            SFXPath = "SFX/sfx-amb-desert-4",
            SFXVolume = 0.2f,
            CullingRadius = 0f,
            IceFlag = 0,
            terrainMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_AmbientColor0"] = new(0.1098038f, 0.1067107f, 0.1059607f, 1f),
                    ["_AmbientColor1"] = new(0.01777778f, 0.03657109f, 0.07999993f, 1f),
                    ["_AmbientColor2"] = new(0.01944444f, 0.03333334f, 0.04999995f, 1f),
                    ["_Color"] = new(1f, 1f, 1f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_HeightEmissionColor"] = new(0f, 0f, 0f, 0f),
                    ["_LightColorScreen"] = new(0.4900773f, 0.3058822f, 0.6509804f, 1f),
                    ["_Rotation"] = new(0f, 0f, 0f, 1f),
                    ["_SunDir"] = new(-0.9571023f, -0.009684576f, 0.2895883f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_AmbientInc"] = 0.5f,
                    ["_BioFuzzMask"] = -0.759f,
                    ["_BioFuzzStrength"] = 0.277f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_Distance"] = 206.7759f,
                    ["_DstBlend"] = 0f,
                    ["_EmissionStrength"] = 0f,
                    ["_GlossMapScale"] = 1f,
                    ["_Glossiness"] = 0.5f,
                    ["_GlossyReflections"] = 1f,
                    ["_HeightEmissionRadius"] = 50f,
                    ["_Metallic"] = 0f,
                    ["_Mode"] = 0f,
                    ["_Multiplier"] = 1.8f,
                    ["_NormalStrength"] = 0.35f,
                    ["_OcclusionStrength"] = 1f,
                    ["_Parallax"] = 0.02f,
                    ["_Radius"] = 200f,
                    ["_SmoothnessTextureChannel"] = 0f,
                    ["_SpecularHighlights"] = 1f,
                    ["_SrcBlend"] = 1f,
                    ["_StepBlend"] = 0.57f,
                    ["_UVSec"] = 0f,
                    ["_ZWrite"] = 1f
                }
            },
            oceanMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>(),
                Params = new Dictionary<string, float>()
            },
            atmosphereMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_Color"] = new(0.3443396f, 0.734796f, 1f, 1f),
                    ["_Color0"] = new(0.5254902f, 0.5530115f, 0.6980392f, 1f),
                    ["_Color1"] = new(0.6509804f, 0.6927682f, 0.7450981f, 1f),
                    ["_Color2"] = new(0.6745098f, 0.740676f, 0.7843137f, 1f),
                    ["_Color3"] = new(0.8196079f, 0.8130908f, 0.8023961f, 1f),
                    ["_Color4"] = new(1f, 0.7541665f, 0.4184507f, 1f),
                    ["_Color5"] = new(0.7783019f, 0.8407032f, 1f, 1f),
                    ["_Color6"] = new(0.2862745f, 0.6274511f, 1f, 1f),
                    ["_Color7"] = new(0.4568785f, 0.5271677f, 0.608f, 1f),
                    ["_Color8"] = new(1f, 1f, 1f, 1f),
                    ["_ColorF"] = new(1f, 0.9559999f, 0.95f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_LocalPos"] = new(0f, 0f, 0f, 0f),
                    ["_PlanetPos"] = new(0f, 0f, 0f, 0f),
                    ["_PlanetRadius"] = new(200f, 100f, 260f, 0f),
                    ["_Sky0"] = new(0.4156862f, 0.5421234f, 0.6509804f, 0.4078431f),
                    ["_Sky1"] = new(0.1310518f, 0.2397371f, 0.2924527f, 0.2588235f),
                    ["_Sky2"] = new(0.8588235f, 0.754014f, 0.6784314f, 0.7490196f),
                    ["_Sky3"] = new(0.3882353f, 0.4889933f, 0.5960784f, 0.4f),
                    ["_Sky4"] = new(1f, 0.7522883f, 0.3370455f, 1f)
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
                    ["_FarFogDensity"] = 0.1f,
                    ["_FogDensity"] = 1f,
                    ["_FogSaturate"] = 0.7f,
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
                    ["_RimFogPower"] = 3f,
                    ["_SkyAtmosPower"] = 9f,
                    ["_SmoothnessTextureChannel"] = 0f,
                    ["_SpecularHighlights"] = 1f,
                    ["_SrcBlend"] = 1f,
                    ["_SunColorAdd"] = 1f,
                    ["_SunColorSkyUse"] = 1f,
                    ["_SunColorUse"] = 1f,
                    ["_SunRiseScatterPower"] = 60f,
                    ["_UVSec"] = 0f,
                    ["_ZWrite"] = 1f
                }
            },
            minimapMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_Color"] = new(1f, 1f, 1f, 1f),
                    ["_ColorBio0"] = new(0.8962264f, 0.8545086f, 0.8243592f, 1f),
                    ["_ColorBio1"] = new(0.6792453f, 0.3022062f, 0.1954432f, 1f),
                    ["_ColorBio2"] = new(0.1792453f, 0.1792453f, 0.1792453f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_HeightSettings"] = new(-0.1f, 0.3f, 0.1f, 0.1f),
                    ["_RimColor"] = new(1f, 0.4232803f, 0.03301889f, 1f),
                    ["_ShoreLineColor"] = new(0f, 0f, 0f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_BioStrength"] = 0.38f,
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
                    ["_ZWrite"] = 1f
                }
            },
            thumbMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_AmbientColor"] = new(0.2274797f, 0.001245998f, 0.2641509f, 1f),
                    ["_Color"] = new(0.9254902f, 0.8611792f, 0.8235294f, 1f),
                    ["_ColorBio0"] = new(1f, 0.948292f, 0.8726415f, 1f),
                    ["_ColorBio1"] = new(0.699f, 0.178944f, 0f, 1f),
                    ["_ColorBio2"] = new(0.01886791f, 0.01886791f, 0.01886791f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_FlowColor"] = new(0.7924528f, 0.7924528f, 0.7924528f, 0.7882353f),
                    ["_HeightSettings"] = new(-0.1f, 0.3f, 0.1f, 0.1f),
                    ["_HoloColor"] = new(0.3f, 0.7f, 0.25f, 0.2f),
                    ["_NotVisibleColor"] = new(0f, 0.03f, 0.07499998f, 0.2f),
                    ["_RimColor"] = new(1f, 0.6575472f, 0.4292452f, 1f),
                    ["_Rotation"] = new(0f, 0f, 0f, 1f),
                    ["_ShoreLineColor"] = new(0f, 0f, 0f, 0f),
                    ["_SpecularColor"] = new(0.5188679f, 0.3004048f, 0.1737718f, 1f),
                    ["_Spot0"] = new(0.6f, -0.3f, -0.5f, 1f),
                    ["_SunDir"] = new(0f, 0f, 1f, 0f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_BioStrength"] = 0.6f,
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
                    ["_ZWrite"] = 1f
                }
            }
        };
    }
}