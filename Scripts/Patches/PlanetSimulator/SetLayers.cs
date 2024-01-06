using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnPlanetSimulator
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetSimulator), "SetLayers")]
        public static bool SetLayers(PlanetSimulator __instance) //Temporary to fix existing scarlet ice lakes. Can be removed for GS2
        {
            if (!__instance.layerSet)
            {
                __instance.planetData.gameObject.layer = 0;
                __instance.planetData.bodyObject.SetLayer(9, true);
                if (__instance.oceanCollider != null)
                {
                    if (__instance.planetData.iceFlag > 0 || !GS2.IsMenuDemo && !GS2.Vanilla && GS2.GetGSPlanet(__instance.planetData.id)?.GsTheme?.WaterItemId == -2)
                    {
                        __instance.oceanCollider.gameObject.layer = 9;
                        __instance.oceanCollider.isTrigger = false;
                    }
                    else
                    {
                        __instance.oceanCollider.gameObject.layer = 4;
                    }
                }

                if (__instance.atmoTrans0 != null) __instance.atmoTrans0.gameObject.layer = 0;

                if (__instance.atmoTrans1 != null) __instance.atmoTrans1.gameObject.layer = 0;

                if (__instance.cloudSimulator != null) __instance.cloudSimulator.gameObject.layer = 0;

                __instance.layerSet = true;
            }

            return false;
        }
    }
}