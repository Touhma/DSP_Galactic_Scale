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
            var segments = GameMain.localPlanet.segment;
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
            gratbox.Extend(longitudeRadPerGrid * extend_grid, BlueprintUtils.GetLatitudeRadPerGrid(200) * extend_grid);
            __result = gratbox;
            return false;
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BlueprintUtils), "GetExtendedGratBox", typeof(BPGratBox), typeof(float), typeof(float))]
        public static bool GetExtendedGratBoxB(ref BPGratBox __result, BPGratBox gratbox, float extend_lng_grid = 0.5f, float extend_lat_grid = 0.5f)
        {
            var segments = GameMain.localPlanet.segment;
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
            gratbox.Extend(longitudeRadPerGrid * extend_lng_grid, BlueprintUtils.GetLatitudeRadPerGrid(200) * extend_lat_grid);
            __result = gratbox;
            return false;
        }
    }
}