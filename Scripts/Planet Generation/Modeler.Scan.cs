using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static PlanetModelingManager;

namespace GalacticScale
{
    public static partial class Modeler
    {
        public static void Scan()
        {
            Thread thread = null;
            var threadFlagLock = planetProcessingLock;
            lock (threadFlagLock)
            {
                thread = planetScanThread;
            }

            for (;;)
            {
                // GS2.Log("Calc");
                PlanetData calcPlanet = null;
                lock (planetProcessingLock)
                {
                    lock (planetScanThreadFlagLock)
                    {
                        if (thread != planetScanThread)
                        {
                            GS2.Log($"End due to planetCalculateThread mismatch");
                            return;
                        }

                        if (planetScanThreadFlag != ThreadFlag.Running)
                        {
                            GS2.Log("ThreadFlag: Ending => Ended");
                            planetScanThreadFlag = ThreadFlag.Ended;
                            return;
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
                        processing.Add(calcPlanet);
                        try
                        {
                            ScanPlanetGS(calcPlanet);
                        }
                        catch (Exception ex)
                        {
                            lock (planetScanThreadError)
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

        public static void ScanPlanetGS(PlanetData calcPlanet)
        {
            // Note: We don't use vanilla's GetUnloadedCopy method, as it will not get the correct GS2 
            PlanetAlgorithm planetAlgorithm = Algorithm(calcPlanet);

            if (planetAlgorithm != null)
            {
                bool isGS2PlanetAlgo = planetAlgorithm is GS2PlanetAlgorithm;
                GS2.Log($"Scanning {calcPlanet.name}" + (isGS2PlanetAlgo ? " (GS)" : ""));
                HighStopwatch highStopwatch = new HighStopwatch();

                // Calculate landPercent
                highStopwatch.Begin();                                
                calcPlanet.data = new PlanetRawData(calcPlanet.precision);
                calcPlanet.modData = calcPlanet.data.InitModData(calcPlanet.modData);
                calcPlanet.data.CalcVerts();
                calcPlanet.aux = new PlanetAuxData(calcPlanet);
                planetAlgorithm.GenerateTerrain(calcPlanet.mod_x, calcPlanet.mod_y);
                planetAlgorithm.CalcWaterPercent();
                double duration = highStopwatch.duration;

                // Calculate vege
                highStopwatch.Begin();
                calcPlanet.data.vegeCursor = 1;
                if (calcPlanet.type != EPlanetType.Gas) planetAlgorithm.GenerateVegetables();
                double duration2 = highStopwatch.duration;

                // Calculate resource count
                highStopwatch.Begin();
                calcPlanet.data.veinCursor = 1;
                if (calcPlanet.type != EPlanetType.Gas) planetAlgorithm.GenerateVeins();
                calcPlanet.SummarizeVeinGroups();
                calcPlanet.GenBirthPoints();
                CleanPlanetDataForScan(calcPlanet);
                double duration3 = highStopwatch.duration;

                if (planetScanThreadLogs != null)
                {
                    lock (planetScanThreadLogs)
                    {
                        string timerMessage = $"[Terrain]:{duration:F3}s [Vegetables]:{duration2:F3}s [Veins]:{duration3:F3}s  Planet: {calcPlanet.displayName}";
                        planetScanThreadLogs.Add(timerMessage);
                    }
                }

                GS2.Log($"Finished calculating planet {calcPlanet.name}");
                calcPlanet.NotifyScanEnded();
            }
        }

        private static void CleanPlanetDataForScan(PlanetData calcPlanet)
        {
            // Clear data and leave only landPercent, veinGroups and other useful data 
            PlanetData emptyCopy = new();
            emptyCopy.CopyScannedDataFrom(calcPlanet);
            emptyCopy.gasItems = calcPlanet.gasItems; // Those are not include in CopyScannedDataFrom
            emptyCopy.gasSpeeds = calcPlanet.gasSpeeds;
            emptyCopy.gasHeatValues = calcPlanet.gasSpeeds;
            emptyCopy.gasTotalHeat = calcPlanet.gasTotalHeat;
            calcPlanet.Free();
            calcPlanet.CopyScannedDataFrom(emptyCopy);
            calcPlanet.gasItems = emptyCopy.gasItems;
            calcPlanet.gasSpeeds = emptyCopy.gasSpeeds;
            calcPlanet.gasSpeeds = emptyCopy.gasHeatValues;
            calcPlanet.gasTotalHeat = emptyCopy.gasTotalHeat;
            emptyCopy.Free();
        }
    }
}
