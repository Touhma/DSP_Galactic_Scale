using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnGameData
    {
        [HarmonyPostfix, HarmonyPatch(typeof(GameData), "Import")]
        public static void GameData_Import(GameData __instance)
        {
            for (int i = 0; i < __instance.factoryCount; i++)
            {
                PlanetFactory factory = __instance.factories[i];
                for (int j = 1; j < factory.cargoTraffic.monitorCursor; j++)
                {
                    if (factory.cargoTraffic.monitorPool[j].id == j)
                    {
                        if (factory.cargoTraffic.monitorPool[j].speakerId >=
                            factory.digitalSystem.speakerCursor)
                        {
                            int speakerId = factory.cargoTraffic.monitorPool[j].speakerId;
                            int entityId = factory.cargoTraffic.monitorPool[j].entityId;
                            GS2.Warn($"{factory.planet.displayName}: Remove monitor {j} for speakerId {speakerId} is out of bound");
                            factory.entityPool[entityId].speakerId = 0;
                            if (factory.entityPool[entityId].warningId >= __instance.warningSystem.warningCursor)
                                factory.entityPool[entityId].warningId = 0;
                            factory.RemoveEntityWithComponents(entityId, false);
                        }
                    }
                }
            }
        }
    }
}