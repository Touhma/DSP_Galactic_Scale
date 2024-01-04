﻿namespace GalacticScale
{
    public static partial class TerrainAlgorithms
    {
        public static void GenerateTerrain00(GSPlanet gsPlanet, double modX = 0.0, double modY = 0.0)
        {
            if (gsPlanet == null)
            {
                GS3.Warn("gsPlanet Null");
                return;
            }

            //GS3.Log($"USING GSTA00 FOR {gsPlanet.Name}");
            if (gsPlanet.planetData == null)
            {
                GS3.Warn("gsPlanet.planetData Null");
                return;
            }

            var data = gsPlanet.planetData.data;
            if (data == null) return;
            for (var i = 0; i < data.dataLength; i++)
            {
                if (data.heightData == null) return;
                data.heightData[i] = (ushort)(gsPlanet.planetData.radius * 100.1);
                data.biomoData[i] = 0;
            }
        }
    }
}