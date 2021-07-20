﻿using System.Collections.Generic;

namespace GalacticScale
{
    public static class HandleLocalStarPlanets
    {
        private static string status = "Start";
        private static string lastStatus = "";
        private static StarData closestStar;
        private static PlanetData closestPlanet;
        private static GSPlanet gsPlanet;
        private static readonly Dictionary<PlanetData, double> TransitionRadii = new Dictionary<PlanetData, double>();

        private static void LogStatus(string incoming = "")
        {
            if (incoming != "") status = incoming;

            if (status == lastStatus) return;

            lastStatus = status;
            GS2.Log($"Current Status:{status}");
        }

        private static int GetLoadedPlanetCount(StarData star)
        {
            var planetsLoaded = 0;
            for (var i = 0; i < star.planetCount; i++)
                if (star.planets[i].loaded)
                    planetsLoaded++;
            return planetsLoaded;
        }

        public static string GetStarLoadingStatus(StarData star)
        {
            var planetsLoaded = GetLoadedPlanetCount(star);
            if (star.loaded) return "done".Translate();
            return $"{planetsLoaded}/{star.planetCount}";
        }

        public static bool Update()
        {
            var localStar = GameMain.localStar;
            var localPlanet = GameMain.localPlanet;
            closestStar = localStar;
            closestPlanet = localPlanet;
            var warping = GameMain.mainPlayer.warping;

            if (localStar != null && !localStar.loaded)
            {
                //We assume the star is still loading, so wait.
                LogStatus(
                    $"Star {localStar.name} loading {GetStarLoadingStatus(localStar)} localPlanet:{localPlanet?.name}");
                if (localPlanet != null) EnsurePlanetStillLocal();
                if (localPlanet != null && closestPlanet == null)
                {
                    GS2.Log($"Leaving Planet {localPlanet.name} as it is not the closest planet");
                    GameMain.data.LeavePlanet();
                }

                if (localPlanet == null && closestPlanet == null) // Try and speed up planet acquisition :)~
                {
                    SearchPlanet();
                    if (closestPlanet != null && closestPlanet.loaded)
                    {
                        GS2.Log($"Arriving at Planet {closestPlanet.name}");
                        GameMain.data.ArrivePlanet(closestPlanet);
                        return true;
                    }
                }

                //GS2.Warn($"localPlanet:{localPlanet?.name} closestPlanet:{closestPlanet?.name}");
                return false;
            }

            if (localStar != null && localPlanet != null &&
                (!localPlanet.loaded || !localPlanet.factoryLoaded || localPlanet.loading))
            {
                //We assume the planet is still loading, so wait.
                LogStatus($"Planet  {localPlanet.name} Loading");
                return false;
            }

            if (closestStar != null) EnsureStarStillLocal();

            if (closestStar != null && closestPlanet != null) EnsurePlanetStillLocal();

            if (closestStar == null) SearchStar();

            if (!warping)
            {
                if (closestStar != null && closestPlanet == null) SearchPlanet();
            }
            else
            {
                closestPlanet = null;
            }

            if (closestStar != null && GameMain.data.guideRunning && GameMain.data.guideMission.forceLocalPlanet)
                //Force closestPlanet for prologue use only
                closestPlanet = GameMain.data.guideMission.localPlanet;
            var resetCamera = false;
            if (localStar != null)
            {
                if (localPlanet != null)
                {
                    if (localPlanet != closestPlanet)
                    {
                        resetCamera = true;
                        GS2.Log($"Leaving Planet {localPlanet.name} as it is not closest");
                        GameMain.data.LeavePlanet();
                    }
                }
                else if (closestPlanet != null)
                {
                    resetCamera = true;
                    GS2.Log($"Arriving at Planet {closestPlanet.name}");
                    GameMain.data.ArrivePlanet(closestPlanet);
                }

                if (localStar == closestStar) return resetCamera;

                resetCamera = true;
                GS2.Log($"Leaving Star {localStar.name} as it is not closest");
                GameMain.data.LeaveStar();
            }
            else if (closestStar != null)
            {
                resetCamera = true;
                GS2.Log($"Arriving at Star {closestStar.name}");
                GameMain.data.ArriveStar(closestStar);
            }

            return resetCamera;
        }

        private static void EnsureStarStillLocal()
        {
            LogStatus($"Ensure {closestStar.name} still local...");
            if (!(DistanceTo(closestStar) > TransisionDistance(closestStar))) return;
            GS2.Log(
                $"Leaving star {closestStar.name} as its too far away {DistanceTo(closestStar) / 40000}AU < {TransisionDistance(closestStar) / 40000}AU");
            GameMain.data.LeaveStar();
            closestStar = null;
        }

        private static void EnsurePlanetStillLocal()
        {
            if (!(DistanceTo(closestPlanet) > TransisionDistance(closestPlanet))) return;
            GS2.Log($"Leaving planet {closestPlanet.name} as its too far away");
            closestPlanet = null;
        }

        private static void SearchPlanet()
        {
            for (var i = 0; closestStar != null && closestPlanet == null && i < closestStar.planetCount; i++)
            {
                var planet = closestStar.planets[i];
                if (DistanceTo(planet) < TransisionDistance(planet)) closestPlanet = planet;
            }
        }

        private static void SearchStar()
        {
            for (var i = 0; closestStar == null && i < GameMain.galaxy.starCount; i++)
            {
                var star = GameMain.galaxy.stars[i];
                if (star.planetCount == 0) continue;

                if (GS2.GetGSStar(star).Decorative) continue;

                if (DistanceTo(star) < TransisionDistance(star))
                {
                    GS2.Log($"Found Star {star.name}");
                    closestStar = star;
                }

                if (!GameMain.isRunning || closestStar == null || closestStar.loaded) continue;
                closestStar.Load();
                return;
            }
        }

        private static double DistanceTo(PlanetData planet)
        {
            return (GameMain.mainPlayer.uPosition - planet.uPosition).magnitude - planet.realRadius;
        }

        private static double DistanceTo(StarData star)
        {
            return (GameMain.mainPlayer.uPosition - star.uPosition).magnitude;
        }

        private static double TransisionDistance(StarData star)
        {
            return (star.systemRadius + 2) * 40000;
        }

        /// <summary>
        ///     Calculates a transition distance for a planet, ensuring it is less than the orbit of the first moon, in order to
        ///     allow the player to land on said moon.
        /// </summary>
        /// <param name="planet">The planet to calculate a transition distance for</param>
        /// <returns></returns>
        private static double TransisionDistance(PlanetData planet)
        {
            if (TransitionRadii.ContainsKey(planet)) return TransitionRadii[planet];

            double transitionDistance = planet.realRadius * 2;
            if (gsPlanet == null || gsPlanet.planetData != planet) gsPlanet = GS2.GetGSPlanet(planet);
            if (gsPlanet.MoonsCount > 0)
            {
                var moon = gsPlanet.Moons[0].planetData;
                var distance = (moon.uPosition - planet.uPosition).magnitude - planet.realRadius -
                               TransisionDistance(moon) - 100;
                //GS2.Log($"Distance of {planet.name} is {distance}, transitionDistance is {transitionDistance}");
                if (distance < transitionDistance)
                {
                    LogStatus($"Transition Distance of {planet.name} reduced to {distance}");
                    TransitionRadii.Add(planet, distance);
                    return distance;
                }
            }

            TransitionRadii.Add(planet, transitionDistance);
            return transitionDistance;
        }
    }
}