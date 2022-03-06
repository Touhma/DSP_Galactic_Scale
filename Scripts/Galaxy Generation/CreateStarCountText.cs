using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public partial class PatchOnUIGalaxySelect
    {
        public static GameObject CreateStarCountText(Slider slider)
        {
            var starCountSlider = slider.GetComponent<RectTransform>();
            slider.gameObject.SetActive(false);
            var template = slider.GetComponentInParent<Text>();
            RectTransform starCountText;
            if (template != null)
            {
                var rect = template.GetComponent<RectTransform>();
                if (rect == null) return null;

                starCountText = Object.Instantiate(rect, template.GetComponentInParent<RectTransform>().parent, false);
                starCountText.anchoredPosition = new Vector2(starCountText.anchoredPosition.x + 140, starCountSlider.GetComponentInParent<RectTransform>().anchoredPosition.y);
                Object.DestroyImmediate(starCountText.GetComponent<Localizer>());
                starCountText.name = "GS Star Count";
                return starCountText.gameObject;
            }

            return null;
        }
    }
}