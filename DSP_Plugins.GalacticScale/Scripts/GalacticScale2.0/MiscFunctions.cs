using System;
using System.Collections.Generic;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static GSPlanet GetGSPlanet(PlanetData planet)
        {
            return gsPlanets[planet.id];
        }
        public static GSPlanet GetGSPlanet(string name)
        {
            Log("Checking gsPlanets. All " + gsPlanets.Count + " of them.");
            foreach (var kvp in gsPlanets)
            {
                GSPlanet p = kvp.Value;
                Log("Checking "+ p.Name + " == " + name);
                if (p.Name == name) return p;
            }
            return null;
        }
        public static GSPlanet GetGSPlanet(int vanillaID)
        {
            Log("Finding GSPlanet By ID");
            GSPlanet p = gsPlanets[vanillaID];
            //for (var i = 0; i < galaxy.stars.Length; i++)
            //{
            //    for (var j = 0; j < galaxy.stars[i].planets.Length; j++)
            //    {
            //        GSPlanet p = kvp.Value;
            //        Log("Checking " + p.Name + " == " + name);
            //        if (galaxy.stars[i].planets[j].id == vanillaID) return p;
            //    }

            //}
            if (p != null) return p;
            Error("Failed to get GSPlanet by ID " + vanillaID);
            return null;
        }
        public static void EndGame()
        {
            GameMain.End();
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
        

       
        
    }
}