using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static PlanetModelingManager;

namespace GalacticScale
{
    public static partial class Modeler
    {
        static PlanetData calcPlanet = null;

        public static void ForceCalculate(PlanetData planet)
        {
            Modeler.processing.Add(planet);
           
            try
            {
                if (planet == null) return;
                // GS2.Warn($"Trying to force calculate {planet.name}");
                PlanetData unloadedCopy = PlanetData.GetUnloadedCopy(planet);
                PlanetAlgorithm planetAlgorithm = Algorithm(unloadedCopy);
                if (planetAlgorithm != null)
                {
                    // GS2.Log($"Scanning {planet.name}");
                    HighStopwatch highStopwatch = new HighStopwatch();
                    highStopwatch.Begin();
                    unloadedCopy.data = new PlanetRawData(unloadedCopy.precision);
                    unloadedCopy.modData = unloadedCopy.data.InitModData(planet.modData);
                    unloadedCopy.data.CalcVerts();
                    unloadedCopy.aux = new PlanetAuxData(unloadedCopy);
                    planetAlgorithm.GenerateTerrain(unloadedCopy.mod_x, unloadedCopy.mod_y);
                    planetAlgorithm.CalcWaterPercent();
                    double duration = highStopwatch.duration;
                    highStopwatch.Begin();
                    unloadedCopy.data.vegeCursor = 1;
                    if (unloadedCopy.type != EPlanetType.Gas)
                    {
                        planetAlgorithm.GenerateVegetables();
                    }

                    double duration2 = highStopwatch.duration;
                    highStopwatch.Begin();
                    unloadedCopy.data.veinCursor = 1;
                    if (unloadedCopy.type != EPlanetType.Gas)
                    {
                        planetAlgorithm.GenerateVeins();
                    }

                    unloadedCopy.SummarizeVeinGroups();
                    unloadedCopy.GenBirthPoints();
                    planet.CopyScannedDataFrom(unloadedCopy);
                    PlanetData.ReleaseCopy(unloadedCopy);
                    double duration3 = highStopwatch.duration;
                    if (PlanetModelingManager.planetScanThreadLogs != null)
                    {
                        List<string> list = PlanetModelingManager.planetScanThreadLogs;
                        lock (list)
                        {
                            PlanetModelingManager.planetScanThreadLogs.Add(string.Format("{0}\r\nGenerate Terrain {1:F5} s\r\nGenerate Vegetables {2:F5} s\r\nGenerate Veins {3:F5} s\r\n", new object[] { calcPlanet.displayName, duration, duration2, duration3 }));
                        }
                    }

                    // GS2.Log($"Finished forced calculating planet {planet.name}");
                    planet.NotifyScanEnded();
                    planet.scanning = false;
                    planet.scanned = true;
                }
            }
            catch (Exception ex)
            {
                var obj5 = planetScanThreadError;
                lock (obj5)
                {
                    if (string.IsNullOrEmpty(planetScanThreadError)) planetScanThreadError = ex.ToString();
                }

                GS2.Warn($"Error during forced calculating planet {planet?.name}: {ex}");
                if (planet == null) return;
                planet.scanned = false;
                processing.Remove(planet);
            }
        }

        public static void Calculate()
        {
            // GS2.Log("Calc");
            object obj = null;
            var threadFlagLock = planetProcessingLock;
            lock (threadFlagLock)
            {
                obj = planetScanThread;
            }

            for (;;)
            {
                // GS2.Log("Calc");
                calcPlanet = null;
                lock (planetProcessingLock)
                {
                    ThreadFlagLock threadFlagLock2 = planetScanThreadFlagLock;
                    lock (threadFlagLock2)
                    {
                        if (planetScanThreadFlag != ThreadFlag.Running)
                        {
                            planetScanThreadFlag = ThreadFlag.Ended;
                            break;
                        }

                        if (obj != planetScanThread)
                        {
                            GS2.Warn($"End due to planetCalculateThread mismatch ({obj} != {planetScanThread})");
                            break;
                        }
                    }


                    var queue = scnPlanetReqList;
                    lock (queue)
                    {
                        if (scnPlanetReqList.Count > 0) calcPlanet = scnPlanetReqList.Dequeue();
                    }

                    if (calcPlanet != null && calcPlanet.loading)
                    {
                        calcPlanet.scanning = false;
                        calcPlanet = null;
                    }

                    if (calcPlanet != null)
                    {
                        GS2.Warn($"Adding {calcPlanet.name} to processing");
                        processing.Add(calcPlanet);
                        // try
                        // {
                        //     var planetAlgorithm = Algorithm(calcPlanet);
                        //     if (planetAlgorithm != null)
                        //     {
                        //         var highStopwatch = new HighStopwatch();
                        //         highStopwatch.Begin();
                        //         calcPlanet.data = new PlanetRawData(calcPlanet.precision);
                        //         if (calcPlanet == null || calcPlanet.data == null) return; calcPlanet.modData = calcPlanet.data.InitModData(calcPlanet.modData);
                        //         if (calcPlanet == null || calcPlanet.data == null) return; calcPlanet.data.CalcVerts();
                        //         if (calcPlanet == null || calcPlanet.data == null) return; calcPlanet.aux = new PlanetAuxData(calcPlanet);
                        //         if (calcPlanet == null || calcPlanet.data == null || planetAlgorithm == null) return; planetAlgorithm.GenerateTerrain(calcPlanet.mod_x, calcPlanet.mod_y);
                        //         if (calcPlanet == null || calcPlanet.data == null || planetAlgorithm == null)
                        //         {
                        //             GS2.Log("Aborted");
                        //         }
                        //         else planetAlgorithm.CalcWaterPercent();
                        //         if (calcPlanet == null || calcPlanet.data == null) return; var duration = highStopwatch.duration;
                        //         highStopwatch.Begin();
                        //         if (calcPlanet == null || calcPlanet.data == null || planetAlgorithm == null) return; if (calcPlanet.type != EPlanetType.Gas) planetAlgorithm.GenerateVegetables();
                        //         if (calcPlanet == null || calcPlanet.data == null) return; var duration2 = highStopwatch.duration;
                        //         highStopwatch.Begin();
                        //         if (calcPlanet == null || calcPlanet.data == null || planetAlgorithm == null) return; if (calcPlanet.type != EPlanetType.Gas) planetAlgorithm.GenerateVeins();
                        //         if (calcPlanet == null || calcPlanet.data == null) return; calcPlanet.SummarizeVeinGroups();
                        //         if (calcPlanet == null || calcPlanet.data == null) return; calcPlanet.GenBirthPoints();
                        //         if (calcPlanet == null || calcPlanet.data == null) return;
                        //         var duration3 = highStopwatch.duration;
                        //         if (planetScanThreadLogs != null)
                        //         {
                        //             var obj4 = planetScanThreadLogs;
                        //             lock (obj4)
                        //             {
                        //                 planetScanThreadLogs.Add(string.Format("{0}\r\nGenerate Terrain {1:F5} s\r\nGenerate Vegetables {2:F5} s\r\nGenerate Veins {3:F5} s\r\n", calcPlanet.displayName, duration, duration2, duration3));
                        //             }
                        //         }
                        //
                        //         calcPlanet.NotifyScanEnded();
                        //         processing.Remove(calcPlanet);
                        //     }
                        // }
                        try
                        {
                            if (calcPlanet == null) break;
                            GS2.Warn($"Trying to calculate {calcPlanet.name}");
                            PlanetData unloadedCopy = PlanetData.GetUnloadedCopy(calcPlanet);
                            PlanetAlgorithm planetAlgorithm = Algorithm(unloadedCopy);
                            if (planetAlgorithm != null)
                            {
                                GS2.Log($"Scanning {calcPlanet.name}");
                                HighStopwatch highStopwatch = new HighStopwatch();
                                highStopwatch.Begin();
                                unloadedCopy.data = new PlanetRawData(unloadedCopy.precision);
                                unloadedCopy.modData = unloadedCopy.data.InitModData(calcPlanet.modData);
                                unloadedCopy.data.CalcVerts();
                                unloadedCopy.aux = new PlanetAuxData(unloadedCopy);
                                planetAlgorithm.GenerateTerrain(unloadedCopy.mod_x, unloadedCopy.mod_y);
                                planetAlgorithm.CalcWaterPercent();
                                double duration = highStopwatch.duration;
                                highStopwatch.Begin();
                                unloadedCopy.data.vegeCursor = 1;
                                if (unloadedCopy.type != EPlanetType.Gas)
                                {
                                    planetAlgorithm.GenerateVegetables();
                                }

                                double duration2 = highStopwatch.duration;
                                highStopwatch.Begin();
                                unloadedCopy.data.veinCursor = 1;
                                if (unloadedCopy.type != EPlanetType.Gas)
                                {
                                    planetAlgorithm.GenerateVeins();
                                }

                                unloadedCopy.SummarizeVeinGroups();
                                unloadedCopy.GenBirthPoints();
                                GS2.Log("UnloadedCopy == null? " + (unloadedCopy == null));
                                if (unloadedCopy != null) calcPlanet.CopyScannedDataFrom(unloadedCopy);
                                PlanetData.ReleaseCopy(unloadedCopy);
                                double duration3 = highStopwatch.duration;
                                if (PlanetModelingManager.planetScanThreadLogs != null)
                                {
                                    List<string> list = PlanetModelingManager.planetScanThreadLogs;
                                    lock (list)
                                    {
                                        PlanetModelingManager.planetScanThreadLogs.Add(string.Format("{0}\r\nGenerate Terrain {1:F5} s\r\nGenerate Vegetables {2:F5} s\r\nGenerate Veins {3:F5} s\r\n", new object[] { calcPlanet.displayName, duration, duration2, duration3 }));
                                    }
                                }

                                GS2.Log($"Finished calculating planet {calcPlanet.name}");
                                calcPlanet.NotifyScanEnded();
                            }
                        }
                        catch (Exception ex)
                        {
                            var obj5 = planetScanThreadError;
                            lock (obj5)
                            {
                                if (string.IsNullOrEmpty(planetScanThreadError)) planetScanThreadError = ex.ToString();
                            }

                            GS2.Warn($"Error during calculating planet {calcPlanet.name}: {ex}");
                            calcPlanet.scanned = false;
                            processing.Remove(calcPlanet);
                        }

                        calcPlanet.scanning = false;
                        calcPlanet.scanned = true;
                        if (processing.Contains(calcPlanet)) processing.Remove(calcPlanet);
                    }
                }

                if (calcPlanet == null)
                    Thread.Sleep(50);
                else Thread.Sleep(2);
            }
        }
    }
}