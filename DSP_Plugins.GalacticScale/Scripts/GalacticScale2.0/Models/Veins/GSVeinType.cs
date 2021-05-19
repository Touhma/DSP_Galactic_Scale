using UnityEngine;
using System.Collections.Generic;
using System;
using FullSerializer;

namespace GalacticScale
{
    [fsObject(Converter = typeof(GSFSVeinTypeConverter))]
    public class GSVeinType
    {
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
            ["Stone"] = EVeinType.Stone
        };
        public static Dictionary<EVeinType, string> insaneVeinTypes = new Dictionary<EVeinType, string>()
        {
            [EVeinType.Iron]="Iron",
            [EVeinType.Copper]="Copper",
            [EVeinType.Coal]="Coal",
            [EVeinType.Oil]="Oil",
            [EVeinType.Crysrub]="Organic",
            [EVeinType.Bamboo]="Spiriform",
            [EVeinType.Silicium]="Silicon",
            [EVeinType.Fractal]="Fractal",
            [EVeinType.Titanium]="Titanium",
            [EVeinType.Diamond]="Kimberlite",
            [EVeinType.Fireice]="Fireice",
            [EVeinType.Grat]="Optical",
            [EVeinType.Mag ]="Unipolar",
            [EVeinType.Stone]="Stone",
        };

        public List<GSVein> veins = new List<GSVein>();
        public EVeinType type = EVeinType.None;
        public bool rare = false;
        public int generate;
        public int Count { get => veins.Count; }
        [NonSerialized]
        public PlanetData planet;
        public GSVeinType Clone()
        {
            GSVeinType clone = (GSVeinType)MemberwiseClone();
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

}