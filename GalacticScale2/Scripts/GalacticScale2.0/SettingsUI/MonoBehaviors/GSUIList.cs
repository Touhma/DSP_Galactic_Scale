using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace GalacticScale
{
    public class GSUIList : MonoBehaviour
    {
        public GameObject List;
        public List<GameObject> Contents;
        public GameObject ListHeading;
        public GameObject ListCollapseButton;
        public GameObject ListExpandButton;
        public GameObject ListContents;
        public GSUITemplates templates;
        public bool Collapsible = false;
        public bool ShowHeader = true;
        public Text _labelText;
        public Text _hintText;
        public string Hint
        {
            get => _hintText.text;
            set => _hintText.text = value;
        }
        public string Label
        {
            get => _labelText.text;
            set => _labelText.text = value;
        }
        public GameObject AddItem(GSUITemplate template)
        {
            GS2.Log($"Adding Item Template Null?:{template == null} ListContents Null?{ListContents == null}");
            var newItem = Instantiate(template.gameObject, ListContents.transform, false);
            Contents.Add(newItem);
            return newItem;
        }

        public GSUIHeader AddHeader()
        {
            var go = AddItem(templates.header);
            go.SetActive(true);
            return go.GetComponent<GSUIHeader>();
        }

        public void Start()
        {
            if (!ShowHeader)
            {
                Collapsible = false;
                ListHeading.SetActive(false);
            }
            if (!Collapsible)
            {
                ListExpandButton.SetActive(false);
                ListCollapseButton.SetActive(false);
                ListContents.SetActive(true);
            }


        }

        public GSUIRangeSlider AddRangeSlider()
        {
            var go = AddItem(templates.rangeslider);
            go.SetActive(true);
            return go.GetComponent<GSUIRangeSlider>();
        }
        public GSUISlider AddSlider()
        {
            var go = AddItem(templates.slider);
            go.SetActive(true);
            return go.GetComponent<GSUISlider>();
        }

        public GSUIToggle AddToggle()
        {
            var go = AddItem(templates.toggle);
            go.SetActive(true);
            return go.GetComponent<GSUIToggle>();
        }
        public GSUIDropdown AddDropdown()
        {
            var go = AddItem(templates.dropdown);
            go.SetActive(true);
            return go.GetComponent<GSUIDropdown>();
        }
        public GSUIButton AddButton()
        {
            var go = AddItem(templates.button);
            go.SetActive(true);
            return go.GetComponent<GSUIButton>();
        }
        public GSUIInput AddInput()
        {
            var go = AddItem(templates.input);
            go.SetActive(true);
            return go.GetComponent<GSUIInput>();
        }

        internal GSUIList AddList()
        {
            var go = AddItem(templates.list);
            go.SetActive(true);
            return go.GetComponent<GSUIList>();
        }
        public void initialize(GSUI group)
        {
            var data = (GSUIGroupConfig)group.Data;
            Label = group.Label;
            Collapsible = data.collapsible;
            ShowHeader = data.header;

        }
    }



}