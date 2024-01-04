using UnityEngine;

namespace GalacticScale
{
    public class ExternalThemeSelector : MonoBehaviour
    {
        // // public ThemeLibrary ThemeLibrary = new ThemeLibrary();
        // public RectTransform itemTemplate;
        // public RectTransform groupTemplate;
        // public RectTransform itemList;
        // public Toggle masterToggle;
        // public List<ThemeSelectItem> items = new List<ThemeSelectItem>();
        //
        // public List<ThemeSelectGroup> groups = new List<ThemeSelectGroup>();
        //
        // // Start is called before the first frame update
        // private void Start()
        // {
        //     if (GS3.availableExternalThemes.Count == 0)
        //     {
        //         gameObject.SetActive(false);
        //         // GS3.Warn(gameObject.name);
        //
        //         return;
        //     }
        //
        //     itemTemplate.gameObject.SetActive(false);
        //     groupTemplate.gameObject.SetActive(false);
        //     // GS3.LogJson(GS3.availableExternalThemes);
        //     GS3.themeSelector = this;
        //     if (GS3.availableExternalThemes.ContainsKey("Root"))
        //         foreach (var t in GS3.availableExternalThemes["Root"])
        //         {
        //             GS3.Warn($"Adding {t.Key}");
        //             var item = Instantiate(itemTemplate, itemList, false);
        //             var tsi = item.GetComponent<ThemeSelectItem>();
        //             tsi.label = t.Value.DisplayName;
        //             tsi.theme = t.Value;
        //             tsi.gameObject.SetActive(true);
        //             items.Add(tsi);
        //         }
        //
        //     foreach (var td in GS3.availableExternalThemes)
        //     {
        //         if (td.Key == "Root") continue;
        //         // GS3.Warn($"Adding {td.Key}");
        //         var item = Instantiate(groupTemplate, itemList, false);
        //         var tsg = item.GetComponent<ThemeSelectGroup>();
        //         tsg.label = td.Key;
        //         tsg.themes = td.Value;
        //         tsg.gameObject.SetActive(true);
        //         groups.Add(tsg);
        //     }
        //
        //     var names = GS3.Config.ExternalThemeNames;
        //     // var groupNames = new List<string>();
        //     // var itemNames = new List<string>();
        //     if (names.Count > 0)
        //     {
        //         masterToggle.isOn = true;
        //         GS3.Config.SetUseExternalThemes(true);
        //     }
        //     else
        //     {
        //         masterToggle.isOn = false;
        //         GS3.Config.SetUseExternalThemes(false);
        //         
        //     }
        //
        //     foreach (var name in names)
        //     {
        //         var flag = false;
        //         var fragments = name.Split('|');
        //         var label = fragments[0];
        //         if (fragments[1] != "*") label = fragments[1];
        //         foreach (var group in groups)
        //             if (group.label == label)
        //             {
        //                 group.Set(true);
        //                 flag = true;
        //             }
        //
        //         if (flag) continue;
        //         foreach (var item in items)
        //             if (item.theme.Name == label)
        //                 item.Set(true);
        //     }
        // }
        //
        // public void Init()
        // {
        //     foreach (var theme in GS3.availableExternalThemes) Debug.Log("Test" + theme.Value);
        // }
        //
        // public void CollapseAll()
        // {
        //     foreach (var t in items) t.gameObject.SetActive(false);
        // }
        //
        // public void ExpandAll()
        // {
        //     foreach (var t in items) t.gameObject.SetActive(true);
        // }
        //
        // public void CheckAll()
        // {
        //     // GS3.Warn("CheckAll");
        //     foreach (var t in items) t.Set(true);
        // }
        //
        // public void SetAll(bool val)
        // {
        //     if (val) CheckAll();
        //     else UnCheckAll();
        // }
        //
        // public void ToggleAll()
        // {
        //     // GS3.Warn("Toggle");
        //     if (!masterToggle.isOn) UnCheckAll();
        //     else CheckAll();
        // }
        //
        // public void UnCheckAll()
        // {
        //     // GS3.Warn("Uncheck All");
        //     foreach (var t in items) t.Set(false);
        // }
        //
        // public List<string> Get()
        // {
        //     var output = new List<string>();
        //     foreach (var tsi in items)
        //         if (tsi.ticked)
        //             output.Add("Root|" + tsi.theme.Name);
        //     foreach (var tsg in groups) if (tsg.ticked) output.Add(tsg.label + "|*");
        //     return output;
        // }
        //
        // public void MasterToggleClick()
        // {
        //     // Debug.Log("Click");
        //     Debug.Log(masterToggle.isOn.ToString());
        //     GS3.Config.SetUseExternalThemes(masterToggle.isOn);
        // }
    }
}