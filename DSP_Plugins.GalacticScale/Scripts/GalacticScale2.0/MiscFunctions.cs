using System;
using System.Collections.Generic;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static bool AbortGameStart(string message)
        {
            Error("Aborting Game Start|" + message);
            Failed = true;
            UIRoot.instance.CloseLoadingUI();
            UIRoot.instance.CloseGameUI();
            UIRoot.instance.launchSplash.Restart();
            DSPGame.StartDemoGame(0);
            UIMessageBox.Show("Somewhat Fatal Error", "Cannot Start Game. Possibly reason: "+message, "Rats!", 3, new UIMessageBox.Response(() => {
                UIRoot.instance.OpenMainMenuUI();
                UIRoot.ClearFatalError();
            }));
            UIRoot.ClearFatalError();
            return false;
        }
        public static GSPlanet GetGSPlanet(PlanetData planet)
        {
            return GetGSPlanet(planet.id);
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
            Log("Finding GSPlanet By ID "+vanillaID);
            if (vanillaID < 0)
            {
                Warn("Failed to get GSPlanet. ID less than 0. ID:" + vanillaID);
                return null;
            }
            Log("2Finding GSPlanet By ID " + vanillaID);
            if (!gsPlanets.ContainsKey(vanillaID))
            {
                Warn("Failed to get GSPlanet. ID does not exist. ID:" + vanillaID);
                return null;
            }
            Log("3Finding GSPlanet By ID " + vanillaID);
            if (gsPlanets[vanillaID] == null)
            {
                Warn("Failed to get GSPlanet. ID exists, but GSPlanet is null. ID:" + vanillaID);
                return null;
            }
            return gsPlanets[vanillaID];
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