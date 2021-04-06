using HarmonyLib;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(GameMain))]
    public class PatchOnGameMain
    {
        [HarmonyPrefix]
        [HarmonyPatch("OnMainCameraPostRender")]
        static bool PatchOnMainCameraPostRender(Camera cam) 
        {
            if (GameMain.data != null) GameMain.data.OnPostDraw();
            return false;
        }

        [HarmonyPostfix, HarmonyPatch("FixedUpdate")]
        static void DebugCommand()
        {
            if (VFInput.alt && Input.GetKeyDown(KeyCode.D))
            {
                PlanetData planet = GameMain.localPlanet;
                Vector3 position = GameMain.mainPlayer.position;
                Patch.Debug("DebugCommand", BepInEx.Logging.LogLevel.Message, true);
                Patch.Debug("---------------", BepInEx.Logging.LogLevel.Message, true);
                Patch.Debug("Planet Segment Division: " + ((int)(planet.radius / 4f + 0.1f) * 4), BepInEx.Logging.LogLevel.Message, true);
                Patch.Debug("Planet Segments: " + planet.segment, BepInEx.Logging.LogLevel.Message, true);
                Patch.Debug("Position: " + position, BepInEx.Logging.LogLevel.Message, true);
                Patch.Debug("Height: " + planet.data.QueryHeight(position), BepInEx.Logging.LogLevel.Message, true);
                Patch.Debug("ModifiedHeight: " + planet.data.QueryModifiedHeight(position), BepInEx.Logging.LogLevel.Message, true);
                Patch.Debug("Last 2 ModData's: " + planet.data.modData[planet.data.modData.Length -2] + ", " + planet.data.modData[planet.data.modData.Length - 2], BepInEx.Logging.LogLevel.Message, true);
                Patch.Debug("---------------", BepInEx.Logging.LogLevel.Message, true);
            }
        }
    }
}