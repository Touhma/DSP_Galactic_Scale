using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public static class GSUIBrushSizePanel
    {
        private static readonly int[] Sizes = { 20, 10, 5, 1 };
        private static bool _created;

        private const float Width = 28f;
        private const float Height = 14f;
        private const float HGap = 4f;
        private const float VGap = 2f;

        public static void CreateInstance(UIBuildMenu buildMenu)
        {
            if (_created) return;

            var f1 = buildMenu.childButtons[1];
            if (f1 == null) return;

            var anchor = f1.transform as RectTransform;

            float anchorWidth = anchor.sizeDelta.x > 0 ? anchor.sizeDelta.x : 42f;
            float columnX = anchor.anchoredPosition.x - (anchorWidth / 2f + HGap + Width / 2f);

            float totalHeight = Sizes.Length * Height + (Sizes.Length - 1) * VGap;
            float topY = anchor.anchoredPosition.y + totalHeight / 2f - Height / 2f + 14f;

            for (int i = 0; i < Sizes.Length; i++)
            {
                float y = topY - i * (Height + VGap);
                PlaceButton(anchor, columnX, y, Sizes[i]);
            }

            _created = true;
        }

        private static void PlaceButton(RectTransform anchor, float x, float y, int size)
        {
            var btn = GSUIUtil.MakeSmallTextButton($"{size}x", Width, Height, 11);
            btn.gameObject.name = $"gs-brush-{size}x";

            btn.tips.tipTitle = "Brush Size";
            btn.tips.tipText = $"{size}x{size} area";

            var rect = btn.transform as RectTransform;
            rect.SetParent(anchor.parent);
            rect.anchorMin = anchor.anchorMin;
            rect.anchorMax = anchor.anchorMax;
            rect.pivot = anchor.pivot;
            rect.anchoredPosition3D = new Vector3(x, y, 0f);
            rect.localScale = Vector3.one;

            btn.onClick += _ => OnBrushSizeClick(size);
        }

        private static void OnBrushSizeClick(int size)
        {
            var reformTool = GameMain.mainPlayer?.controller.actionBuild.reformTool;
            if (reformTool != null) reformTool.brushSize = size;
        }
    }
}