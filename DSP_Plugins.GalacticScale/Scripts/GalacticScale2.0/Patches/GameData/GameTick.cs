using GalacticScale;
using HarmonyLib;
namespace GalacticScale
{
    /// <summary>
    /// Don't know why adding this vanilla code fixes Nebula, but it does, so here it is.
    /// </summary>
    public partial class PatchOnGameData
    {
        [HarmonyPrefix, HarmonyPatch(typeof(GameData), "GameTick")]
        public static bool GameTick(ref GameData __instance, long time)
        {
            if (GS2.IsMenuDemo || GS2.Vanilla) return true;
            if (NebulaCompatibility.IsMasterClient || !NebulaCompatibility.Initialized) return true;
            double gameTime = GameMain.gameTime;
            __instance.statistics.PrepareTick();
            __instance.history.PrepareTick();
            if (__instance.localPlanet != null && __instance.localPlanet.factoryLoaded)
            {
                __instance.localPlanet.physics.GameTick();
            }
            if (__instance.guideMission != null)
            {
                __instance.guideMission.GameTick();
            }
            if (__instance.mainPlayer != null && !__instance.demoTicked)
            {
                __instance.mainPlayer.GameTick(time);
            }
            __instance.DetermineRelative();
            for (int i = 0; i < __instance.dysonSpheres.Length; i++)
            {
                if (__instance.dysonSpheres[i] != null)
                {
                    __instance.dysonSpheres[i].BeforeGameTick(time);
                }
            }
            for (int j = 0; j < __instance.factoryCount; j++)
            {
                Assert.NotNull(__instance.factories[j]);
                if (__instance.factories[j] != null)
                {
                    __instance.factories[j].BeforeGameTick(time);
                }
            }
            for (int k = 0; k < __instance.factoryCount; k++)
            {
                if (__instance.factories[k] != null)
                {
                    __instance.factories[k].GameTick(time);
                }
            }
            __instance.trashSystem.GameTick(time);
            for (int l = 0; l < __instance.dysonSpheres.Length; l++)
            {
                if (__instance.dysonSpheres[l] != null)
                {
                    __instance.dysonSpheres[l].GameTick(time);
                }
            }
            if (__instance.localPlanet != null && __instance.localPlanet.factoryLoaded)
            {
                __instance.localPlanet.audio.GameTick();
            }
            if (!DSPGame.IsMenuDemo)
            {
                __instance.statistics.GameTick(time);
            }
            if (!DSPGame.IsMenuDemo)
            {
                __instance.warningSystem.GameTick(time);
            }
            __instance.history.AfterTick();
            __instance.statistics.AfterTick();
            __instance.preferences.Collect();
            if (DSPGame.IsMenuDemo)
            {
                __instance.demoTicked = true;
            }
            return false;
        }
    }
}