using UnityEngine;
using System.Collections.Generic;
using System;
using FullSerializer;

namespace GalacticScale
{
    public class GSVein
    {
        public float richness;
        public int count;
        //[NonSerialized]
        public Vector3 position;
        //public float density = -1f;
        public GSVein(int count,float richness, Vector3 position)
        {
            this.richness = richness;
            this.position = position;
            this.count = count;
            //this.density = density;
    }
        public GSVein(int count, float richness)
        {
            GS2.Log("Creating Vein with " + count + " count and " + richness + " richness");
            this.count = count;
            this.richness = richness;
            this.position = Vector3.zero;
        }
        public GSVein (GSPlanet gsPlanet, int seed = -1)
        {
            if (seed < 0) seed = GSSettings.Seed;
            GS2.Random random = new GS2.Random(seed);
            this.richness =  (float)random.NextDouble() * gsPlanet.planetData.star.resourceCoef;//(int)(random.Next(50000, 150000)
            this.count = (int)random.Next(1, 30);
            this.position = Vector3.zero;
        }
        public GSVein ()
        {
            this.richness = 0;
            this.count = 0;
            this.position = Vector3.zero;
        }
        public GSVein Clone()
        {
            return (GSVein)MemberwiseClone();
        }
    }
    public class GSFSVeinTypeConverter : fsDirectConverter<GSVeinType>
    {
        public override object CreateInstance(fsData data, Type storageType)
        {
            return new GSVeinType();
        }

        protected override fsResult DoSerialize(GSVeinType model, Dictionary<string, fsData> serialized)
        {
            // Serialize name manually
            List<fsData> list = new List<fsData>();
            for (var i = 0; i < model.veins.Count; i++)
            {
                list.Add(new fsData(model.veins[i].count + "," + model.veins[i].richness));
            }
            SerializeMember(serialized, null, "type", model.type);
            serialized["veins"] = new fsData(list);
            SerializeMember(serialized, null, "rare", model.rare);
            // Serialize age using helper methods


            return fsResult.Success;
        }

        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref GSVeinType model)
        {
            GS2.Log("DoDeserialize");
            var result = fsResult.Success;
            model = new GSVeinType();
            // Deserialize name mainly manually (helper methods CheckKey and CheckType)
            fsData veinData;
            if ((result += CheckKey(data, "veins", out veinData)).Failed) return result;
            if ((result += CheckType(veinData, fsDataType.Array)).Failed) return result;
            GS2.Warn("Trying to cast to list");
            var veins = veinData.AsList;
            GS2.Warn("List success");
            model.veins = new List<GSVein>();
            for (var i = 0; i < veins.Count; i++)
            {
                GS2.Warn("Trying to cast to string");
                var d = veins[i].AsString.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                GS2.Log("String cast");
                float richness;
                int count;
                if (!float.TryParse(d[1], out richness))
                {
                    GS2.Log("Failed parsing: " + d[1]);
                    return fsResult.Fail("VeinRichness Not Float: " + d[1]);
                }
                if (!int.TryParse(d[0], out count))
                {
                    GS2.Log("Failed parsing: " + d[0]);
                    return fsResult.Fail("VeinCount Not Int: " + d[0]);
                }
                model.veins.Add(new GSVein(count, richness));
            }


            if ((result += DeserializeMember(data, null, "type", out model.type)).Failed) return result;
            if ((result += DeserializeMember(data, null, "rare", out model.rare)).Failed) return result;
            return result;
        }
    }
    [fsObject(Converter = typeof(GSFSVeinTypeConverter))]
    public class GSVeinType
    {
        public List<GSVein> veins = new List<GSVein>();
        public EVeinType type = EVeinType.None;
        public bool rare = false;
        public int Count { get => veins.Count; }
        [NonSerialized]
        public PlanetData planet;
        public GSVeinType Clone()
        {
            GSVeinType clone = (GSVeinType)this.MemberwiseClone();
            clone.veins = new List<GSVein>();
            for (var i = 0; i < veins.Count; i++) clone.veins.Add(veins[i].Clone());
            return clone;
        }
        public GSVeinType (EVeinType type)
        {
            this.type = type;
        }
        public GSVeinType() { }
    }



    public class GSVeinSettings
    {
        public List<GSVeinType> VeinTypes = new List<GSVeinType>();
        public string VeinAlgorithm = "GS2";
        public float VeinPadding = 1f;
        public GSVeinSettings ()
        {

        }
        public GSVeinSettings Clone()
        {
            GSVeinSettings clone = (GSVeinSettings)this.MemberwiseClone();
            clone.VeinTypes = new List<GSVeinType>();
            for (var i = 0; i < VeinTypes.Count; i++) clone.VeinTypes.Add(VeinTypes[i].Clone());
            return clone;
        }
    }


    public class GSVeinDescriptor
    {
        public EVeinType type;
        public int count;
        public float richness;
        public float density;
        public Vector3 position;
        public bool rare;
    }
}