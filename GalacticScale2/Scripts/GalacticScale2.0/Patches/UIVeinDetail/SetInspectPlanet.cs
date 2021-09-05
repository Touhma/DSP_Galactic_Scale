using HarmonyLib;

namespace GalacticScale
{
    //The following patch enables configuration of whether vein labels are visible or not, on a per type basis

    public class PatchOnUIVeinDetail
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIVeinDetail), "SetInspectPlanet")]
        public static bool SetInspectPlanet(ref UIVeinDetail __instance, PlanetData planet)
        {
            __instance.inspectPlanet = planet;
            for (var i = 0; i < __instance.allTips.Count; i++)
                if (__instance.allTips[i] != null)
                {
                    __instance.allTips[i]._Free();
                    __instance.allTips[i].inspectPlanet = __instance.inspectPlanet;
                }

            if (__instance.inspectPlanet != null)
            {
                var veinTipConfig = GS2.Config.VeinTips;
                for (var j = 0; j < planet.veinGroups.Length; j++)
                {
                    var typeNum = (int)planet.veinGroups[j].type;
                    if (veinTipConfig.ContainsKey(typeNum) && veinTipConfig[typeNum]) __instance.CreateOrOpenATip(planet, j);
                }
            }

            return false;
        }
    }
}