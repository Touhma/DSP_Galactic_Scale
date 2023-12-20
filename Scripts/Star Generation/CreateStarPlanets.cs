using System;
using UnityEngine;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static void CreateStarPlanets(ref StarData star, GameDesc gameDesc, Random random)
        {
            //Log($"Start|{star.name}");
            var gsStar = GSSettings.Stars[star.index];
            gsStar.counter = 0;
            while (gsStar.bodyCount > 99)
            {
                Log($"Truncating planets for star {star.name} as it has {gsStar.bodyCount}");
                gsStar.Planets.RemoveAt(gsStar.Planets.Count - 1);
                Warn($"New BodyCount = {gsStar.bodyCount}, existing planetCount was {star.planetCount}");
                star.planetCount = gsStar.bodyCount;
            }

            StarGen.SetHiveOrbitsConditionsTrue();
            star.planets = new PlanetData[Math.Min(100, gsStar.bodyCount)];
            //Log("Creating Planet");
            for (var i = 0; i < gsStar.PlanetCount; i++) CreatePlanet(ref star, gsStar.Planets[i], random);
            star.planetCount = star.planets.Length;

            //0.10...
            for (int m = 0; m < star.planetCount; m++)
            {
                PlanetData planetData = star.planets[m];
                int orbitIndex6 = planetData.orbitIndex;
                int orbitAroundOrbitIndex =
                    (planetData.orbitAroundPlanet != null) ? planetData.orbitAroundPlanet.orbitIndex : 0;
                StarGen.SetHiveOrbitConditionFalse(orbitIndex6, orbitAroundOrbitIndex,
                    planetData.sunDistance / star.orbitScaler, star.index);
            }

            star.hiveAstroOrbits = new AstroOrbitData[8];
            AstroOrbitData[] hiveAstroOrbits = star.hiveAstroOrbits;
            int num22 = 0;
            for (int n = 0; n < StarGen.hiveOrbitCondition.Length; n++)
            {
                if (StarGen.hiveOrbitCondition[n])
                {
                    num22++;
                }
            }

            for (int num23 = 0; num23 < 8; num23++)
            {
                double num24 = random.NextDouble() * 2.0 - 1.0;
                double num25 = random.NextDouble();
                double num26 = random.NextDouble();
                num24 = (double)Math.Sign(num24) * Math.Pow(Math.Abs(num24), 0.7) * 90.0;
                num25 *= 360.0;
                num26 *= 360.0;
                float num27 = 0.3f;
                Assert.Positive(num22);
                if (num22 > 0)
                {
                    int num28 = (star.index != 0) ? 5 : 2;
                    num28 = ((num22 > num28) ? num28 : num22);
                    int num29 = num28 * 100;
                    int num30 = num29 * 100;
                    int num31 = random.Next(num29);
                    int num32 = num31 * num31 / num30;
                    for (int num33 = 0; num33 < StarGen.hiveOrbitCondition.Length; num33++)
                    {
                        if (StarGen.hiveOrbitCondition[num33])
                        {
                            if (num32 == 0)
                            {
                                num27 = StarGen.hiveOrbitRadius[num33];
                                StarGen.hiveOrbitCondition[num33] = false;
                                num22--;
                                break;
                            }

                            num32--;
                        }
                    }
                }

                hiveAstroOrbits[num23] = new AstroOrbitData();
                hiveAstroOrbits[num23].orbitRadius = num27 * star.orbitScaler;
                Warn($"End|{star.name} {hiveAstroOrbits[num23].orbitRadius}");
                hiveAstroOrbits[num23].orbitInclination = (float)num24;
                hiveAstroOrbits[num23].orbitLongitude = (float)num25;
                hiveAstroOrbits[num23].orbitPhase = (float)num26;
                hiveAstroOrbits[num23].orbitalPeriod = Math.Sqrt(39.47841760435743 * (double)num27 * (double)num27 *
                    (double)num27 / (1.3538551990520382E-06 * (double)star.mass));
                hiveAstroOrbits[num23].orbitRotation =
                    Quaternion.AngleAxis(hiveAstroOrbits[num23].orbitLongitude, Vector3.up) *
                    Quaternion.AngleAxis(hiveAstroOrbits[num23].orbitInclination, Vector3.forward);
                hiveAstroOrbits[num23].orbitNormal = Maths
                    .QRotateLF(hiveAstroOrbits[num23].orbitRotation, new VectorLF3(0f, 1f, 0f)).normalized;
            }


            Log($"End|{star.name}");
        }
    }
}