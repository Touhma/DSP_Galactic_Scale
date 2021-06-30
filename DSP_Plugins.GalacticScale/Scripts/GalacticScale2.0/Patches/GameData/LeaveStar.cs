using HarmonyLib;
using static GalacticScale.GS2;

namespace GalacticScale
{
    public partial class PatchOnGameData
    {
        [HarmonyPrefix, HarmonyPatch(typeof(GameData), "LeaveStar")]
        public static bool LeaveStar()
        {
            Warn($"LeaveStar {GameMain.localStar?.name}");

            return true;
        }
    }
}