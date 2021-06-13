using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale {
    public class GSButton : GSUI {
        public override string Type { get => "Button"; }
        public new object DefaultValue {
            get {
                GS2.Error("Trying to get default value for button {label}");
                return null;
            }
        }

        public new void Reset() => GS2.Warn($"Trying to reset button {label}");
        public override bool Disable() {
            if (disabled) {
                GS2.Warn($"Trying to disable UI Button {label} that is already disabled");
                return false;
            }
            var c = RectTransform.GetComponentsInChildren<Image>();
            foreach (Image i in c) {
                int id = i.GetInstanceID();
                if (colors.ContainsKey(id)) colors[id] = i.color;
                else colors.Add(id, i.color);
                i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a / 2);
            }
            Button button = RectTransform.GetComponentInChildren<Button>();
            if (button is null) {
                GS2.Error($"Cannot find Button component on {label}");
                return false;
            }
            button.interactable = false; 
            disabled = true; 
            return true;

        }
        public override bool Enable() {
            if (!disabled) {
                GS2.Warn("Trying to enable UI Button that is already enabled");
                return false;
            }
            var c = RectTransform.GetComponentsInChildren<Image>();
            foreach (Image i in c) {
                int id  = i.GetInstanceID();
                i.color = colors[id];
            }
            Button button = RectTransform.GetComponentInChildren<Button>();
            if (button is null) {
                GS2.Error($"Cannot find Button component on {label}");
                return false;
            }
            button.interactable = true; 
            disabled = false; 
            return true;
        }
        public override bool Set(GSSliderConfig s) { GS2.Error($"Trying to set UI Button {label}"); return false; }
        public override bool Set(object o) { GS2.Error($"Trying to set UI Button {label}"); return false; }
        public override bool SetItems(List<string> items) { GS2.Error($"Trying to set items on a UI Button {label}"); return false; }

        public GSButton(string label, string caption, GSOptionCallback callback, string tip = "") {
            this.key = null;
            this.label = label;
            this.data = caption;
            this.callback = callback;
            this.tip = tip;
        }
    }
}
