using System.Reflection;
using HarmonyLib;
namespace GalacticScale.Scripts.PatchStarSystemGeneration
{

    public static class PatchOnUISpaceGuideSetVisible
    {
        [HarmonyPatch]
        public static MethodBase TargetMethod()
        {
            return AccessTools.Method(typeof(UISpaceGuide), "CheckVisible");
        }
        public static bool Prefix(ref bool __result, ref UISpaceGuide __instance, int pId0, int astroId, VectorLF3 upos, VectorLF3 camUPos)
        {
            //Patch.Debug(pId0 + " " + astroId + " " + upos + " " + camUPos, BepInEx.Logging.LogLevel.Warning, true);
            if (pId0 <= 99)
            {
                __result = true;
                return false;
            }
            VectorLF3 vectorLf3_1 = upos - camUPos;
            double magnitude = vectorLf3_1.magnitude;
            vectorLf3_1 /= magnitude;
            for (int index = pId0; index < pId0 + 10; ++index)
            {
                if (index != astroId)
                {
                    float uRadius = __instance.astroPoses[index].uRadius;

                    if ((double)uRadius >= 1.0)
                    {
                        float num1 = uRadius + 12.5f;
                        if (index == pId0)
                            num1 *= 1.05f;
                        VectorLF3 vectorLf3_2 = __instance.astroPoses[index].uPos - camUPos;
                        double num2 = vectorLf3_2.x * vectorLf3_1.x + vectorLf3_2.y * vectorLf3_1.y + vectorLf3_2.z * vectorLf3_1.z;
                        if (num2 < magnitude && num2 >= 0.0 && vectorLf3_2.x * vectorLf3_2.x + vectorLf3_2.y * vectorLf3_2.y + vectorLf3_2.z * vectorLf3_2.z - num2 * num2 < (double)num1 * (double)num1)
                        {
                            __result = false;
                            return false;
                        }
                    }
                    else
                        break;
                }
            }
            __result = true;
            return false;
        }
    }
}