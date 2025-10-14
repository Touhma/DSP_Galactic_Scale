﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Rendering;
using static GalacticScale.GS2;
using static PlanetModelingManager;

namespace GalacticScale
{
    public static partial class Modeler
    {
        static PlanetData compPlanet = null;
        
        public static bool Compute()
        {
            Thread thread = null;
            lock (planetComputeThreadFlagLock)
            {
                thread = planetComputeThread;
            }

            var cycles = 0;
            while (true)
            {
                cycles++;
                compPlanet = null;
                var pqsw = new HighStopwatch();
                pqsw.Begin();
                lock (planetProcessingLock)
                {
                    lock (planetComputeThreadFlagLock)
                    {
                        if (thread != planetComputeThread)
                        {
                            Log("End due to planetComputeThread mismatch");
                            return false;
                        }

                        if (planetComputeThreadFlag != ThreadFlag.Running)
                        {
                            planetComputeThreadFlag = ThreadFlag.Ended;
                            planetQueue.Clear();
                            planetModQueue.Clear();
                            planetQueueSorted = false;
                            planetModQueueSorted = false;
                            Log($"Ended after:{pqsw.duration:F5}");
                            return false;
                        }
                    }

                    lock (genPlanetReqList)
                    {
                        if (genPlanetReqList.Count > 0)
                        {
                            Log("Processing List");
                            planetQueueSorted = false;
                            while (genPlanetReqList.Count > 0) planetQueue.Add(genPlanetReqList.Dequeue());
                        }
                    }

                    if (!planetQueueSorted && planetQueue.Count > 1)
                        lock (planetQueue)
                        {
                            Log($"Sorting Queue with {planetQueue.Count} entries where Player:{GameMain.mainPlayer?.uPosition} localPlanet:{GameMain.localPlanet?.name}:{GameMain.localPlanet?.uPosition}");
                            planetQueue.Sort(DistanceComparison);
                            planetQueueSorted = true;
                            Log("Sorted");
                        }

                    if (planetQueue.Count > 0)
                    {


                        compPlanet = planetQueue[0];
                        if (!processing.Contains(compPlanet)) processing.Add(compPlanet);
                        ModellingDone = false;
                        planetQueue.RemoveAt(0);
                        Log($"Retrieved sorted planet from list: {compPlanet.name}");

                    }

                    if (compPlanet != null && planetComputeThreadFlag == ThreadFlag.Running)
                    {
                        Log($"Preamble time taken:{pqsw.duration:F5}");
                        try
                        {
                            var planetAlgorithm = Algorithm(compPlanet);
                            if (planetAlgorithm != null)
                            {
                                var highStopwatch = new HighStopwatch();
                                var num2 = 0.0;
                                var num3 = 0.0;
                                var num4 = 0.0;
                                if (compPlanet.data == null)
                                {
                                    Log($"Creating Planet {compPlanet.name}");
                                    highStopwatch.Begin();
                                    compPlanet.data = new PlanetRawData(compPlanet.precision);
                                    if (compPlanet == null) return false;
                                    compPlanet.modData = compPlanet.data.InitModData(compPlanet.modData);
                                    if (compPlanet == null) return false;
                                    compPlanet.data.CalcVerts();
                                    if (compPlanet == null) return false;
                                    compPlanet.aux = new PlanetAuxData(compPlanet);
                                    Log("Generating Terrain");
                                    if (compPlanet == null) return false;
                                    planetAlgorithm.GenerateTerrain(compPlanet.mod_x, compPlanet.mod_y);
                                    if (compPlanet == null) return false;
                                    if (!UIRoot.instance.backToMainMenu) planetAlgorithm.CalcWaterPercent();
                                    num2 = highStopwatch.duration;
                                }

                                if (compPlanet.factory == null && !UIRoot.instance.backToMainMenu)
                                {
                                    Log("Creating Factory");
                                    highStopwatch.Begin();
                                    if (compPlanet.type != EPlanetType.Gas && compPlanet.data != null)
                                    {
                                        compPlanet.data.vegeCursor = 1;
                                        planetAlgorithm.GenerateVegetables();
                                    }
                                    num3 = highStopwatch.duration;
                                    highStopwatch.Begin();
                                    if (compPlanet.type != EPlanetType.Gas && compPlanet.data != null) planetAlgorithm.GenerateVeins();
                                    if (compPlanet.data != null) compPlanet.SummarizeVeinGroups();
                                    num4 = highStopwatch.duration;
                                }

                                // else if (planetData.galaxy.birthPlanetId == planetData.id) //Added after 0.9.25 update
                                // {
                                //     planetData.GenBirthPoints();
                                // }//end add 0.9.25 update
                                if (planetComputeThreadLogs != null)
                                    lock (planetComputeThreadLogs)
                                    {
                                        // Change: Compress it to one line
                                        string timerMessage = $"[Terrain]:{num2:F3}s [Vegetables]:{num3:F3}s [Veins]:{num4:F3}s  Planet: {compPlanet.displayName}";
                                        planetComputeThreadLogs.Add(timerMessage);
                                        Log(timerMessage);
                                        //planetComputeThreadLogs.Add($"{compPlanet.displayName}\r\nGenerate Terrain {num2:F5} s\r\nGenerate Vegetables {num3:F5} s\r\nGenerate Veins {num4:F5} s\r\n");
                                        //Log($"{compPlanet.displayName}\r\nGenerate Terrain {num2:F5} s\r\nGenerate Vegetables {num3:F5} s\r\nGenerate Veins {num4:F5} s\r\n");
                                    }

                                compPlanet?.NotifyScanEnded();
                                if (processing.Contains(compPlanet)) processing.Remove(compPlanet);
                            }
                        }
                        catch (Exception ex)
                        {
                            lock (planetComputeThreadError)
                            {
                                if (processing.Contains(compPlanet)) processing.Remove(compPlanet);
                                if (string.IsNullOrEmpty(planetComputeThreadError))
                                    planetComputeThreadError = ex.ToString();
                            }
                        }

                        lock (modPlanetReqList)
                        {
                            //Log($"Queuing {planetData.name} in modPlanetReqList after {pqsw.duration:F5}");
                            modPlanetReqList.Enqueue(compPlanet);
                        }
                    }
                }

                if (compPlanet == null)
                {
                    ModellingDone = true;
                    Thread.Sleep(50);
                }
                else
                {
                    Thread.Sleep(2);
                }
            }
        }

    }
}