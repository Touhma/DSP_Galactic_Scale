using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale.Scripts.PatchUI
{

    public class PatchOnUIPlanetDetail
    {
        [HarmonyPostfix, HarmonyPatch(typeof(UIPlanetDetail),"OnPlanetDataSet")]
        public static void OnPlanetDataSet(ref UIPlanetDetail __instance, Text ___obliquityValueText,ref PlanetData ____planet)
        {
            // Add the planets radius to the Planet Detail UI
            if (___obliquityValueText.transform.parent.transform.parent.childCount == 6)
            {

                GameObject radiusLabel;
                GameObject obliquityLabel = ___obliquityValueText.transform.parent.gameObject;
                radiusLabel = GameObject.Instantiate(obliquityLabel, obliquityLabel.transform.parent.transform);

                radiusLabel.transform.localPosition += (Vector3.down * 20);
                Text radiusLabelText = radiusLabel.GetComponent<Text>();
                radiusLabelText.GetComponent<Localizer>().enabled = false;
                Image radiusIcon = radiusLabel.transform.GetChild(1).GetComponent<Image>();
                UIButton uiButton = radiusLabel.transform.GetChild(1).GetComponent<UIButton>();
                uiButton.tips.tipText = "How large the planet is. Standard is 200";
                uiButton.tips.tipTitle = "Planet Radius";

                //GS2.LogJson(uiButton.button);
                if (uiButton.button == null) uiButton.button = uiButton.gameObject.AddComponent<Button>();
                uiButton.button.transform.SetParent(uiButton.transform);
               
                radiusIcon.sprite = Utils.GetSpriteAsset("ruler");
                Text radiusValueText = radiusLabel.transform.GetChild(0).GetComponent<Text>();
                radiusLabelText.text = "Planetary Radius";
                radiusValueText.text = __instance.planet.realRadius.ToString();
            }
            if (___obliquityValueText.transform.parent.transform.parent.childCount == 7)
            {
                Transform p = ___obliquityValueText.transform.parent.parent;
                GameObject radiusLabel = p.GetChild(p.childCount - 1).gameObject;
                Text radiusValueText = radiusLabel.transform.GetChild(0).GetComponent<Text>();
                if (__instance.planet != null) radiusValueText.text = __instance.planet.realRadius.ToString();
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch("SetResCount")]
        public static bool SetResCount(int count, ref RectTransform ___rectTrans, ref RectTransform ___paramGroup) // Adjust the height of the PlanetDetail UI to allow for Radius Text
        {
            ___rectTrans.sizeDelta = new Vector2(___rectTrans.sizeDelta.x, (float)(190 + count * 20) + 20f);
            ___paramGroup.anchoredPosition = new Vector2(___paramGroup.anchoredPosition.x, (float)(-90 - count * 20));
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIStarmap), "OnCursorFunction3Click")]
        public static bool OnCursorFunction3Click(PlanetData ___viewPlanet)
        {
            var go = GameObject.Find("UI Root/Overlay Canvas/In Game/Starmap UIs/starmap-screen-ui/cursor-view/bg");
            if (___viewPlanet != null &&  (VFInput.control))
            {
                Bootstrap.TeleportPlanet = ___viewPlanet;
                Bootstrap.TeleportEnabled = true;
                return false;
            }
            else return true;
            
        }

    }
}