using System;
using BepInEx.Logging;
using UnityEngine;
using UnityRandom = UnityEngine.Random;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;
using PatchSize = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;
using Random = System.Random;
using GalacticScale.Scripts.PatchPlanetSize;

namespace GalacticScale.Scripts.PatchStarSystemGeneration {
    public static class ReworkPlanetGen {
        public static float pi2Rad = 39.4784176043574f;

        public static PlanetData ReworkCreatePlanet(
            ref GalaxyData galaxy,
            ref StarData star,
            ref GameDesc gameDesc,
            int index,
            int orbitAround,
            int orbitIndex,
            int number,
            bool gasGiant,
            int info_seed,
            int gen_seed) {
            Patch.Debug("ReworkCreatePlanet", LogLevel.Debug,
                Patch.DebugReworkPlanetGen);

            var planetData = new PlanetData();
            var mainSeed = new Random(info_seed);

            //Base data of the planet : 
            planetData.index = index;
            planetData.galaxy = star.galaxy;
            planetData.star = star;
            planetData.seed = gen_seed;
            // Index of the thing it's orbiting around , 0 for star , anything else mean it's a moon
            planetData.orbitAround = orbitAround;
            // Current orbit Index around the planet
            planetData.orbitIndex = orbitIndex;
            // index of the planet in the system ? 
            planetData.number = number;
            // Global Id of the entity 
            planetData.id = star.id * 100 + planetData.index + 1;

            var stars = galaxy.stars;

            Patch.Debug("Basic Information Setup", LogLevel.Debug,
                Patch.DebugReworkPlanetGen);

            var randomNumber1 = mainSeed.NextDouble();
            var randomNumber2 = mainSeed.NextDouble();
            var randomNumber3 = mainSeed.NextDouble();
            var randomNumber4 = mainSeed.NextDouble();
            var randomNumber5 = mainSeed.NextDouble();
            var randomNumber6 = mainSeed.NextDouble();
            var randomNumber7 = mainSeed.NextDouble();
            var randomNumber8 = mainSeed.NextDouble();
            var randomNumber9 = mainSeed.NextDouble();
            var randomNumber10 = mainSeed.NextDouble();
            var randomNumber11 = mainSeed.NextDouble();
            var randomNumber12 = mainSeed.NextDouble();
            var randomNumber13 = mainSeed.NextDouble();
            var randomNumber14 = mainSeed.NextDouble();


            // Orbit definition
            Patch.Debug("Orbit definition", LogLevel.Debug,
                Patch.DebugReworkPlanetGen);
            var baselineOrbitVariation = Mathf.Pow(1.2f, (float) (randomNumber1 * (randomNumber2 - 0.5) * 0.5));
            var orbitInclination =
                UnityRandom.Range(0, Patch.MaxOrbitInclination.Value) * MathUtils.RangePlusMinusOne(mainSeed);

            Patch.Debug("Rotation definition", LogLevel.Debug,
                Patch.DebugReworkPlanetGen);


            // Planet
            Patch.Debug("Body Stuff", LogLevel.Debug,
                Patch.DebugReworkPlanetGen);

            //orbit longitude
            planetData.orbitLongitude = (float) (randomNumber4 * 360.0);
            //runtimeOrbitRotation
            planetData.runtimeOrbitRotation = Quaternion.AngleAxis(planetData.orbitLongitude, Vector3.up) *
                                              Quaternion.AngleAxis(orbitInclination, Vector3.forward);

            planetData.runtimeSystemRotation = planetData.runtimeOrbitRotation * Quaternion.AngleAxis(planetData.obliquity, Vector3.forward);

            if (planetData.IsNotAMoon()) {
                Patch.Debug("Planets Stuff", LogLevel.Debug,
                    Patch.DebugReworkPlanetGen);
                //orbit
                var baselineOrbitSize = Patch.OrbitRadiusPlanetArray[orbitIndex] * star.orbitScaler;
                var orbitSize = (float) ((baselineOrbitVariation - 1.0) / Mathf.Max(1f, baselineOrbitSize) + 1.0);
                planetData.orbitRadius = baselineOrbitSize * orbitSize;

                // orbit Inclination + periods
                planetData.orbitInclination = orbitInclination;
                planetData.orbitalPeriod = Math.Sqrt(pi2Rad * planetData.orbitRadius *
                                                     planetData.orbitRadius * planetData.orbitRadius /
                                                     (1.35385519905204E-06 * star.mass));
                // rotation period
                planetData.rotationPeriod = randomNumber8 * randomNumber9 * Patch.RotationPeriodVariabilityFactor.Value +
                                            Patch.RotationPeriodBaseTime.Value;

                //rotation period
                if (planetData.IsGasGiant() || planetData.star.type == EStarType.NeutronStar)
                    planetData.rotationPeriod *= 0.200000002980232;
                else if (planetData.star.type == EStarType.BlackHole) planetData.rotationPeriod *= 0.150000005960464;

                planetData.sunDistance = planetData.orbitRadius;

                //Tidal Lock Management
                if (randomNumber13 < Patch.ChanceTidalLock.Value) {
                    if (randomNumber13 < Patch.ChanceTidalLock1.Value) {
                        planetData.obliquity *= 0.01f;
                        planetData.rotationPeriod = planetData.orbitalPeriod;
                        planetData.IsTidallyLocked(TidalLevel.TidalLocked);
                    }
                    else if (randomNumber13 < Patch.ChanceTidalLock2.Value) {
                        planetData.obliquity *= 0.1f;
                        planetData.rotationPeriod = planetData.orbitalPeriod * 0.5;
                        planetData.IsTidallyLocked(TidalLevel.TidalLocked2);
                    }
                    else {
                        planetData.obliquity *= 0.2f;
                        planetData.rotationPeriod = planetData.orbitalPeriod * 0.25;
                        planetData.IsTidallyLocked(TidalLevel.TidalLocked4);
                    }
                }

                Patch.Debug("Planets Stuff Done", LogLevel.Debug,
                    Patch.DebugReworkPlanetGen);
            }

            // Moon 
            else {
                // the previous algo is using the number of the planet it's orbiting around, not the actual index --> so minus 1
                orbitAround -= 1;

                Patch.Debug("Moon Stuff", LogLevel.Debug,
                    Patch.DebugReworkPlanetGen);
                //affect the data of the planet of the moon
                Patch.Debug("orbitAround " + orbitAround, LogLevel.Debug, Patch.DebugReworkPlanetGen);

                planetData.orbitAroundPlanet = star.planets[orbitAround];
                Patch.Debug("orbitAround id : " + star.planets[orbitAround].index, LogLevel.Debug, Patch.DebugReworkPlanetGen);
                var orbitRadiusScaled = Patch.OrbitRadiusArrayMoons[orbitIndex] * star.orbitScaler *
                                        Mathf.Lerp(baselineOrbitVariation, 1f, 0.5f) *
                                        planetData.orbitAroundPlanet.GetGasGiantOrbitScaler();
                //orbit
                planetData.orbitRadius = orbitRadiusScaled;
                Patch.Debug("orbitRadius " + planetData.orbitRadius, LogLevel.Debug, Patch.DebugReworkPlanetGen);
                // orbit Inclination + periods
                planetData.orbitInclination = orbitInclination * Patch.MoonOrbitInclinationFactor.Value;
                Patch.Debug("orbitInclination " + planetData.orbitInclination, LogLevel.Debug,
                    Patch.DebugReworkPlanetGen);

                planetData.orbitalPeriod = Math.Sqrt(pi2Rad * planetData.orbitRadius *
                    planetData.orbitRadius * planetData.orbitRadius / 1.08308421068537E-08);
                Patch.Debug("orbitalPeriod " + planetData.orbitalPeriod, LogLevel.Debug, Patch.DebugReworkPlanetGen);

                planetData.rotationPeriod *= Mathf.Pow(planetData.orbitRadius, 0.25f);
                Patch.Debug("rotationPeriod " + planetData.rotationPeriod, LogLevel.Debug, Patch.DebugReworkPlanetGen);


                // distance = planet of the moon
                planetData.sunDistance = planetData.orbitAroundPlanet.orbitRadius;

                Patch.Debug("sunDistance " + planetData.sunDistance, LogLevel.Debug, Patch.DebugReworkPlanetGen);

                // rotation period
                planetData.rotationPeriod =
                    1.0 / (1.0 / planetData.orbitAroundPlanet.orbitalPeriod + 1.0 / planetData.rotationPeriod);
                Patch.Debug("rotationPeriod - 2 " + planetData.rotationPeriod, LogLevel.Debug,
                    Patch.DebugReworkPlanetGen);


                Patch.Debug("Tidal Lock " + planetData.rotationPeriod, LogLevel.Debug, Patch.DebugReworkPlanetGen);
                //Tidal Lock Management
                if (randomNumber13 < Patch.ChanceTidalLock.Value) {
                    if (randomNumber13 < Patch.ChanceTidalLock1.Value) {
                        planetData.obliquity *= 0.01f;
                        planetData.rotationPeriod = planetData.orbitAroundPlanet.orbitalPeriod;
                        planetData.IsTidallyLocked(TidalLevel.TidalLocked);
                    }
                    else if (randomNumber13 < Patch.ChanceTidalLock2.Value) {
                        planetData.obliquity *= 0.1f;
                        planetData.rotationPeriod = planetData.orbitAroundPlanet.orbitalPeriod * 0.5;
                        planetData.IsTidallyLocked(TidalLevel.TidalLocked2);
                    }
                    else {
                        planetData.obliquity *= 0.2f;
                        planetData.rotationPeriod = planetData.orbitAroundPlanet.orbitalPeriod * 0.25;
                        planetData.IsTidallyLocked(TidalLevel.TidalLocked4);
                    }
                }

                Patch.Debug("End Tidal Lock " + planetData.rotationPeriod, LogLevel.Debug, Patch.DebugReworkPlanetGen);

                //runtimeOrbitRotation

                planetData.runtimeOrbitRotation = planetData.orbitAroundPlanet.runtimeOrbitRotation *
                                                  planetData.runtimeOrbitRotation;
                Patch.Debug("runtimeOrbitRotation " + planetData.runtimeOrbitRotation, LogLevel.Debug,
                    Patch.DebugReworkPlanetGen);
                Patch.Debug("Moon Stuff Done", LogLevel.Debug,
                    Patch.DebugReworkPlanetGen);
            }

            Patch.Debug("Body orbit Phase", LogLevel.Debug,
                Patch.DebugReworkPlanetGen);
            // orbit phase 
            planetData.orbitPhase = (float) (randomNumber5 * 360.0);

            Patch.Debug("Body Rotation Phase", LogLevel.Debug,
                Patch.DebugReworkPlanetGen);
            //rotation phase
            planetData.rotationPhase = (float) (randomNumber10 * 360.0);

            Patch.Debug("Body Orbit Obliquity", LogLevel.Debug,
                Patch.DebugReworkPlanetGen);
            // orbit obliquity
            Patch.Debug("Body Obliquity Modification", LogLevel.Debug,
                Patch.DebugReworkPlanetGen);
            if (randomNumber13 < Patch.ChancePlanetLaySide.Value) {
                planetData.obliquity = (float) randomNumber6 * MathUtils.RangePlusMinusOne(mainSeed) *
                                       Patch.LaySideBaseAngle.Value;
                planetData.obliquity += Patch.LaySideAddingAngle.Value * MathUtils.RangePlusMinusOne(mainSeed);
                planetData.HasLayingObliquity();
            }
            else if (randomNumber13 < Patch.ChanceBigObliquity.Value) {
                planetData.obliquity = (float) randomNumber6 * MathUtils.RangePlusMinusOne(mainSeed) *
                                       Patch.BigObliquityBaseAngle.Value;
                planetData.obliquity += Patch.BigObliquityAddingAngle.Value * MathUtils.RangePlusMinusOne(mainSeed);
            }
            else {
                planetData.obliquity = (float) randomNumber6 * MathUtils.RangePlusMinusOne(mainSeed) *
                                       Patch.StandardObliquityAngle.Value;
            }

            Patch.Debug("Body runtimeSystemRotation", LogLevel.Debug,
                Patch.DebugReworkPlanetGen);
            //runtimeOrbitRotation obliquity adjustment
            planetData.runtimeSystemRotation = planetData.runtimeOrbitRotation *
                                               Quaternion.AngleAxis(planetData.obliquity, Vector3.forward);


            Patch.Debug("Body Retrograde", LogLevel.Debug, Patch.DebugReworkPlanetGen);
            //Define if the orbit is retrograde
            if (randomNumber14 < Patch.ChanceRetrogradeOrbit.Value) planetData.HasRetrogradeOrbit();

            Patch.Debug("Body Neutron Star", LogLevel.Debug, Patch.DebugReworkPlanetGen);
            // Anomaly around neutron stars
            if (planetData.star.type == EStarType.NeutronStar) planetData.OrbitAroundNeutronStar();

            Patch.Debug("Body Type Definition", LogLevel.Debug, Patch.DebugReworkPlanetGen);
            // type of the planet :
            if (gasGiant) {
                Patch.Debug("Body is Gas Giant", LogLevel.Debug, Patch.DebugReworkPlanetGen);
                planetData.type = EPlanetType.Gas;
                planetData.habitableBias = 100f;
            }
            else {
                Patch.Debug("Body TypeDefinition ( planet / Moon )", LogLevel.Debug, Patch.DebugReworkPlanetGen);
                var sunDistance = planetData.sunDistance;
                var ratioHabitableDistance = Patch.HabitabilityBaseConstant.Value;
                Patch.Debug("Body Habitability", LogLevel.Debug, Patch.DebugReworkPlanetGen);
                if (star.habitableRadius > 0.0 && sunDistance > 0.0) ratioHabitableDistance = sunDistance / star.habitableRadius;

                Patch.Debug("Star Habitability radius ", LogLevel.Debug, Patch.DebugReworkPlanetGen);
                var minRadiusHabitable = star.habitableRadius - Patch.HabitableRadiusAreaBaseline.Value;
                var maxRadiusHabitable = star.habitableRadius + Patch.HabitableRadiusAreaBaseline.Value;

                if (planetData.sunDistance < maxRadiusHabitable && planetData.sunDistance > minRadiusHabitable) planetData.habitableBias = Patch.ChanceBeingHabitable.Value;

                Patch.Debug("Body Temperature ( planet / Moon )", LogLevel.Debug, Patch.DebugReworkPlanetGen);
                planetData.temperatureBias = (float) (1.20000004768372 / (ratioHabitableDistance + 0.200000002980232) - 1.0);


                if (randomNumber11 < planetData.habitableBias) {
                    Patch.Debug("Body Type Ocean ( planet / Moon )", LogLevel.Debug,
                        Patch.DebugReworkPlanetGen);
                    planetData.type = EPlanetType.Ocean;
                    ++star.galaxy.habitableCount;
                }
                else if (ratioHabitableDistance < Patch.VolcanoPlanetDistanceRatio.Value) {
                    Patch.Debug("Body Type Volcano ( planet / Moon )", LogLevel.Debug,
                        Patch.DebugReworkPlanetGen);
                    planetData.type = EPlanetType.Vocano;
                }
                else if (ratioHabitableDistance > Patch.IcePlanetDistanceRatio.Value) {
                    Patch.Debug("Body Type Ice ( planet / Moon )", LogLevel.Debug,
                        Patch.DebugReworkPlanetGen);
                    planetData.type = EPlanetType.Ice;
                }
                else {
                    Patch.Debug("Body Type Desert ( planet / Moon )", LogLevel.Debug,
                        Patch.DebugReworkPlanetGen);
                    planetData.type = EPlanetType.Desert;
                }

                Patch.Debug("Body Type Defined ( planet / Moon )", LogLevel.Debug,
                    Patch.DebugReworkPlanetGen);
            }

            Patch.Debug("Body Luminosity( planet / Moon )", LogLevel.Debug,
                Patch.DebugReworkPlanetGen);
            //Luminosity
            planetData.luminosity = Mathf.Pow(planetData.star.lightBalanceRadius / (planetData.sunDistance + 0.01f), 0.6f);
            if (planetData.luminosity > 1.0) {
                planetData.luminosity = Mathf.Log(planetData.luminosity) + 1f;
                planetData.luminosity = Mathf.Log(planetData.luminosity) + 1f;
                planetData.luminosity = Mathf.Log(planetData.luminosity) + 1f;
            }

            planetData.luminosity = Mathf.Round(planetData.luminosity * 100f) / 100f;

            Patch.Debug("Body Size Def ( planet / Moon )", LogLevel.Debug,
                Patch.DebugReworkPlanetGen);
            //Size related stuff : 
            if (planetData.type == EPlanetType.Gas) {
                var radiusGasGiantWanted = PatchSize.VanillaGasGiantSize;
                if (PatchSize.EnableResizingFeature.Value) {
                    //Default : 0.25
                    var minScalingGasGiantRatio =
                        (PatchSize.BaseGasGiantSize.Value - PatchSize.BaseGasGiantSizeVariationFactor.Value) /
                        (PatchSize.BaseGasGiantSize.Value + PatchSize.BaseGasGiantSizeVariationFactor.Value);

                    radiusGasGiantWanted = PatchSize.BaseGasGiantSize.Value +
                                           MathUtils.RangePlusMinusOne(mainSeed) *
                                           PatchSize.BaseGasGiantSizeVariationFactor.Value;
                    radiusGasGiantWanted -= radiusGasGiantWanted % 10;
                }

                planetData.scale = PatchSize.VanillaGasGiantScale;
                planetData.radius = radiusGasGiantWanted / planetData.scale;
                if (PatchSize.EnableLimitedResizingFeature.Value || PatchSize.EnableResizingFeature.Value) {
                    int segments = (int) (planetData.radius / 4f + 0.1f) * 4;
                    SetLuts(segments, planetData.radius);
                }
                planetData.precision = 64;
                planetData.segment = 2;
            }
            else if (planetData.type != EPlanetType.None) {
                if (PatchSize.EnableResizingFeature.Value) {
                    var radiusTelluricWanted = PatchSize.VanillaTelluricSize;
                    if (planetData.IsNotAMoon() || !PatchSize.EnableMoonSizeFailSafe.Value) {
                        radiusTelluricWanted = PatchSize.BaseTelluricSize.Value +
                                               MathUtils.RangePlusMinusOne(mainSeed) *
                                               PatchSize.BaseTelluricSizeVariationFactor.Value;
                    }
                    else {
                        //A moon can only be smaller than it's host
                        if (planetData.orbitAroundPlanet.type != EPlanetType.Gas) {
                            radiusTelluricWanted = planetData.orbitAroundPlanet.radius -
                                                   (float) mainSeed.NextDouble() *
                                                   PatchSize.BaseTelluricSizeVariationFactor.Value;
                            // clamp to avoid weird sizes
                            radiusTelluricWanted = Mathf.Clamp(radiusTelluricWanted,
                                PatchSize.MinTelluricSize.Value,
                                planetData.orbitAroundPlanet.radius);
                        }
                        else {
                            radiusTelluricWanted = PatchSize.BaseTelluricSize.Value +
                                                   MathUtils.RangePlusMinusOne(mainSeed) *
                                                   PatchSize.BaseTelluricSizeVariationFactor.Value;
                        }
                    }

                    radiusTelluricWanted -= radiusTelluricWanted % 10;
                    planetData.radius = Mathf.RoundToInt(radiusTelluricWanted);
                    Patch.Debug(" planetData.radius" + planetData.radius, LogLevel.Debug,
                        Patch.DebugReworkPlanetGenDeep);

                    planetData.scale = planetData.GetScaleFactored();
                    Patch.Debug(" planetData.scale" + planetData.scale, LogLevel.Debug,
                        Patch.DebugReworkPlanetGenDeep);

                    planetData.precision = planetData.GetPrecisionFactored();
                    Patch.Debug(" planetData.precision" + planetData.precision, LogLevel.Debug,
                        Patch.DebugReworkPlanetGenDeep);
                }
                else if (PatchSize.EnableLimitedResizingFeature.Value) {
                    var choice = mainSeed.NextDouble();

                    foreach (var planetSizeParam in PatchSize.PlanetSizeParams) {
                        if (choice <= planetSizeParam.Value) {
                            planetData.radius = planetSizeParam.Key;
                            int segments;
                            if (PatchSize.EnableLimitedResizingFeature.Value || PatchSize.EnableResizingFeature.Value) {
                                //planetData.precision = Mathf.Max(planetSizeParam.Key, 200);
                                planetData.precision = planetSizeParam.Key;
                                segments = (int) (planetData.radius / 4f + 0.1f) * 4;

                                SetLuts(segments, planetData.radius);
                            }
                            if (planetData.IsAMoon() && PatchSize.EnableMoonSizeFailSafe.Value) {
                                if (planetData.orbitAroundPlanet.radius <= planetData.radius) {
                                    for (var i = 0; i < PatchSize.PlanetSizeParams.Count; i++) {
                                        if (PatchSize.PlanetSizeList[i] == planetData.orbitAroundPlanet.radius) {
                                            if (i != 0) {
                                                planetData.radius = PatchSize.PlanetSizeList[i - 1];
                                                if (PatchSize.EnableLimitedResizingFeature.Value || PatchSize.EnableResizingFeature.Value) {
                                                    //planetData.precision = Mathf.Max(PatchSize.PlanetSizeList[i - 1], 200);
                                                    planetData.precision = PatchSize.PlanetSizeList[i - 1];
                                                    segments = (int) (planetData.radius / 4f + 0.1f) * 4;
                                                    SetLuts(segments, planetData.radius);
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    }
                }
                else {
                    planetData.radius = PatchSize.VanillaTelluricSize;
                    planetData.scale = PatchSize.VanillaTelluricScale;
                    planetData.precision = PatchSize.VanillaTelluricPrecision;
                }

                planetData.segment = 5;
            }
            else {
                planetData.radius = PatchSize.VanillaTelluricSize;
                planetData.precision = 64;
                planetData.segment = 2;
            }

            star.planets[planetData.index] = planetData;
            planetData.star = star;

            Patch.Debug("Body Theme Def ( planet / Moon )", LogLevel.Debug, Patch.DebugReworkPlanetGen);
            Patch.Debug("planetData \n" +
                        "planetData.star.index " + planetData.star.index + "\n" +
                        "planetData.index " + planetData.index + "\n" +
                        "planetData.temperatureBias " + planetData.temperatureBias + "\n" +
                        "planetData.planets " + planetData.star.planets + "\n" +
                        "planetData.planets index : " + planetData.star.planets[planetData.index].type + "\n" +
                        "planetData.planets Lenght " + planetData.star.planets.Length + "\n" +
                        "planetData.type " + planetData.type + "\n" +
                        "planetData.mod_x " + planetData.mod_x + "\n" +
                        "planetData.mod_y " + planetData.mod_y + "\n" +
                        "planetData.algoId " + planetData.algoId + "\n"
                , LogLevel.Debug, Patch.DebugReworkPlanetGen);

            star.galaxy.astroPoses[planetData.id].uRadius = planetData.realRadius;


            Patch.Debug("ReworkCreatePlanet Done !", LogLevel.Debug,
                Patch.DebugReworkPlanetGen);

            return planetData;
        }

        public static void SetLuts(int segments, float planetRadius)
        {
            if (PatchOnPlanetGrid.keyedLUTs.ContainsKey(segments) && PatchOnPlatformSystem.keyedLUTs.ContainsKey(segments) && PatchUIBuildingGrid.LUT512.ContainsKey(segments))
            {
                return;
            }
            Patch.Debug("Setting Planet LUTs for size " + planetRadius, LogLevel.Debug, true);
            int numSegments = segments / 4;
            int[] lut = new int[numSegments];
            float segmentAngle = (Mathf.PI / 2f) / numSegments;

            float lastMajorRadius = planetRadius;
            int lastMajorRadiusCount = numSegments * 4;

            int[] classicLUT = new int[512];
            classicLUT[0] = 1;

            for (int cnt = 0; cnt < numSegments; cnt++)
            {
                float segmentXAngle = (Mathf.PI / 2f) - (cnt * segmentAngle);
                float segmentLineHeight = Mathf.Cos(segmentXAngle);
                float segmentCylinderHeight = segmentLineHeight * planetRadius * 2;

                float ringradius = Mathf.Sqrt((planetRadius * planetRadius) - ((segmentCylinderHeight * segmentCylinderHeight) / 4.0f));
                int classicIdx = Mathf.CeilToInt(Mathf.Abs(Mathf.Cos((float)((cnt+1) / (segments / 4f) * Math.PI * 0.5))) * (float)segments);

                if (ringradius < (0.9 * lastMajorRadius)) {
                    lastMajorRadius = ringradius;
                    lastMajorRadiusCount = (int) (ringradius / 4.0) * 4;
                }
                lut[cnt] = lastMajorRadiusCount;
                classicLUT[classicIdx] = lastMajorRadiusCount;
                //Patch.Debug("Index " + cnt + " is classicIndex " + classicIdx + " with value " + lastMajorRadiusCount, LogLevel.Debug, true);
            }

            int last = 1;
            for(int oldlLutIdx = 1; oldlLutIdx < 512; oldlLutIdx++)
            {
                if(classicLUT[oldlLutIdx] > last)
                {
                    //Offset of 1 is required to avoid mismatch of some longitude circles
                    int temp = classicLUT[oldlLutIdx];
                    classicLUT[oldlLutIdx] = last;
                    last = temp;
                }
                else
                {
                    classicLUT[oldlLutIdx] = last;
                }
            }

            //DebugClassicLut(classicLUT); //<-- Debug print function for whole LUT

            //Fill all Look Up Tables (Dictionaries really)
            if (!PatchOnPlanetGrid.keyedLUTs.ContainsKey(segments))
            {
                PatchOnPlanetGrid.keyedLUTs.Add(segments, lut);
            }
            if (!PatchOnPlatformSystem.keyedLUTs.ContainsKey(segments)) {
                PatchOnPlatformSystem.keyedLUTs.Add(segments, lut);
            }
            if (!PatchUIBuildingGrid.LUT512.ContainsKey(segments))
            {
                PatchUIBuildingGrid.LUT512.Add(segments, classicLUT);
            }
        }

        private static void DebugClassicLut(int[] classicLUT)
        {

            Patch.Debug("Classic LUT:", LogLevel.Debug, true);
            for (int a = 0; a < 32; a++)
            {
                string str = "";
                for (int b = 0; b < 16; b++)
                {
                    str += classicLUT[a * 16 + b] + ", ";
                }
                Patch.Debug(str, LogLevel.Debug, true);
            }
        }
    }
}
