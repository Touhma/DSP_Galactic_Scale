using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using static GalacticScale.GS2;
using Random = UnityEngine.Random;

namespace GalacticScale.Generators
{
    public partial class GS2Generator2 : iConfigurableGenerator
    {
        public void GenerateBinaryStar(GSStar star)
        {
            var starType = random.Item(new List<EStar>()
            { EStar.M, EStar.BlackHole, EStar.WhiteDwarf, EStar.G, EStar.WhiteDwarf, EStar.WhiteDwarf, EStar.WhiteDwarf, EStar.WhiteDwarf, EStar.WhiteDwarf,EStar.M,EStar.M,EStar.M
            }).Convert();
            var binary = GSSettings.Stars.Add(new GSStar(random.Next(), star.Name+ "-B", starType.Item2,  starType.Item1, new GSPlanets()));
            binary.genData.Add("binary", true);
            star.genData.Add("hasBinary", true);
            var binaryRadius = StarDefaults.Radius(binary)* preferences.GetFloat("starSizeMulti", 2f);;
            binary.radius = Mathf.Clamp(star.radius * .6f, 0.01f, binaryRadius);
            binary.Decorative = true;
            var offset = (star.RadiusLY + binary.RadiusLY) * random.NextFloat(1.1f, 1.3f);
            star.genData.Add("binaryOffset", offset);
            binary.position = new VectorLF3(offset, 0, 0);
            star.luminosity += binary.luminosity;
            binary.luminosity = 0;
        }
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
                // star.dysonRadius =
                //     star.dysonRadius * Mathf.Clamp(preferences.GetFloat("starSizeMulti", 10f), 0.5f, 100f);
                star.dysonRadius = (float)((star.physicsRadius * 1.5f) / 40000.0);
                // Warn($"Dyson Radius for {star.Name}:{star.dysonRadius}");
                star.dysonRadius = Mathf.Clamp(star.dysonRadius, 0, 10f);
                //Warn($"Habitable zone for {star.Name} {Utils.CalculateHabitableZone(star.luminosity)}");
                star.Seed = random.Next();
                GSSettings.Stars.Add(star);
                if (preferences.GetInt("binaryChance", -1) != -1)
                {
                    var chance = preferences.GetInt("binaryChance") / 100.0;
                    if (i < starCount - 1 && random.NextPick(chance))
                    {
                        GS2.Log($"Creating Binary Companion Star for {star.Name}");
                        GenerateBinaryStar(star);
                        i++;
                    }
                }
                
            }
            var bsInt = preferences.GetInt("birthStar", 14);
            if (bsInt < 14)
            {
                var birthStarDesc = ((EStar)bsInt).Convert();
                var availBirthStars = (from s in GSSettings.Stars
                    where s.Type == birthStarDesc.Item1
                    where s.Spectr == birthStarDesc.Item2
                    where (s.Decorative == false)
                                       select s).ToList<GSStar>();
                // GS2.WarnJson(availBirthStars);
                // GS2.WarnJson(GSSettings.Stars);
                    birthStar = random.Item(availBirthStars);                   
            } else
            {
                var availBirthStars = (from s in GSSettings.Stars
                    where (s.Decorative == false)
                    select s).ToList<GSStar>();
                birthStar = random.Item(availBirthStars);
            }

            GS2.Warn(birthStar.Name + " is birthstar");
            if (forcedBirthStar != null)
            {
                // GS2.Warn("Forcing birthStar");
                foreach (var star in GSSettings.Stars)
                {
                    if (star.Name == forcedBirthStar) {
                        birthStar = star;
                        // GS2.Warn("birthStar forced");
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
            var size =  Mathf.Clamp(ClampedNormalSize(random, min, max, GetSizeBiasForStar(star)), min, GetMaxPlanetSizeForStar(star));
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
            return ClampedNormalSize(random, min, max, bias);
        }
        private FloatPair CalculateHabitableZone(GSStar star)
        {
            var lum = star.luminosity;
            var flp = Utils.CalculateHabitableZone(lum);
            var min = flp.low;
            var max = flp.high;
            min += star.RadiusAU;
            max += star.RadiusAU;
            var sl = GetTypeLetterFromStar(star);
            if (preferences.GetBool($"{sl}hzOverride"))
            {
                var fp = preferences.GetFloatFloat($"{sl}hz", new FloatPair(0,2));
                min = fp.low;
                max = fp.high;
            }
            if (star.genData.Get("hasBinary", false))
            {
                min += star.genData.Get("binaryOffset", 1);
                max += star.genData.Get("binaryOffset", 1);
            }
            star.genData.Set("minHZ", min);
            star.genData.Set("maxHZ", max);
            // GS2.Warn($"HZ of {star.Name} {min}:{max}");
            return new FloatPair(min, max);
        }

        private float CalculateMinimumOrbit(GSStar star)
        {
            var sl = GetTypeLetterFromStar(star);
            
            var radius = star.RadiusAU;
            var lum = star.luminosity;
            var min = radius +( 0.2f * radius * Mathf.Sqrt(Mathf.Sqrt(lum)));
            if (star.genData.Get("hasBinary", false)) min += star.genData.Get("binaryOffset", 1);
            if (preferences.GetBool($"{sl}orbitOverride"))
            {
                var fp = preferences.GetFloatFloat($"{sl}orbits", new FloatPair(0.02f,20f));
                min = fp.low;
            }
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
            var maxOrbitByRadius = Mathf.Pow(star.radius, 0.75f);
            var maxOrbitByHabitableZone = 2f * hzMax;
            var maxByPlanetCount = star.bodyCount * 0.3f;
            // float density = (2f*GetSystemDensityBiasForStar(star))/100f;
            // GS2.Warn($"Density:{density} MaxOrbit:{star.MaxOrbit}");
            var max = Mathf.Clamp(Mathf.Max(maxByPlanetCount, minMaxOrbit, maxOrbitByLuminosity, maxOrbitByRadius, maxOrbitByHabitableZone), star.genData.Get("minOrbit")*2f, star.MaxOrbit);
            if (preferences.GetBool($"{sl}orbitOverride"))
            {
                var fp = preferences.GetFloatFloat($"{sl}orbits", new FloatPair(0.02f,20f));
                max = fp.high;
            }
            // Warn($"Getting Max Orbit for Star {star.Name} HardCap:{star.MaxOrbit} MaxbyRadius({star.radius}):{maxOrbitByRadius} MaxbyPlanets({star.PlanetCount}):{maxByPlanetCount} MaxbyLum({lum}):{maxOrbitByLuminosity} MaxByHZ({hzMax}):{maxOrbitByHabitableZone} Max({max}):{max} HabitableZone:{star.genData.Get("minHZ")}:{hzMax}");
            if (star.genData.Get("hasBinary", false)) max += star.genData.Get("binaryOffset", 1);
            star.genData.Set("maxOrbit", max);
            return max;
        }
    }
}