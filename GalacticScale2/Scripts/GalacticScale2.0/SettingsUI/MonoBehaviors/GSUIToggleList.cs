using UnityEngine.UI;

namespace GalacticScale
{
    public class GSUIToggleList : GSUIList
    {
        public Toggle toggle;

        public GSOptionCallback OnChange;


        public bool Value
        {
            get => toggle.isOn;
            set =>
                // GS2.Warn($"Setting Value for {Label} to {value}"); 
                toggle.isOn = value;
        }

        public void _OnToggleChange(bool value)
        {
            // GS2.Log(value.ToString());
            Value = value;
            OnChange?.Invoke(value);
        }

        public new void Initialize(GSUI group)
        {
            var data = (GSUIGroupConfig)group.Data;
            Label = group.Label;
            Hint = group.Hint;
            Collapsible = data.collapsible;
            ShowHeader = data.header;
            OnChange = group.callback;
        }
    }
}