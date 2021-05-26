using System;

namespace GalacticScale
{
    public class GSTerrainSettings
    {
        [NonSerialized]
        public double xFactor = 0;
        [NonSerialized] 
        public double yFactor = 0;
        [NonSerialized]
        public double zFactor = 0;
        public double RandomFactor = 1;
        public double LandModifier = 0;
        public double BiomeHeightMulti = 1;
        public double BiomeHeightModifier = 0;
        public double HeightMulti = 1;
        public double BaseHeight = 0;
        public string Algorithm = "Vanilla";
        public bool   BrightnessFix = false; //Fix for Lava
        public GSTerrainSettings Clone()
        {
            return (GSTerrainSettings)MemberwiseClone();
        }
    }
}