using HarmonyLib;
using System.IO;

namespace GalacticScale {
    public static partial class PatchOnGameDesc {
        [HarmonyPostfix, HarmonyPatch(typeof(GameDesc), "Import")]
        public static void Import(BinaryReader r, ref GameDesc __instance) {
            if (!GS2.IsMenuDemo) {
                GS2.Log("Not Menu Demo. Importing");
                GS2.Import(r);
                return;
            }
            GS2.Log("Menu Demo: " + GS2.IsMenuDemo.ToString());
            GSSettings.Instance.imported = false;
            return;
        }
    }
}