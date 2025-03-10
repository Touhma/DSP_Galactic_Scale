using HarmonyLib;

namespace GalacticScale
{
    public static partial class PatchOnGameScenarioLogic
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameScenarioLogic), "Init")]
        public static void Init_Postfix(GameScenarioLogic __instance)
        {
            GS2.Log("GameScenarioLogic.Init");
            // Ensure arrays are initialized
            if (__instance.cosmicMessageManager != null)
            {
                GS2.Log("GameScenarioLogic.Init Not Null");
                if (__instance.cosmicMessageManager.messages == null)
                {
                    __instance.cosmicMessageManager.messages = new CosmicMessageData[102];
                    __instance.cosmicMessageManager.CreateCosmicMessages();
                }
                if (__instance.cosmicMessageManager.messageSimulators == null)
                {
                    __instance.cosmicMessageManager.messageSimulators = new CosmicMessageSimulator[102];
                }
            }
            GS2.Log($"GameScenarioLogic.Init_Postfix {__instance.cosmicMessageManager.messages?.Length}");
            if (__instance.cosmicMessageManager != null && __instance.cosmicMessageManager.messages.Length == 102 && __instance.cosmicMessageManager.messages[101].nearStar != GameMain.localStar)
            {
                GS2.Warn("Setting last cosmic message star to localStar");
                __instance.cosmicMessageManager.messages[101] = __instance.cosmicMessageManager.CreateCosmicMessage(101, GameMain.localStar.seed);
            }
        }
    }
} 