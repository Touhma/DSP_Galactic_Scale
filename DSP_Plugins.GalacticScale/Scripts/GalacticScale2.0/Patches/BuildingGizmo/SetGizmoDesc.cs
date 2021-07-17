using HarmonyLib;
using UnityEngine;

/// <summary>
/// Rotate minerfan to follow curvature of smaller planets
/// </summary>
namespace GalacticScale
{
    internal static class PatchOnBuildingGizmo
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(BuildingGizmo), "SetGizmoDesc")]
        public static void SetGizmoDesc(BuildGizmoDesc _desc, Transform ___minerFan)
        {
            if (_desc.desc.minerType == EMinerType.Vein)
            {
                if (GameMain.localPlanet == null) return;

                var angle = 200 / GameMain.localPlanet.radius * Mathf.PI - Mathf.PI;
                ___minerFan.localEulerAngles = new Vector3(angle, 180f, 0f);
            }
        }
    }
}