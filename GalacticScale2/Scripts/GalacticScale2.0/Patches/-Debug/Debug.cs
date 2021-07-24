namespace GalacticScale

{
    public class PatchOnWhatever
    {
        // [HarmonyPrefix, HarmonyPatch(typeof(PlanetGrid), "CalcLocalGridSize")]
        // public static bool CalcLocalGridSize(Vector3 posR, Vector3 dir, ref float __result, ref PlanetGrid __instance)
        // {
        //  float f = Vector3.Dot(Vector3.Cross(posR, Vector3.up).normalized, dir);
        //  float magnitude = posR.magnitude;
        //  posR.Normalize();
        //  if ((double)Mathf.Abs(f) < 0.7)
        //  {
        //   __result = magnitude * 3.1415927f * 2f / (float)(__instance.segment * 5);
        //   // Log("1 "+__result);
        //   return false;
        //  }
        //  float num = Mathf.Asin(posR.y);
        //  float f2 = num / 6.2831855f * (float)__instance.segment;
        //  float num2 = (float)PlanetGrid.DetermineLongitudeSegmentCount(Mathf.FloorToInt(Mathf.Max(0f, Mathf.Abs(f2))), __instance.segment);
        //  
        //  // Warn($"num:{num} f2:{f2} seg:{__instance.segment} magnitude:{magnitude} num2:{num2} calc:{Mathf.Cos(num) * 3.1415927f * 2f / (num2 * 5f)}");
        //  float num3 = Mathf.Max(0.0031415927f, Mathf.Cos(num) * 3.1415927f * 2f / (num2 * 5f));
        //  __result = (magnitude * num3);
        //  if (GameMain.localPlanet.radius > 480) __result -= 0.3f;
        //  // Log("2 "+__result);
        //  return false;
        // } 
    }
}