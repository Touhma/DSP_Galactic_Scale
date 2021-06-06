using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnUIVirtualStarmap
    {
        [HarmonyPrefix, HarmonyPatch(typeof(UIVirtualStarmap), "_OnLateUpdate")]
        public static bool _OnLateUpdate(ref UIVirtualStarmap __instance)
        {
            int index1 = -1;
            float num1 = 1.7f;
            for (int index2 = 0; index2 < __instance.starPool.Count; ++index2)
            {
                if (__instance.starPool[index2].active)
                {
                    StarData starData = __instance.starPool[index2].starData;
                    Vector2 rectPoint = Vector2.zero;
                    UIRoot.ScreenPointIntoRect(Camera.main.WorldToScreenPoint((Vector3)starData.position), __instance.textGroup, out rectPoint);
                    rectPoint.x += 18f;
                    rectPoint.y += 6f;
                    __instance.starPool[index2].nameText.rectTransform.anchoredPosition = rectPoint;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    float num2 = NGPT.Kit.ClosestPoint2Straight(ray.origin, ray.GetPoint(300f), (Vector3)starData.position);
                    float num3 = Vector3.Distance(ray.GetPoint(300f * num2), (Vector3)starData.position);
                    if ((double)num3 < (double)num1)
                    {
                        num1 = (double)num3 >= (double)__instance.starPool[index2].pointRenderer.transform.localScale.x * 0.25 ? num3 : 0.0f;
                        index1 = index2;
                    }
                    //GS2.Warn($"index2 = {index2} GSSettings.birthStarId:{GSSettings.birthStarId}");
                    if (index2 == GSSettings.BirthStarId -1)
                    {
                        Color color = __instance.starColors.Evaluate(starData.color);
                        __instance.starPointBirth.gameObject.SetActive(true);
                        __instance.starPointBirth.material.SetColor("_TintColor", color);
                        __instance.starPointBirth.transform.localPosition = (Vector3)starData.position;
                    }
                }
            }
            bool pressing = VFInput.rtsConfirm.pressing;
            bool flag1 = !string.IsNullOrEmpty(__instance.clickText);
            for (int index2 = 1; index2 < __instance.starPool.Count; ++index2)
            {
                bool flag2 = __instance.starPool[index2].active && index2 == index1;
                __instance.starPool[index2].nameText.gameObject.SetActive(flag2);
                
                if (flag2 & flag1)
                {
                    if (pressing)
                        __instance.starPool[index2].nameText.text = __instance.starPool[index2].textContent + "\r\n" + __instance.clickText.Translate();
                    __instance.starPool[index2].nameText.rectTransform.sizeDelta = new Vector2(__instance.starPool[index2].nameText.preferredWidth, __instance.starPool[index2].nameText.preferredHeight);
                }
                else if (!flag2 & flag1)
                {
                    __instance.starPool[index2].nameText.text = __instance.starPool[index2].textContent;
                    __instance.starPool[index2].nameText.rectTransform.sizeDelta = new Vector2(__instance.starPool[index2].nameText.preferredWidth, __instance.starPool[index2].nameText.preferredHeight);
                }
            }
            bool flag3 = index1 >= 0 && __instance.starPool[index1].active;
            __instance.starPointSelection.gameObject.SetActive(flag3);
            __instance.starPool[GSSettings.BirthStarId -1].nameText.gameObject.SetActive(true);
            if (!flag3)
                return false;
            StarData starData1 = __instance.starPool[index1].starData;
            Color color1 = __instance.starColors.Evaluate(starData1.color);
            if (starData1.type == EStarType.NeutronStar)
                color1 = __instance.neutronStarColor;
            else if (starData1.type == EStarType.WhiteDwarf)
                color1 = __instance.whiteDwarfColor;
            else if (starData1.type == EStarType.BlackHole)
                color1 = __instance.blackholeColor;
            float num4 = 1.2f;
            if (starData1.type == EStarType.GiantStar)
                num4 = 3f;
            else if (starData1.type == EStarType.WhiteDwarf)
                num4 = 0.6f;
            else if (starData1.type == EStarType.NeutronStar)
                num4 = 0.6f;
            else if (starData1.type == EStarType.BlackHole)
                num4 = 0.8f;
            __instance.starPointSelection.material.SetColor("_TintColor", color1);
            __instance.starPointSelection.transform.localPosition = (Vector3)starData1.position;
            __instance.starPointSelection.transform.localScale = Vector3.one * (float)((double)num4 * 0.600000023841858 + 0.600000023841858);
            return false;
        }
    }
}