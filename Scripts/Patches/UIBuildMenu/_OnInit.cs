using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnUIBuildMenu
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIBuildMenu), "_OnInit")]
        public static void _OnInit(UIBuildMenu __instance)
        {
            GSUIBrushSizePanel.CreateInstance(__instance);
        }
    }
}