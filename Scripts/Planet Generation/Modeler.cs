using System;
using System.Collections.Generic;
using static GalacticScale.GS2;
using static PlanetModelingManager;

namespace GalacticScale
{
    public static partial class Modeler
    {
        public static List<PlanetData> planetModQueue = new();
        public static bool planetModQueueSorted;
        public static List<PlanetData> planetQueue = new();
        public static bool planetQueueSorted;
        public static List<PlanetData> processing = new();
        public static bool Idle => processing.Count == 0;
        
        public static void Reset()
        {
            scnPlanetReqList = new Queue<PlanetData>(100);
            genPlanetReqList = new Queue<PlanetData>(100);
            modPlanetReqList = new Queue<PlanetData>(100);
            fctPlanetReqList = new Queue<PlanetData>(100);
            planetModQueue = new List<PlanetData>();
            planetModQueueSorted = false;
            planetQueue = new List<PlanetData>();
            planetQueueSorted = false;
        }

        public static int DistanceComparison(PlanetData p1, PlanetData p2)
        {
            var d1 = distanceTo(p1);
            var d2 = distanceTo(p2);
            if (d1 > d2) return 1;
            return -1;
        }

        private static double distanceTo(PlanetData planet)
        {
            return (GameMain.mainPlayer.uPosition - planet.uPosition).magnitude;
        }

        public static void ModelingCoroutine()
        {
            if (currentModelingPlanet == null)
            {
                PlanetData planetData = null;
                lock (modPlanetReqList)
                {
                    if (modPlanetReqList.Count > 0)
                    {
                        planetModQueueSorted = false;
                        while (modPlanetReqList.Count > 0) planetModQueue.Add(modPlanetReqList.Dequeue());
                    }
                }

                if (!planetModQueueSorted && planetModQueue.Count > 1)
                    lock (planetModQueue)
                    {
                        planetModQueue.Sort(DistanceComparison);
                        planetModQueueSorted = true;
                    }

                if (planetModQueue.Count > 0)
                {
                    planetData = planetModQueue[0];
                    planetModQueue.RemoveAt(0);
                    Log($"Modelling {planetData?.name} - {GetGSPlanet(planetData)?.Theme}");
                }

                if (planetData != null)
                {
                    currentModelingPlanet = planetData;
                    currentModelingStage = 0;
                    currentModelingSeamNormal = 0;
                }
            }

            if (currentModelingPlanet != null)
                try
                {
                    // Call the vanilla method - our transpiler patches will be applied automatically
                    GS2.Log($"Modeler.ModelingCoroutine: Calling ModelingPlanetMain for {currentModelingPlanet.name}");
                    PlanetModelingManager.ModelingPlanetMain(currentModelingPlanet);
                    GS2.Log($"Modeler.ModelingCoroutine: ModelingPlanetMain completed for {currentModelingPlanet.name}");
                }
                catch (Exception message)
                {
                    Error(message.ToString());
                    if (currentModelingPlanet != null)
                    {
                        currentModelingPlanet.Unload();
                        currentModelingPlanet.factoryLoaded = false;
                        currentModelingPlanet = null;
                        currentModelingStage = 0;
                        currentModelingSeamNormal = 0;
                    }
                }
        }
    }
}