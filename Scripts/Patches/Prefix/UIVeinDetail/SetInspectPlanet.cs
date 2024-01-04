using HarmonyLib;

namespace GalacticScale.Patches
{
    //The following patch enables configuration of whether vein labels are visible or not, on a per type basis

    public class PatchOnUIVeinDetail
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIVeinDetail), nameof(UIVeinDetail.SetInspectPlanet))]
        public static bool SetInspectPlanet(ref UIVeinDetail __instance, PlanetData planet)
        {
            var planetFactory = planet == null ? null : planet.factory;
            if (planetFactory == null || !planet.factoryLoaded) planet = null;
            __instance.inspectPlanet = planet;
            for (var i = 0; i < __instance.allTips.Count; i++)
                if (__instance.allTips[i] != null)
                {
                    __instance.allTips[i]._Free();
                    __instance.allTips[i].inspectFactory = planetFactory;
                }

            if (__instance.inspectPlanet != null)
            {
                var veinTipConfig = GS3.Config.VeinTips;
                for (var j = 1; j < planetFactory.veinGroups.Length; j++)
                {
                    var typeNum = (int)planetFactory.veinGroups[j].type;
                    if (veinTipConfig.ContainsKey(typeNum) && veinTipConfig[typeNum]) __instance.CreateOrOpenATip(planetFactory, j);
                }
            }

            return false;
        }
    }
}