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
            lock (planetProcessingLock)
            {
                scnPlanetReqList ??= new Queue<PlanetData>(100);
                genPlanetReqList ??= new Queue<PlanetData>(100);
                modPlanetReqList ??= new Queue<PlanetData>(100);
                fctPlanetReqList ??= new Queue<PlanetData>(100);
                lock (scnPlanetReqList)
                    while (scnPlanetReqList.Count > 0) scnPlanetReqList.Dequeue().scanning = false;
                lock (genPlanetReqList) genPlanetReqList.Clear();
                lock (modPlanetReqList) modPlanetReqList.Clear();
                lock (fctPlanetReqList) fctPlanetReqList.Clear();
                planetModQueue.Clear();
                planetModQueueSorted = false;
                planetQueue.Clear();
                planetQueueSorted = false;
            }
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

        private static readonly Dictionary<int, int> modelingFailures = new();
        private const int MaxModelingRetries = 2;

        public static void AbortModeling(PlanetData planet)
        {
            if (tmpMeshList != null)
                foreach (var mesh in tmpMeshList)
                    if (mesh != null) UnityEngine.Object.Destroy(mesh);
            if (tmpPlanetGameObject != null) UnityEngine.Object.Destroy(tmpPlanetGameObject);
            tmpPlanetGameObject = null;
            tmpPlanetBodyGameObject = null;
            tmpPlanetReformGameObject = null;
            tmpPlanetReformRenderer = null;
            tmpOceanCollider = null;
            tmpMeshList?.Clear();
            tmpMeshRendererList?.Clear();
            tmpMeshColliderList?.Clear();
            tmpVerts?.Clear();
            tmpNorms?.Clear();
            tmpTgnts?.Clear();
            tmpUvs?.Clear();
            tmpUv2s?.Clear();
            tmpTris?.Clear();

            currentModelingPlanet = null;
            currentModelingStage = 0;
            currentModelingSeamNormal = 0;
            if (planet == null) return;

            planet.loading = false;
            planet.Unload();
            planet.factoryLoaded = false;
            if (planet.star != GameMain.localStar) return;
            modelingFailures.TryGetValue(planet.id, out var failures);
            if (failures >= MaxModelingRetries)
            {
                Warn($"Not re-requesting {planet.name}: modeling failed {failures} times already");
                return;
            }

            modelingFailures[planet.id] = failures + 1;
            Warn($"Re-requesting {planet.name} after failed modeling (attempt {failures + 1}/{MaxModelingRetries})");
            planet.Load();
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
                    ModelingPlanetMain(currentModelingPlanet);
                }
                catch (Exception message)
                {
                    Error(message.ToString());
                    AbortModeling(currentModelingPlanet);
                }
        }
    }
}
