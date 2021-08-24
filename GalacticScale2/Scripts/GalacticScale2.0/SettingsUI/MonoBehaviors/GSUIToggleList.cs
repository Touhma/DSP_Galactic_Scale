using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace GalacticScale
{
    public class GSUIToggleList : GSUIList
    {

        public Toggle toggle;
       

        public bool Value
        {
            get => toggle.isOn;
            set
            {
                // GS2.Warn($"Setting Value for {Label} to {value}"); 
                toggle.isOn = value;
            }
        }

        public GSOptionCallback OnChange;

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