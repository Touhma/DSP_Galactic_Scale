using System;
using System.Collections.Generic;
using System.Linq;
using GSSerializer;

namespace GalacticScale
{
    public class GSFSThemeConverter : fsDirectConverter<GSTheme>
    {
        public override object CreateInstance(fsData data, Type storageType)
        {
            return new GSTheme();
        }

        protected override fsResult DoSerialize(GSTheme model, Dictionary<string, fsData> serialized)
        {
            //GS2.Log("GSFSThemeConverter|DoSerialize|" + model.Name);
            var based = model.BaseName != null && model.BaseName != "";
            SerializeMember(serialized, null, "Name", model.Name);
            var baseTheme = GSSettings.ThemeLibrary["Mediterranean"];
            if (based)
            {
                if (!GSSettings.ThemeLibrary.ContainsKey(model.BaseName))
                    GS2.Error($"Theme Missing: {model.BaseName}");
                else baseTheme = GSSettings.ThemeLibrary[model.BaseName];
            }
            // var baseTheme = based ? GS2.ThemeLibrary[model.BaseName] : GS2.ThemeLibrary["Mediterranean"];

            // GS2.Warn($"Serializing. Theme:{model.Name} Base:{baseTheme.Name} Based:{based}");
            if (GS2.Config.MinifyJson)
            {
                // GS2.Log("Minifying");
                if (!based || model.PlanetType != baseTheme.PlanetType)
                    SerializeMember(serialized, null, "PlanetType", model.PlanetType);
                if (!based || model.ThemeType != baseTheme.ThemeType)
                    SerializeMember(serialized, null, "ThemeType", model.ThemeType);
                if (!based || model.StarTypes.Except(baseTheme.StarTypes).Count() != 0 || baseTheme.StarTypes.Except(model.StarTypes).Count() != 0)
                    SerializeMember(serialized, null, "StarTypes", model.StarTypes);
                if (!based || model.Algo != baseTheme.Algo) SerializeMember(serialized, null, "Algo", model.Algo);

                if (!based || model.CustomGeneration != baseTheme.CustomGeneration)
                    SerializeMember(serialized, null, "CustomGeneration", model.CustomGeneration);

                if (model.DisplayName != model.Name)
                    SerializeMember(serialized, null, "DisplayName", model.DisplayName);

                if (model.BaseName != null && model.BaseName != "")
                    SerializeMember(serialized, null, "BaseName", model.BaseName);

                if (!based || model.MinRadius != baseTheme.MinRadius)
                    SerializeMember(serialized, null, "MinRadius", model.MinRadius);
                if (!based || model.MaxRadius != baseTheme.MaxRadius)
                    SerializeMember(serialized, null, "MaxRadius", model.MaxRadius);
                if (!based || model.Temperature != baseTheme.Temperature)
                    SerializeMember(serialized, null, "Temperature", model.Temperature);
                if (!based || model.Distribute != baseTheme.Distribute)
                    SerializeMember(serialized, null, "Distribute", model.Distribute);

                if (!based || !model.TerrainSettings.Equals(baseTheme.TerrainSettings))
                    SerializeMember(serialized, null, "TerrainSettings", model.TerrainSettings);
                if (!based || !model.VeinSettings.Equals(baseTheme.VeinSettings))
                    SerializeMember(serialized, null, "VeinSettings", model.VeinSettings);
                if (!based || !model.VegeSettings.Equals(baseTheme.VegeSettings))
                    SerializeMember(serialized, null, "VegeSettings", model.VegeSettings);
                //if ((!based || model.Vegetables0 != baseTheme.Vegetables0) && model.Vegetables0 != null && model.Vegetables0.Length > 0 && model.VegeSettings.Group1.Count == 0) SerializeMember(serialized, null, "Vegetables0", model.Vegetables0);
                //if ((!based || model.Vegetables1 != baseTheme.Vegetables1) && model.Vegetables1 != null && model.Vegetables1.Length > 0 && model.VegeSettings.Group2.Count == 0) SerializeMember(serialized, null, "Vegetables1", model.Vegetables1);
                //if ((!based || model.Vegetables2 != baseTheme.Vegetables2) && model.Vegetables2 != null && model.Vegetables2.Length > 0 && model.VegeSettings.Group3.Count == 0) SerializeMember(serialized, null, "Vegetables2", model.Vegetables2);
                //if ((!based || model.Vegetables3 != baseTheme.Vegetables3) && model.Vegetables3 != null && model.Vegetables3.Length > 0 && model.VegeSettings.Group4.Count == 0) SerializeMember(serialized, null, "Vegetables3", model.Vegetables3);
                //if ((!based || model.Vegetables4 != baseTheme.Vegetables4) && model.Vegetables4 != null && model.Vegetables4.Length > 0 && model.VegeSettings.Group5.Count == 0) SerializeMember(serialized, null, "Vegetables4", model.Vegetables4);
                //if ((!based || model.Vegetables5 != baseTheme.Vegetables5) && model.Vegetables5 != null && model.Vegetables5.Length > 0 && model.VegeSettings.Group6.Count == 0) SerializeMember(serialized, null, "Vegetables5", model.Vegetables5);
                if ((!based || model.GasItems != baseTheme.GasItems) && model.GasItems != null && model.GasItems.Length > 0) SerializeMember(serialized, null, "GasItems", model.GasItems);

                if ((!based || model.GasSpeeds != baseTheme.GasSpeeds) && model.GasSpeeds != null && model.GasSpeeds.Length > 0) SerializeMember(serialized, null, "GasSpeeds", model.GasSpeeds);

                if (!based || model.Wind != baseTheme.Wind) SerializeMember(serialized, null, "Wind", model.Wind);

                if (!based || model.IonHeight != baseTheme.IonHeight)
                    SerializeMember(serialized, null, "IonHeight", model.IonHeight);

                if (!based || model.WaterHeight != baseTheme.WaterHeight)
                    SerializeMember(serialized, null, "WaterHeight", model.WaterHeight);

                if (!based || model.WaterItemId != baseTheme.WaterItemId)
                    SerializeMember(serialized, null, "WaterItemId", model.WaterItemId);

                if ((!based || !Utils.ArrayCompare(model.Musics, baseTheme.Musics)) && model.Musics != null && model.Musics.Length > 0) SerializeMember(serialized, null, "Musics", model.Musics);

                if (!based || model.SFXPath != baseTheme.SFXPath)
                    SerializeMember(serialized, null, "SFXPath", model.SFXPath);

                if (!based || model.SFXVolume != baseTheme.SFXVolume)
                    SerializeMember(serialized, null, "SFXVolume", model.SFXVolume);

                if (!based || model.MaterialPath != baseTheme.MaterialPath)
                    SerializeMember(serialized, null, "MaterialPath", model.MaterialPath);

                if ((!based || !model.terrainMaterial.Equals(baseTheme.terrainMaterial)) && model.terrainMaterial != null)
                    SerializeMember(serialized, null, "terrainMaterial", model.terrainMaterial);
                //if ((!based || model.terrainTint != baseTheme.terrainTint) && model.terrainTint != new UnityEngine.Color()) SerializeMember(serialized, null, "terrainTint", model.terrainTint);
                if ((!based || !model.oceanMaterial.Equals(baseTheme.oceanMaterial)) && model.oceanMaterial != null)
                    SerializeMember(serialized, null, "oceanMaterial", model.oceanMaterial);
                //if ((!based || model.oceanTint != baseTheme.oceanTint) && model.oceanTint != new UnityEngine.Color()) SerializeMember(serialized, null, "oceanTint", model.oceanTint);
                if ((!based || !model.atmosphereMaterial.Equals(baseTheme.atmosphereMaterial)) && model.atmosphereMaterial != null)
                    SerializeMember(serialized, null, "atmosphereMaterial", model.atmosphereMaterial);
                //if ((!based || model.atmosphereTint != baseTheme.atmosphereTint) && model.atmosphereTint != new UnityEngine.Color()) SerializeMember(serialized, null, "atmosphereTint", model.atmosphereTint);
                if ((!based || !model.thumbMaterial.Equals(baseTheme.thumbMaterial)) && model.thumbMaterial != null)
                    SerializeMember(serialized, null, "thumbMaterial", model.thumbMaterial);
                //if ((!based || model.thumbTint != baseTheme.thumbTint) && model.thumbTint != new UnityEngine.Color()) SerializeMember(serialized, null, "thumbTint", model.thumbTint);
                if ((!based || !model.minimapMaterial.Equals(baseTheme.minimapMaterial)) && model.minimapMaterial != null)
                    SerializeMember(serialized, null, "minimapMaterial", model.minimapMaterial);
                //if ((!based || model.minimapMaterial.Tint != baseTheme.minimapTint) && model.minimapTint != new UnityEngine.Color()) SerializeMember(serialized, null, "minimapTint", model.minimapTint);
                //if ((!based || model.ambient != baseTheme.ambient) && model.ambient != null) SerializeMember(serialized, null, "ambient", model.ambient);
                if ((!based || model.AmbientSettings.ToString() != baseTheme.AmbientSettings.ToString()) && model.AmbientSettings != null)
                    SerializeMember(serialized, null, "AmbientSettings", model.AmbientSettings);
            }
            else
            {
                SerializeMember(serialized, null, "PlanetType", model.PlanetType);
                SerializeMember(serialized, null, "Algo", model.Algo);
                SerializeMember(serialized, null, "CustomGeneration", model.CustomGeneration);
                SerializeMember(serialized, null, "DisplayName", model.DisplayName);
                SerializeMember(serialized, null, "BaseName", model.BaseName);
                SerializeMember(serialized, null, "MaxRadius", model.MaxRadius);
                SerializeMember(serialized, null, "MinRadius", model.MinRadius);
                SerializeMember(serialized, null, "ThemeType", model.ThemeType);
                SerializeMember(serialized, null, "StarTypes", model.StarTypes);
                SerializeMember(serialized, null, "Temperature", model.Temperature);
                SerializeMember(serialized, null, "Distribute", model.Distribute);
                SerializeMember(serialized, null, "TerrainSettings", model.TerrainSettings);
                SerializeMember(serialized, null, "VeinSettings", model.VeinSettings);
                SerializeMember(serialized, null, "VegeSettings", model.VegeSettings);
                //if ((!based || model.Vegetables0 != baseTheme.Vegetables0) && model.Vegetables0 != null && model.Vegetables0.Length > 0 && model.VegeSettings.Group1.Count == 0) SerializeMember(serialized, null, "Vegetables0", model.Vegetables0);
                //if ((!based || model.Vegetables1 != baseTheme.Vegetables1) && model.Vegetables1 != null && model.Vegetables1.Length > 0 && model.VegeSettings.Group2.Count == 0) SerializeMember(serialized, null, "Vegetables1", model.Vegetables1);
                //if ((!based || model.Vegetables2 != baseTheme.Vegetables2) && model.Vegetables2 != null && model.Vegetables2.Length > 0 && model.VegeSettings.Group3.Count == 0) SerializeMember(serialized, null, "Vegetables2", model.Vegetables2);
                //if ((!based || model.Vegetables3 != baseTheme.Vegetables3) && model.Vegetables3 != null && model.Vegetables3.Length > 0 && model.VegeSettings.Group4.Count == 0) SerializeMember(serialized, null, "Vegetables3", model.Vegetables3);
                //if ((!based || model.Vegetables4 != baseTheme.Vegetables4) && model.Vegetables4 != null && model.Vegetables4.Length > 0 && model.VegeSettings.Group5.Count == 0) SerializeMember(serialized, null, "Vegetables4", model.Vegetables4);
                //if ((!based || model.Vegetables5 != baseTheme.Vegetables5) && model.Vegetables5 != null && model.Vegetables5.Length > 0 && model.VegeSettings.Group6.Count == 0) SerializeMember(serialized, null, "Vegetables5", model.Vegetables5);
                SerializeMember(serialized, null, "GasItems", model.GasItems);
                SerializeMember(serialized, null, "GasSpeeds", model.GasSpeeds);
                SerializeMember(serialized, null, "Wind", model.Wind);
                SerializeMember(serialized, null, "IonHeight", model.IonHeight);
                SerializeMember(serialized, null, "WaterHeight", model.WaterHeight);
                SerializeMember(serialized, null, "WaterItemId", model.WaterItemId);
                SerializeMember(serialized, null, "Musics", model.Musics);
                SerializeMember(serialized, null, "SFXPath", model.SFXPath);
                SerializeMember(serialized, null, "SFXVolume", model.SFXVolume);
                SerializeMember(serialized, null, "MaterialPath", model.MaterialPath);
                SerializeMember(serialized, null, "terrainMaterial", model.terrainMaterial);
                SerializeMember(serialized, null, "oceanMaterial", model.oceanMaterial);
                SerializeMember(serialized, null, "atmosphereMaterial", model.atmosphereMaterial);
                SerializeMember(serialized, null, "thumbMaterial", model.thumbMaterial);
                SerializeMember(serialized, null, "minimapMaterial", model.minimapMaterial);
                //SerializeMember(serialized, null, "ambient", model.ambient);
                SerializeMember(serialized, null, "AmbientSettings", model.AmbientSettings);
            }

            //GS2.Log("GSFSThemeConverter|DoSerialize|End");
            return fsResult.Success;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref GSTheme model)
        {
            //GS2.Log("GSFSThemeConverter|DoDeserialize");
            var result = fsResult.Success;
            model = new GSTheme();

            DeserializeMember(data, null, "BaseName", out model.BaseName);
            if (model.baseTheme != null) model.CopyFrom(model.baseTheme);
            //GS2.Log("ThemeConverter Deserialization CopyFrom Finished");
            if (data.ContainsKey("Name"))
                DeserializeMember(data, null, "Name", out model.Name);
            else
                model.Name = "Unnamed";

            if (data.ContainsKey("PlanetType")) DeserializeMember(data, null, "PlanetType", out model.PlanetType);
            if (data.ContainsKey("StarTypes")) DeserializeMember(data, null, "StarTypes", out model.StarTypes);
            if (data.ContainsKey("Algo")) DeserializeMember(data, null, "Algo", out model.Algo);

            if (data.ContainsKey("CustomGeneration"))
                DeserializeMember(data, null, "CustomGeneration", out model.CustomGeneration);

            if (data.ContainsKey("DisplayName"))
                DeserializeMember(data, null, "DisplayName", out model.DisplayName);
            else
                model.DisplayName = model.Name;

            if (data.ContainsKey("Temperature")) DeserializeMember(data, null, "Temperature", out model.Temperature);
            var modelThemeType = EThemeType.Null;
            if (data.ContainsKey("ThemeType")) DeserializeMember(data, null, "ThemeType", out modelThemeType);
            model.ThemeType = modelThemeType;
            if (data.ContainsKey("MaxRadius")) DeserializeMember(data, null, "MaxRadius", out model.MaxRadius);

            if (data.ContainsKey("MinRadius")) DeserializeMember(data, null, "MinRadius", out model.MinRadius);

            if (data.ContainsKey("Distribute")) DeserializeMember(data, null, "Distribute", out model.Distribute);

            if (data.ContainsKey("TerrainSettings"))
                DeserializeMember(data, null, "TerrainSettings", out model.TerrainSettings);

            if (data.ContainsKey("VeinSettings")) DeserializeMember(data, null, "VeinSettings", out model.VeinSettings);

            if (data.ContainsKey("VegeSettings")) DeserializeMember(data, null, "VegeSettings", out model.VegeSettings);

            if (data.ContainsKey("Vegetables0")) DeserializeMember(data, null, "Vegetables0", out model.Vegetables0);

            if (data.ContainsKey("Vegetables1")) DeserializeMember(data, null, "Vegetables1", out model.Vegetables1);

            if (data.ContainsKey("Vegetables2")) DeserializeMember(data, null, "Vegetables2", out model.Vegetables2);

            if (data.ContainsKey("Vegetables3")) DeserializeMember(data, null, "Vegetables3", out model.Vegetables3);

            if (data.ContainsKey("Vegetables4")) DeserializeMember(data, null, "Vegetables4", out model.Vegetables4);

            if (data.ContainsKey("Vegetables5")) DeserializeMember(data, null, "Vegetables5", out model.Vegetables5);

            if (data.ContainsKey("GasItems")) DeserializeMember(data, null, "GasItems", out model.GasItems);

            if (data.ContainsKey("GasSpeeds")) DeserializeMember(data, null, "GasSpeeds", out model.GasSpeeds);

            if (data.ContainsKey("Wind")) DeserializeMember(data, null, "Wind", out model.Wind);

            if (data.ContainsKey("IonHeight")) DeserializeMember(data, null, "IonHeight", out model.IonHeight);

            if (data.ContainsKey("WaterHeight")) DeserializeMember(data, null, "WaterHeight", out model.WaterHeight);

            if (data.ContainsKey("WaterItemId")) DeserializeMember(data, null, "WaterItemId", out model.WaterItemId);

            if (data.ContainsKey("Musics")) DeserializeMember(data, null, "Musics", out model.Musics);

            if (data.ContainsKey("SFXPath")) DeserializeMember(data, null, "SFXPath", out model.SFXPath);

            if (data.ContainsKey("SFXVolume")) DeserializeMember(data, null, "SFXVolume", out model.SFXVolume);

            if (data.ContainsKey("MaterialPath")) DeserializeMember(data, null, "MaterialPath", out model.MaterialPath);

            if (data.ContainsKey("terrainMaterial"))
                DeserializeMember(data, null, "terrainMaterial", out model.terrainMaterial);
            //if (data.ContainsKey("terrainTint")) DeserializeMember(data, null, "terrainTint", out model.terrainMaterial.Tint);
            if (data.ContainsKey("oceanMaterial"))
                DeserializeMember(data, null, "oceanMaterial", out model.oceanMaterial);
            //if (data.ContainsKey("oceanTint")) DeserializeMember(data, null, "oceanTint", out model.oceanMaterial.Tint);
            if (data.ContainsKey("atmosphereMaterial"))
                DeserializeMember(data, null, "atmosphereMaterial", out model.atmosphereMaterial);
            //if (data.ContainsKey("atmosphereTint")) DeserializeMember(data, null, "atmosphereTint", out model.atmosphereMaterial.Tint);
            if (data.ContainsKey("thumbMaterial"))
                DeserializeMember(data, null, "thumbMaterial", out model.thumbMaterial);
            //if (data.ContainsKey("thumbTint")) DeserializeMember(data, null, "thumbTint", out model.thumbMaterial.Tint);
            if (data.ContainsKey("minimapMaterial"))
                DeserializeMember(data, null, "minimapMaterial", out model.minimapMaterial);
            //if (data.ContainsKey("minimapTint")) DeserializeMember(data, null, "minimapTint", out model.minimapMaterial.Tint);
            //if (data.ContainsKey("ambient")) DeserializeMember(data, null, "ambient", out model.ambient);
            if (data.ContainsKey("AmbientSettings"))
                DeserializeMember(data, null, "AmbientSettings", out model.AmbientSettings);

            model.Init();
            //GS2.Log("Finished initializing " + model.Name + " custom gen? :" + model.CustomGeneration);
            return result;
        }
    }
}