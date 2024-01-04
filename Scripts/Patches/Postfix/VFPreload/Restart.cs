using HarmonyLib;

namespace GalacticScale.Patches
{
    //The Patches in this class Add a PreLoad Splash
    public partial class PatchOnVFPreload
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(VFPreload), nameof(VFPreload.Restart))]
        public static void Restart(VFPreload __instance)
        {
            GS3.splashImage.sprite = Utils.GetSplashSprite() ?? null;
        }
    }
}