using HarmonyLib;

namespace GalacticScale
{
    public class PatchOnBuildTool_Inserter
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BuildTool_Inserter), "CheckBuildConditions")]
        public static void BuildToolInserter(BuildTool_Inserter __instance, ref bool __result)
        {
            if (__instance.buildPreviews.Count == 0) return;
            // if (__instance.buildPreviews == null) return;
            var preview = __instance.buildPreviews[0];
            // GS2.Warn(preview?.condition.ToString());

            if (__instance.planet.realRadius < 20)
                if (preview.condition == EBuildCondition.TooSkew)
                {
                    preview.condition = EBuildCondition.Ok;
                    // GS2.Warn("TooSkew");
                    __instance.cursorValid = true; // Prevent red text
                    __result = true; // Override the build condition check
                    UICursor.SetCursor(ECursor.Default); // Get rid of that ban cursor
                    __instance.actionBuild.model.cursorText = "Click to build";
                    __instance.actionBuild.model.cursorState = 0;
                }
        }
    }
}