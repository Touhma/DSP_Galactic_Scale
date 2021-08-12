using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using static GalacticScale.GS2;

namespace GalacticScale.Generators
{
    public partial class GS2Generator2 : iConfigurableGenerator
    {
        
        public void GenerateStars(int starCount, int startID = 0)
        {
            Log("Generating Stars");
            var birthIndex = random.Next(starCount);
            for (var i = startID; i < starCount; i++)
            {
                var (type, spectr) = ChooseStarType((i == birthIndex));
                var star = new GSStar(random.Next(), SystemNames.GetName(i), spectr, type,
                    new GSPlanets());
                if (star.Type != EStarType.BlackHole) star.radius *= preferences.GetFloat("starSizeMulti", 10f);
                if (star.Type == EStarType.BlackHole && preferences.GetFloat("starSizeMulti", 10f) < 2.01f)
                    star.radius *= preferences.GetFloat("starSizeMulti", 2f);
                star.dysonRadius =
                    star.dysonRadius * Mathf.Clamp(preferences.GetFloat("starSizeMulti", 10f), 0.5f, 100f);
                //Warn($"Habitable zone for {star.Name} {Utils.CalculateHabitableZone(star.luminosity)}");
                star.Seed = random.Next();
                GSSettings.Stars.Add(star);
            }
            var bsInt = preferences.GetInt("birthStar", 14);
            if (bsInt < 14)
            {
                var birthStarDesc = ((EStar)bsInt).Convert();
                var availBirthStars = (from s in GSSettings.Stars
                                       where s.Type == birthStarDesc.Item1
                                       where s.Spectr == birthStarDesc.Item2
                                       select s).ToList<GSStar>();
                // GS2.WarnJson(availBirthStars);
                // GS2.WarnJson(GSSettings.Stars);
                    birthStar = random.Item(availBirthStars);                   
            } else  birthStar = random.Item(GSSettings.Stars);

            if (forcedBirthStar != null)
            {
                GS2.Warn("Forcing birthStar");
                foreach (var star in GSSettings.Stars)
                {
                    if (star.Name == forcedBirthStar) {
                        birthStar = star;
                        GS2.Warn("birthStar forced");
                        break;
                    }
                }
            }
            
        }
        private int GetStarPlanetCount(GSStar star)
        {
            var min = GetMinPlanetCountForStar(star);
            var max = GetMaxPlanetCountForStar(star);
            //int result = random.NextInclusive(min, max);
            var result = ClampedNormal(new GS2.Random(star.Seed), min, max, GetCountBiasForStar(star));
            //Log($"{star.Name} count :{result} min:{min} max:{max}");
            return result;
        }
        private int GetStarMoonSize(GSStar star, int hostRadius, bool hostGas)
        {
            if (hostGas) hostRadius *= 10;
            var min = Utils.ParsePlanetSize(GetMinPlanetSizeForStar(star));
            int max;
            if (preferences.GetBool("moonsAreSmall", true))
            {
                float divider = 2;
                if (hostGas) divider = 4;
                max = Utils.ParsePlanetSize(Mathf.RoundToInt(hostRadius / divider));
            }
            else
            {
                max = Utils.ParsePlanetSize(hostRadius - 10);
            }

            if (max <= min) return min;
            float average = (max - min) / 2 + min;
            var range = max - min;
            var sd = (float) range / 4;
            //int size = Utils.ParsePlanetSize(random.Next(min, max));
            var size =  Mathf.Clamp(ClampedNormalSize(new GS2.Random(star.Seed), min, max, GetSizeBiasForStar(star)), min, GetMaxPlanetSizeForStar(star));
            //if (size > hostRadius)
            //{
            //Warn($"MoonSize {size} selected for {star.Name} moon with host size {hostRadius} avg:{average} sd:{sd} max:{max} min:{min} range:{range} hostGas:{hostGas}");
            //    size = Utils.ParsePlanetSize(hostRadius - 10);
            //}
            return size;
        }
        private int GetStarPlanetSize(GSStar star)
        {
            var min = GetMinPlanetSizeForStar(star);
            var max = GetMaxPlanetSizeForStar(star);
            var bias = GetSizeBiasForStar(star);
            return ClampedNormalSize(new GS2.Random(star.Seed), min, max, bias);
        }
        private (float min, float max) CalculateHabitableZone(GSStar star)
        {
            var lum = star.luminosity;
            var (min, max) = Utils.CalculateHabitableZone(lum);
            var sl = GetTypeLetterFromStar(star);
            if (preferences.GetBool($"{sl}hzOverride")) (min, max) = preferences.GetFloatFloat($"{sl}hz", (0,2));
            star.genData.Set("minHZ", min);
            star.genData.Set("maxHZ", max);
            // GS2.Warn($"HZ of {star.Name} {min}:{max}");
            return (min, max);
        }

        private float CalculateMinimumOrbit(GSStar star)
        {
            var sl = GetTypeLetterFromStar(star);
            
            var radius = star.RadiusAU;
            var lum = star.luminosity;
            var min = radius +( 0.2f * radius * Mathf.Sqrt(Mathf.Sqrt(lum)));
            
            if (preferences.GetBool($"{sl}orbitOverride")) (min, _) = preferences.GetFloatFloat($"{sl}orbits", (0.02f,20f));
            min = Mathf.Clamp(min , radius * 1.1f, 100f);
            star.genData.Set("minOrbit", min);
            // Warn($"Getting Min Orbit for Star {star.Name} Min:{min}");
            return min;
        }

        private float CalculateMaximumOrbit(GSStar star)
        {
            var sl = GetTypeLetterFromStar(star);

            var minMaxOrbit = 5f;
            var lum = star.luminosity;
            var hzMax = star.genData.Get("maxHZ");
            var maxOrbitByLuminosity = lum * 4f;
            var maxOrbitByRadius = Mathf.Sqrt(star.radius);
            var maxOrbitByHabitableZone = 2f * hzMax;
            var maxByPlanetCount = star.bodyCount * 0.3f;
            // float density = (2f*GetSystemDensityBiasForStar(star))/100f;
            // GS2.Warn($"Density:{density} MaxOrbit:{star.MaxOrbit}");
            var max = Mathf.Clamp(Mathf.Max(maxByPlanetCount, minMaxOrbit, maxOrbitByLuminosity, maxOrbitByRadius, maxOrbitByHabitableZone), star.genData.Get("minOrbit")*2f, star.MaxOrbit);
            if (preferences.GetBool($"{sl}orbitOverride")) (_, max) = preferences.GetFloatFloat($"{sl}orbits", (0.02f,20f));
            // Warn($"Getting Max Orbit for Star {star.Name} MaxbyRadius({star.radius}):{maxOrbitByRadius} MaxbyPlanets({star.PlanetCount}):{maxByPlanetCount} MaxbyLum({lum}):{maxOrbitByLuminosity} MaxByHZ({hzMax}):{maxOrbitByHabitableZone} Max({max}):{max} HabitableZone:{star.genData.Get("minHZ")}:{hzMax}");
            star.genData.Set("maxOrbit", max);
            return max;
        }
    }
}