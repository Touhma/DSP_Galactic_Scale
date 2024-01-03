using System;
using System.Collections.Generic;
using UnityEngine;

namespace GalacticScale
{
    public static partial class GS2
    {
        private static void ConfigureBirthStarHiveSettings(Random random, StarData starData)
        {
	        starData.hivePatternLevel = 0;
	        starData.safetyFactor = 0.847f + (float)random.NextDouble() * 0.026f;
	        var maxDensity = gameDesc.combatSettings.maxDensity;
	        float initialColonize = gameDesc.combatSettings.initialColonize;
	        Log($"Setting up Birth Star Hive System for {starData.name} Initial Colonize: {initialColonize} MaxDensity: {gameDesc.combatSettings.maxDensity}");


	        starData.maxHiveCount = Mathf.RoundToInt(maxDensity * 4f / 3f);
	        starData.maxHiveCount = Mathf.Clamp(starData.maxHiveCount, 1, 8);
	        starData.initialHiveCount = Mathf.Clamp(starData.initialHiveCount, 1, starData.maxHiveCount);
	        if (initialColonize < 0.015f)
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
	        // Log("Generating Hive Settings to " + star.name + "");
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
        
        private static void CreateDarkFogHive(StarData star, Random random)
        {
	        // Log($"Generating Hive Orbits For {star.name}");
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
                hiveAstroOrbits[i].orbitInclination = random.NextFloat();
                hiveAstroOrbits[i].orbitLongitude = random.NextFloat();
                hiveAstroOrbits[i].orbitPhase = random.NextFloat();
                hiveAstroOrbits[i].orbitalPeriod = Utils.CalculateOrbitPeriod(hiveAstroOrbits[i].orbitRadius);
                hiveAstroOrbits[i].orbitRotation = Quaternion.AngleAxis(hiveAstroOrbits[i].orbitLongitude, Vector3.up) *
                                                   Quaternion.AngleAxis(hiveAstroOrbits[i].orbitInclination,
                                                       Vector3.forward);
                hiveAstroOrbits[i].orbitNormal =
                    Maths.QRotateLF(hiveAstroOrbits[i].orbitRotation, new VectorLF3(0f, 1f, 0f)).normalized;
            }
            // Log($"Darkfog Hive Orbits Generated for {gsStar.Name}");
        }

        private static List<float> GeneratePossibleHiveOrbits(GSStar gsStar, int count = 10, Random random = null)
        {
            if (gsStar.PlanetCount == 0 || gsStar.Decorative) return new List<float>{1f,2f,3f,4f,5f,6f,7f,8f};
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
                    // GS2.Warn("Adding Extra Orbit Gap");
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