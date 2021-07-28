using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale
{
    public class ExternalThemeSelector : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            //itemTemplate.SetActive(false);
        }


        // public ThemeLibrary ThemeLibrary = new ThemeLibrary();
        public GameObject itemTemplate;
        public Toggle masterToggle;
        public List<ThemeSelectItem> items = new List<ThemeSelectItem>();

        public void Init(List<string> themes)
        {
            foreach (var theme in themes)
            {
               // var go = Object.Instantiate(itemTemplate, gameObject.transform);
               // go.SetActive(true);
               // var tsi = go.GetComponent<ThemeSelectItem>();
              //  items.Add(tsi);
              //  tsi.label = theme;
              //  tsi.Set(false);
              Debug.Log("Test"+theme);
            }
        }

        public List<string> Get()
        {
            var output = new List<string>();
            foreach (var tsi in items)
            {
                if (tsi.ticked) output.Add(tsi.label);
            }

            return output;
        }

        public void MasterToggleClick()
        {
            Debug.Log("Click");
            Debug.Log(masterToggle.isOn.ToString());
            Init(new List<string>() {"Test1", "test2", "gds", "fasdfda"});
        }
    }
}