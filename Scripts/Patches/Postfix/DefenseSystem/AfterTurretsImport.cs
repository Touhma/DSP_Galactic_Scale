using HarmonyLib;

namespace GalacticScale.Patches
{
    public partial class PatchOnDefenseSystem
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(DefenseSystem), nameof(DefenseSystem.AfterTurretsImport))]
        public static void AfterTurretsImport(ref DefenseSystem __instance)
        {
            TurretComponent[] buffer = __instance.turrets.buffer;
            for (int i = 1; i < __instance.turrets.cursor; i++)
            {
                if (buffer[i].id == 1) TurretComponentTranspiler.AddTurret(__instance, ref buffer[i]);
            }
        }

    }
}