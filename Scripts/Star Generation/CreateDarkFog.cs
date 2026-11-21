using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static bool GetActiveGeneratorBoolPreference(string key, bool defaultValue)
        {
            var configurable = ActiveGenerator as iConfigurableGenerator;
            return configurable == null ? defaultValue : configurable.Export().GetBool(key, defaultValue);
        }

        public static bool AllowDarkFogOnBirthPlanet => GetActiveGeneratorBoolPreference("birthDarkFogPlanet", true);

        public static void ApplyActiveGeneratorDarkFogSettings(GameDesc desc)
        {
            var configurable = ActiveGenerator as iConfigurableGenerator;
            if (configurable == null) return;
            var settings = configurable.Export();
            if (!settings.ContainsKey("darkFogEnabled")) return;

            desc.isPeaceMode = !settings.GetBool("darkFogEnabled", true);
            var combat = desc.combatSettings;
            combat.aggressiveness = MapDarkFogAggressiveness(settings.GetFloat("darkFogAggressiveness", 1f));
            combat.initialLevel = Mathf.Clamp(Mathf.Round(settings.GetFloat("darkFogInitialLevel", 0f)), 0f, 10f);
            combat.initialGrowth = MapDarkFogGrowth(settings.GetFloat("darkFogInitialGrowth", 1f));
            combat.initialColonize = MapDarkFogColonize(settings.GetFloat("darkFogInitialColonize", 1f));
            combat.maxDensity = 1f + Mathf.Clamp(Mathf.Round(settings.GetFloat("darkFogMaxDensityStep", 0f)), 0f, 4f) * 0.5f;
            combat.growthSpeedFactor = MapDarkFogGrowthSpeed(settings.GetFloat("darkFogGrowthSpeed", 1f));
            combat.powerThreatFactor = MapDarkFogThreat(settings.GetFloat("darkFogPowerThreat", 1f));
            combat.battleThreatFactor = MapDarkFogThreat(settings.GetFloat("darkFogBattleThreat", 1f));
            combat.battleExpFactor = MapDarkFogThreat(settings.GetFloat("darkFogBattleExp", 1f));
            desc.combatSettings = combat;
        }

        private static float MapDarkFogAggressiveness(float value) => value < .5f ? -1f : value < 1.5f ? 0f : value < 2.5f ? .5f : value < 3.5f ? 1f : value < 4.5f ? 2f : 3f;
        private static float MapDarkFogGrowth(float value) => value < .5f ? 0f : value < 1.5f ? .25f : value < 2.5f ? .5f : value < 3.5f ? .75f : value < 4.5f ? 1f : value < 5.5f ? 1.5f : 2f;
        private static float MapDarkFogColonize(float value) => value < .5f ? .01f : value < 1.5f ? .25f : value < 2.5f ? .5f : value < 3.5f ? .75f : value < 4.5f ? 1f : value < 5.5f ? 1.5f : 2f;
        private static float MapDarkFogGrowthSpeed(float value) => value < .5f ? .25f : value < 1.5f ? .5f : value < 2.5f ? 1f : value < 3.5f ? 2f : 3f;
        private static float MapDarkFogThreat(float value) => value < .5f ? .01f : value < 1.5f ? .1f : value < 2.5f ? .2f : value < 3.5f ? .5f : value < 4.5f ? 1f : value < 5.5f ? 2f : value < 6.5f ? 5f : value < 7.5f ? 8f : 10f;

        public static void ConfigureBirthStarHiveSettings(Random random, StarData starData)
        {
	        starData.hivePatternLevel = 0;
	        starData.safetyFactor = 0.847f + (float)random.NextDouble() * 0.026f;
	        var maxDensity = gameDesc.combatSettings.maxDensity;
	        float initialColonize = gameDesc.combatSettings.initialColonize;
	        Log($"Setting up Birth Star Hive System for {starData.name} Initial Colonize: {initialColonize} MaxDensity: {gameDesc.combatSettings.maxDensity}");


	        starData.maxHiveCount = Mathf.RoundToInt(maxDensity * 4f / 3f);
	        starData.maxHiveCount = Mathf.Clamp(starData.maxHiveCount, 1, 8);
	        starData.initialHiveCount = Mathf.RoundToInt(initialColonize/2 * (starData.maxHiveCount -0.2f));
	        starData.initialHiveCount = Mathf.Clamp(starData.initialHiveCount, 1, starData.maxHiveCount);
	        if (!GetActiveGeneratorBoolPreference("birthDarkFogSystem", true))
	        {
		        starData.initialHiveCount = 0;
		        starData.maxHiveCount = 0;
	        }
	        else if (initialColonize < 0.015f)
	        {
		        Log("Preventing Birth System from having a Hive");
		        starData.initialHiveCount = 0;
	        }
	        Log($"Birth System ({starData.name}) Hive Settings Applied : " + starData.initialHiveCount + " / " + starData.maxHiveCount);
	    }

        public static void ConfigureStarHiveSettings(Random random, StarData star)
        {
	        float initialColonize = gameDesc.combatSettings.initialColonize;
	        var maxDensity = gameDesc.combatSettings.maxDensity;
	        Log("Generating Hive Settings to " + star.name + "");
	        var level = star.level;
	        bool epic = star.type == EStarType.BlackHole || star.type == EStarType.NeutronStar;
	        star.hivePatternLevel = 0;
	        star.safetyFactor = 0.847f + (float)random.NextDouble() * 0.026f;
	        if (epic) star.safetyFactor = 1;
	        star.safetyFactor *= (maxDensity / 3f);
	        star.safetyFactor = Mathf.Clamp01(star.safetyFactor);
	        
	        
			star.maxHiveCount = Mathf.RoundToInt(maxDensity * 4f / 3f);
			star.maxHiveCount += Mathf.RoundToInt(4f*level);
	        
	        
	        // Log($"Setting up Star Hive System for {star.name} {star.typeString} {star.spectr} Initial Colonize: {initialColonize} MaxDensity: {gameDesc.combatSettings.maxDensity}");

	        if (epic) star.maxHiveCount += 2;
	        
	        
	        if (initialColonize < 0.015f)
	        {
		        // Log("Preventing System from having a Hive");
		        star.initialHiveCount = 0;
	        }
	        else
	        {
		        star.initialHiveCount = Mathf.RoundToInt(initialColonize/2 * (star.maxHiveCount -0.2f));
		        star.initialHiveCount += Mathf.RoundToInt(level * 2f);
		        
		        
	        }
	        star.maxHiveCount = Mathf.Clamp(star.maxHiveCount, 0, 8);
	        star.initialHiveCount = Mathf.Clamp(star.initialHiveCount, 0, star.maxHiveCount);
	        // Log($"Level {star.level} System ({star.name} {star.typeString} {star.spectr}) Hive Settings Applied : " + star.initialHiveCount + " / " + star.maxHiveCount +$"Initial Colonize: {initialColonize} MaxDensity: {gameDesc.combatSettings.maxDensity}");
        }
        
        public static void CreateDarkFogHive(StarData star, Random random)
        {
	        Log($"Generating Hive Orbits For {star.name}");
            GSStar gsStar = GetGSStar(star);


            // if (gsStar == GSSettings.BirthStar)
            // {
            //     star.initialHiveCount = 1;
            //     star.maxHiveCount = 2;
            //     star.hivePatternLevel = 1;
            // }
            var possibleHiveOrbits = GeneratePossibleHiveOrbits(gsStar);

            var hiveCount = 8;
            if (gsStar.Decorative || gsStar.PlanetCount == 0)
            {
	            star.initialHiveCount = 0;
	            star.maxHiveCount = 0;
	            star.hivePatternLevel = 0;
	            // star.hiveAstroOrbits = new AstroOrbitData[] { };
	            // return;
            }
            star.hiveAstroOrbits = new AstroOrbitData[hiveCount];
            AstroOrbitData[] hiveAstroOrbits = star.hiveAstroOrbits;
            for (int i = 0; i < hiveCount; i++)
            {
                hiveAstroOrbits[i] = new AstroOrbitData();
                var orbit = random.ItemAndRemove(possibleHiveOrbits);
                hiveAstroOrbits[i].orbitRadius = orbit;
                // Warn($"Created Hive Orbit at {star.name} {Utils.Round2DP(hiveAstroOrbits[i].orbitRadius)}");
                SetHiveOrbitOrientation(hiveAstroOrbits[i], random);
                hiveAstroOrbits[i].orbitalPeriod = Utils.CalculateOrbitPeriod(hiveAstroOrbits[i].orbitRadius);
            }
            // Log($"Darkfog Hive Orbits Generated for {gsStar.Name}");
        }

        public static void SetHiveOrbitOrientation(AstroOrbitData orbit, Random random)
        {
            var inclinationSample = random.NextDouble() * 2.0 - 1.0;
            orbit.orbitInclination = (float)(Math.Sign(inclinationSample) *
                                             Math.Pow(Math.Abs(inclinationSample), 0.7) * 90.0);
            orbit.orbitLongitude = random.NextFloat(360f);
            orbit.orbitPhase = random.NextFloat(360f);
            orbit.orbitRotation = Quaternion.AngleAxis(orbit.orbitLongitude, Vector3.up) *
                                  Quaternion.AngleAxis(orbit.orbitInclination, Vector3.forward);
            orbit.orbitNormal = Maths.QRotateLF(orbit.orbitRotation, new VectorLF3(0f, 1f, 0f)).normalized;
        }

        public static List<float> GeneratePossibleHiveOrbits(GSStar gsStar, int count = 10, Random random = null)
        {
            var minimumHiveOrbit = Mathf.Max(0f, gsStar.RadiusAU + 0.1f);
            var orbitCount = Mathf.Max(0, count - 1);
            if (gsStar.PlanetCount == 0 || gsStar.Decorative)
            {
                var fallbackOrbits = new List<float>(orbitCount);
                for (var i = 1; i <= orbitCount; i++)
                    fallbackOrbits.Add(minimumHiveOrbit + i * 0.5f);
                return fallbackOrbits;
            }

            List<(float inner, float outer)> ExistingOrbits = new();
            List<(float inner, float outer)> ExistingGaps = new();
            List<float> PossibleOrbits = new();
            var MaxOrbit = Mathf.Max(gsStar.SystemRadius, minimumHiveOrbit + 1f);//gsStar.luminosity * 2f;
            if (random == null) random = new Random(Mathf.CeilToInt( gsStar.luminosity*gsStar.age*gsStar.SystemRadius));
            foreach (GSPlanet p in gsStar.Planets)
            {
                ExistingOrbits.Add((p.OrbitRadius-p.SystemRadius, p.OrbitRadius+p.SystemRadius));
            }
            ExistingOrbits.Sort((left, right) => left.inner.CompareTo(right.inner));
            if (ExistingOrbits.Count == 0)
            {
                for (var i = 1; i <= orbitCount; i++)
                    PossibleOrbits.Add(minimumHiveOrbit + i * 0.5f);
                return PossibleOrbits;
            }

            if (ExistingOrbits[0].inner > minimumHiveOrbit + 0.1f)
                ExistingGaps.Add((minimumHiveOrbit, ExistingOrbits[0].inner));
            for (var i=0;i<ExistingOrbits.Count-1;i++)
            {
                var gapInner = Mathf.Max(ExistingOrbits[i].outer, minimumHiveOrbit);
                var gapOuter = ExistingOrbits[i+1].inner;
                if (gapOuter - gapInner > 0.1f)
                    ExistingGaps.Add((gapInner, gapOuter));
            }


            var outerGapInner = Mathf.Max(ExistingOrbits[ExistingOrbits.Count -1].outer, minimumHiveOrbit);
            if (outerGapInner < MaxOrbit)
            {
                ExistingGaps.Add((outerGapInner, MaxOrbit));
            }
            for (var i = 0; i < orbitCount; i++)
            {
                if (ExistingGaps.Count == 0)
                {
                    // GS2.Error($"No Existing Gaps! Cycle:{i}/{count} Possible Orbits: {PossibleOrbits.Count} Existing Orbits: {ExistingOrbits.Count} Existing Gaps: {ExistingGaps.Count}");
                    // LogJson(ExistingOrbits);
                    // GS2.Warn("Adding Extra Orbit Gap");
                    var oldMaxOrbit = Mathf.Max(MaxOrbit, minimumHiveOrbit);
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
