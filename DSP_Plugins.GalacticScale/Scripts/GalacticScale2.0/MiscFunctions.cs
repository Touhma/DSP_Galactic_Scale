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
            public float NextFloat(float max)
            {
                return (float)Range(0f,max);
            }
            public bool Bool(double chance) => (NextDouble() < chance);
            public bool Bool() => (NextDouble() < 0.5);
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
            private Random() { }
        }

        public class SingletonExample //left here for future use
        {
            private SingletonExample() {}
            public static SingletonExample Instance { get { return Internal.instance; } }
            private class Internal { static Internal() {} internal static readonly SingletonExample instance = new SingletonExample(); }
        }
        

       
        
    }
}