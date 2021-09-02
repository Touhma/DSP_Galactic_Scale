using HarmonyLib;

namespace GalacticScale

{
    public class PatchOnGraticulePoser
    {
        public static PlanetData camPlanet;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GraticulePoser), "Calculate")]
        public static bool PlanetPos(ref GraticulePoser __instance)
        {
            if (GameMain.localPlanet != null && GameMain.localPlanet != camPlanet)
            {
                //GS2.Warn("Setting Planet");
                camPlanet = GameMain.localPlanet;
                __instance.planetRadius = camPlanet.radius;
                GS2.Log((__instance == GameCamera.instance.blueprintPoser).ToString());
                GameCamera.instance.blueprintPoser.planetRadius = camPlanet.radius;
            }

            return true;
        }
    }
}