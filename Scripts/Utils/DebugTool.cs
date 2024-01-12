using System;
using UnityEngine;
using UnityEngine.UI;
using Object = System.Object;

namespace GalacticScale
{
    public class DebugTool:MonoBehaviour
    {
        public GameObject go;
        Text label;
        Text value;
        public string Label;
        public string Value;
        private void FixedUpdate()
        {
            if (label == null || value == null)
            {
                GS2.Devlog("Error, label or value null");
                return;
            }
            label.text = Label;
            value.text = Value;
        }

        public void Set(string title, string val)
        {
            if (label != null && value != null)
            {
                Label = title;
                Value = val;
            }
        }
        public static DebugTool Init()
        {
            var filepath = "assets/debugdisplay.prefab";
            var prefab = GS2.Bundle.LoadAsset<GameObject>(filepath);
            var g = GameObject.Instantiate(prefab, UIRoot.instance.uiGame.transform, false);
            var dt = g.AddComponent<DebugTool>();
            var texts = g.GetComponentsInChildren(typeof(Text));
            dt.label = texts[1] as Text;
            dt.value = texts[0] as Text;
            dt.go = g;
            return dt;
        }
    }
}