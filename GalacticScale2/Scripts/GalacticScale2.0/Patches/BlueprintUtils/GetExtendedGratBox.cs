using System;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnBlueprintUtils
    {
        [HarmonyPrefix, HarmonyPatch(typeof(BlueprintUtils), "GetExtendedGratBox")]

        public static bool GetExtendedGratBox(ref BPGratBox __result, BPGratBox gratbox, float extend_grid = 0.5f)
        {
            int _segmentCnt = GameMain.localPlanet.segment;
            float longitudeRadPerGrid;
            if (gratbox.y * gratbox.w > 0f && gratbox.y > 0f)
            {
                longitudeRadPerGrid = BlueprintUtils.GetLongitudeRadPerGrid(gratbox.y, _segmentCnt);
            }
            else if (gratbox.y * gratbox.w > 0f && gratbox.y < 0f)
            {
                longitudeRadPerGrid = BlueprintUtils.GetLongitudeRadPerGrid(gratbox.w, _segmentCnt);
            }
            else
            {
                longitudeRadPerGrid = BlueprintUtils.GetLongitudeRadPerGrid(0f, _segmentCnt);
            }
            gratbox.Extend(longitudeRadPerGrid * extend_grid, BlueprintUtils.GetLatitudeRadPerGrid(_segmentCnt) * extend_grid);
            __result = gratbox;
            return false;
        }
    }
}