using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Rendering;
using static GalacticScale.GS3;
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
        // public static async Task KillPlanetCompute()
        // {
        //     // Warn("Killing Planet Compute");
        //     // if (planetComputeThreadFlag == ThreadFlag.Running) EndPlanetComputeThread();
        //     // shouldAbort = true;
        //     // while (true)
        //     // {
        //     //     if (planetComputeThreadFlag == ThreadFlag.Ended) break;
        //     //     Warn($"Waiting for Planet Compute to end {planetComputeThreadFlag}");
        //     //     await Task.Delay(10);
        //     // }
        //     //
        //     // Warn("Planet Compute Ended");
        //     genPlanetReqList = new Queue<PlanetData>(100);
        //     modPlanetReqList = new Queue<PlanetData>(100);
        //     fctPlanetReqList = new Queue<PlanetData>(100);
        // }

        // public static async Task KillPlanetCalc()
        // {
        //     Warn("Killing Planet Calc");
        //     // shouldAbortCalc = true;
        //     // if (planetCalculateThreadFlag == ThreadFlag.Running) EndPlanetCalculateThread();
        //     // while (true)
        //     // {
        //     //     Warn($"Waiting for planetCalc to end {planetCalculateThreadFlag}");
        //     //     if (planetCalculateThreadFlag == ThreadFlag.Ended) break;
        //     //     await Task.Delay(10);
        //     // }
        //
        //     // Warn("Planet planetCalc Ended");
        //     calPlanetReqList = new Queue<PlanetData>(100);
        //     Warn("Planet Calc Thread Killed");
        // }

        // public static async Task RestartPlanetCalcThread()
        // {
        //     Warn("Restarting Planet Calc Thread");
        //
        //     await KillPlanetCalc();
        //     Warn("Planet Calc Thread Killed");
        //
        //     StartPlanetCalculateThread();
        //     Warn("Planet Calc Thread Restarted");
        // }

        // public static async Task RestartPlanetComputeThread()
        // {
        //     Warn("Restarting Planet Compute Thread");
        //     await KillPlanetCompute();
        //     Warn("Planet Compute Thread Killed");
        //     StartPlanetComputeThread();
        //     Warn("Planet Compute Thread Restarted");
        // }

        // public static void StopModellingPlanets()
        // {
        //     Warn($"Restarting Planet Modeler {processing.Count}");
        //     Reset();
        //     Bootstrap.WaitUntil(() => processing.Count == 0, StopModellingPlanetsPart2);
        //
        // }
        // public async static void StopModellingPlanetsPart2()
        // {
        //     Warn($"Restarting Planet Modeler2 {processing.Count}");
        //     // await Task.Delay(1000);
        //     Warn("Modeler continue restart. Killing Calc and Compute");
        //     // await KillPlanetCalc();
        //     // await KillPlanetCompute();
        //     Warn("Compute and Calc Ended, Resetting.");
        //     Warn("Reset Complete. Starting Compute and Calculate");
        //     Warn($"Restarting Planet Modeler3 {processing.Count}");
        //     // StartPlanetCalculateThread();
        //     // StartPlanetComputeThread();
        //     Warn($"Restarting Planet Modeler4 {Idle}");
        //
        // }
        public static void Reset()
        {
            calPlanetReqList = new Queue<PlanetData>(100);
            genPlanetReqList = new Queue<PlanetData>(100);
            modPlanetReqList = new Queue<PlanetData>(100);
            fctPlanetReqList = new Queue<PlanetData>(100);
            planetModQueue = new List<PlanetData>();
            planetModQueueSorted = false;
            planetQueue = new List<PlanetData>();
            planetQueueSorted = false;
            // calcPlanet = null;
            // compPlanet = null;
            // shouldAbort = true;
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
                        //Log("Processing List");
                        planetModQueueSorted = false;
                        while (modPlanetReqList.Count > 0) planetModQueue.Add(modPlanetReqList.Dequeue());
                    }
                }

                if (!planetModQueueSorted && planetModQueue.Count > 1)
                    lock (planetModQueue)
                    {
                        var hsw = new HighStopwatch();
                        hsw.Begin();
                        //Log($"Sorting ModQueue with {planetModQueue.Count} entries");
                        planetModQueue.Sort(DistanceComparison);
                        planetModQueueSorted = true;
                        //Log($"Sorted ModQueue in {hsw.duration:F5}");
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
                    // GS3.Warn($"Modelling {currentModelingPlanet.name}");
                    ModelingPlanetMain(currentModelingPlanet);
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