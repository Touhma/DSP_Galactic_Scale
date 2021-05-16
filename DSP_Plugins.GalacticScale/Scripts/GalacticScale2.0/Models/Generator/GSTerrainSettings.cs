namespace GalacticScale
{
    public class GSTerrainSettings
    {
        public double xFactor = 0;
        public double yFactor = 0;
        public double zFactor = 0;
        public double randomFactor = 1;
        public double landModifier = 0;
        public double biomeHeightMulti = 1;
        public double biomeHeightModifier = 0;
        public double heightMulti = 1;
        public double baseHeight = 0;
        public string terrainAlgorithm = "Vanilla";
        public bool brightnessFix = false; //Fix for Lava
        public GSTerrainSettings Clone()
        {
            return (GSTerrainSettings)MemberwiseClone();
        }
    }
}