using System;
using GSSerializer;

namespace GalacticScale
{
    public class GSTerrainSettings
    {
        public string Algorithm = "Vanilla";
        public double BaseHeight = 0;
        public double BiomeHeightModifier = 0;
        public double BiomeHeightMulti = 1;
        public bool BrightnessFix = false; //Fix for Lava
        public double HeightMulti = 1;
        public double LandModifier = 0;
        public double RandomFactor = 1;

        [NonSerialized] public double xFactor = 0;

        [NonSerialized] public double yFactor = 0;

        [NonSerialized] public double zFactor = 0;

        public bool Equals(GSTerrainSettings other)
        {
            var serializer = new fsSerializer();
            serializer.TrySerialize(this, out var thisData).AssertSuccessWithoutWarnings();
            serializer.TrySerialize(this, out var otherData).AssertSuccessWithoutWarnings();
            return thisData.Equals(otherData);
        }

        public GSTerrainSettings Clone()
        {
            return (GSTerrainSettings)MemberwiseClone();
        }
    }
}