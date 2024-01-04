using HarmonyLib;
using NebulaCompatibility;

namespace GalacticScale.Patches
{
    public partial class PatchOnUIVirtualStarmap
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIVirtualStarmap), nameof(UIVirtualStarmap._OnLateUpdate))]
        public static bool _OnLateUpdate(ref UIVirtualStarmap __instance)
        {
            if (NebulaCompat.IsClient && !NebulaCompat.IsMPGameLoaded()) return true; // Use Nebula code when in client's lobby
            SystemDisplay.OnUpdate(__instance);
            return false;
        }
    }
}