using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(BuildingGizmo))]
    static class PatchBuildingGizmo
    {
        [HarmonyPostfix]
        [HarmonyPatch("SetGizmoDesc")]
        public static void SetGizmoDesc(BuildGizmoDesc _desc, Transform ___minerFan)
        {
            if (_desc.desc.minerType == EMinerType.Vein)
            {
                if (GameMain.localPlanet == null) return;
                float angle = ((200 / GameMain.localPlanet.radius) * Mathf.PI - Mathf.PI);
                ___minerFan.localEulerAngles = new Vector3(angle, 180f, 0f);
            }
        }
    }
}