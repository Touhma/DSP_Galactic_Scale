using HarmonyLib;

namespace GalacticScale
{
    public class PatchOnBuildTool_Path
    {
        [HarmonyPostfix, HarmonyPatch(typeof(BuildTool_Path), "CheckBuildConditions")]
        public static bool CheckBuildConditions(bool __result,
                           BuildTool_Path __instance,
                           ref bool ___cursorValid)
        {
            int count = __instance.buildPreviews.Count;
            if (count < 2)
            {
                return __result; // Check we are building
            }

            BuildPreview preview = __instance.buildPreviews[0];
            int objId = preview.inputObjId;
            //GS2.Warn(objId.ToString());
            PlanetFactory ___factory = __instance.factory;
            if (objId < 0 || objId >= ___factory.entityPool.Length)
            {
                return __result; // Sanity Check
            }

            EntityData entity = ___factory.entityPool[objId];
            if (entity.isNull)
            {
                return __result;
            }

            ItemProto itemProto = LDB.items.Select(entity.protoId); // Grab the prototype of the first object in the chain
            if (itemProto == null)
            {
                return __result;
            }

            if (itemProto.prefabDesc == null)
            {
                return __result;
            }

            if (itemProto.prefabDesc.oilMiner) // Check that we are connected to an oil miner
            {
                if (preview.condition == EBuildCondition.JointCannotLift) // Make sure the error is that the endpoint must be horizontal
                {
                    preview.condition = EBuildCondition.Ok; // Ignore that endpoint horizontal error
                    for (int i = 0; i < count; i++) // Check the rest of the belt for errors
                    {
                        if ((__instance.buildPreviews[i].condition != EBuildCondition.Ok && __instance.buildPreviews[i].condition != EBuildCondition.JointCannotLift))
                        {
                            __result = false;
                            return __result; //If there's some other problem with the belt, bail out.
                        }
                    }
                    //___cursorText = "Click to build";
                    ___cursorValid = true; // Prevent red text
                    __result = true; // Override the build condition check
                    UICursor.SetCursor(ECursor.Default); // Get rid of that ban cursor
                    __instance.actionBuild.model.cursorText = "Click to build";
                    __instance.actionBuild.model.cursorState = 0;

                }
            }

            return __result;
        }
    }
}