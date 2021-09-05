namespace GalacticScale
{
    public static partial class TerrainAlgorithms
    {
        public static void GenerateTerrain00(GSPlanet gsPlanet, double modX = 0.0, double modY = 0.0)
        {
            if (gsPlanet == null)
            {
                GS2.Warn("gsPlanet Null");
                return;
            }

            //GS2.Log($"USING GSTA00 FOR {gsPlanet.Name}");
            if (gsPlanet.planetData == null)
            {
                GS2.Warn("gsPlanet.planetData Null");
                return;
            }

            var data = gsPlanet.planetData.data;
            for (var i = 0; i < data.dataLength; i++)
            {
                data.heightData[i] = (ushort)(gsPlanet.planetData.radius * 100.1);
                data.biomoData[i] = 0;
            }
        }
    }
}