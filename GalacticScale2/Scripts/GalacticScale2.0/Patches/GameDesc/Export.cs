using System.IO;
using HarmonyLib;

namespace GalacticScale
{
    public static partial class PatchOnGameDesc
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameDesc), "Export")]
        public static void Export(BinaryWriter w)
        {
            if (GS2.IsMenuDemo || GS2.Vanilla) return;

            GS2.Export(w);
        }
    }
}