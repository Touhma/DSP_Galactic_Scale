using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class GS2
    {
        private static void CreateDarkFogHive(StarData star, Random random)
        {



            
            
            
            
            GSStar gsStar = GetGSStar(star);
            if (gsStar.Decorative || gsStar.PlanetCount == 0)
            {
                star.initialHiveCount = 0;
                star.maxHiveCount = 0;
                star.hivePatternLevel = 0;
                star.hiveAstroOrbits = new AstroOrbitData[] { };
                return;
            }
            // for (int m = 0; m < star.planetCount; m++)
            // {
            //     PlanetData planetData = star.planets[m];
            //     GSPlanet gsPlanet = GetGSPlanet(planetData);
            //
            //     Log($"Galaxy BirthPlanetId is set to {galaxy.birthPlanetId}");
            //     //Looks like we are setting every planets 'orbit index' false. However GS2 doesnt use orbit indexes. The plot thickens.
            //     int orbitIndex6 = planetData.orbitIndex;
            //     int orbitAroundOrbitIndex = (planetData.orbitAroundPlanet != null) ? planetData.orbitAroundPlanet.orbitIndex : 0;
            //     StarGen.SetHiveOrbitConditionFalse(orbitIndex6, orbitAroundOrbitIndex, planetData.sunDistance / star.orbitScaler, star.index);
            // }
            // var possibleHiveOrbits = Utils.DeSerialize<List<float>>(gsStar.genData.Get("hiveOrbits"));
            // LogJson(possibleHiveOrbits);
            // if (possibleHiveOrbits == null || possibleHiveOrbits == new List<float>())
            // {
               var possibleHiveOrbits = GeneratePossibleHiveOrbits(gsStar);
               // LogJson(possibleHiveOrbits);
            // }
            // star.hiveAstroOrbits = new AstroOrbitData[8];
            
            // int numHives = 0;
            // for (int n = 0; n < StarGen.hiveOrbitCondition.Length; n++)
            // {
            //     if (StarGen.hiveOrbitCondition[n])
            //     {
            //         numHives++;
            //     }
            // }
            //
            // for (int i = 0; i < 8; i++)
            // {
            //     double r1 = random.NextDouble() * 2.0 - 1.0;
            //     double r2 = random.NextDouble();
            //     double r3 = random.NextDouble();
            //     r1 = (double)Math.Sign(r1) * Math.Pow(Math.Abs(r1), 0.7) * 90.0;
            //     r2 *= 360.0;
            //     r3 *= 360.0;
            //     float num27 = 0.3f;
            //     Assert.Positive(numHives);
            //     if (numHives > 0)
            //     {
            //         int num28 = (star.index != 0) ? 5 : 2;
            //         num28 = ((numHives > num28) ? num28 : numHives);
            //         int num29 = num28 * 100;
            //         int num30 = num29 * 100;
            //         int num31 = random.Next(num29);
            //         int num32 = num31 * num31 / num30;
            //         for (int num33 = 0; num33 < StarGen.hiveOrbitCondition.Length; num33++)
            //         {
            //             if (StarGen.hiveOrbitCondition[num33])
            //             {
            //                 if (num32 == 0)
            //                 {
            //                     num27 = StarGen.hiveOrbitRadius[num33];
            //                     StarGen.hiveOrbitCondition[num33] = false;
            //                     numHives--;
            //                     break;
            //                 }
            //
            //                 num32--;
            //             }
            //         }
            //     }
            var hiveCount = 8;
            star.hiveAstroOrbits = new AstroOrbitData[hiveCount];
            AstroOrbitData[] hiveAstroOrbits = star.hiveAstroOrbits;
            for (int i = 0; i < hiveCount; i++)
            {
                hiveAstroOrbits[i] = new AstroOrbitData();
                var orbit = random.ItemAndRemove(possibleHiveOrbits);
                hiveAstroOrbits[i].orbitRadius = orbit;
                // Warn($"Created Hive Orbit at {star.name} {hiveAstroOrbits[i].orbitRadius}");
                hiveAstroOrbits[i].orbitInclination = random.NextFloat();
                hiveAstroOrbits[i].orbitLongitude = random.NextFloat();
                hiveAstroOrbits[i].orbitPhase = random.NextFloat();
                hiveAstroOrbits[i].orbitalPeriod = Utils.CalculateOrbitPeriod(hiveAstroOrbits[i].orbitRadius);
                hiveAstroOrbits[i].orbitRotation = Quaternion.AngleAxis(hiveAstroOrbits[i].orbitLongitude, Vector3.up) *
                                                   Quaternion.AngleAxis(hiveAstroOrbits[i].orbitInclination,
                                                       Vector3.forward);
                hiveAstroOrbits[i].orbitNormal =
                    Maths.QRotateLF(hiveAstroOrbits[i].orbitRotation, new VectorLF3(0f, 1f, 0f)).normalized;
                // Log("Created Hive Orbit.");
                // Log($"Orbit Radius: {hiveAstroOrbits[i].orbitRadius}");
            }
            Log($"Darkfog Hive Orbits Generated for {gsStar.Name}");
        }

        private static List<float> GeneratePossibleHiveOrbits(GSStar gsStar, int count = 10, Random random = null)
        {
            if (gsStar.PlanetCount == 0 || gsStar.Decorative) return default;
            List<(float inner, float outer)> ExistingOrbits = new();
            List<(float inner, float outer)> ExistingGaps = new();
            List<float> PossibleOrbits = new();
            var MaxOrbit = gsStar.luminosity * 2f;
            if (random == null) random = new Random(Mathf.CeilToInt( gsStar.luminosity*gsStar.age*gsStar.SystemRadius));
            foreach (GSPlanet p in gsStar.Planets)
            {
                ExistingOrbits.Add((p.OrbitRadius-p.SystemRadius, p.OrbitRadius+p.SystemRadius));
            }
            if (ExistingOrbits[0].inner > gsStar.RadiusAU + 0.1f) ExistingGaps.Add((gsStar.RadiusAU+0.1f, ExistingOrbits[0].inner));
            for (var i=0;i<ExistingOrbits.Count-1;i++)
            {
                ExistingGaps.Add((ExistingOrbits[i].outer, ExistingOrbits[i+1].inner));
            }


            if (ExistingOrbits[ExistingOrbits.Count -1].outer < MaxOrbit)
            {
                ExistingGaps.Add((ExistingOrbits[ExistingOrbits.Count - 1].outer, MaxOrbit));
            }
            for (var i = 0; i < count - 1; i++)
            {
                if (ExistingGaps.Count == 0)
                {
                    // GS2.Error($"No Existing Gaps! Cycle:{i}/{count} Possible Orbits: {PossibleOrbits.Count} Existing Orbits: {ExistingOrbits.Count} Existing Gaps: {ExistingGaps.Count}");
                    // LogJson(ExistingOrbits);
                    GS2.Warn("Adding Extra Orbit Gap");
                    var oldMaxOrbit = MaxOrbit;
                    MaxOrbit += 5f;
                    ExistingGaps.Add((oldMaxOrbit, MaxOrbit));
                }
                var gap = random.Item(ExistingGaps);
                var orbit = random.ClampedNormal(gap.inner, gap.outer, 50);
                PossibleOrbits.Add(orbit);
                ExistingGaps.Remove(gap);
                (float inner,float outer) newGapInner = (gap.inner, orbit - 0.05f);
                (float inner,float outer) newGapOuter = (orbit + 0.05f, gap.outer);
                if (newGapInner.outer - newGapInner.inner > 0.1f) ExistingGaps.Add(newGapInner);
                if (newGapOuter.outer - newGapOuter.inner > 0.1f) ExistingGaps.Add(newGapOuter);
            }
            
            return PossibleOrbits;
        }
    }
}