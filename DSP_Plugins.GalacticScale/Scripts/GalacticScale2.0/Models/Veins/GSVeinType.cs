using GSSerializer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale {
    [fsObject(Converter = typeof(GSFSVeinTypeConverter))]
    public class GSVeinType {
        public static Dictionary<string, EVeinType> saneVeinTypes = new Dictionary<string, EVeinType>()
        {
            ["Iron"] = EVeinType.Iron,
            ["Copper"] = EVeinType.Copper,
            ["Coal"] = EVeinType.Coal,
            ["Oil"] = EVeinType.Oil,
            ["Organic"] = EVeinType.Crysrub,
            ["Spiriform"] = EVeinType.Bamboo,
            ["Silicon"] = EVeinType.Silicium,
            ["Fractal"] = EVeinType.Fractal,
            ["Titanium"] = EVeinType.Titanium,
            ["Kimberlite"] = EVeinType.Diamond,
            ["Fireice"] = EVeinType.Fireice,
            ["Optical"] = EVeinType.Grat,
            ["Unipolar"] = EVeinType.Mag,
            ["Stone"] = EVeinType.Stone,
            ["Bamboo"] = EVeinType.Bamboo,
            ["Mag"] = EVeinType.Mag,
            ["Crysrub"] = EVeinType.Crysrub,
            ["Silicium"] = EVeinType.Silicium,
            ["Grat"] = EVeinType.Grat,
            ["Diamond"] = EVeinType.Diamond
        };
        public static Dictionary<EVeinType, string> insaneVeinTypes = new Dictionary<EVeinType, string>()
        {
            [EVeinType.Iron] = "Iron",
            [EVeinType.Copper] = "Copper",
            [EVeinType.Coal] = "Coal",
            [EVeinType.Oil] = "Oil",
            [EVeinType.Crysrub] = "Organic",
            [EVeinType.Bamboo] = "Spiriform",
            [EVeinType.Silicium] = "Silicon",
            [EVeinType.Fractal] = "Fractal",
            [EVeinType.Titanium] = "Titanium",
            [EVeinType.Diamond] = "Kimberlite",
            [EVeinType.Fireice] = "Fireice",
            [EVeinType.Grat] = "Optical",
            [EVeinType.Mag] = "Unipolar",
            [EVeinType.Stone] = "Stone",
        };

        public List<GSVein> veins = new List<GSVein>();
        public EVeinType type = EVeinType.None;
        public bool rare = false;
        public int generate;
        public int Count => veins.Count;

        [NonSerialized]
        public PlanetData planet;
        public GSVeinType Clone() {
            GSVeinType clone = (GSVeinType)MemberwiseClone();
            clone.veins = new List<GSVein>();
            for (var i = 0; i < veins.Count; i++) {
                clone.veins.Add(veins[i].Clone());
            }

            return clone;
        }
        public GSVeinType(EVeinType type) {
            this.type = type;
        }
        public GSVeinType() { }
        public static GS2.Random random = new GS2.Random(GSSettings.Seed);
        public static GSVeinType Generate(EVeinType type, int min, int max, float min_richness, float max_richness, int min_patchSize, int max_patchSize, bool rare) {
            GSVeinType vt = new GSVeinType(type);
            vt.rare = rare;
            int amount = Mathf.RoundToInt(Mathf.Clamp(random.Next(min, max + 1), 0, 99));
            for (var i = 0; i < amount; i++) {
                vt.veins.Add(new GSVein(random.Next(min_patchSize, max_patchSize + 1), random.NextFloat(min_richness, max_richness + float.MinValue)));
            }

            return vt;
        }
    }

}