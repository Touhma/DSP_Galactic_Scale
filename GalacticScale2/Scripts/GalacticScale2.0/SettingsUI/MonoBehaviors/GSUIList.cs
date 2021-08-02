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
            var go = AddItem(templates.minmax);
            go.SetActive(true);
            return go.GetComponent<GSUIRangeSlider>();
        }

        public object AddToggle()
        {
            var go = AddItem(templates.toggle);
            go.SetActive(true);
            return go.GetComponent<GSUIToggle>();
        }
    }



}