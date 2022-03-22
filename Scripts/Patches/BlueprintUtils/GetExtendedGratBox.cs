using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnBlueprintUtils
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BlueprintUtils), "GetExtendedGratBox", typeof(BPGratBox), typeof(float))]
        public static bool GetExtendedGratBoxA(ref BPGratBox __result, BPGratBox gratbox, float extend_grid = 0.5f)
        {
            var segments = (int)GameMain.localPlanet.realRadius;//4* Mathf.RoundToInt (GameMain.localPlanet.realRadius/4f);
            float longitudeRadPerGrid;
            if (gratbox.y * gratbox.w > 0f && gratbox.y > 0f)
            {
                longitudeRadPerGrid = BlueprintUtils.GetLongitudeRadPerGrid(gratbox.y, segments);
            }
            else if (gratbox.y * gratbox.w > 0f && gratbox.y < 0f)
            {
                longitudeRadPerGrid = BlueprintUtils.GetLongitudeRadPerGrid(gratbox.w, segments);
            }
            else
            {
                longitudeRadPerGrid = BlueprintUtils.GetLongitudeRadPerGrid(0f, segments);
            }
            // GS2.Warn($"Gratbox Start : {gratbox.y} {gratbox.w} {gratbox.endLatitudeRad} {gratbox.endLongitudeRad} {gratbox.startLatitudeRad} {gratbox.startLongitudeRad}");
            // GS2.Warn($"Extending Gratbox {segments} {extend_grid} {BlueprintUtils.GetLatitudeRadPerGrid(segments) * extend_grid} {longitudeRadPerGrid}");
            gratbox.Extend(longitudeRadPerGrid * extend_grid, BlueprintUtils.GetLatitudeRadPerGrid(segments) * extend_grid);
            // GS2.Warn($"Gratbox End: {gratbox.y} {gratbox.w} {gratbox.endLatitudeRad} {gratbox.endLongitudeRad} {gratbox.startLatitudeRad} {gratbox.startLongitudeRad}");
       //     [Warning: GS2]   26:PatchOnBlueprintUtils | GetExtendedGratBoxA | Gratbox Start: -0.2777829 - 0.2777829 - 0.2777829 - 0.5456451 - 0.2777829 - 0.5456451
       //  [Warning: GS2]   27:PatchOnBlueprintUtils | GetExtendedGratBoxA | Extending Gratbox 380 0.5 0.00165347 0.00330694
       //[Warning: GS2]   29:PatchOnBlueprintUtils | GetExtendedGratBoxA | Gratbox End: -0.2794364 - 0.2761295 - 0.2761295 - 0.5439916 - 0.2794364 - 0.5472986


            __result = gratbox;
            return false;
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BlueprintUtils), "GetExtendedGratBox", typeof(BPGratBox), typeof(float), typeof(float))]
        public static bool GetExtendedGratBoxB(ref BPGratBox __result, BPGratBox gratbox, float extend_lng_grid = 0.5f, float extend_lat_grid = 0.5f)
        {
            //var segments = GameMain.localPlanet.segment;
            var segments = 4 * Mathf.RoundToInt(GameMain.localPlanet.realRadius / 4f);
            float longitudeRadPerGrid;
            if (gratbox.y * gratbox.w > 0f && gratbox.y > 0f)
            {
                longitudeRadPerGrid = BlueprintUtils.GetLongitudeRadPerGrid(gratbox.y, segments);
            }
            else if (gratbox.y * gratbox.w > 0f && gratbox.y < 0f)
            {
                longitudeRadPerGrid = BlueprintUtils.GetLongitudeRadPerGrid(gratbox.w, segments);
            }
            else
            {
                longitudeRadPerGrid = BlueprintUtils.GetLongitudeRadPerGrid(0f, segments);
            }
            // GS2.Warn($"BGratbox Start : {gratbox.y} {gratbox.w} {gratbox.endLatitudeRad} {gratbox.endLongitudeRad} {gratbox.startLatitudeRad} {gratbox.startLongitudeRad}");
            // GS2.Warn($"BExtending Gratbox {segments} {extend_lat_grid} {BlueprintUtils.GetLatitudeRadPerGrid(segments) * extend_lat_grid} {longitudeRadPerGrid}");
            gratbox.Extend(longitudeRadPerGrid * extend_lng_grid, BlueprintUtils.GetLatitudeRadPerGrid(segments) * extend_lat_grid);
            // GS2.Warn($"BGratbox End: {gratbox.y} {gratbox.w} {gratbox.endLatitudeRad} {gratbox.endLongitudeRad} {gratbox.startLatitudeRad} {gratbox.startLongitudeRad}");

            __result = gratbox;
            return false;
        }
    }
}