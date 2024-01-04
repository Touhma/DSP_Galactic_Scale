using HarmonyLib;

namespace GalacticScale.Patches

{
    public class PatchOnGraticulePoser
    {
        public static PlanetData camPlanet;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(GraticulePoser), nameof(GraticulePoser.Calculate))]
        public static bool PlanetPos(ref GraticulePoser __instance)
        {
            if (GameMain.localPlanet != null && GameMain.localPlanet != camPlanet)
            {
                //GS3.Warn("Setting Planet");
                camPlanet = GameMain.localPlanet;
                __instance.planetRadius = camPlanet.radius;
                // GS3.Log((__instance == GameCamera.instance.blueprintPoser).ToString());
                GameCamera.instance.blueprintPoser.planetRadius = camPlanet.radius;
            }

            return true;
        }
    }
}