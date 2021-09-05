using System.Collections.Generic;
using UnityEngine.UI;

namespace GalacticScale
{
    public partial class GSUI
    {
        public bool Set(GSSliderConfig cfg)
        {
            //GS2.Warn("Setting Slider? : " + label);
            if (RectTransform == null) return false;
            var slider = RectTransform.GetComponentInChildren<Slider>();
            if (slider == null) return false;
            // GS2.Warn($"{Label} Slider Setting...({slider.value}) {DefaultValue} -> {cfg.defaultValue} {cfg.minValue}:{cfg.maxValue}");
            slider.minValue = cfg.minValue >= 0 ? cfg.minValue : slider.minValue;
            slider.maxValue = cfg.maxValue >= 0 ? cfg.maxValue : slider.maxValue;
            slider.value = cfg.defaultValue >= 0 ? cfg.defaultValue : slider.value;
            //GS2.Warn("Slider Set.");
            return true;
        }

        public bool Set(Val o)
        {
            //GS2.Log($"Set called by {GS2.GetCaller()} to set {o} for {Label}");

            if (RectTransform == null)
                //GS2.Warn($"RectTransform for {Label} null");

                return false;
            switch (Type)
            {
                case "Selector":
                    RectTransform.GetComponent<GSUISelector>().value = o;
                    return true;
                case "RangeSlider":
                    // GS2.Warn($"Trying to valuetuple {o} for {Label}");
                    FloatPair ff = o;
                    RectTransform.GetComponent<GSUIRangeSlider>().LowValue = ff.low;
                    RectTransform.GetComponent<GSUIRangeSlider>().HighValue = ff.high;
                    RectTransform.GetComponent<GSUIRangeSlider>().LowValue = ff.low;
                    // GS2.Warn($"{RectTransform.GetComponent<GSUIRangeSlider>().LowValue} {RectTransform.GetComponent<GSUIRangeSlider>().HighValue}");
                    return true;
                case "Slider":
                    RectTransform.GetComponentInChildren<Slider>().value = o;
                    return true;
                case "Input":
                    if (o == null)
                    {
                        GS2.Error($"Failed to set input {Label} as value was null");
                        return false;
                    }

                    RectTransform.GetComponentInChildren<InputField>().text = o;
                    return true;
                case "Checkbox":
                    var toggle = RectTransform.GetComponent<GSUIToggle>();
                    if (toggle is null)
                    {
                        GS2.Error($"Failed to find Toggle for {Label}");
                        return false;
                    }

                    // GS2.Log($"Found toggle for {Label} setting isOn:{o}");
                    toggle.Value = o;
                    return true;
                case "Group":
                    var gtoggle = RectTransform.GetComponent<GSUIToggleList>();
                    if (gtoggle is null)
                    {
                        GS2.Error($"Failed to find Toggle for {Label}");
                        return false;
                    }

                    GS2.Log($"Found togglelist for {Label} setting isOn:{o}");
                    gtoggle.Value = o;
                    return true;
                case "Combobox":
                    var cb = RectTransform.GetComponentInChildren<Dropdown>();
                    if (cb is null)
                    {
                        GS2.Error($"Failed to find UICombobox for {Label}");
                        return false;
                    }

                    if (o > cb.options.Count - 1)
                    {
                        GS2.Error($"Failed to set {o} for combobox '{Label}': Value > Item Count");
                        return false;
                    }

                    comboDefault = o;
                    cb.value = o;
                    return true;
            }

            return false;
        }

        public bool SetItems(List<string> items)
        {
            if (Type != "Combobox")
            {
                GS2.Warn("Trying to Set Items on non Combobox UI Element");
                return false;
            }

            GS2.Warn("TEST");
            GS2.Warn((items == null).ToString());
            Data = items;
            if (RectTransform != null) RectTransform.GetComponent<GSUIDropdown>().Items = items;
            return true;
        }
    }
}