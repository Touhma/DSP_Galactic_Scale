using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GalacticScale
{
    public class GSAmbientSettings
    {
        public Color BiomeColor1 = new Color(1f, 1f, 1f, 0f);
        public Color BiomeColor2 = new Color(1f, 1f, 1f, 0f);
        public Color BiomeColor3 = new Color(1f, 1f, 1f, 0f);
        public int BiomeSound1;
        public int BiomeSound2;
        public int BiomeSound3;
        public Color Color1 = Color.black;
        public Color Color2 = Color.black;
        public Color Color3 = Color.black;
        public string CubeMap = "Vanilla";
        public Color DustColor1 = new Color(1f, 1f, 1f, 0f);
        public Color DustColor2 = new Color(1f, 1f, 1f, 0f);
        public Color DustColor3 = new Color(1f, 1f, 1f, 0f);
        public float DustStrength1 = 1f;
        public float DustStrength2 = 1f;
        public float DustStrength3 = 1f;
        public float LutContribution;

        [NonSerialized] public Texture2D LutTexture;

        [NonSerialized] public Cubemap ReflectionMap;

        public Color Reflections;
        public string ResourcePath;
        public Color WaterColor1 = Color.black;
        public Color WaterColor2 = Color.black;
        public Color WaterColor3 = Color.black;

        public void FromTheme(GSTheme theme)
        {
            //GS2.Log("Start " + (theme.ambientDesc == null));
            if (theme.ambientDesc == null)
                //GS2.Warn(theme.Name + " has no AmbientDesc");
                return;

            Color1 = theme.ambientDesc.ambientColor0; //Trees Day Tint
            Color2 = theme.ambientDesc.ambientColor1; //Trees Twilight Tint
            Color3 = theme.ambientDesc.ambientColor2; //Trees Night Tint
            WaterColor1 = theme.ambientDesc.waterAmbientColor0; //Water as above
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
            //GS2.Log(".....");
            if (ReflectionMap?.name?.Split('_')[0] == "def")
                CubeMap = "Vanilla";
            //GS2.Log("___");
            else
                CubeMap = ReflectionMap?.name;

            //GS2.Log("_");
        }

        public void ToTheme(GSTheme theme)
        {
            // var highStopwatch = new HighStopwatch();highStopwatch.Begin();

            //GS2.Log("Start");
            // This should already been defaulted by the base theme if that exists
            if (CubeMap == "Vanilla" || CubeMap == null)
            {
                //GS2.Log("Vanilla");
                //do nothing
            } //////////// FIX RARE SPAWNING IN THEME
            else
            {
                // GS2.Log(CubeMap);
                // GS2.Log(Reflections.ToString());
                //Should move this out of here
                //AssetBundle bundle = GS2.bundle;
                //if (bundle == null) bundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetAssembly(typeof(GS2)).Location), "galacticbundle"));
                if (CubeMap == "GS2")
                {
                    var x = GS2.Bundle.LoadAsset<Cubemap>("cube2");
                    if (Reflections.a != 0)
                    {
                        ReflectionMap = Utils.TintCubeMap(x, Reflections);
                        // GS2.Log("Set Reflection Map to Tinted One");
                    }
                    else
                    {
                        ReflectionMap = x;
                    }
                }
            }

            //GS2.Log("Processing AmbientDesc for ");
            //GS2.Log(theme.Name);
            //theme.ambientDesc = new AmbientDesc();
            theme.ambientDesc.ambientColor0 = Color1; //GS2.Log("Processing AmbientDesc1");
            theme.ambientDesc.ambientColor1 = Color2; //GS2.Log("Processing AmbientDesc2");
            theme.ambientDesc.ambientColor2 = Color3; //GS2.Log("Processing AmbientDesc3");
            theme.ambientDesc.waterAmbientColor0 = WaterColor1; //GS2.Log("Processing AmbientDesc4");
            theme.ambientDesc.waterAmbientColor1 = WaterColor2; //GS2.Log("Processing AmbientDesc5");
            theme.ambientDesc.waterAmbientColor2 = WaterColor3; //GS2.Log("Processing AmbientDesc6");
            theme.ambientDesc.biomoColor0 = BiomeColor1; //GS2.Log("Processing AmbientDesc7");
            theme.ambientDesc.biomoColor1 = BiomeColor2; //GS2.Log("Processing AmbientDesc8");
            theme.ambientDesc.biomoColor2 = BiomeColor3; //GS2.Log("Processing AmbientDesc9");
            theme.ambientDesc.biomoDustColor0 = DustColor1; //GS2.Log("Processing AmbientDesc10");
            theme.ambientDesc.biomoDustColor1 = DustColor2;
            theme.ambientDesc.biomoDustColor2 = DustColor3;
            theme.ambientDesc.biomoDustStrength0 = DustStrength1;
            theme.ambientDesc.biomoDustStrength1 = DustStrength2;
            theme.ambientDesc.biomoDustStrength2 = DustStrength3;
            theme.ambientDesc.biomoSound0 = BiomeSound1;
            theme.ambientDesc.biomoSound1 = BiomeSound2;
            theme.ambientDesc.biomoSound2 = BiomeSound3;
            theme.ambientDesc.lutContribution = LutContribution;
            if (ReflectionMap != null)
                //GS2.Log("Processing ReflectionMap");
                theme.ambientDesc.reflectionMap = ReflectionMap;
            if (LutTexture != null) theme.ambientDesc.lutTexture = LutTexture;
            // GS2.Log($"Ambient Took {highStopwatch.duration:F5}s");
        }

        public GSAmbientSettings Clone()
        {
            var a = (GSAmbientSettings)MemberwiseClone();
            if (ReflectionMap != null && ReflectionMap.name.Split('_')[0] != "def")
                a.ReflectionMap = Object.Instantiate(ReflectionMap);

            if (ReflectionMap != null && ReflectionMap.name.Split('_')[0] == "def") a.ReflectionMap = ReflectionMap;

            //GS2.Log("*");
            if (LutTexture != null) a.LutTexture = LutTexture;

            return a;
        }

        public override string ToString()
        {
            return "->" + Color1 + Color2 + Color3 + WaterColor1 + WaterColor2 + WaterColor3 + BiomeColor1 + BiomeColor2 + BiomeColor3 + DustColor1 + DustColor2 + DustColor3 + DustStrength1 + DustStrength2 + DustStrength3 + BiomeSound1 + BiomeSound2 + BiomeSound3 + LutContribution + ReflectionMap?.name + LutTexture?.name;
        }
    }
}