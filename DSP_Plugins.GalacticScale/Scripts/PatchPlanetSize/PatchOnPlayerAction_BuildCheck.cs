using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using BepInEx.Logging;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(PlayerAction_Build))]
    public class PatchOnPlayerAction_BuildCheck {
       
        [HarmonyPostfix]
        [HarmonyPatch("CheckBuildConditions")]
        static bool PatchBuildConditionsCheck(bool __result, PlayerAction_Build __instance, PlanetFactory ___factory, ref string ___cursorText, ref bool ___cursorWarning, ref bool ___cursorValid, ref bool ___waitConfirm)
        {
            if (__instance.buildPreviews.Count > 1) // Check we are building
            {
                ItemProto itemProto = LDB.items.Select((int)___factory.entityPool[__instance.buildPreviews[0].inputObjId].protoId); // Grab the prototype of the first object in the chain
                if (itemProto != null && itemProto.prefabDesc.oilMiner) // Check that we are connected to an oil miner
                {
                    if (__instance.buildPreviews[0].condition == EBuildCondition.JointCannotLift) // Make sure the error is that the endpoint must be horizontal
                    {
                        __instance.buildPreviews[0].condition = EBuildCondition.Ok; // Ignore that endpoint horizontal error
                       for (int i = 0; i < __instance.buildPreviews.Count(); i++) // Check the rest of the belt for errors
                        {
                            if ((__instance.buildPreviews[i].condition != EBuildCondition.Ok && __instance.buildPreviews[i].condition != EBuildCondition.JointCannotLift)) return (bool)false; //If there's some other problem with the belt, bail out.
                        }
                        ___cursorText = "Click to build";
                        ___cursorWarning = false; // Prevent red text
                        __result = true; // Override the build condition check
                        UICursor.SetCursor(ECursor.Default); // Get rid of that ban cursor
                    }
                }
            }
            return __result;
        }
    }
}