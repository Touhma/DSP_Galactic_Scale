using UnityEngine;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static void CreateStarPlanetsAstroPoses(Random random)
        {
            var highStopwatch = new HighStopwatch();
            highStopwatch.Begin();
            var gSize = galaxy.starCount * 1000;
            galaxy.astroPoses = new AstroPose[gSize];
            Log("Creating Stars");
            for (var i = 0; i < GSSettings.StarCount; i++) galaxy.stars[i] = CreateStar(i, random);
            if (!GSSettings.Instance.imported)
                foreach (var star in GSSettings.Stars)
                    if (star.BinaryCompanion != null)
                    {
                        var binary = GetGSStar(star.BinaryCompanion);
                        if (binary == null)
                        {
                            Error($"Could not find Binary Companion:{star.BinaryCompanion}");
                            continue;
                        }

                        // Log($"Moving Companion Star {star.BinaryCompanion} who has offset {binary.position}");
                        // GS2.Warn("Setting Binary Star Position");
                        galaxy.stars[binary.assignedIndex].position = binary.position = star.position + binary.position;
                        galaxy.stars[binary.assignedIndex].uPosition = galaxy.stars[binary.assignedIndex].position * 2400000.0;
                        // GS2.Log($"Host ({star.Name})Position:{star.position} . Companion ({binary.Name}) Position {binary.position }");
                    }


            //for (var i = 0; i < galaxy.stars.Length; i++) GS2.Warn($"Star {galaxy.stars[i].index} id:{galaxy.stars[i].id} name:{galaxy.stars[i].name} GSSettings:{GSSettings.Stars[i].Name}");
            Log($"Stars Created in {highStopwatch.duration:F5}s");
            highStopwatch.Begin();
            Log("Creating Planets");
            for (var i = 0; i < GSSettings.StarCount; i++) CreateStarPlanets(ref galaxy.stars[i], gameDesc, random);
            Log($"Planets Created in {highStopwatch.duration:F5}s");
            highStopwatch.Begin();
            Log("Planets have been created");
            galaxy.starCount = galaxy.stars.Length;
            var astroPoses = galaxy.astroPoses;
            for (var index = 0; index < galaxy.astroPoses.Length; ++index)
            {
                astroPoses[index].uRot.w = 1f;
                astroPoses[index].uRotNext.w = 1f;
            }

            Log($"Astroposes Reset in {highStopwatch.duration:F5}s");
            highStopwatch.Begin();
            for (var index = 0; index < GSSettings.StarCount; ++index)
            {
                astroPoses[galaxy.stars[index].id * 100].uPos = astroPoses[galaxy.stars[index].id * 100].uPosNext = galaxy.stars[index].uPosition;
                astroPoses[galaxy.stars[index].id * 100].uRot = astroPoses[galaxy.stars[index].id * 100].uRotNext = Quaternion.identity;
                astroPoses[galaxy.stars[index].id * 100].uRadius = galaxy.stars[index].physicsRadius;
            }

            Log($"Astroposes filled in {highStopwatch.duration:F5}s");
            highStopwatch.Begin();
            Log("Updating Poses");
            // galaxy.UpdatePoses(0.0);

            for (var i = 0; i < galaxy.stars.Length; i++)
            {
                if (galaxy.stars[i] == null) Error($"GalaxyStars[{i}] null");
                galaxy.stars[i].planetCount = galaxy.stars[i].planets.Length;
                for (var j = 0; j < galaxy.stars[i].planets.Length; j++)
                    if (galaxy.stars[i].planets[j] == null) Error($"GalaxyStars[{i}].planets[{j}] null");
                    else galaxy.stars[i].planets[j].UpdateRuntimePose(0.0);
            }

            Log($"Astroposes Initialized in {highStopwatch.duration:F5}s");
            highStopwatch.Begin();
            Log("End");
        }
    }
}