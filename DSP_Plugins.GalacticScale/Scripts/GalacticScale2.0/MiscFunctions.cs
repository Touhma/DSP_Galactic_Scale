using System;
using System.Collections.Generic;
using System.Linq;
using BCE;
namespace GalacticScale
{
    public static partial class GS2
    {
        public static class Utils
        {
            public static UnityEngine.Cubemap TintCubeMap(UnityEngine.Cubemap input, UnityEngine.Color color)
            {
                GS2.Log("Tinting Cubemap");
                UnityEngine.Cubemap output = UnityEngine.Object.Instantiate(input);
                var colors = output.GetPixels(UnityEngine.CubemapFace.PositiveX);
                GS2.Log(colors[0].ToString());
                var tinted = new UnityEngine.Color[colors.Length];
                for (var i=0;i<colors.Length;i++) tinted[i] = UnityEngine.Color.Lerp(new UnityEngine.Color(colors[i].grayscale, colors[i].grayscale, colors[i].grayscale),new UnityEngine.Color(color.r,color.g,color.b), color.a);
                Log(tinted[0].ToString());
                output.SetPixels(tinted, UnityEngine.CubemapFace.PositiveX);
                output.SetPixels(tinted, UnityEngine.CubemapFace.PositiveY);
                output.SetPixels(tinted, UnityEngine.CubemapFace.PositiveZ);
                output.SetPixels(tinted, UnityEngine.CubemapFace.NegativeX);
                output.SetPixels(tinted, UnityEngine.CubemapFace.NegativeY);
                output.SetPixels(tinted, UnityEngine.CubemapFace.NegativeZ);
                Log("End");
                return output;
            }
            public static GSPlanet GetGSPlanet(PlanetData planet)
            {
                return gsPlanets[planet.id];
            }
            public static bool ArrayCompare<T>(T[] a1, T[] a2) {
                return a1.SequenceEqual(a2);
            }
            public static T ReverseLookup<T, W>(Dictionary<T, W> dict, W val)
            {
                foreach (KeyValuePair<T, W> kvp in dict) if (kvp.Value.ToString() == val.ToString()) return kvp.Key;
                return default(T);
            }
            public static float diff(float a, float b)
            {
                return (!(a > b)) ? (b - a) : (a - b);
            }
            public static float ParsePlanetSize(float radius)
            {
                if (radius < 8f) return 5f;
                return UnityEngine.Mathf.RoundToInt(UnityEngine.Mathf.Clamp(radius, 10, 510) / 10) * 10;
            }
            public static List<VectorLF3> RegularPointsOnSphere(float radius, int count)
            {
                List<VectorLF3> points = new List<VectorLF3>();
                if (count == 0) return points;
                double a = 4.0 * Math.PI * (Math.Pow(radius, 2) / count);
                double d = Math.Sqrt(a);
                int m_theta = (int)Math.Round(Math.PI / d);
                double d_theta = Math.PI / m_theta;
                double d_phi = a / d_theta;
                for (int m = 0; m < m_theta; m++)
                {
                    double theta = Math.PI * (m + 0.5) / m_theta;
                    int m_phi = (int)Math.Round(2.0 * Math.PI * Math.Sin(theta) / d_phi);
                    for (int n = 0; n < m_phi; n++)
                    {
                        double phi = 2.0 * Math.PI * n / m_phi;
                        double x = radius * Math.Sin(theta) * Math.Cos(phi);
                        double y = radius * Math.Sin(theta) * Math.Sin(phi);
                        double z = radius * Math.Cos(theta);
                        points.Add(new VectorLF3(x, y, z));
                    }
                }
                return points;
            }
        }
        public static void EndGame()
        {
            UIRoot.instance.backToMainMenu = true;
            DSPGame.EndGame();
        }
        public class Random : System.Random
        {
            public float NextFloat()
            {
                return (float)NextDouble();
            }
            public int Range(int min, int max) => (UnityEngine.Mathf.RoundToInt((float)Range((float)min, (float)max)));
            public float Range(float min, float max)=> (float)Math.Round((double)min + (NextDouble() * (double)(max - min)), 8);
            public float Normal(float averageValue, float standardDeviation) => averageValue + standardDeviation * (float)(Math.Sqrt(-2.0 * Math.Log(1.0 - NextDouble())) * Math.Sin(2.0 * Math.PI * NextDouble()));
            public float RangePlusMinusOne() => UnityEngine.Mathf.Sin((float)(NextDouble() * (2 * UnityEngine.Mathf.PI)));
            public VectorLF3 PointOnSphere(double radius)
            {
                double z = (NextDouble() * 2 * radius) - radius;
                double phi = NextDouble() * Math.PI * 2;
                double x = Math.Sqrt((Math.Pow(radius,2) - Math.Pow(z ,2))) * Math.Cos(phi);
                double y = Math.Sqrt((Math.Pow(radius,2) - Math.Pow(z ,2))) * Math.Sin(phi);
                return new VectorLF3(x, y, z);
            }
            public Random(int seed) : base(seed) { }
            public Random() : base(GSSettings.Seed) { }
        }

        public class SingletonExample //left here for future use
        {
            private SingletonExample() {}
            public static SingletonExample Instance { get { return Internal.instance; } }
            private class Internal { static Internal() {} internal static readonly SingletonExample instance = new SingletonExample(); }
        }
        private static int progCount = 0;
        public static void prog(string name, bool end = false)
        {
            if (end) { console.Write("]\n", ConsoleColor.Gray); progCount = -1; }
            else
            {
                if (progCount == 0) { console.Write(name, ConsoleColor.Cyan); console.Write(" [", ConsoleColor.Gray); }
                else console.Write((progCount % 5 == 0) ? "|" : ".", ConsoleColor.Green);
            }
            progCount++;
        }

        public static int GenerateTempPoses(
            int seed,
            int targetCount,
            int iterCount,
            double minDist,
            double minStepLen,
            double maxStepLen,
            double flatten)
        {
            if (tmp_poses == null)
            {
                tmp_poses = new List<VectorLF3>();
                tmp_drunk = new List<VectorLF3>();
            }
            else
            {
                tmp_poses.Clear();
                tmp_drunk.Clear();
            }
            if (iterCount < 1)
                iterCount = 1;
            else if (iterCount > 16)
                iterCount = 16;
            RandomPoses(seed, targetCount * iterCount, minDist, minStepLen, maxStepLen, flatten);
            for (int index = tmp_poses.Count - 1; index >= 0; --index)
            {
                if (index % iterCount != 0)
                    tmp_poses.RemoveAt(index);
                if (tmp_poses.Count <= targetCount)
                    break;
            }
            return tmp_poses.Count;
        }
        private static void RandomPoses(
          int seed,
          int maxCount,
          double minDist,
          double minStepLen,
          double maxStepLen,
          double flatten)
        {
            System.Random random = new System.Random(seed);
            double num1 = random.NextDouble();
            tmp_poses.Add(VectorLF3.zero);
            int num2 = 6;
            int num3 = 8;
            if (num2 < 1)
                num2 = 1;
            if (num3 < 1)
                num3 = 1;
            int num4 = (int)(num1 * (double)(num3 - num2) + (double)num2);
            for (int index = 0; index < num4; ++index)
            {
                int num5 = 0;
                while (num5++ < 256)
                {
                    double num6 = random.NextDouble() * 2.0 - 1.0;
                    double num7 = (random.NextDouble() * 2.0 - 1.0) * flatten;
                    double num8 = random.NextDouble() * 2.0 - 1.0;
                    double num9 = random.NextDouble();
                    double d = num6 * num6 + num7 * num7 + num8 * num8;
                    if (d <= 1.0 && d >= 1E-08)
                    {
                        double num10 = Math.Sqrt(d);
                        double num11 = (num9 * (maxStepLen - minStepLen) + minDist) / num10;
                        VectorLF3 pt = new VectorLF3(num6 * num11, num7 * num11, num8 * num11);
                        if (!CheckCollision(tmp_poses, pt, minDist))
                        {
                            tmp_drunk.Add(pt);
                            tmp_poses.Add(pt);
                            if (tmp_poses.Count >= maxCount)
                                return;
                            break;
                        }
                    }
                }
            }
            int num12 = 0;
            while (num12++ < 256)
            {
                for (int index = 0; index < tmp_drunk.Count; ++index)
                {
                    if (random.NextDouble() <= 0.7)
                    {
                        int num5 = 0;
                        while (num5++ < 256)
                        {
                            double num6 = random.NextDouble() * 2.0 - 1.0;
                            double num7 = (random.NextDouble() * 2.0 - 1.0) * flatten;
                            double num8 = random.NextDouble() * 2.0 - 1.0;
                            double num9 = random.NextDouble();
                            double d = num6 * num6 + num7 * num7 + num8 * num8;
                            if (d <= 1.0 && d >= 1E-08)
                            {
                                double num10 = Math.Sqrt(d);
                                double num11 = (num9 * (maxStepLen - minStepLen) + minDist) / num10;
                                VectorLF3 pt = new VectorLF3(tmp_drunk[index].x + num6 * num11, tmp_drunk[index].y + num7 * num11, tmp_drunk[index].z + num8 * num11);
                                if (!CheckCollision(tmp_poses, pt, minDist))
                                {
                                    tmp_drunk[index] = pt;
                                    tmp_poses.Add(pt);
                                    if (tmp_poses.Count >= maxCount)
                                        return;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        private static bool CheckCollision(List<VectorLF3> pts, VectorLF3 pt, double min_dist)
        {
            double num1 = min_dist * min_dist;
            foreach (VectorLF3 pt1 in pts)
            {
                double num2 = pt.x - pt1.x;
                double num3 = pt.y - pt1.y;
                double num4 = pt.z - pt1.z;
                if (num2 * num2 + num3 * num3 + num4 * num4 < num1)
                    return true;
            }
            return false;
        }
        
    }
}