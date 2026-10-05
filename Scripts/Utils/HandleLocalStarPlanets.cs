using System.Collections.Generic;
using UnityEngine;
using static GalacticScale.GS2;


namespace GalacticScale
{
    public static class HandleLocalStarPlanets
    {
        private const double PlanetPreloadDistance = 80000.0;
        private static string status = "Start";
        private static string lastStatus = "";
        private static StarData closestStar;
        private static PlanetData closestPlanet;
        private static PlanetData legacyOverlapMoon;
        private static PlanetData legacyOverlapGasGiant;
        public static readonly Dictionary<PlanetData, double> TransitionRadii = new();

        static HandleLocalStarPlanets()
        {
        }

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

            if (legacyOverlapGasGiant != null &&
                DistanceTo(legacyOverlapGasGiant) > ApproachDistance(legacyOverlapGasGiant))
            {
                legacyOverlapMoon = null;
                legacyOverlapGasGiant = null;
            }

            var factoryLoadingPlanet = PlanetModelingManager.currentFactingPlanet;
            if (factoryLoadingPlanet != null && factoryLoadingPlanet.factoryLoading)
            {
                LogStatus($"Waiting for {factoryLoadingPlanet.name} factory load before changing locality");
                return false;
            }

            if (localStar != null && localStar.loaded) PreloadNearestPlanet(localStar);

            closestStar = localStar;
            closestPlanet = localPlanet;
            if (localPlanet != null && VFInput.shift && VFInput.alt && Config.DevMode)
            {
                var radii = TransitionRadii.ContainsKey(localPlanet) ? TransitionRadii[localPlanet].ToString() : "N/A";

                
                    GS2.debugtool.Label = "Transition Distance";
                    GS2.debugtool.Value = radii; //alt ctrl L
                
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
                var approachingPlanet = FindPlanetInApproach(localStar);
                if (approachingPlanet != null && approachingPlanet != localPlanet &&
                    approachingPlanet.loaded && approachingPlanet.factoryLoaded &&
                    !IsPreferredOver(localPlanet, approachingPlanet) &&
                    DistanceTo(approachingPlanet) + 1000.0 < DistanceTo(localPlanet))
                {
                    Log($"Leaving loading planet {localPlanet.name} for ready approaching planet {approachingPlanet.name}");
                    GameMain.data.LeavePlanet();
                    GameMain.mainPlayer.NotifyLocalAstroChange();
                    GameMain.data.ArrivePlanet(approachingPlanet);
                    return true;
                }

                //We assume the planet is still loading, so wait.
                LogStatus($"Planet  {localPlanet.name} Loading");
                return false;
            }

            if (closestStar != null) EnsureStarStillLocal();

            if (closestStar != null && closestPlanet != null)
            {
                EnsurePlanetStillLocal();

                if (closestPlanet != null)
                {
                    var approachingPlanet = FindPlanetInApproach(closestStar);
                    if (approachingPlanet != null && approachingPlanet != closestPlanet &&
                        (IsPreferredOver(approachingPlanet, closestPlanet) ||
                         (!IsPreferredOver(closestPlanet, approachingPlanet) &&
                          DistanceTo(approachingPlanet) + 1000.0 < DistanceTo(closestPlanet))))
                    {
                        Log($"Switching local planet from {closestPlanet.name} to approaching {approachingPlanet.name}");
                        closestPlanet = approachingPlanet;
                    }
                }
            }

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
                if (Config.DebugMode) Utils.LogDFInfo(closestStar);
                GameMain.data.ArriveStar(closestStar);
            }

            return resetCamera;
        }

        private static void EnsureStarStillLocal()
        {
            if (closestStar.loaded) LogStatus($"Ensure {closestStar.name} still local...");
            if (DistanceTo(closestStar) > TransitionDistance(closestStar))
            {
                Log($"Leaving star {closestStar.name} as its too far away {DistanceTo(closestStar) / 40000}AU < {TransitionDistance(closestStar) / 40000}AU");
                GameMain.data.LeaveStar();
                closestStar = null;
                return;
            }

            // Overlapping systems (#294): GS2 orbits can be wide enough that a planet of a
            // NEIGHBORING star is right here while the current star's own bodies are dozens of
            // AU away. The current star's transition sphere still contains the player, so the
            // exit check above never fires and the neighbor's planet can never load (invisible,
            // no collider, factory never mounts). Yield only on decisive contact: the player is
            // inside the transition sphere of a planet belonging to another eligible star.
            var foreign = TouchedPlanetOfOtherStar();
            if (foreign != null)
            {
                Log($"Leaving star {closestStar.name}: in contact with {foreign.name} of {foreign.star.name} (overlapping systems)");
                GameMain.data.LeaveStar();
                closestStar = null;
            }
        }

        /// <summary>
        ///     Returns a planet of a different, locality-eligible star whose transition sphere
        ///     contains the player, or null. Only stars whose own system sphere contains the
        ///     player are examined, so in non-overlapping space this is a single distance check.
        /// </summary>
        private static PlanetData TouchedPlanetOfOtherStar()
        {
            var stars = GameMain.galaxy?.stars;
            if (stars == null) return null;

            // Hysteresis: while still in approach of a body we already own, never yield.
            if (closestStar?.planets != null)
                for (var j = 0; j < closestStar.planetCount; j++)
                {
                    var planet = closestStar.planets[j];
                    if (planet != null && DistanceTo(planet) < ApproachDistance(planet))
                        return null;
                }

            PlanetData best = null;
            var bestDist = double.MaxValue;
            for (var i = 0; i < GameMain.galaxy.starCount; i++)
            {
                var star = stars[i];
                if (star == null || star == closestStar || star.planetCount == 0) continue;
                var gs = GetGSStar(star);
                if (gs == null || gs.Decorative) continue;
                if (DistanceTo(star) >= TransitionDistance(star)) continue;
                for (var j = 0; j < star.planetCount; j++)
                {
                    var planet = star.planets[j];
                    if (planet == null) continue;
                    var d = DistanceTo(planet);
                    if (d < ApproachDistance(planet) && d < bestDist)
                    {
                        best = planet;
                        bestDist = d;
                    }
                }
            }

            return best;
        }

        private static void EnsurePlanetStillLocal()
        {
            // GS2.Log($"{DistanceTo(closestPlanet)} > {TransisionDistance(closestPlanet)}?");
            if (!(DistanceTo(closestPlanet) > TransitionDistance(closestPlanet))) return;

            closestPlanet = null;
        }

        private static void SearchPlanet()
        {
            closestPlanet = FindPlanetInApproach(closestStar);
        }

        private static void PreloadNearestPlanet(StarData star)
        {
            if (star?.planets == null) return;

            PlanetData nearest = null;
            var nearestCenterDistance = double.MaxValue;
            PlanetData onApproach = null;
            var nearestApproachDistance = double.MaxValue;
            var player = GameMain.mainPlayer;
            var velocity = player.uVelocity;
            var speedSquared = velocity.x * velocity.x + velocity.y * velocity.y + velocity.z * velocity.z;

            for (var i = 0; i < star.planetCount; i++)
            {
                var planet = star.planets[i];
                if (planet == null) continue;

                var toCenter = planet.uPosition - player.uPosition;
                var centerDistance = toCenter.magnitude;
                if (centerDistance < nearestCenterDistance)
                {
                    nearest = planet;
                    nearestCenterDistance = centerDistance;
                }

                if (speedSquared <= 1.0) continue;

                var timeToClosestApproach = VectorLF3.Dot(toCenter, velocity) / speedSquared;
                if (timeToClosestApproach <= 0.0) continue;

                var distanceAlongPath = timeToClosestApproach * System.Math.Sqrt(speedSquared);
                if (distanceAlongPath >= PlanetPreloadDistance || distanceAlongPath >= nearestApproachDistance) continue;

                var missDistance = (toCenter - velocity * timeToClosestApproach).magnitude;
                if (missDistance > ApproachDistance(planet)) continue;

                onApproach = planet;
                nearestApproachDistance = distanceAlongPath;
            }

            var target = onApproach ?? nearest;
            var targetDistance = onApproach != null ? nearestApproachDistance : nearestCenterDistance;
            if (target == null || targetDistance >= PlanetPreloadDistance || target.loaded || target.loading) return;

            Log($"Preloading approaching planet {target.name} at {targetDistance:0}m");
            target.Load();
        }

        private static PlanetData FindPlanetInApproach(StarData star)
        {
            if (star?.planets == null) return null;

            PlanetData best = null;
            var bestDistance = double.MaxValue;

            for (var i = 0; i < star.planetCount; i++)
            {
                var planet = star.planets[i];
                if (planet == null) continue;

                var distance = DistanceTo(planet);
                if (distance >= ApproachDistance(planet)) continue;

                if (best == null || IsPreferredOver(planet, best) ||
                    (!IsPreferredOver(best, planet) && distance < bestDistance))
                {
                    best = planet;
                    bestDistance = distance;
                }
            }

            return best;
        }

        private static bool IsPreferredOver(PlanetData candidate, PlanetData other)
        {
            if (candidate == null || other == null || candidate == other) return false;

            if (IsEmbeddedMoonOf(candidate, other))
            {
                legacyOverlapMoon = candidate;
                legacyOverlapGasGiant = other;
                return true;
            }

            if (candidate == legacyOverlapMoon && other == legacyOverlapGasGiant) return true;
            return false;
        }

        private static bool IsEmbeddedMoonOf(PlanetData moon, PlanetData possibleGasGiant)
        {
            if (moon == null || possibleGasGiant == null || possibleGasGiant.type != EPlanetType.Gas ||
                moon.type == EPlanetType.Gas) return false;

            var ancestor = moon.orbitAroundPlanet;
            while (ancestor != null)
            {
                if (ancestor == possibleGasGiant)
                {
                    var centerDistance = (moon.uPosition - possibleGasGiant.uPosition).magnitude;
                    var overlapMargin = System.Math.Max(1000.0, moon.realRadius * 0.25);
                    return centerDistance <= possibleGasGiant.realRadius + moon.realRadius + overlapMargin;
                }

                ancestor = ancestor.orbitAroundPlanet;
            }

            return false;
        }

        private static void SearchStar()
        {
            // Overlapping systems (#294): several stars' transition spheres can contain the
            // player at once. The old first-index-wins pick could hand locality to a star whose
            // bodies are dozens of AU away while the player is parked ON a planet of another.
            // Pick, among containing stars: first any star with a planet whose transition sphere
            // contains the player (we are AT one of its bodies), else the star with the nearest
            // center. With a single candidate this is exactly the old behavior.
            StarData byPlanet = null;
            StarData byCenter = null;
            var bestCenterDist = double.MaxValue;
            var bestPlanetDist = double.MaxValue;
            for (var i = 0; i < GameMain.galaxy.starCount; i++)
            {
                var star = GameMain.galaxy.stars[i];
                if (star.planetCount == 0) continue;

                var gs = GetGSStar(star);
                if (gs == null || gs.Decorative) continue;

                var dist = DistanceTo(star);
                if (dist >= TransitionDistance(star)) continue;

                if (dist < bestCenterDist)
                {
                    byCenter = star;
                    bestCenterDist = dist;
                }

                for (var j = 0; j < star.planetCount; j++)
                {
                    var planet = star.planets[j];
                    if (planet == null) continue;
                    var pd = DistanceTo(planet);
                    if (pd < ApproachDistance(planet) && pd < bestPlanetDist)
                    {
                        byPlanet = star;
                        bestPlanetDist = pd;
                    }
                }
            }

            closestStar = byPlanet ?? byCenter;
            if (closestStar == null) return;
            Log($"Found Star {closestStar.name}" + (byPlanet != null ? " (owns the planet at hand)" : ""));

            if (GameMain.isRunning && !closestStar.loaded) closestStar.Load();
        }

        /// <summary>
        ///     Wide sphere used to decide that the player is meaningfully AT a foreign star's
        ///     planet. Much larger than the landing transition sphere so the ownership switch
        ///     (and with it rendering/collision of the planet) happens on approach instead of
        ///     after blindly hitting an invisible surface.
        /// </summary>
        private static double ApproachDistance(PlanetData planet)
        {
            var speedLead = Mathf.Clamp((float)(GameMain.mainPlayer.uVelocity.magnitude * 1.5), 2000f, 20000f);
            var wide = planet.realRadius * 4.0 + speedLead;
            var transition = TransitionDistance(planet);
            return wide > transition ? wide : transition;
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

        private static double TransitionDistance(StarData star)
        {
            GSStar s = GetGSStar(star);
            return (s.SystemRadius + 2) * 40000;
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
        private static double TransitionDistance(PlanetData planet)
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
