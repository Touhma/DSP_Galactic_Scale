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
        public static void PatchSetGizmoDesc(BuildGizmoDesc _desc, Transform ___minerFan)
        {
            if (_desc.desc.minerType == EMinerType.Vein)
            {
                if (GameMain.localPlanet == null) return;
                //float angle = Mathf.Rad2Deg * (-1.0f * Mathf.Abs((Mathf.PI / 200) - Mathf.PI / ((GameMain.localPlanet.radius / 4.0f) * (int)4))); // Hopefully get difference in tangent in radians and turns into degrees
                float angle = ((200 / GameMain.localPlanet.radius) * Mathf.PI - Mathf.PI);
                //Quaternion q = Quaternion.Euler(0, -10.0f * angle, 0);
                //Patch.Log(angle);
                ___minerFan.localEulerAngles = new Vector3(angle, 180f, 0f);
            }
        }
    }
}