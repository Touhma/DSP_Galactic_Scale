using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public static class GSUIUtil
    {
    public static UIButton MakeSmallTextButton(string label = "", float width = 0, float height = 0, int fontSize = 12)
    {
    var assemblerWindow = UIRoot.instance.uiGame.assemblerWindow;
    var go = Object.Instantiate(assemblerWindow.copyButton.gameObject);
    var btn = go.GetComponent<UIButton>();

    var oldChild = go.transform.Find("Text");
    var font = oldChild.GetComponent<Text>()?.font;
    Object.DestroyImmediate(oldChild.gameObject);

    var textGo = new GameObject("Text", typeof(RectTransform), typeof(Text));
    textGo.transform.SetParent(go.transform, false);
    var textRect = textGo.GetComponent<RectTransform>();
    textRect.anchorMin = Vector2.zero;
    textRect.anchorMax = Vector2.one;
    textRect.offsetMin = Vector2.zero;
    textRect.offsetMax = Vector2.zero;

    var txt = textGo.GetComponent<Text>();
    txt.font = font;
    txt.text = label;
    txt.fontSize = fontSize;
    txt.color = Color.white;
    txt.alignment = TextAnchor.MiddleCenter;
    txt.horizontalOverflow = HorizontalWrapMode.Overflow;
    txt.verticalOverflow = VerticalWrapMode.Overflow;

    btn.tips.tipText = "";
    btn.tips.tipTitle = "";
    btn.tips.delay = 0.6f;

    if (width > 0 || height > 0)
    {
        var rect = (RectTransform)go.transform;
        if (width == 0) width = rect.sizeDelta.x;
        if (height == 0) height = rect.sizeDelta.y;
        rect.sizeDelta = new Vector2(width, height);
    }

    go.transform.localScale = Vector3.one;

    return btn;
        }
    }
}