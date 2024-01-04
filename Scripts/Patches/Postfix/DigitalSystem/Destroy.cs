using HarmonyLib;

namespace GalacticScale.Patches
{
    public partial class PatchOnDigitalSystem
    {
        [HarmonyPostfix, HarmonyPatch(typeof(DigitalSystem), nameof(DigitalSystem.Import))]
        public static void DigitalSystem_Import(DigitalSystem __instance)
        {
            if (__instance.speakerCursor <= 0)
            {
                __instance.speakerCursor = 1;
                __instance.speakerRecycleCursor = 0;
                __instance.SetSpeakerCapacity(256);
            }
        }
    }
}