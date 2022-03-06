using System.Collections.Generic;
using UnityEngine;
using static GalacticScale.GS2;


namespace GalacticScale
{
    public static class HandleLocalStarPlanets
    {
        private static string status = "Start";
        private static string lastStatus = "";
        private static StarData closestStar;
        private static PlanetData closestPlanet;
        public static readonly Dictionary<PlanetData, double> TransitionRadii = new();

        private static void LogStatus(string incoming = "")
        {
            if (incoming != "") status = incoming;

            if (status == lastStatus) return;

            lastStatus = status;
            Log($"Current Status:{status}");
        }

        public static int GetLoadedPlanetCount(StarData star)
        {
            var planetsLoaded = 0;
            for (var i = 0; i < star?.planetCount; i++)
                if (star.planets[i]?.loaded ?? false)
                    planetsLoaded++;
            return planetsLoaded;
        }

        public static string GetStarLoadingStatus(StarData star)
        {
            if (star is null) return "Error :D";
            var planetsLoaded = GetLoadedPlanetCount(star);
            if (star.loaded) return "done".Translate();
            return $"{planetsLoaded}/{star.planetCount}";
        }

        public static bool Update()
        {
            var localStar = GameMain.data.localStar;
            var localPlanet = GameMain.data.localPlanet;
            closestStar = localStar;
            closestPlanet = localPlanet;
            if (localPlanet != null && VFInput.shift && VFInput.alt)
            {
                var radii = TransitionRadii.ContainsKey(localPlanet) ? TransitionRadii[localPlanet].ToString() : "N/A";

                Warn($"DistanceTo: {DistanceTo(localPlanet)} / {radii}");
            }

            var warping = GameMain.mainPlayer.warping;

            if (localStar != null && !localStar.loaded)
            {
                EnsureStarStillLocal();
                if (closestStar == null)
                {
                    localStar.Unload();
                    LogStatus("Left Star...Searching");
                    SearchStar();
                }

                //We assume the star is still loading, so wait.
                LogStatus($"Star {localStar.name} loading {GetStarLoadingStatus(localStar)} localPlanet:{localPlanet?.name}");
                if (localPlanet != null) EnsurePlanetStillLocal();
                if (localPlanet != null && closestPlanet == null)
                {
                    LogStatus($"Leaving Planet {localPlanet.name} as it is not the closest planet");
                    GameMain.data.LeavePlanet();
                }

                if (localPlanet == null && closestPlanet == null) // Try and speed up planet acquisition :)~
                {
                    SearchPlanet();
                    if (closestPlanet != null && closestPlanet.loaded)
                    {
                        LogStatus($"Arriving at Planet {closestPlanet.name}");
                        GameMain.data.ArrivePlanet(closestPlanet);
                        return true;
                    }
                }

                //GS2.Log($"localPlanet:{localPlanet?.name} closestPlanet:{closestPlanet?.name}");
                return false;
            }

            if (localStar != null && localPlanet != null && (!localPlanet.loaded || !localPlanet.factoryLoaded || localPlanet.loading))
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
                        LogStatus($"Leaving Planet {localPlanet.name} as it is not closest");
                        GameMain.data.LeavePlanet();
                        GameMain.mainPlayer.NotifyLocalAstroChange();
                    }
                }
                else if (closestPlanet != null)
                {
                    resetCamera = true;
                    LogStatus($"Arriving at Planet {closestPlanet.name}");
                    GameMain.data.ArrivePlanet(closestPlanet);
                }

                if (localStar == closestStar) return resetCamera;

                resetCamera = true;
                LogStatus($"Leaving Star {localStar.name} as it is not closest");
                if (GameMain.data.localStar != null) GameMain.data.LeaveStar();
            }
            else if (closestStar != null)
            {
                resetCamera = true;
                LogStatus($"Arriving at Star {closestStar.name}");
                GameMain.data.ArriveStar(closestStar);
            }

            return resetCamera;
        }

        private static void EnsureStarStillLocal()
        {
            if (closestStar.loaded) LogStatus($"Ensure {closestStar.name} still local...");
            if (!(DistanceTo(closestStar) > TransisionDistance(closestStar))) return;
            // GS2.Log(
            //     $"Leaving star {closestStar.name} as its too far away {DistanceTo(closestStar) / 40000}AU < {TransisionDistance(closestStar) / 40000}AU");
            GameMain.data.LeaveStar();
            closestStar = null;
        }

        private static void EnsurePlanetStillLocal()
        {
            // GS2.Log($"{DistanceTo(closestPlanet)} > {TransisionDistance(closestPlanet)}?");
            if (!(DistanceTo(closestPlanet) > TransisionDistance(closestPlanet))) return;

            closestPlanet = null;
        }

        private static void SearchPlanet()
        {
            for (var i = 0; closestStar != null && closestPlanet == null && i < closestStar.planetCount; i++)
            {
                var planet = closestStar.planets[i];
                if (DistanceTo(planet) < TransisionDistance(planet))
                {
                    // GS2.Log($"Switching to {planet.name}");
                    closestPlanet = planet;
                    break;
                }
            }
        }

        private static void SearchStar()
        {
            for (var i = 0; closestStar == null && i < GameMain.galaxy.starCount; i++)
            {
                var star = GameMain.galaxy.stars[i];
                if (star.planetCount == 0) continue;

                if (GetGSStar(star).Decorative) continue;

                if (DistanceTo(star) < TransisionDistance(star))
                {
                    Log($"Found Star {star.name}");
                    closestStar = star;
                }

                if (!GameMain.isRunning || closestStar == null || closestStar.loaded) continue;
                closestStar.Load();
                return;
            }
        }

        public static double DistanceTo(PlanetData planet)
        {
            // GS2.Log((GameMain.mainPlayer.uPosition - planet.uPosition).magnitude.ToString());
            return (GameMain.mainPlayer.uPosition - planet.uPosition).magnitude - planet.realRadius;
        }

        public static double DistanceTo(StarData star)
        {
            return (GameMain.mainPlayer.uPosition - star.uPosition).magnitude;
        }

        private static double TransisionDistance(StarData star)
        {
            return (star.systemRadius + 2) * 40000;
        }
        //
        // private static void CheckTransitionDistanceOfMoon(GSPlanet moon)
        // {
        //     Log($"Checking TransitionDistanceOfMoon {moon.Name}");
        //     var currentDistance = TransitionRadii[moon.planetData];
        //     var host = moon.planetData.orbitAroundPlanet;
        //     if (host.orbitAroundPlanet == null) return; //If the host is the main planet return
        //     var gsHost = GetGSPlanet(host);
        //
        //     //Ensure the transitiondistance doesn't interfere with the host planet
        //     var distanceBetweenMoonAndHost = moon.OrbitRadius * 40000f;
        //     // var distanceBetweenSurfaces = distanceBetweenMoonAndHost - host.realRadius - moon.planetData.realRadius;
        //     var calcDistanceForMoon = distanceBetweenMoonAndHost / 2 - 100f;
        //     if (calcDistanceForMoon < currentDistance)
        //         //GS2.Log($"1 Adjusting TransitionDistance of {moon.Name} to {calcDistanceForMoon}");
        //         TransitionRadii[moon.planetData] = calcDistanceForMoon;
        //     if (gsHost.MoonCount > 1)
        //     {
        //         //Ensure the transitiondistance doesn't interfere with the other moons
        //         var index = gsHost.Moons.IndexOf(moon);
        //         if (index > 0)
        //         {
        //             var prevMoon = gsHost.Moons[index - 1];
        //             var prvMoonSystemOrbit = (prevMoon.OrbitRadius + prevMoon.SystemRadius) * 40000f;
        //             var differenceBetweenPreviousMoon = moon.OrbitRadius - prvMoonSystemOrbit;
        //             calcDistanceForMoon = differenceBetweenPreviousMoon / 2 - 100f;
        //             if (calcDistanceForMoon < currentDistance)
        //                 //GS2.Log($"2 Adjusting TransitionDistance of {moon.Name} to {calcDistanceForMoon}");
        //                 TransitionRadii[moon.planetData] = calcDistanceForMoon;
        //         }
        //
        //         if (index < gsHost.MoonCount - 1)
        //         {
        //             //I dont think this should ever be called? Maybe if I use this function for moons that aren't most distant satellite
        //             var nextMoon = gsHost.Moons[index + 1];
        //             var nextMoonSystemOrbit = (nextMoon.SystemRadius - nextMoon.OrbitRadius) * 40000f;
        //             var differenceBetweenNextMoon = nextMoonSystemOrbit - moon.OrbitRadius;
        //             calcDistanceForMoon = differenceBetweenNextMoon / 2 - 100f;
        //             if (calcDistanceForMoon < currentDistance)
        //                 GS2.Log($"3 Adjusting TransitionDistance of {moon.Name} to {calcDistanceForMoon}");
        //                 // TransitionRadii[moon.planetData] = calcDistanceForMoon;
        //         }
        //     }
        // }

        /// <summary>
        ///     Calculates a transition distance for a planet, ensuring it is less than the orbit of the first moon, in order to
        ///     allow the player to land on said moon.
        /// </summary>
        /// <param name="planet">The planet to calculate a transition distance for</param>
        /// <returns></returns>
        private static double TransisionDistance(PlanetData planet)
        {
            if (TransitionRadii.ContainsKey(planet)) return TransitionRadii[planet];
            var gsPlanet = GetGSPlanet(planet);
            var transitionDistance = Mathf.Clamp(planet.realRadius * 2, 1, 1000); //Most Simple Transition Distance. Clamped to 1000m off the surface.
            //
            //   The distances between objects we need to check to calculate a body's transition radius
            //   Star
            //     - Planet A             - Needs to check FirstMoon.OrbitInnermostSystemRadiusAU-RadiusAU && (NextSibling.OrbitInnermostSystemRadiusAU) - Self.OrbitOutermostSurfaceRadiusAU
            //       - Moon A1            - Needs to check (OrbitRadius - Radius - Host.Radius) && (FirstMoon.OrbitRadius - FirstMoon.SystemRadius)
            //         - SubMoon A1c
            //         - SubMoon A1b
            //       - Moon A2
            //         - SubMoon A2a
            //         - SubMoon A2b
            //         - SubMoon A2c
            //     - Planet B             - Closest Items are First Moons Most Distant Satellite. PreviousSiblings Most Distant Satellite (if no self moons). NextSiblings Most Distant Sattelite (if no self moons)
            //       - Moon B1            - Needs to check (OrbitRadius - Radius - Host.Radius) && (FirstMoon.OrbitRadius - FirstMoon.SystemRadius)
            //         - SubMoon B1c
            //         - SubMoon B1b
            //       - Moon B2            - Closest Items are First Moons Most Distant Satellite. PreviousSiblings Most Distant Satellite (if no self moons). NextSiblings Most Distant Sattelite (if no self moons). Host surcface if no previous sibling.
            //         - SubMoon B2a
            //         - SubMoon B2b
            //         - SubMoon B2c
            //        
            // Check: If (Moons >0) First Child's Inner System Radius - Self.RadiusAU
            // Check: if (Moons == 0) {
            //    Self.OrbitInnermostSurfaceRadiusAU - Host.RadiusAU
            //    Previous Sibling exists, OrbitInnermostSurfaceRadiusAU - PreviousSibling.OrbitOutermostSystemRadiusAU
            //    No Moons, Next Sibling Exists: NextSibling.OrbitInnermostSystemRadiusAU - OPrbitOutermostSurfaceRadiusAU
            //  }
            if (gsPlanet.MoonCount > 0) //If this has a moon
            {
                //First Child's Inner System Radius - Self.RadiusAU
                var distanceAU = gsPlanet.Moons[0].OrbitInnermostSystemRadiusAU - gsPlanet.RadiusAU;
                Log($"Distance to first moons Last Satellite's Surface from {planet.name} surface is {distanceAU * 40000f}");
                if (distanceAU * 20000f - 100f < transitionDistance)
                {
                    Log($"Changed Transition Distance for {planet.name} from {transitionDistance} to {distanceAU * 20000f - 100f}");
                    transitionDistance = distanceAU * 20000f - 100f;
                }
            }
            else if (planet.orbitAroundPlanet != null) //If this is a moon
            {
                var Host = GetGSPlanet(planet.orbitAroundPlanet);
                if (Host.MoonCount > 1 && Host.Moons.IndexOf(gsPlanet) > 0)
                {
                    //OrbitInnermostSurfaceRadiusAU - PreviousSibling.OrbitOutermostSystemRadiusAU
                    var index = Host.Moons.IndexOf(gsPlanet);
                    var PreviousSibling = Host.Moons[index - 1];
                    var distanceAU = gsPlanet.OrbitInnermostSurfaceRadiusAU - PreviousSibling.OrbitOutermostSystemRadiusAU;
                    Log($"Distance to previous siblings Last Satellite's Surface from {planet.name} surface is {distanceAU * 40000f}");
                    if (distanceAU * 20000f - 100f < transitionDistance)
                    {
                        Log($"Changed Transition Distance for {planet.name} from {transitionDistance} to {distanceAU * 20000f - 100f}");
                        transitionDistance = distanceAU * 20000f - 100f;
                    }
                }
                else if (Host.MoonCount > 1)
                {
                    // Check Self.OrbitInnermostSurfaceRadiusAU - Host.RadiusAU
                    var distanceAU = gsPlanet.OrbitInnermostSurfaceRadiusAU - Host.RadiusAU;
                    Log($"Distance to hosts surface from {planet.name} surface is {distanceAU * 40000f}");
                    if (distanceAU * 20000f - 100f < transitionDistance)
                    {
                        Log($"Changed Transition Distance for {planet.name} from {transitionDistance} to {distanceAU * 20000f - 100f}");
                        transitionDistance = distanceAU * 20000f - 100f;
                    }
                }

                if (Host.MoonCount > 1 && Host.Moons.IndexOf(gsPlanet) < Host.Moons.Count - 1)
                {
                    var index = Host.Moons.IndexOf(gsPlanet);
                    var NextSibling = Host.Moons[index + 1];
                    var distanceAU = NextSibling.OrbitInnermostSystemRadiusAU - gsPlanet.OrbitOutermostSurfaceRadiusAU;
                    Log($"Distance to Next Siblings Last Satellite's Surface from {planet.name} surface is {distanceAU * 40000f}");
                    if (distanceAU * 20000f - 100f < transitionDistance)
                    {
                        Log($"Changed Transition Distance for {planet.name} from {transitionDistance} to {distanceAU * 20000f - 100f}");
                        transitionDistance = distanceAU * 20000f - 100f;
                    }
                    //NextSibling.OrbitInnermostSystemRadiusAU - OPrbitOutermostSurfaceRadiusAU
                }
            }
            else if (planet.orbitAroundPlanet == null) //If this is a planet
            {
                var Host = GetGSStar(planet.star);
                if (Host.PlanetCount > 1 && Host.Planets.IndexOf(gsPlanet) > 0)
                {
                    //OrbitInnermostSurfaceRadiusAU - PreviousSibling.OrbitOutermostSystemRadiusAU
                    var index = Host.Planets.IndexOf(gsPlanet);
                    var PreviousSibling = Host.Planets[index - 1];
                    if (PreviousSibling.OrbitRadius != gsPlanet.OrbitRadius)
                    {
                        var distanceAU = gsPlanet.OrbitInnermostSurfaceRadiusAU - PreviousSibling.OrbitOutermostSystemRadiusAU;
                        Log($"Distance to previous siblings Last Satellite's Surface from {planet.name} surface is {distanceAU * 40000f} where PreviousSibling is {PreviousSibling.Name} and OrbitOutermostSystemRadiusAU is {PreviousSibling.OrbitOutermostSystemRadiusAU}");
                        if (distanceAU * 20000f - 100f < transitionDistance)
                        {
                            Log($"Changed Transition Distance for {planet.name} from {transitionDistance} to {distanceAU * 20000f - 100f}");
                            transitionDistance = distanceAU * 20000f - 100f;
                        }
                    }
                }
                else if (Host.PlanetCount > 1)
                {
                    // Check Self.OrbitInnermostSurfaceRadiusAU - Host.RadiusAU

                    var distanceAU = gsPlanet.OrbitInnermostSurfaceRadiusAU - Host.RadiusAU;
                    Log($"Distance to hosts surface from {planet.name} surface is {distanceAU * 40000f}");
                    if (distanceAU * 20000f - 100f < transitionDistance)
                    {
                        Log($"Changed Transition Distance for {planet.name} from {transitionDistance} to {distanceAU * 20000f - 100f}");
                        transitionDistance = distanceAU * 20000f - 100f;
                    }
                }

                if (Host.PlanetCount > 1 && Host.Planets.IndexOf(gsPlanet) < Host.Planets.Count - 1)
                {
                    var index = Host.Planets.IndexOf(gsPlanet);
                    var NextSibling = Host.Planets[index + 1];
                    if (NextSibling.OrbitRadius != gsPlanet.OrbitRadius)
                    {
                        var distanceAU = NextSibling.OrbitInnermostSystemRadiusAU - gsPlanet.OrbitOutermostSurfaceRadiusAU;
                        Log($"Distance to Next Siblings Last Satellite's Surface from {planet.name} surface is {distanceAU * 40000f}");
                        //NextSibling.OrbitInnermostSystemRadiusAU - OPrbitOutermostSurfaceRadiusAU
                        if (distanceAU * 20000f - 100f < transitionDistance)
                        {
                            Log($"Changed Transition Distance for {planet.name} from {transitionDistance} to {distanceAU * 20000f - 100f}");
                            transitionDistance = distanceAU * 20000f - 100f;
                        }
                    }
                }
            }

            if (transitionDistance < 0) //(planet.realRadius + 10))
            {
                Warn($"changing {planet.name} transition distance from {transitionDistance} to {planet.realRadius + 10}");
                transitionDistance = planet.realRadius + 10;
            }

            if (!TransitionRadii.ContainsKey(planet)) TransitionRadii.Add(planet, transitionDistance);
            else
                Warn("ALREADY CONTAINS DISTANCE");
            Log($"Transition Radius: {transitionDistance} for {planet.name} with radius {planet.realRadius}");
            return transitionDistance;
        }
    }
}