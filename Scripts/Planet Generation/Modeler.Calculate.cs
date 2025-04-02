using System;
using System.Threading;
using UnityEngine;
using static PlanetModelingManager;

namespace GalacticScale
{
    public static partial class Modeler
    {
        static PlanetData calcPlanet = null;
        public static void Calculate()
        {
            GS2.Log("Calc");
            object obj = null;
            var obj2 = planetScanThreadFlagLock;
            lock (obj2)
            {
                obj = planetScanThreadFlag;
            }

            for (;;)
            {
                calcPlanet = null;
                lock (planetProcessingLock)
                {
                    obj2 = planetScanThreadFlagLock;
                    lock (obj2)
                    {
                        if (planetScanThreadFlag != ThreadFlag.Running)
                        {
                            planetScanThreadFlag = ThreadFlag.Ended;
                            break;
                        }

                        if (obj != planetScanThread)
                        {
                            GS2.Warn("End due to planetCalculateThread mismatch");
                            break;
                        }
                    }


                    var obj3 = scnPlanetReqList;
                    lock (obj3)
                    {
                        if (scnPlanetReqList.Count > 0) calcPlanet = scnPlanetReqList.Dequeue();
                    }

                    if (calcPlanet != null)
                    {
                        processing.Add(calcPlanet);
                        try
                        {
                            var planetAlgorithm = Algorithm(calcPlanet);
                            if (planetAlgorithm != null)
                            {
                                var highStopwatch = new HighStopwatch();
                                highStopwatch.Begin();
                                calcPlanet.data = new PlanetRawData(calcPlanet.precision);
                                if (calcPlanet == null || calcPlanet.data == null) return; calcPlanet.modData = calcPlanet.data.InitModData(calcPlanet.modData);
                                if (calcPlanet == null || calcPlanet.data == null) return; calcPlanet.data.CalcVerts();
                                if (calcPlanet == null || calcPlanet.data == null) return; calcPlanet.aux = new PlanetAuxData(calcPlanet);
                                if (calcPlanet == null || calcPlanet.data == null || planetAlgorithm == null) return; planetAlgorithm.GenerateTerrain(calcPlanet.mod_x, calcPlanet.mod_y);
                                if (calcPlanet == null || calcPlanet.data == null || planetAlgorithm == null)
                                {
                                    GS2.Log("Aborted");
                                }
                                else planetAlgorithm.CalcWaterPercent();
                                if (calcPlanet == null || calcPlanet.data == null) return; var duration = highStopwatch.duration;
                                highStopwatch.Begin();
                                if (calcPlanet == null || calcPlanet.data == null || planetAlgorithm == null) return; if (calcPlanet.type != EPlanetType.Gas) planetAlgorithm.GenerateVegetables();
                                if (calcPlanet == null || calcPlanet.data == null) return; var duration2 = highStopwatch.duration;
                                highStopwatch.Begin();
                                if (calcPlanet == null || calcPlanet.data == null || planetAlgorithm == null) return; if (calcPlanet.type != EPlanetType.Gas) planetAlgorithm.GenerateVeins();
                                if (calcPlanet == null || calcPlanet.data == null) return; calcPlanet.SummarizeVeinGroups();
                                if (calcPlanet == null || calcPlanet.data == null) return; calcPlanet.GenBirthPoints();
                                if (calcPlanet == null || calcPlanet.data == null) return;
                                var duration3 = highStopwatch.duration;
                                if (planetScanThreadLogs != null)
                                {
                                    var obj4 = planetScanThreadLogs;
                                    lock (obj4)
                                    {
                                        planetScanThreadLogs.Add(string.Format("{0}\r\nGenerate Terrain {1:F5} s\r\nGenerate Vegetables {2:F5} s\r\nGenerate Veins {3:F5} s\r\n", calcPlanet.displayName, duration, duration2, duration3));
                                    }
                                }

                                calcPlanet.NotifyScanEnded();
                                processing.Remove(calcPlanet);
                            }
                        }
                        catch (Exception ex)
                        {
                            var obj5 = planetScanThreadError;
                            lock (obj5)
                            {
                                if (string.IsNullOrEmpty(planetScanThreadError)) planetScanThreadError = ex.ToString();
                            }

                            calcPlanet.scanned = false;
                            processing.Remove(calcPlanet);

                        }

                        calcPlanet.scanning = false;
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