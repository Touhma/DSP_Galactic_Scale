using HarmonyLib;
using UnityEngine;
namespace GalacticScale.Scripts.PatchPlanetSize
{
    [HarmonyPatch(typeof(PlayerAction_Build))]
    public class PatchOnPlayerAction_BuildCheck // - innominata
    {
        [HarmonyPostfix]
        [HarmonyPatch("CheckBuildConditions")]
        static bool BuildConditionsCheck(
            bool __result,
            PlayerAction_Build __instance, 
            ref string ___cursorText,
            ref bool ___cursorWarning,
            ref PlanetFactory ___factory)
        {
            int count = __instance.buildPreviews.Count;
            if (count < 2) return __result; // Check we are building
            BuildPreview preview = __instance.buildPreviews[0];
            int objId = preview.inputObjId;
            if (objId < 0 || objId >= ___factory.entityPool.Length) return __result; // Sanity Check
            EntityData entity = ___factory.entityPool[objId];
            if (entity.isNull) return __result;
            ItemProto itemProto = LDB.items.Select((int)entity.protoId); // Grab the prototype of the first object in the chain
            if (itemProto == null) return __result;
            if (itemProto.prefabDesc == null) return __result;
            if (itemProto.prefabDesc.oilMiner) // Check that we are connected to an oil miner
            {
                if (preview.condition == EBuildCondition.JointCannotLift) // Make sure the error is that the endpoint must be horizontal
                {
                    preview.condition = EBuildCondition.Ok; // Ignore that endpoint horizontal error
                    for (int i = 0; i < count; i++) // Check the rest of the belt for errors
                    {
                        if ((__instance.buildPreviews[i].condition != EBuildCondition.Ok && __instance.buildPreviews[i].condition != EBuildCondition.JointCannotLift))
                        {
                            __result = (bool)false;
                            return __result; //If there's some other problem with the belt, bail out.
                        }
                    }
                    ___cursorText = "Click to build";
                    ___cursorWarning = false; // Prevent red text
                    __result = true; // Override the build condition check
                    UICursor.SetCursor(ECursor.Default); // Get rid of that ban cursor
                }
            }
            return __result;
        }
    }
}