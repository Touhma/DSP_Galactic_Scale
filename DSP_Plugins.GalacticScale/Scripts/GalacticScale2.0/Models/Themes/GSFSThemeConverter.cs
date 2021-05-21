using System.Collections.Generic;
using System;
using FullSerializer;

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
            bool based = model.BaseName != null && model.BaseName != "";
            GS2.Warn("Serializing Theme "+model.Name);
            SerializeMember(serialized, null, "Name", model.Name);
            GSTheme baseTheme = (based)?GS2.ThemeLibrary[model.BaseName]:GS2.ThemeLibrary["Mediterranean"];
            if (!based || model.PlanetType != baseTheme.PlanetType) SerializeMember(serialized, null, "PlanetType", model.PlanetType);
            if (!based || model.Algo != baseTheme.Algo) SerializeMember(serialized, null, "Algo", model.Algo);
            if (!based || model.CustomGeneration != baseTheme.CustomGeneration) SerializeMember(serialized, null, "CustomGeneration", model.CustomGeneration);
            if (model.DisplayName != model.Name) SerializeMember(serialized, null, "DisplayName", model.DisplayName);
            if (model.BaseName != null && model.BaseName != "") SerializeMember(serialized, null, "BaseName", model.BaseName);
            if (!based || model.Temperature != baseTheme.Temperature) SerializeMember(serialized, null, "Temperature", model.Temperature);
            if (!based || model.Distribute != baseTheme.Distribute) SerializeMember(serialized, null, "Distribute", model.Distribute);
            if (!based || model.TerrainSettings != baseTheme.TerrainSettings) SerializeMember(serialized, null, "TerrainSettings", model.TerrainSettings);
            SerializeMember(serialized, null, "VeinSettings", model.VeinSettings);
            if ((!based || model.Vegetables0 != baseTheme.Vegetables0)&&model.Vegetables0 != null &&model.Vegetables0.Length > 0) SerializeMember(serialized, null, "Vegetables0", model.Vegetables0);
            if ((!based || model.Vegetables1 != baseTheme.Vegetables1)&& model.Vegetables1 != null && model.Vegetables1.Length > 0)  SerializeMember(serialized, null, "Vegetables1", model.Vegetables1);
            if ((!based || model.Vegetables2 != baseTheme.Vegetables2)&& model.Vegetables2 != null && model.Vegetables2.Length > 0)  SerializeMember(serialized, null, "Vegetables2", model.Vegetables2);
            if ((!based || model.Vegetables3 != baseTheme.Vegetables3)&& model.Vegetables3 != null && model.Vegetables3.Length > 0)  SerializeMember(serialized, null, "Vegetables3", model.Vegetables3);
            if ((!based || model.Vegetables4 != baseTheme.Vegetables4)&& model.Vegetables4 != null && model.Vegetables4.Length > 0)  SerializeMember(serialized, null, "Vegetables4", model.Vegetables4);
            if ((!based || model.Vegetables5 != baseTheme.Vegetables5)&& model.Vegetables5 != null && model.Vegetables5.Length > 0)  SerializeMember(serialized, null, "Vegetables5", model.Vegetables5);
            if ((!based || model.GasItems != baseTheme.GasItems)&& model.GasItems != null && model.GasItems.Length > 0)  SerializeMember(serialized, null, "GasItems", model.GasItems);
            if ((!based || model.GasSpeeds != baseTheme.GasSpeeds)&& model.GasSpeeds != null && model.GasSpeeds.Length > 0)  SerializeMember(serialized, null, "GasSpeeds", model.GasSpeeds);
            if (!based || model.Wind != baseTheme.Wind) SerializeMember(serialized, null, "Wind", model.Wind);
            if (!based || model.IonHeight != baseTheme.IonHeight) SerializeMember(serialized, null, "IonHeight", model.IonHeight);
            if (!based || model.WaterHeight != baseTheme.WaterHeight) SerializeMember(serialized, null, "WaterHeight", model.WaterHeight);
            if ((!based || model.Musics != baseTheme.Musics) && model.Musics != null && model.Musics.Length > 0) SerializeMember(serialized, null, "Musics", model.Musics);
            if (!based || model.SFXPath != baseTheme.SFXPath) SerializeMember(serialized, null, "SFXPath", model.SFXPath);
            if (!based || model.SFXVolume != baseTheme.SFXVolume) SerializeMember(serialized, null, "SFXVolume", model.SFXVolume);
            if (!based || model.MaterialPath != baseTheme.MaterialPath) SerializeMember(serialized, null, "MaterialPath", model.MaterialPath);
            if ((!based || model.terrainMaterial != baseTheme.terrainMaterial) && model.terrainMaterial != null) SerializeMember(serialized, null, "terrainMaterial", model.terrainMaterial);
            if ((!based || model.terrainTint != baseTheme.terrainTint) && model.terrainTint != new UnityEngine.Color()) SerializeMember(serialized, null, "terrainTint", model.terrainTint);
            if ((!based || model.oceanMaterial != baseTheme.oceanMaterial) && model.oceanMaterial != null) SerializeMember(serialized, null, "oceanMaterial", model.oceanMaterial);
            if ((!based || model.oceanTint != baseTheme.oceanTint) && model.oceanTint != new UnityEngine.Color()) SerializeMember(serialized, null, "oceanTint", model.oceanTint);
            if ((!based || model.atmosphereMaterial != baseTheme.atmosphereMaterial) && model.atmosphereMaterial != null) SerializeMember(serialized, null, "atmosphereMaterial", model.atmosphereMaterial);
            if ((!based || model.atmosphereTint != baseTheme.atmosphereTint) && model.atmosphereTint != new UnityEngine.Color()) SerializeMember(serialized, null, "atmosphereTint", model.atmosphereTint);
            if ((!based || model.thumbMaterial != baseTheme.thumbMaterial) && model.thumbMaterial != null) SerializeMember(serialized, null, "thumbMaterial", model.thumbMaterial);
            if ((!based || model.thumbTint != baseTheme.thumbTint) && model.thumbTint != new UnityEngine.Color()) SerializeMember(serialized, null, "thumbTint", model.thumbTint);
            if ((!based || model.minimapMaterial != baseTheme.minimapMaterial) && model.minimapMaterial != null) SerializeMember(serialized, null, "minimapMaterial", model.minimapMaterial);
            if ((!based || model.minimapTint != baseTheme.minimapTint) && model.minimapTint != new UnityEngine.Color()) SerializeMember(serialized, null, "minimapTint", model.minimapTint);
            if ((!based || model.ambient != baseTheme.ambient)&&model.ambient != null) SerializeMember(serialized, null, "ambient", model.ambient);
            return fsResult.Success;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref GSTheme model)
        {
            GS2.Log("DoDeserialize");
            var result = fsResult.Success;
            model = new GSTheme();
            //// Deserialize name mainly manually (helper methods CheckKey and CheckType)
            //fsData veinData;
            //if (CheckKey(data, "veins", out veinData).Succeeded)
            //{
            //    GS2.Log("processing veins");
            //    if ((result += CheckType(veinData, fsDataType.Array)).Failed) return result;
            //    GS2.Log("VeinData is Array");
            //    var veins = veinData.AsList;
            //    model.veins = new List<GSVein>();
            //    if (veins[0].IsString)

            //    {
            //        GS2.Log("Veins[0] is string");
            //        //new method
            //        for (var i = 0; i < veins.Count; i++)
            //        {
            //            var d = veins[i].AsString.Split(new[] { 'x' }, StringSplitOptions.RemoveEmptyEntries);
            //            int groupCount;
            //            float richness;
            //            int count;
            //            if (!int.TryParse(d[0], out groupCount)) return fsResult.Fail("groupCount Not Int: " + d[0]);
            //            if (!float.TryParse(d[2], out richness)) return fsResult.Fail("VeinRichness Not Float: " + d[2]);
            //            if (!int.TryParse(d[1], out count)) return fsResult.Fail("VeinCount Not Int: " + d[1]);
            //            for (var j=0;j<groupCount;j++)
            //            model.veins.Add(new GSVein(count, richness));
            //        }
            //    }// end new method
            //    else
            //    {
            //        GS2.Log("Veins[0] not string");
            //        if ((result += DeserializeMember(data, null, "veins", out model.veins)).Failed) return result;
            //    }

            //    //end old method
            //}
            //fsData generate;
            //if (CheckKey(data, "generate", out generate).Succeeded)
            //{
            //    if (!generate.IsInt64) return fsResult.Fail("generate number not an integer"); 
            //    var numToGenerate = (int)generate.AsInt64;
            //    if (numToGenerate < 0) { GS2.Warn("generate number < 0"); numToGenerate = 0; }
            //    if (numToGenerate > 64) { GS2.Warn("generate number > 64"); numToGenerate = 64; }
            //    if (numToGenerate < model.veins.Count) { GS2.Warn("generate number < existing vein count"); numToGenerate = 0; }
            //    numToGenerate -= model.veins.Count;
            //    for (var i = 0; i < numToGenerate; i++) {
            //        model.veins.Add(new GSVein());
            //    }
            //}
            //string type;
            //if ((result += DeserializeMember(data, null, "type", out type)).Failed) return result;
            //GS2.Log(type);
            //model.type = GSVeinType.saneVeinTypes[type];
            //if ((result += DeserializeMember(data, null, "rare", out model.rare)).Failed) return result;
            //fsData d;
            //if (CheckKey(data, "PlanetType", out d).Succeeded) 
            DeserializeMember(data, null, "Name", out model.Name);
            DeserializeMember(data, null, "PlanetType", out model.PlanetType);
            DeserializeMember(data, null, "Algo", out model.Algo);
            DeserializeMember(data, null, "CustomGeneration", out model.CustomGeneration);
            DeserializeMember(data, null, "DisplayName", out model.DisplayName);
            DeserializeMember(data, null, "BaseName", out model.BaseName);
            DeserializeMember(data, null, "Temperature", out model.Temperature); 
            DeserializeMember(data, null, "Distribute", out model.Distribute); 
            DeserializeMember(data, null, "TerrainSettings", out model.TerrainSettings); 
            DeserializeMember(data, null, "VeinSettings", out model.VeinSettings);
            DeserializeMember(data, null, "Vegetables0", out model.Vegetables0); 
            DeserializeMember(data, null, "Vegetables1", out model.Vegetables1); 
            DeserializeMember(data, null, "Vegetables2", out model.Vegetables2);
            DeserializeMember(data, null, "Vegetables3", out model.Vegetables3);
            DeserializeMember(data, null, "Vegetables4", out model.Vegetables4); 
            DeserializeMember(data, null, "Vegetables5", out model.Vegetables5); 
            DeserializeMember(data, null, "GasItems", out model.GasItems); 
            DeserializeMember(data, null, "GasSpeeds", out model.GasSpeeds); 
            DeserializeMember(data, null, "Wind", out model.Wind); 
            DeserializeMember(data, null, "IonHeight", out model.IonHeight);
            DeserializeMember(data, null, "WaterHeight", out model.WaterHeight);
            DeserializeMember(data, null, "Musics", out model.Musics); 
            DeserializeMember(data, null, "SFXPath", out model.SFXPath); 
            DeserializeMember(data, null, "SFXVolume", out model.SFXVolume);
            DeserializeMember(data, null, "MaterialPath", out model.MaterialPath);
            DeserializeMember(data, null, "terrainMaterial", out model.terrainMaterial); 
            DeserializeMember(data, null, "terrainTint", out model.terrainTint); 
            DeserializeMember(data, null, "oceanMaterial", out model.oceanMaterial); 
            DeserializeMember(data, null, "oceanTint", out model.oceanTint); 
            DeserializeMember(data, null, "atmosphereMaterial", out model.atmosphereMaterial); 
            DeserializeMember(data, null, "atmosphereTint", out model.atmosphereTint); 
            DeserializeMember(data, null, "thumbMaterial", out model.thumbMaterial); 
            DeserializeMember(data, null, "thumbTint", out model.thumbTint); 
            DeserializeMember(data, null, "minimapMaterial", out model.minimapMaterial); 
            DeserializeMember(data, null, "minimapTint", out model.minimapTint); 
            DeserializeMember(data, null, "ambient", out model.ambient);
            return result;
        }
    }
}