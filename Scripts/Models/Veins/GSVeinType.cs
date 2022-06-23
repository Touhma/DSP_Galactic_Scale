using System;
using System.Collections.Generic;
using GSSerializer;
using UnityEngine;

namespace GalacticScale
{
    [fsObject(Converter = typeof(GSFSVeinTypeConverter))]
    public class GSVeinType
    {
        public static Dictionary<string, EVeinType> saneVeinTypes = new()
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

        public static Dictionary<EVeinType, string> insaneVeinTypes = new()
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
            [EVeinType.Stone] = "Stone"
        };

        public static GS2.Random random = new(GSSettings.Seed);
        public int generate;

        [NonSerialized] public PlanetData planet;

        public bool rare;
        public EVeinType type = EVeinType.None;

        public List<GSVein> veins = new();

        public GSVeinType(EVeinType type)
        {
            this.type = type;
        }

        public GSVeinType()
        {
        }

        public int Count => veins.Count;

        public GSVeinType Clone()
        {
            var clone = (GSVeinType)MemberwiseClone();
            clone.veins = new List<GSVein>();
            for (var i = 0; i < veins.Count; i++) clone.veins.Add(veins[i].Clone());

            return clone;
        }

        public static GSVeinType Generate(EVeinType type, int min, int max, float min_richness, float max_richness, int min_patchSize, int max_patchSize, bool rare)
        {
            var vt = new GSVeinType(type);
            vt.rare = rare;
            var amount = Mathf.RoundToInt(Mathf.Clamp(random.Next(min, max + 1), 0, 3000));
            for (var i = 0; i < amount; i++)
                vt.veins.Add(new GSVein(random.Next(min_patchSize, max_patchSize + 1), random.NextFloat(min_richness, max_richness)));

            return vt;
        }
    }
}