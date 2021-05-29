using HarmonyLib;
using UnityEngine;


namespace GalacticScale {
    [HarmonyPatch(typeof(BuildingGizmo))]
    static class PatchOnBuildingGizmo
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