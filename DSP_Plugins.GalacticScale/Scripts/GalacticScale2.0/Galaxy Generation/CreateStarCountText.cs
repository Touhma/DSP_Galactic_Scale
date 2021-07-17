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
            var starCountText = Instantiate(template.GetComponent<RectTransform>(),
                template.GetComponentInParent<RectTransform>().parent, false);
            starCountText.anchoredPosition = new Vector2(starCountText.anchoredPosition.x + 140,
                starCountSlider.GetComponentInParent<RectTransform>().anchoredPosition.y);
            DestroyImmediate(starCountText.GetComponent<Localizer>());
            starCountText.name = "GS Star Count";
            return starCountText.gameObject;
        }
    }
}