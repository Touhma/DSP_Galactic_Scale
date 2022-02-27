using HarmonyLib;
using UnityEngine;
using UnityEngine.Experimental.UIElements;

namespace GalacticScale
{
    public partial class PatchOnUIGalaxySelect
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIGalaxySelect), "_OnUpdate")]
        
        public static bool _OnUpdate(UIGalaxySelect __instance)
        {
            __instance.starmap._Update();
            if (GS2.ModellingDone)
            {
                StartButton.GetComponent<UIButton>().button.onClick.RemoveAllListeners();
                StartButton.GetComponent<UIButton>().button.onClick.AddListener(__instance.EnterGame);
            }
            else
            {
                StartButton.GetComponent<UIButton>().button.onClick.RemoveAllListeners();
            }
            if (Mathf.Abs(Mathf.DeltaAngle(__instance.lastCameraYaw, __instance.cameraPoser.yawWanted)) > 1f)
            {
                __instance.autoCameraYaw = false;
            }
            if (__instance.autoCameraYaw)
            {
                __instance.autoRotateSpeed += Time.deltaTime * 6f;
                if (__instance.autoRotateSpeed > 12f)
                {
                    __instance.autoRotateSpeed = 12f;
                }
                __instance.cameraPoser.yawWanted += Time.deltaTime * __instance.autoRotateSpeed;
                __instance.cameraPoser.pitchWanted += (__instance.cameraPoser.pitchBegin - __instance.cameraPoser.pitchWanted) * 0.005f * Mathf.Clamp01(__instance.autoRotateSpeed / 6f);
            }
            __instance.lastCameraYaw = __instance.cameraPoser.yawWanted;
            if (VFInput.escape)
            {
                SystemDisplay.OnBackClick(__instance);
            }

            return false;
        }
    }
}