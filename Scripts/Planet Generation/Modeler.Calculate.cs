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
            var obj2 = planetCalculateThreadFlagLock;
            lock (obj2)
            {
                obj = planetCalculateThread;
            }

            for (;;)
            {
                calcPlanet = null;
                var num = 0;
                obj2 = planetCalculateThreadFlagLock;
                lock (obj2)
                {
                    if (planetCalculateThreadFlag != ThreadFlag.Running)
                    {
                        planetCalculateThreadFlag = ThreadFlag.Ended;

                        break;
                    }

                    if (obj != planetCalculateThread) break;
                }

                
                var obj3 = calPlanetReqList;
                lock (obj3)
                {
                    if (calPlanetReqList.Count > 0) calcPlanet = calPlanetReqList.Dequeue();
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
                            if (calcPlanet == null || calcPlanet.data == null) return;                            calcPlanet.modData = calcPlanet.data.InitModData(calcPlanet.modData);
                            if (calcPlanet == null || calcPlanet.data == null) return;                            calcPlanet.data.CalcVerts();
                            if (calcPlanet == null || calcPlanet.data == null) return;                            calcPlanet.aux = new PlanetAuxData(calcPlanet);
                            if (calcPlanet == null || calcPlanet.data == null || planetAlgorithm == null) return;                            planetAlgorithm.GenerateTerrain(calcPlanet.mod_x, calcPlanet.mod_y);
                            if (calcPlanet == null || calcPlanet.data == null || planetAlgorithm == null)
                            {
                                GS2.Log("Aborted");
                            } else planetAlgorithm.CalcWaterPercent();
                            if (calcPlanet == null || calcPlanet.data == null) return;                            var duration = highStopwatch.duration;
                            highStopwatch.Begin();
                            if (calcPlanet == null || calcPlanet.data == null || planetAlgorithm == null) return;                            if (calcPlanet.type != EPlanetType.Gas) planetAlgorithm.GenerateVegetables();
                            if (calcPlanet == null || calcPlanet.data == null) return;                            var duration2 = highStopwatch.duration;
                            highStopwatch.Begin();
                            if (calcPlanet == null || calcPlanet.data == null || planetAlgorithm == null) return;                            if (calcPlanet.type != EPlanetType.Gas) planetAlgorithm.GenerateVeins();
                            if (calcPlanet == null || calcPlanet.data == null) return;                            calcPlanet.CalculateVeinGroups();
                            if (calcPlanet == null || calcPlanet.data == null) return;                            calcPlanet.GenBirthPoints();
                            if (calcPlanet == null || calcPlanet.data == null) return;
                            var duration3 = highStopwatch.duration;
                            if (planetCalculateThreadLogs != null)
                            {
                                var obj4 = planetCalculateThreadLogs;
                                lock (obj4)
                                {
                                    planetCalculateThreadLogs.Add(string.Format("{0}\r\nGenerate Terrain {1:F5} s\r\nGenerate Vegetables {2:F5} s\r\nGenerate Veins {3:F5} s\r\n", calcPlanet.displayName, duration, duration2, duration3));
                                }
                            }
                            
                            calcPlanet.NotifyCalculated();
                            processing.Remove(calcPlanet);
                        }
                    }
                    catch (Exception ex)
                    {
                        var obj5 = planetCalculateThreadError;
                        lock (obj5)
                        {
                            if (string.IsNullOrEmpty(planetCalculateThreadError)) planetCalculateThreadError = ex.ToString();
                        }

                        calcPlanet.calculated = false;
                        processing.Remove(calcPlanet);

                    }

                    calcPlanet.calculating = false;
                    if (processing.Contains( calcPlanet)) processing.Remove(calcPlanet);

                }

                if (calcPlanet == null)
                    Thread.Sleep(50);
                else if (num % 20 == 0) Thread.Sleep(2);
            }
        }
    }
}