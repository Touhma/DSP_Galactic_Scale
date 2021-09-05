using HarmonyLib;

namespace GalacticScale
{
    //The Patches in this class Add a PreLoad Splash
    public partial class PatchOnVFPreload
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(VFPreload), "Restart")]
        public static void Restart(VFPreload __instance)
        {
            GS2.splashImage.sprite = Utils.GetSplashSprite() ?? null;
        }
    }
}