namespace GalacticScale
{
    public class GSTerrainSettings
    {
        public double num1 = 0.01;
        public double num2 = 0.012;
        public double num3 = 0.01;
        public double num6 = 0.9;
        public double landModifier = 0.5;
        public double num8 = 2.5;
        public double biomeHeightModifier = 0.3;
        public double heightMulti = 3.0;
        public double baseHeight = -0.2;
    }
    public static partial class GSPlanetAlgorithm
    {
        public static void GenerateTerrain(GSPlanet gsPlanet)
        {
            
            GSTerrainSettings t = GS2.ThemeLibrary[gsPlanet.Theme].TerrainSettings;
            //GS2.Log("Generate Terrain for "+gsPlanet.Name + " " + t.landModifier );
            double num1 = t.num1;
            double num2 = t.num2;
            double num3 = t.num3;
            double heightMulti = t.heightMulti; //Height Factor default 3.0
            double baseHeight = t.baseHeight; //Base Height offset -2.5 - 2.5
            double num6 = t.num6;
            double landModifier = t.landModifier;
            double num8 = t.num8;
            double num9 = t.biomeHeightModifier;
            double num20;

            //double num6 = 0.9;      //  0.9		//  0.9?		// Highlands amount
            //double num7 = 0.7;      //  0.5		//	0.5?		// Land amount?
            //double num8 = 2.9;      //  2.5		//  3.5?		// Lake size?
            //double num9 = 0.5;      //  0.3		//  0.5?		// Land blob number? Land blobiness?




            int seed1 = random.Next();
            int seed2 = random.Next();
            SimplexNoise simplexNoise1 = new SimplexNoise(seed1);
            SimplexNoise simplexNoise2 = new SimplexNoise(seed2);
            PlanetRawData data = gsPlanet.planetData.data;
            int maxHD = -999;
            int minHD = 999;
            int maxBH = -999;
            int minBH = 999;
            for (int i = 0; i < data.dataLength; ++i)
            {
                double x = data.vertices[i].x * (double)gsPlanet.planetData.radius;
                double y = data.vertices[i].y * (double)gsPlanet.planetData.radius;
                double z = data.vertices[i].z * (double)gsPlanet.planetData.radius;
                double noise1 = simplexNoise1.Noise3DFBM(x * num1, y * num2, z * num3, 6) * heightMulti + baseHeight;
                double noise2 = simplexNoise2.Noise3DFBM(x * (1.0 / 400.0), y * (1.0 / 400.0), z * (1.0 / 400.0), 3) * heightMulti * num6 + landModifier;
                noise2 = noise2 <= 0.0 ? noise2 : noise2 * 0.5;
                double noise = noise1 + noise2;
                double f = noise <= 0.0 ? noise * 1.6 : noise * 0.5;
                double num17 = f <= 0.0 ? Maths.Levelize2(f, 0.5) : Maths.Levelize3(f, 0.7);
                double noise3 = simplexNoise2.Noise3DFBM(x * num1 * 2.5, y * num2 * 8.0, z * num3 * 2.5, 2) * 0.6 - 0.3;
                double num19 = f * num8 + noise3 + num9;
                //double
                num20 = num19 >= 1.0 ? (num19 - 1.0) * 0.8 + 1.0 : num19;
                var hd = (int)((gsPlanet.planetData.radius + num17 + 0.2) * 100.0);
                data.heightData[i] = (ushort)UnityEngine.Mathf.Clamp(hd,0,65000);
                var bh = (byte)UnityEngine.Mathf.Clamp((float)(num20 * 100.0), 0.0f, 200f);
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