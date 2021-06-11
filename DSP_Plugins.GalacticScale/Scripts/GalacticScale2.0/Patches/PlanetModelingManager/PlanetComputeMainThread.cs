using System;
using System.Threading;
using HarmonyLib;
using static PlanetModelingManager;

namespace GalacticScale
{
    public partial class PatchOnPlanetModelingManager
    {
        [HarmonyPrefix, HarmonyPatch(typeof(PlanetModelingManager), "PlanetComputeThreadMain")]
        public static bool PlanetComputeThreadMain(ThreadFlag ___planetComputeThreadFlag, ThreadFlagLock ___planetComputeThreadFlagLock, Thread ___planetComputeThread)
        {
            object obj = null;
            lock (planetComputeThreadFlagLock)
            {
                obj = planetComputeThread;
            }
            while (true)
            {
                int num = 0;
                lock (planetComputeThreadFlagLock)
                {
                    if (planetComputeThreadFlag != ThreadFlag.Running)
                    {
                        planetComputeThreadFlag = ThreadFlag.Ended;
                        return false;
                    }
                    if (obj != planetComputeThread)
                    {
                        return false;
                    }
                }
                PlanetData planetData = null;
                lock (genPlanetReqList)
                {
                    if (genPlanetReqList.Count > 0)
                    {
                        planetData = genPlanetReqList.Dequeue();
                        while (planetData.star != GameMain.localStar && (genPlanetReqList.Count > GameMain.localStar.planetCount || !Utils.PlanetInStar(planetData, GameMain.localStar)) && Utils.ContainsLocalStarPlanet(genPlanetReqList)) //Added check to see if we are trying to load a planet we don't care about
                        {
                            GS2.Log($"Skipping generation of {planetData.name} as there are local planets waiting");
                            genPlanetReqList.Enqueue(planetData); //Put it to back of the list
                            planetData = genPlanetReqList.Dequeue(); //Get another and try again
                        }
                    }
                }
                if (planetData != null)
                {
                    try
                    {
                        PlanetAlgorithm planetAlgorithm = PlanetModelingManager.Algorithm(planetData);
                        if (planetAlgorithm != null)
                        {
                            HighStopwatch highStopwatch = new HighStopwatch();
                            double num2 = 0.0;
                            double num3 = 0.0;
                            double num4 = 0.0;
                            if (planetData.data == null)
                            {
                                highStopwatch.Begin();
                                planetData.data = new PlanetRawData(planetData.precision);
                                planetData.modData = planetData.data.InitModData(planetData.modData);
                                planetData.data.CalcVerts();
                                planetData.aux = new PlanetAuxData(planetData);
                                planetAlgorithm.GenerateTerrain(planetData.mod_x, planetData.mod_y);
                                planetAlgorithm.CalcWaterPercent();
                                num2 = highStopwatch.duration;
                            }
                            if (planetData.factory == null)
                            {
                                highStopwatch.Begin();
                                if (planetData.type != EPlanetType.Gas)
                                {
                                    planetAlgorithm.GenerateVegetables();
                                }
                                num3 = highStopwatch.duration;
                                highStopwatch.Begin();
                                if (planetData.type != EPlanetType.Gas)
                                {
                                    planetAlgorithm.GenerateVeins(sketchOnly: false);
                                }
                                num4 = highStopwatch.duration;
                            }
                            if (planetComputeThreadLogs != null)
                            {
                                lock (planetComputeThreadLogs)
                                {
                                    planetComputeThreadLogs.Add($"{planetData.displayName}\r\nGenerate Terrain {num2:F5} s\r\nGenerate Vegetables {num3:F5} s\r\nGenerate Veins {num4:F5} s\r\n");
                                    GS2.Log($"{planetData.displayName}\r\nGenerate Terrain {num2:F5} s\r\nGenerate Vegetables {num3:F5} s\r\nGenerate Veins {num4:F5} s\r\n");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        lock (planetComputeThreadError)
                        {
                            if (string.IsNullOrEmpty(planetComputeThreadError))
                            {
                                planetComputeThreadError = ex.ToString();
                            }
                        }
                    }
                    lock (modPlanetReqList)
                    {
                        modPlanetReqList.Enqueue(planetData);
                    }
                }
                if (planetData == null)
                {
                    Thread.Sleep(50);
                }
                else if (num % 20 == 0)
                {
                    Thread.Sleep(2);
                }
            }
        }
    }
}