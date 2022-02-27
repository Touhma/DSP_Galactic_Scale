using UnityEngine;

namespace GalacticScale
{
    public static partial class TerrainAlgorithms
    {
        public static void GenerateTerrain1(GSPlanet gsPlanet, double modX = 0.0, double modY = 0.0)
        {
            if (gsPlanet == null)
            {
                GS2.Warn("gsPlanet Null");
                return;
            }

            random = new GS2.Random(gsPlanet.Seed);
            GS2.Log($"USING GSTA1 FOR {gsPlanet.Name} with seed {GSSettings.Seed}");
            var t = gsPlanet.GsTheme.TerrainSettings;
            GS2.Log("Generate Terrain for " + gsPlanet.Name + " ");
            var seed1 = random.Next();
            var seed2 = random.Next();
            var simplexNoise1 = new SimplexNoise(seed1);
            var simplexNoise2 = new SimplexNoise(seed2);
            if (gsPlanet.planetData == null)
            {
                GS2.Warn("gsPlanet.planetData Null");
                return;
            }

            var data = gsPlanet.planetData.data;
            GS2.Log("GenerateTerrain1|" + gsPlanet.Name + "|"+(data == null));
            var maxHD = -999;
            var minHD = 999999;
            var maxBH = -999;
            var minBH = 999999;
            for (var i = 0; i < data.dataLength; ++i)
            {
                var x = data.vertices[i].x * (double)gsPlanet.planetData.radius;
                var y = data.vertices[i].y * (double)gsPlanet.planetData.radius;
                var z = data.vertices[i].z * (double)gsPlanet.planetData.radius;
                var noise1 = simplexNoise1.Noise3DFBM(x * (t.xFactor + 0.01), y * (0.012 + t.yFactor), z * (0.01 + t.zFactor), 6) * 3 * t.HeightMulti + (-0.2 + t.BaseHeight);
                var noise2 = simplexNoise2.Noise3DFBM(x * (1.0 / 400.0), y * (1.0 / 400.0), z * (1.0 / 400.0), 3) * 3 * t.HeightMulti * (t.RandomFactor + 0.9) + (t.LandModifier + 0.5);
                noise2 = noise2 <= 0.0 ? noise2 : noise2 * 0.5;
                var noise = noise1 + noise2;
                var f = noise <= 0.0 ? noise * 1.6 : noise * 0.5;
                var num17 = f <= 0.0 ? Maths.Levelize2(f, 0.5) : Maths.Levelize3(f, 0.7);
                var noise3 = simplexNoise2.Noise3DFBM(x * (t.xFactor + 0.01) * 2.5, y * (0.012 + t.yFactor) * 8.0, z * (0.01 + t.zFactor) * 2.5, 2) * 0.6 - 0.3;
                var num19 = f * t.BiomeHeightMulti + noise3 + t.BiomeHeightModifier * 2.5 + 0.3;
                var num20 = num19 >= 1.0 ? (num19 - 1.0) * 0.8 + 1.0 : num19;
                var hd = (int)((gsPlanet.planetData.radius + num17 + 0.2) * 100.0);
                data.heightData[i] = (ushort)Mathf.Clamp(hd, ushort.MinValue, ushort.MaxValue);
                var bh = (byte)Mathf.Clamp((float)(num20 * 100.0), 0.0f, 200f);
                data.biomoData[i] = bh;
                if (hd > maxHD) maxHD = hd;

                if (hd < minHD) minHD = hd;

                if (bh > maxBH) maxBH = bh;

                if (bh < minBH) minBH = bh;
            }
            GS2.Log("--------------------");
            GS2.Log("R:" + gsPlanet.Radius + " Planet : " + gsPlanet.Name + " minHD:"+minHD+" maxHD:" + maxHD + " minBH:" + minBH + "maxBH:"  + maxBH);
        }
    }
}