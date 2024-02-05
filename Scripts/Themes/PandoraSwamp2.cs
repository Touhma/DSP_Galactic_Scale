using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class Themes
    {
        public static GSTheme PandoraSwamp2 = new()
        {
            Name = "PandoraSwamp2",
            Base = true,
            DisplayName = "Pandora Swamp II".Translate(),
            PlanetType = EPlanetType.Ocean,
            ThemeType = EThemeType.Telluric,
            LDBThemeId = 25,
            Algo = 13,
            BriefIntroduction = "潘多拉沼泽介绍B",
            Eigenbit = 19,
            MinRadius = 5,
            MaxRadius = 510,
            MaterialPath = "Universe/Materials/Planets/Desert 11/",
            Temperature = 0f,
            Distribute = EThemeDistribute.Interstellar,
            Habitable = false,
            ModX = new Vector2(1f, 1f),
            ModY = new Vector2(3f, 3f),
            VeinSettings = new GSVeinSettings
            {
                Algorithm = "Vanilla",
                VeinTypes = new GSVeinTypes()
            },
//AmbientSettings 1
            AmbientSettings = new GSAmbientSettings
            {
                Color1 = new Color(0.117647f, 0.06905881f, 0.1117054f, 1f),
                Color2 = new Color(0.1498965f, 0.1110345f, 0.322f, 1f),
                Color3 = new Color(0.0642857f, 0.133404f, 0.33f, 1f),
                WaterColor1 = new Color(0f, 0f, 0f, 1f),
                WaterColor2 = new Color(0f, 0f, 0f, 1f),
                WaterColor3 = new Color(0f, 0f, 0f, 1f),
                BiomeColor1 = new Color(0.1058824f, 0.2078432f, 0.3019608f, 1f),
                BiomeColor2 = new Color(0f, 0.2924528f, 0.2297844f, 1f),
                BiomeColor3 = new Color(0.08566215f, 0.6378558f, 0.7264151f, 1f),
                DustColor1 = new Color(0.1483179f, 0.2328379f, 0.3113208f, 0f),
                DustColor2 = new Color(0.3098039f, 0.6588235f, 0.5179729f, 0.8f),
                DustColor3 = new Color(0.5977661f, 0.9046117f, 0.9528302f, 0.5019608f),
                DustStrength1 = 0f,
                DustStrength2 = 5f,
                DustStrength3 = 6f,
                BiomeSound1 = 1,
                BiomeSound2 = 0,
                BiomeSound3 = 2,
                CubeMap = "Vanilla",
                Reflections = new Color(),
                LutContribution = 0.6f
            },
            Vegetables0 = new[]
            {
                681,
                682,
                683,
                684,
                685,
                151,
                148,
                149,
                1085
            },
            Vegetables1 = new[]
            {
                1085,
                681,
                142,
                685,
                143,
                682,
                1086,
                144,
                150
            },
            Vegetables2 = new[]
            {
                1084,
                1081,
                1082,
                1083,
                1085,
                1086
            },
            Vegetables3 = new[]
            {
                141,
                145
            },
            Vegetables4 = new[]
            {
                1084,
                681,
                682,
                683,
                684,
                1086,
                685,
                146,
                147
            },
            Vegetables5 = new int[]
            {
            },
            VeinSpot = new[]
            {
                8,
                3,
                8,
                1,
                3,
                9,
                20
            },
            VeinCount = new[]
            {
                0.7f,
                0.6f,
                1f,
                1f,
                0.6f,
                1f,
                1f
            },
            VeinOpacity = new[]
            {
                0.7f,
                0.5f,
                1f,
                1f,
                0.7f,
                1.2f,
                1f
            },
            RareVeins = new[]
            {
                10,
                11,
                13
            },
            RareSettings = new[]
            {
                0f,
                0.5f,
                0.3f,
                1f,
                0f,
                1f,
                0.3f,
                1f,
                0f,
                0.5f,
                0.2f,
                1f
            },
            GasItems = new int[]
            {
            },
            GasSpeeds = new float[]
            {
            },
            UseHeightForBuild = false,
            Wind = 1f,
            IonHeight = 60f,
            WaterHeight = -2f,
            WaterItemId = 0,
            Musics = new[]
            {
                15,
                16
            },
            SFXPath = "SFX/sfx-amb-ocean-3",
            SFXVolume = 1f,
            CullingRadius = 0f,
            atmosphereMaterial = new GSMaterialSettings
            {
                Colors = new Dictionary<string, Color>
                {
                    ["_Color"] = new(0.3443396f, 0.734796f, 1f, 1f),
                    ["_Color0"] = new(0.4316979f, 0f, 0.7176471f, 1f),
                    ["_Color1"] = new(0.4241931f, 0.3649215f, 0.7254902f, 1f),
                    ["_Color2"] = new(0.4965216f, 0.4575917f, 0.8584906f, 1f),
                    ["_Color3"] = new(0.2642171f, 0.671552f, 0.811f, 1f),
                    ["_Color4"] = new(0.6048944f, 0.6950161f, 0.9217973f, 1f),
                    ["_Color5"] = new(0.6918148f, 0f, 0.7547169f, 1f),
                    ["_Color6"] = new(0.4537178f, 0f, 0.7924528f, 1f),
                    ["_Color7"] = new(0.6050931f, 0.4470588f, 0.8392157f, 1f),
                    ["_Color8"] = new(0.902f, 0.804f, 1f, 1f),
                    ["_ColorF"] = new(0.7517568f, 0.6968f, 0.8666667f, 1f),
                    ["_EmissionColor"] = new(0f, 0f, 0f, 1f),
                    ["_LocalPos"] = new(196.7228f, 24.81346f, 37.8507f, 0f),
                    ["_PlanetPos"] = new(0f, 0f, 0f, 0f),
                    ["_PlanetRadius"] = new(200f, 100f, 260f, 0f),
                    ["_Sky0"] = new(0.3254899f, 0.5490196f, 0.7529413f, 0.4901961f),
                    ["_Sky1"] = new(0.3666937f, 0.3083836f, 0.6603774f, 0.3568628f),
                    ["_Sky2"] = new(0.3415806f, 0.3983026f, 0.7169812f, 0.8588235f),
                    ["_Sky3"] = new(0.2766015f, 0.1960783f, 0.5254902f, 0.6784314f),
                    ["_Sky4"] = new(0.5427575f, 0.6441936f, 0.8785993f, 1f)
                },
                Params = new Dictionary<string, float>
                {
                    ["_AtmoDensity"] = 1.1f,
                    ["_AtmoThickness"] = 70f,
                    ["_BumpScale"] = 1f,
                    ["_Cutoff"] = 0.5f,
                    ["_Density"] = 0.005f,
                    ["_DetailNormalMapScale"] = 1f,
                    ["_DistanceControl"] = 0f,
                    ["_DstBlend"] = 0f,
                    ["_FarFogDensity"] = 0.25f,
                    ["_FogDensity"] = 0.9f,
                    ["_FogSaturate"] = 0.85f,
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
                    ["_RimFogPower"] = 2.5f,
                    ["_SkyAtmosPower"] = 7f,
                    ["_SmoothnessTextureChannel"] = 0f,
                    ["_SpecularHighlights"] = 1f,
                    ["_SrcBlend"] = 1f,
                    ["_StencilComp"] = 8f,
                    ["_StencilRef"] = 0f,
                    ["_SunColorAdd"] = 1f,
                    ["_SunColorSkyUse"] = 1f,
                    ["_SunColorUse"] = 0f,
                    ["_SunRiseScatterPower"] = 60f,
                    ["_UVSec"] = 0f,
                    ["_ZWrite"] = 1f
                }
            }
        };
    }
}