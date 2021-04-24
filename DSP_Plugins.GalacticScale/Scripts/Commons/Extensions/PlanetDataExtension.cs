using UnityEngine;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.Bootstrap;

namespace GalacticScale.Scripts {
    public static class PlanetDataExtension {



        public static float VanillaSizeGasGiant = 80f;
        public static int VanillaPrecisionGasGiant = 64;
        public static float VanillaScaleGasGiant = 10f;
        public static float VanillaSizeTelluric = 200f;
        public static int VanillaPrecisionTelluric = 200;
        public static float VanillaScaleTelluric = 1f;
        public static bool IsAMoon(this PlanetData planet) {
            return planet.orbitAround != 0;
        }

        public static bool IsNotAMoon(this PlanetData planet) {
            return planet.orbitAround == 0;
        }

        public static bool IsGasGiant(this PlanetData planet) {
            return planet.type == EPlanetType.Gas;
        }
        public static void ShouldBeHabitable(this PlanetData planet) {
            planet.type = EPlanetType.Ocean;
            planet.theme = 1;
            planet.algoId = 1;
        }
        public static void HasMultipleSatellites(this PlanetData planet) {
            planet.singularity |= EPlanetSingularity.MultipleSatellites;
        }

        public static void HasLayingObliquity(this PlanetData planet) {
            planet.singularity |= EPlanetSingularity.LaySide;
        }

        public static void HasRetrogradeOrbit(this PlanetData planet) {
            planet.rotationPeriod = -planet.rotationPeriod;
            planet.singularity |= EPlanetSingularity.ClockwiseRotate;
        }
        public static float GetGasGiantOrbitScaler(this PlanetData planet) {
            var orbitScaler = 1f;
            if (planet.type == EPlanetType.Gas) {
                orbitScaler = planet.radius / VanillaSizeGasGiant;
                if (orbitScaler < 1) orbitScaler = 1;
            }

            return orbitScaler;
        }
        public static void SetSize(this PlanetData planet, float radius) {
            planet.radius = radius;
            if (planet.type == EPlanetType.Gas) planet.scale = 10f;
        }

        public static float GetScaleFactored(this PlanetData planet) {
            float scale;

            if (planet.type == EPlanetType.Gas) return planet.radius / VanillaSizeGasGiant;

            scale = planet.radius / VanillaSizeTelluric;
                       
            return scale;
        }

        public static int GetPrecisionFactored(this PlanetData planet) {
            // do not use it for gas giant YET : not tested
            if (planet.type == EPlanetType.Gas) return Mathf.RoundToInt(planet.scale * VanillaPrecisionGasGiant);

            var precision = Mathf.RoundToInt(planet.scale * VanillaPrecisionTelluric);

            //In the meantime while i'm fixing the hard coded limit later
            if (precision > VanillaPrecisionTelluric) return VanillaPrecisionTelluric;

            return precision;
        }

        public static void IsTidallyLocked(this PlanetData planet, TidalLevel level) {
            switch (level) {
                case TidalLevel.TidalLocked:
                    planet.singularity |= EPlanetSingularity.TidalLocked;
                    break;
                case TidalLevel.TidalLocked2:
                    planet.singularity |= EPlanetSingularity.TidalLocked2;
                    break;
                case TidalLevel.TidalLocked4:
                    planet.singularity |= EPlanetSingularity.TidalLocked4;
                    break;
            }
        }
        public static bool HasSingularityFlag(this PlanetData planet, EPlanetSingularity singularity)
        {
            return ((planet.singularity & singularity) != EPlanetSingularity.None);
        }
        public static void OrbitAroundNeutronStar(this PlanetData planet) {
          //  planet.orbitInclination *= Patch.NeutronStarOrbitInclinationFactor.Value;
        }
    }
}