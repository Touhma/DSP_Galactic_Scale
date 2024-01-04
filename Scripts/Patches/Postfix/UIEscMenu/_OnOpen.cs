using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

namespace GalacticScale.Patches
{
    public partial class PatchOnUIEscMenu
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(UIEscMenu), nameof(UIEscMenu._OnOpen))]
        public static void _OnOpen(ref Text ___stateText)
        {
            ___stateText.text += "\r\nGalactic Scale v" + GS3.Version;
            var r = GameObject.Find("UI Root/Overlay Canvas/In Game/Esc Menu/combat-detail-btn").GetComponent<RectTransform>();
            if (r != null)
            {
                r.position = new Vector3(r.position.x + 2, r.position.y, r.position.z);
            }
        }
        
    }
}