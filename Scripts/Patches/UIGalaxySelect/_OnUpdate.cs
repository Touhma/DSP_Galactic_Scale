using HarmonyLib;
using NebulaCompatibility;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnUIGalaxySelect
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIGalaxySelect), "_OnUpdate")]
        public static bool _OnUpdate(UIGalaxySelect __instance)
        {
            if (NebulaCompat.IsClient) return true;
            __instance.starmap._Update();
            // if (GS2.ModellingDone)
            // {
            //     StartButton.GetComponent<UIButton>().button.onClick.RemoveAllListeners();
            //     StartButton.GetComponent<UIButton>().button.onClick.AddListener(__instance.EnterGame);
            // }
            // else
            // {
            //     StartButton.GetComponent<UIButton>().button.onClick.RemoveAllListeners();
            // }
            if (Mathf.Abs(Mathf.DeltaAngle(__instance.lastCameraYaw, __instance.cameraPoser.yawWanted)) > 1f) __instance.autoCameraYaw = false;
            if (__instance.autoCameraYaw)
            {
                __instance.autoRotateSpeed += Time.deltaTime * 6f;
                if (__instance.autoRotateSpeed > 12f) __instance.autoRotateSpeed = 12f;
                __instance.cameraPoser.yawWanted += Time.deltaTime * __instance.autoRotateSpeed;
                __instance.cameraPoser.pitchWanted += (__instance.cameraPoser.pitchBegin - __instance.cameraPoser.pitchWanted) * 0.005f * Mathf.Clamp01(__instance.autoRotateSpeed / 6f);
            }

            __instance.lastCameraYaw = __instance.cameraPoser.yawWanted;
            if (VFInput.escape) SystemDisplay.OnBackClick(__instance);
            __instance.darkFogDetailBtn.gameObject.SetActive(__instance.gameDesc.isCombatMode);//0.10...
            __instance.darkFogLogo.color = new Color(1f, 1f, 1f, (__instance.gameDesc.isCombatMode ? 0.86f : 0.06f) + (__instance.darkFogToggle.isMouseEnter ? 0.14f : 0f));
            __instance.darkBackground.gameObject.SetActive(__instance.uiCombat.active);
            if (__instance.uiCombat.active != __instance.dfDemoGroup.gameObject.activeSelf)
            {
                __instance.dfDemoGroup.gameObject.SetActive(__instance.uiCombat.active);
            }
            // __instance.startButtonText.text = (__instance.uiCombat.active ? "黑雾设置页面返回" : "开始游戏").Translate();
            __instance.startButtonRect.sizeDelta = new Vector2((float)(__instance.uiCombat.active ? 250 : 180), 60f);
            __instance.UpdatePropertyGroup();//...0.10
            return false;
        }
    }
}