using HarmonyLib;

namespace GalacticScale
{
    public class PatchOnBuildTool_Path
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BuildTool_Path), "CheckBuildConditions")]
        public static bool CheckBuildConditions(bool __result, BuildTool_Path __instance, ref bool ___cursorValid)
        {
            // GS2.Warn(__instance.buildPreviews[0].condition.ToString());
            var count = __instance.buildPreviews.Count;
            if (count < 2) return __result; // Check we are building

            var preview = __instance.buildPreviews[0];

            var preview2 = __instance.buildPreviews[count - 1];

            var objId = preview.inputObjId;
            var objId2 = preview2.outputObjId;
            //GS2.Warn(objId.ToString());
            var factory = __instance.factory;

            if (objId < 0) return __result;
            if (objId2 < 0) return __result;
            if (objId >= factory.entityPool.Length) return __result;
            if (objId2 >= factory.entityPool.Length) return __result;

            var entity = factory.entityPool[objId];
            var entity2 = factory.entityPool[objId2];
            if (entity.isNull && entity2.isNull) return __result;
            var itemProto = LDB.items.Select(entity.protoId); // Grab the prototype of the first object in the chain
            var itemProto2 = LDB.items.Select(entity2.protoId);
            if (itemProto == null && itemProto2 == null) return __result;

            if (itemProto?.prefabDesc == null && itemProto2?.prefabDesc == null) return __result;
            //GS2.Log($"{itemProto?.prefabDesc?.isStation} | {itemProto2?.prefabDesc?.isStation}");
            //GS2.Log($"{itemProto2?.Name}");
            var requiresPatch = false;
            if (itemProto != null && itemProto.prefabDesc != null)
                if (itemProto.prefabDesc.oilMiner || itemProto.prefabDesc.veinMiner || itemProto.prefabDesc.isStation)
                    requiresPatch = true;

            if (itemProto2 != null && itemProto2.prefabDesc != null && itemProto2.prefabDesc.isStation)
                requiresPatch = true;
            if (requiresPatch)
            {
                //GS2.Log($"{preview?.condition} | {preview2?.condition}");
                if (preview?.condition != EBuildCondition.JointCannotLift && preview2?.condition != EBuildCondition.JointCannotLift) return __result;
                if (preview?.condition == EBuildCondition.JointCannotLift)
                    preview.condition = EBuildCondition.Ok; // Ignore that endpoint horizontal error
                if (preview2?.condition == EBuildCondition.JointCannotLift)
                    preview2.condition = EBuildCondition.Ok; // Ignore that endpoint horizontal error
                for (var i = 0; i < count; i++) // Check the rest of the belt for errors
                    if (__instance.buildPreviews[i].condition != EBuildCondition.Ok && __instance.buildPreviews[i].condition != EBuildCondition.JointCannotLift)
                    {
                        //GS2.Warn("Some other error");
                        __result = false;
                        return false; // If there's some other problem with the belt, bail out.
                    }

                //___cursorText = "Click to build";
                ___cursorValid = true; // Prevent red text
                __result = true; // Override the build condition check
                UICursor.SetCursor(ECursor.Default); // Get rid of that ban cursor
                __instance.actionBuild.model.cursorText = "Click to build";
                __instance.actionBuild.model.cursorState = 0;
            }

            return __result;
        }
    }
}