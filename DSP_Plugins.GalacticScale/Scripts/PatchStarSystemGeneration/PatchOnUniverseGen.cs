using HarmonyLib;
using UnityRandom = UnityEngine.Random;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;
using System;
using System.Reflection;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using BepInEx;

namespace GalacticScale.Scripts.PatchStarSystemGeneration
{
    [HarmonyPatch(typeof(UniverseGen))]
    public class PatchOnUniverseGen
    {
        [HarmonyPrefix]
        [HarmonyPatch("CreateGalaxy")]
        public static bool CreateGalaxy(GameDesc gameDesc, ref GalaxyData __result, ref List<VectorLF3> ___tmp_poses)
        {
            string path = Path.Combine(Path.Combine(Paths.BepInExRootPath, "config"), "GSData.json");
            if (DSPGame.IsMenuDemo) return true;
            if (!GS2.LoadSettingsFromJson(path))
            {
                GS2.CreateDummySettings(12);

            }
            __result = GS2.CreateGalaxy(gameDesc);
            GS2.SaveSettingsToJson(path);
            //GS2.DumpObjectToJson(path + "2", GS2.planetTypes);
            //GS2.DumpObjectToJson(Path.Combine(GS2.DataDir, "galaxy.json"), __result);
            return false;
        }


    }
}
