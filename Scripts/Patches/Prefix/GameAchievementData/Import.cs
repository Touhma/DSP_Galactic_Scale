using System.IO;
using HarmonyLib;

namespace GalacticScale.Patches
{
    public class PatchOnGameAchievementData
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(GameAchievementData), nameof(GameAchievementData.Import))]
        public static bool Import(GameAchievementData __instance, BinaryReader r)
        {
            r.ReadInt32();
            var num = r.ReadInt32();
            for (var i = 0; i < num; i++)
            {
                var num2 = r.ReadInt32();
                if (__instance.runtimeDatas.ContainsKey(num2) && __instance.runtimeDatas[num2] != null)
                {
                    __instance.runtimeDatas[num2].Import(r);
                }
                else
                {
                    if (!LDB.achievements.Exist(num2)) return false;
                    var achievementProto = LDB.achievements.Select(num2);
                    if (!string.IsNullOrEmpty(achievementProto.RuntimeDataName))
                        (__instance.runtimeAsm.CreateInstance(achievementProto.RuntimeDataName) as AchievementRuntimeData)?.Import(r);
                }
            }

            return false;
        }
    }
}