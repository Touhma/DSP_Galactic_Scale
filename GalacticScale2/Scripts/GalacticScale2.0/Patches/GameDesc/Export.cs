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
            GS2.Warn("Exporting");
            if (GS2.IsMenuDemo || GS2.Vanilla) return;
            var minify = GS2.Config.MinifyJson;
            GS2.Config.MinifyJson = false;
            GS2.Export(w);
            GS2.Config.MinifyJson = minify;
        }
    }
}