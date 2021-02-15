using System;
using BepInEx.Logging;
using GalacticScale.Scripts.PatchStarSystemGeneration.Utils;
using UnityEngine;
using UnityRandom = UnityEngine.Random;
using Random = System.Random;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;

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

            PlanetData planetData = new PlanetData();
            Random mainSeed = new Random(info_seed);

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

            StarData[] stars = galaxy.stars;
            
            Patch.Debug("Basic Information Setup", LogLevel.Debug,
                Patch.DebugReworkPlanetGen);

            double randomNumber1 = mainSeed.NextDouble();
            double randomNumber2 = mainSeed.NextDouble();
            double randomNumber3 = mainSeed.NextDouble();
            double randomNumber4 = mainSeed.NextDouble();
            double randomNumber5 = mainSeed.NextDouble();
            double randomNumber6 = mainSeed.NextDouble();
            double randomNumber7 = mainSeed.NextDouble();
            double randomNumber8 = mainSeed.NextDouble();
            double randomNumber9 = mainSeed.NextDouble();
            double randomNumber10 = mainSeed.NextDouble();
            double randomNumber11 = mainSeed.NextDouble();
            double randomNumber12 = mainSeed.NextDouble();
            double randomNumber13 = mainSeed.NextDouble();
            double randomNumber14 = mainSeed.NextDouble();

            double rand1 = mainSeed.NextDouble();
            double rand2 = mainSeed.NextDouble();
            double rand3 = mainSeed.NextDouble();
            double rand4 = mainSeed.NextDouble();

            int theme_seed = mainSeed.Next();


            // Orbit definition
            Patch.Debug("Orbit definition", LogLevel.Debug,
                Patch.DebugReworkPlanetGen);
            float baselineOrbitVariation = Mathf.Pow(1.2f, (float) (randomNumber1 * (randomNumber2 - 0.5) * 0.5));
            float orbitInclination =
                UnityRandom.Range(0, Patch.MaxOrbitInclination) * MathUtils.RangePlusMinusOne(mainSeed);

            Patch.Debug("Rotation definition", LogLevel.Debug,
                Patch.DebugReworkPlanetGen);
            // rotation period
            planetData.rotationPeriod = (randomNumber8 * randomNumber9 * Patch.RotationPeriodVariabilityFactor +
                                         Patch.RotationPeriodBaseTime);

            // Planet
            Patch.Debug("Body Stuff", LogLevel.Debug,
                Patch.DebugReworkPlanetGen);
            if (planetData.IsNotAMoon()) {
                Patch.Debug("Planets Stuff", LogLevel.Debug,
                    Patch.DebugReworkPlanetGen);
                //orbit
                float baselineOrbitSize = Patch.OrbitRadiusPlanetArray[orbitIndex] * star.orbitScaler;
                float orbitSize = (float) ((baselineOrbitVariation - 1.0) / Mathf.Max(1f, baselineOrbitSize) + 1.0);
                planetData.orbitRadius = baselineOrbitSize * orbitSize;

                // orbit Inclination + periods
                planetData.orbitInclination = orbitInclination;
                planetData.orbitalPeriod = Math.Sqrt(pi2Rad * planetData.orbitRadius *
                                                     planetData.orbitRadius * planetData.orbitRadius /
                                                     (1.35385519905204E-06 * star.mass));

                //rotation period
                if (planetData.IsGasGiant() || planetData.star.type == EStarType.NeutronStar) {
                    planetData.rotationPeriod *= 0.200000002980232;
                }
                else if (planetData.star.type == EStarType.BlackHole) {
                    planetData.rotationPeriod *= 0.150000005960464;
                }

                planetData.sunDistance = planetData.orbitRadius;

                // rotation period
                planetData.rotationPeriod = 1.0 / (1.0 / planetData.orbitalPeriod * 2);

                //Tidal Lock Management
                if (randomNumber13 < Patch.ChanceTidalLock) {
                    if (randomNumber13 < Patch.ChanceTidalLock1) {
                        planetData.obliquity *= 0.01f;
                        planetData.rotationPeriod = planetData.orbitalPeriod;
                        planetData.IsTidallyLocked(TidalLevel.TidalLocked);
                    }
                    else if (randomNumber13 < Patch.ChanceTidalLock2) {
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

                //runtimeOrbitRotation
                planetData.runtimeOrbitRotation = Quaternion.AngleAxis(planetData.orbitLongitude, Vector3.up) *
                                                  Quaternion.AngleAxis(planetData.orbitInclination, Vector3.forward);
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
                Patch.Debug("orbitAround " + (orbitAround) , LogLevel.Debug, Patch.DebugReworkPlanetGen);
                Patch.Debug("star.planets[orbitAround] " + star.planets[orbitAround].name, LogLevel.Debug, Patch.DebugReworkPlanetGen);
                planetData.orbitAroundPlanet = star.planets[orbitAround];
                
                //orbit
                planetData.orbitRadius = Patch.OrbitRadiusArrayMoons[orbitIndex] * star.orbitScaler *
                                         Mathf.Lerp(baselineOrbitVariation, 1f, 0.5f);
                Patch.Debug("orbitRadius " + planetData.orbitRadius, LogLevel.Debug, Patch.DebugReworkPlanetGen);
                // orbit Inclination + periods
                planetData.orbitInclination = orbitInclination * Patch.MoonOrbitInclinationFactor;
                Patch.Debug("orbitInclination " + planetData.orbitInclination, LogLevel.Debug, Patch.DebugReworkPlanetGen);
                
                planetData.orbitalPeriod = Math.Sqrt(pi2Rad * planetData.orbitRadius *
                    planetData.orbitRadius * planetData.orbitRadius / 1.08308421068537E-08);
                Patch.Debug("orbitalPeriod " + planetData.orbitalPeriod, LogLevel.Debug, Patch.DebugReworkPlanetGen);

                planetData.rotationPeriod *= Mathf.Pow(planetData.orbitRadius, 0.25f);
                Patch.Debug("rotationPeriod " + planetData.rotationPeriod, LogLevel.Debug, Patch.DebugReworkPlanetGen);
                Patch.Debug("planetData.orbitAroundPlanet name " + planetData.orbitAroundPlanet.name , LogLevel.Debug, Patch.DebugReworkPlanetGen);

                // distance = planet of the moon
                planetData.sunDistance = planetData.orbitAroundPlanet.orbitRadius;
                
                Patch.Debug("sunDistance " + planetData.sunDistance, LogLevel.Debug, Patch.DebugReworkPlanetGen);

                // rotation period
                planetData.rotationPeriod =
                    1.0 / (1.0 / planetData.orbitAroundPlanet.orbitalPeriod + 1.0 / planetData.rotationPeriod);
                Patch.Debug("rotationPeriod - 2 " + planetData.rotationPeriod, LogLevel.Debug, Patch.DebugReworkPlanetGen);
                
                
                Patch.Debug("Tidal Lock " + planetData.rotationPeriod, LogLevel.Debug, Patch.DebugReworkPlanetGen);
                //Tidal Lock Management
                if (randomNumber13 < Patch.ChanceTidalLock) {
                    if (randomNumber13 < Patch.ChanceTidalLock1) {
                        planetData.obliquity *= 0.01f;
                        planetData.rotationPeriod = planetData.orbitAroundPlanet.orbitalPeriod;
                        planetData.IsTidallyLocked(TidalLevel.TidalLocked);
                    }
                    else if (randomNumber13 < Patch.ChanceTidalLock2) {
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
                Patch.Debug("runtimeOrbitRotation " + planetData.runtimeOrbitRotation, LogLevel.Debug, Patch.DebugReworkPlanetGen);
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
            if (randomNumber13 < Patch.ChancePlanetLaySide) {
                planetData.obliquity = (float) randomNumber6 * MathUtils.RangePlusMinusOne(mainSeed) *
                                       Patch.LaySideBaseAngle;
                planetData.obliquity += Patch.LaySideAddingAngle * MathUtils.RangePlusMinusOne(mainSeed);
                planetData.HasLayingObliquity();
            }
            else if (randomNumber13 < Patch.ChanceBigObliquity) {
                planetData.obliquity = (float) randomNumber6 * MathUtils.RangePlusMinusOne(mainSeed) *
                                       Patch.BigObliquityBaseAngle;
                planetData.obliquity += Patch.BigObliquityAddingAngle * MathUtils.RangePlusMinusOne(mainSeed);
            }
            else {
                planetData.obliquity = (float) randomNumber6 * MathUtils.RangePlusMinusOne(mainSeed) *
                                       Patch.StandardObliquityAngle;
            }
        
            Patch.Debug("Body runtimeSystemRotation", LogLevel.Debug,
                Patch.DebugReworkPlanetGen);
            //runtimeOrbitRotation obliquity adjustment
            planetData.runtimeSystemRotation = planetData.runtimeOrbitRotation *
                                               Quaternion.AngleAxis(planetData.obliquity, Vector3.forward);
            
            Patch.Debug("Body Retrograde", LogLevel.Debug,
                Patch.DebugReworkPlanetGen);
            //Define if the orbit is retrograde
            if (randomNumber14 < Patch.ChanceRetrogradeOrbit) {
                planetData.HasRetrogradeOrbit();
            }
            Patch.Debug("Body Neutron Star", LogLevel.Debug,
                Patch.DebugReworkPlanetGen);
            // Anomaly around neutron stars
            if (planetData.star.type == EStarType.NeutronStar) {
                planetData.OrbitAroundNeutronStar();
            }
            Patch.Debug("Body Type Definition", LogLevel.Debug,
                Patch.DebugReworkPlanetGen);
            // type of the planet :
            if (planetData.IsGasGiant()) {
                Patch.Debug("Body is Gas Giant", LogLevel.Debug,
                    Patch.DebugReworkPlanetGen);
                planetData.type = EPlanetType.Gas;
                planetData.habitableBias = 100f;
            }
            else {
                Patch.Debug("Body TypeDefinition ( planet / Moon )", LogLevel.Debug,
                    Patch.DebugReworkPlanetGen);
                float sunDistance = planetData.sunDistance;
                float ratioHabitableDistance = Patch.HabitabilityBaseConstant;
                Patch.Debug("Body Habitability", LogLevel.Debug,
                    Patch.DebugReworkPlanetGen);
                if (star.habitableRadius > 0.0 && sunDistance > 0.0) {
                    ratioHabitableDistance = sunDistance / star.habitableRadius;
                }
                Patch.Debug("Star Habitability radius ", LogLevel.Debug,
                    Patch.DebugReworkPlanetGen);
                float minRadiusHabitable = star.habitableRadius - Patch.HabitableRadiusAreaBaseline;
                float maxRadiusHabitable = star.habitableRadius + Patch.HabitableRadiusAreaBaseline;
                
                if (planetData.sunDistance < maxRadiusHabitable && planetData.sunDistance > minRadiusHabitable) {
                    planetData.habitableBias = Patch.ChanceBeingHabitable;
                }
                
                Patch.Debug("Body Temperature ( planet / Moon )", LogLevel.Debug,
                    Patch.DebugReworkPlanetGen);
                planetData.temperatureBias =
                    (float) (1.20000004768372 / (ratioHabitableDistance + 0.200000002980232) - 1.0);

                if (randomNumber11 < planetData.habitableBias) {
                    Patch.Debug("Body Type Ocean ( planet / Moon )", LogLevel.Debug,
                        Patch.DebugReworkPlanetGen);
                    planetData.type = EPlanetType.Ocean;
                    ++star.galaxy.habitableCount;
                }
                else if (ratioHabitableDistance < Patch.VolcanoPlanetDistanceRatio) {
                    Patch.Debug("Body Type Volcano ( planet / Moon )", LogLevel.Debug,
                        Patch.DebugReworkPlanetGen);
                    planetData.type = EPlanetType.Vocano;
                }
                else if (ratioHabitableDistance > Patch.IcePlanetDistanceRatio) {
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
            if (planetData.type == EPlanetType.Gas ) {
                //Default : 0.25
                float minScalingGasGiantRatio = 
                    (Patch.BaseGasGiantSize - Patch.BaseGasGiantSizeVariationFactor) / (Patch.BaseGasGiantSize + Patch.BaseGasGiantSizeVariationFactor);

                float radiusGasGiantWanted = Patch.BaseGasGiantSize +
                                     MathUtils.RangePlusMinusOne(mainSeed) * Patch.BaseGasGiantSizeVariationFactor;


                planetData.scale = 10f;
                planetData.radius = radiusGasGiantWanted / planetData.scale;

                planetData.precision = 64;
                planetData.segment = 2;
            }
            else if (planetData.type != EPlanetType.None) {

                float radiusTelluricWanted;

                if (planetData.IsNotAMoon()) {
                    radiusTelluricWanted = Patch.BaseTelluricSize +
                                           MathUtils.RangePlusMinusOne(mainSeed) * Patch.BaseTelluricSizeVariationFactor;
                }
                else {
                    
                    //A moon can only be smaller than it's host
                    radiusTelluricWanted = planetData.orbitAroundPlanet.radius -
                                           (float) mainSeed.NextDouble() * Patch.BaseTelluricSizeVariationFactor;

                    // clamp to avoid weird sizes
                    radiusTelluricWanted = Mathf.Clamp(radiusTelluricWanted, Patch.MinTelluricSize, planetData.orbitAroundPlanet.radius);
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
            
                planetData.segment = 5;
                
            }else {
                planetData.radius = Patch.BaseTelluricSize;
                planetData.precision = 64;
                planetData.segment = 2;
            }
            Patch.Debug("Body Theme Def ( planet / Moon )", LogLevel.Debug,
                Patch.DebugReworkPlanetGen);
            
            //set Theme
            PlanetGen.SetPlanetTheme(planetData, star, gameDesc, 0, 0, rand1, rand2, rand3, rand4, theme_seed);
            star.galaxy.astroPoses[planetData.id].uRadius = planetData.realRadius;
            
            // Name Management :
            string name ; 
            if (planetData.IsNotAMoon()) {
                int nbPlanets = 0;
                foreach (var starPlanet in star.planets) {
                    if (starPlanet != null) {
                        if (starPlanet.IsNotAMoon()) {
                            nbPlanets++;
                        }
                    }
                }
                 name = star.name +  " - " + RomanNumbers.roman[nbPlanets + 1];
            }
            else {
                 name = planetData.orbitAroundPlanet.name + " - " + RomanNumbers.roman[number] ;
            }

            planetData.name = name;
           
            
            Patch.Debug("ReworkCreatePlanet Done !" , LogLevel.Debug,
                Patch.DebugReworkPlanetGen);

            return planetData;
        }
    }
}