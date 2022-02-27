using System;
using System.Collections.Generic;
using System.Linq;
using static GalacticScale.GS2;
using static UnityEngine.Mathf;

namespace GalacticScale.Generators
{
    public partial class GS2Generator2 : iConfigurableGenerator
    {
        public void GenerateBinaryStar(GSStar star)
        {
            // { EStar.M, EStar.BlackHole, EStar.WhiteDwarf, EStar.G, EStar.WhiteDwarf, EStar.WhiteDwarf, EStar.WhiteDwarf, EStar.WhiteDwarf, EStar.WhiteDwarf,EStar.M,EStar.M,EStar.M
            // }).Convert();
            var availStarTypes = new List<(EStarType, ESpectrType)>();
            for (var i = 0; i < 14; i++)
            {
                var enabled = preferences.GetBool($"{typeLetter[i]}binaryEnabled");
                if (enabled) availStarTypes.Add(GetStarTypeSpectrFromLetter(typeLetter[i]));
            }

            if (availStarTypes.Count == 0) availStarTypes.Add((EStarType.MainSeqStar, ESpectrType.K));
            var starType = random.Item(availStarTypes);
            var binary = GSSettings.Stars.Add(new GSStar(random.Next(), star.Name + "-B", starType.Item2, starType.Item1, new GSPlanets()));
            binary.genData.Add("binary", true);
            star.genData.Add("hasBinary", true);
            star.BinaryCompanion = binary.Name;
            var binaryRadius = StarDefaults.Radius(binary) * preferences.GetFloat("starSizeMulti", 2f);
            ;
            binary.radius = Clamp(star.radius * .6f, 0.01f, binaryRadius);
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
                var (type, spectr) = ChooseStarType(i == birthIndex);
                var star = new GSStar(random.Next(), SystemNames.GetName(i), spectr, type, new GSPlanets());
                if (star.Type != EStarType.BlackHole) star.radius *= preferences.GetFloat("starSizeMulti", 10f);
                if (star.Type == EStarType.BlackHole && preferences.GetFloat("starSizeMulti", 10f) < 2.01f)
                    star.radius *= preferences.GetFloat("starSizeMulti", 2f);
                // star.dysonRadius =
                //     star.dysonRadius * Mathf.Clamp(preferences.GetFloat("starSizeMulti", 10f), 0.5f, 100f);
                star.dysonRadius = (float)(star.physicsRadius * 1.5f / 40000.0);
                // Warn($"Dyson Radius for {star.Name}:{star.dysonRadius}");
                star.dysonRadius = Clamp(star.dysonRadius, 0, 10f);
                //Warn($"Habitable zone for {star.Name} {Utils.CalculateHabitableZone(star.luminosity)}");
                star.Seed = random.Next();
                GSSettings.Stars.Add(star);
                if (preferences.GetInt("binaryChance") != -1)
                {
                    var chance = preferences.GetInt("binaryChance") / 100.0;
                    if (i < starCount - 1 && random.NextPick(chance) && birthIndex != i + 1)
                    {
                        // GS2.Log($"Creating Binary Companion Star for {star.Name}");
                        GenerateBinaryStar(star);
                        i++;
                    }
                }

                if (GetLuminosityBoostForStar(star) > 0)
                {
                    var lum = Round((float)Math.Pow(star.luminosity, 0.33000001311302185) * 1000f) / 1000f;
                    lum *= GetLuminosityBoostForStar(star);
                    star.luminosity = (float)Math.Pow(lum, 3.0);
                }
            }

            var bsInt = preferences.GetInt("birthStar", 14);
            if (bsInt < 14)
            {
                var birthStarDesc = ((EStar)bsInt).Convert();
                var availBirthStars = (from s in GSSettings.Stars where s.Type == birthStarDesc.Item1 where s.Spectr == birthStarDesc.Item2 where s.Decorative == false select s).ToList();
                // GS2.Warn($"Stars that are {birthStarDesc.Item1} {birthStarDesc.Item2}");
                // GS2.WarnJson(availBirthStars);
                // GS2.WarnJson(GSSettings.Stars);
                birthStar = random.Item(availBirthStars);
            }
            else
            {
                var availBirthStars = (from s in GSSettings.Stars where s.Decorative == false select s).ToList();
                birthStar = random.Item(availBirthStars);
            }

            Log(birthStar.Name + " is birthstar");
            if (forcedBirthStar != null)
                // GS2.Warn("Forcing birthStar");
                foreach (var star in GSSettings.Stars)
                    if (star.Name == forcedBirthStar)
                    {
                        birthStar = star;
                        // GS2.Warn("birthStar forced");
                        break;
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
                max = Utils.ParsePlanetSize(RoundToInt(hostRadius / divider));
            }
            else
            {
                max = Utils.ParsePlanetSize(hostRadius - 10);
            }

            if (max <= min) return ParsePlanetSize(min);
            float average = (max - min) / 2 + min;
            var range = max - min;
            var sd = (float)range / 4;
            //int size = Utils.ParsePlanetSize(random.Next(min, max));
            var size = Clamp(ClampedNormalSize(random, min, max, GetSizeBiasForStar(star)), min, GetMaxPlanetSizeForStar(star));
            //if (size > hostRadius)
            //{
            //Warn($"MoonSize {size} selected for {star.Name} moon with host size {hostRadius} avg:{average} sd:{sd} max:{max} min:{min} range:{range} hostGas:{hostGas}");
            //    size = Utils.ParsePlanetSize(hostRadius - 10);
            //}
            var result = ParsePlanetSize(size);
            // Warn(result +$"is size for {star.Name} moon");
            return result;
            // return size;
        }

        private int GetStarPlanetSize(GSStar star)
        {
            // GS2.Warn("GetStarPlanetSize");
            var min = GetMinPlanetSizeForStar(star);
            var max = GetMaxPlanetSizeForStar(star);
            var bias = GetSizeBiasForStar(star);
            var size = ClampedNormalSize(random, min, max, bias);
            return ParsePlanetSize(size);
        }

        private int ParsePlanetSize(int size)
        {
            var sizes = new List<int>();
            // GS2.Log("Getting List");
            if (preferences.GetBool("limitPlanetSize20")) sizes.Add(20);
            if (preferences.GetBool("limitPlanetSize50")) sizes.Add(50);
            if (preferences.GetBool("limitPlanetSize100")) sizes.Add(100);
            if (preferences.GetBool("limitPlanetSize200")) sizes.Add(200);
            if (preferences.GetBool("limitPlanetSize300")) sizes.Add(300);
            if (preferences.GetBool("limitPlanetSize400")) sizes.Add(400);
            if (preferences.GetBool("limitPlanetSize500")) sizes.Add(500);
            // GS2.Log("Got List "+sizes.Count);
            var count = sizes.Count;
            if (count == 0) return size;
            // Log("Count wasnt 0");
            var largest = sizes[sizes.Count - 1];
            var smallest = sizes[0];
            // GS2.Warn($"Size:{size} Count:{sizes.Count}");

            if (size <= smallest) return smallest;
            if (size >= largest) return largest;
            if (count == 1) return smallest;
            if (count == 2)
            {
                if (Abs(size - smallest) < Abs(largest - size)) return smallest;
                return largest;
            }

            for (var i = 1; i < sizes.Count; i++)
            {
                var larger = sizes[i];
                if (size > larger) continue;
                var smaller = sizes[i - 1];
                if (Abs(size - smaller) < Abs(larger - size)) return smaller;
                return larger;
            }

            return largest;
        }

        private FloatPair CalculateHabitableZone(GSStar star)
        {
            // Warn($"Calculating Habitable Zone for {star.Name}");
            var lum = star.luminosity;
            var flp = Utils.CalculateHabitableZone(lum);
            var min = flp.low;
            var max = flp.high;
            min += star.RadiusAU;
            max += star.RadiusAU;
            var sl = GetTypeLetterFromStar(star);
            if (preferences.GetBool($"{sl}hzOverride"))
            {
                var fp = preferences.GetFloatFloat($"{sl}hz", new FloatPair(0, 2));
                min = fp.low;
                max = fp.high;
            }

            if (star.genData.Get("hasBinary", false))
            {
                // Warn($"{star.Name} Binary offset : {star.genData.Get("binaryOffset", 1f)}");
                // Warn($"Increasing by {star.genData.Get("binaryOffset", 1) * 60f}");
                // Warn($"Star RadiusAU:{star.RadiusAU}");
                // WarnJson(GSSettings.Stars.Select(o => (o.Name, o.RadiusAU)).ToList());
                min += star.genData.Get("binaryOffset", 1f) * 60f;
                max += star.genData.Get("binaryOffset", 1f) * 60f;
            }

            star.genData.Set("minHZ", min);
            star.genData.Set("maxHZ", max);
            // Warn($"HZ of {star.Name} {min}:{max}");
            return new FloatPair(min, max);
        }

        private float CalculateMinimumOrbit(GSStar star)
        {
            // Warn($"Calculating Minimum Orbit for {star.Name}");
            var sl = GetTypeLetterFromStar(star);

            var radius = star.RadiusAU;
            var lum = star.luminosity;
            var min = radius + 0.2f * radius * Sqrt(Sqrt(lum));

            if (preferences.GetBool($"{sl}orbitOverride"))
            {
                var fp = preferences.GetFloatFloat($"{sl}orbits", new FloatPair(0.02f, 20f));
                // Warn($"Using Star Type Override {fp.low}");
                min = fp.low;
            }

            if (star.genData.Get("hasBinary", false))
            {
                // Warn("Increasing for Binary");
                min += star.genData.Get("binaryOffset", 1f) * 60f;
            }

            min = Clamp(min, radius * 1.1f, 100f);
            star.genData.Set("minOrbit", min);
            // Warn($"Getting Min Orbit for Star {star.Name} Min:{min}");
            return min;
        }

        private float CalculateMaximumOrbit(GSStar star)
        {
            var sl = GetTypeLetterFromStar(star);
            // Warn($"Calculating Maximum Orbit for {star.Name}");
            var minMaxOrbit = 5f;
            var lum = star.luminosity;
            var hzMax = star.genData.Get("maxHZ");
            var maxOrbitByLuminosity = lum * 4f;
            var maxOrbitByRadius = Pow(star.radius, 0.75f);
            var maxOrbitByHabitableZone = 2f * hzMax;
            var maxByPlanetCount = star.bodyCount * 0.3f;
            // float density = (2f*GetSystemDensityBiasForStar(star))/100f;
            // GS2.Warn($"Density:{density} MaxOrbit:{star.MaxOrbit}");
            var max = Clamp(Max(maxByPlanetCount, minMaxOrbit, maxOrbitByLuminosity, maxOrbitByRadius, maxOrbitByHabitableZone), star.genData.Get("minOrbit") * 2f, star.MaxOrbit);
            if (preferences.GetBool($"{sl}orbitOverride"))
            {
                var fp = preferences.GetFloatFloat($"{sl}orbits", new FloatPair(0.02f, 20f));
                max = fp.high;
                // Warn($"Using Star Type Override {fp.high} from {fp} in {sl}orbits");
            }

            // Warn($"Getting Max Orbit for Star {star.Name}\r\n HardCap:{star.MaxOrbit} \r\nMaxbyRadius({star.radius}):{maxOrbitByRadius} \r\nMaxbyPlanets({star.PlanetCount}):{maxByPlanetCount} \r\nMaxbyLum({lum}):{maxOrbitByLuminosity} \r\nMaxByHZ({hzMax}):{maxOrbitByHabitableZone} \r\n HabitableZone:{star.genData.Get("minHZ")}:{hzMax}");
            // Warn($"Final Max({max}):{max}");
            if (star.genData.Get("hasBinary", false))
            {
                // Warn("Increasing Max Orbit for Binary");
                max += star.genData.Get("binaryOffset", 1f) * 60f;
            }

            star.genData.Set("maxOrbit", max);
            return max;
        }
    }
}