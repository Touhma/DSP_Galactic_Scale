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
            BuildPreview preview2 = __instance.buildPreviews[__instance.buildPreviews.Count - 1];
            int objId = preview.inputObjId;
            int objId2 = preview2.outputObjId;
            //GS2.Warn(objId.ToString());
            PlanetFactory ___factory = __instance.factory;
            if ((objId < 0 && objId2 < 0) || (objId >= ___factory.entityPool.Length && objId2 >= ___factory.entityPool.Length))
            {
                return __result; // Sanity Check
            }

            EntityData entity = ___factory.entityPool[objId];
            EntityData entity2 = ___factory.entityPool[objId2];
            if (entity.isNull && entity2.isNull)
            {
                return __result;
            }

            ItemProto itemProto = LDB.items.Select(entity.protoId); // Grab the prototype of the first object in the chain
            ItemProto itemProto2 = LDB.items.Select(entity2.protoId);
            if (itemProto == null && itemProto2 == null)
            {
                return __result;
            }

            if (itemProto?.prefabDesc == null && itemProto2?.prefabDesc == null)
            {
                return __result;
            }
            //GS2.Log($"{itemProto?.prefabDesc?.isStation} | {itemProto2?.prefabDesc?.isStation}");
            //GS2.Log($"{itemProto2?.Name}");
            if ((itemProto != null && itemProto.prefabDesc.oilMiner) || (itemProto != null && itemProto.prefabDesc.veinMiner) || (itemProto != null && itemProto.prefabDesc.isStation) || (itemProto2 != null && itemProto2.prefabDesc.isStation)) // Check that we are connected to an oil miner
            {
                //GS2.Log($"{preview?.condition} | {preview2?.condition}");
                if (preview?.condition == EBuildCondition.JointCannotLift || preview2?.condition == EBuildCondition.JointCannotLift) // Make sure the error is that the endpoint must be horizontal
                {
                    if (preview?.condition == EBuildCondition.JointCannotLift) preview.condition = EBuildCondition.Ok; // Ignore that endpoint horizontal error
                    if (preview2?.condition == EBuildCondition.JointCannotLift) preview2.condition = EBuildCondition.Ok; // Ignore that endpoint horizontal error
                    for (int i = 0; i < count; i++) // Check the rest of the belt for errors
                    {
                        if ((__instance.buildPreviews[i].condition != EBuildCondition.Ok && __instance.buildPreviews[i].condition != EBuildCondition.JointCannotLift))
                        {
                            //GS2.Warn("Some other error");
                            __result = false;
                            return __result; // If there's some other problem with the belt, bail out.
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