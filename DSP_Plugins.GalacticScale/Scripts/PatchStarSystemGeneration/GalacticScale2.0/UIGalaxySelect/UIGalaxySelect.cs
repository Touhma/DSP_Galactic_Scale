using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public partial class PatchOnUIGalaxySelect
    {
        public static GameObject CreateStarCountText(Slider slider)
        {
            RectTransform starCountSlider = slider.GetComponent<RectTransform>();
            slider.gameObject.SetActive(false);
            Text template = slider.GetComponentInParent<Text>();
            RectTransform starCountText = GameObject.Instantiate(template.GetComponent<RectTransform>(), template.GetComponentInParent<RectTransform>().parent, false);
            starCountText.anchoredPosition = new Vector2(starCountText.anchoredPosition.x + 140, starCountSlider.GetComponentInParent<RectTransform>().anchoredPosition.y);
            Object.DestroyImmediate(starCountText.GetComponent<Localizer>());
            starCountText.name = "GS Star Count";
            return starCountText.gameObject ;
        }
    }
}