using HarmonyLib;
using UnityEngine.UI;
using Patch = GalacticScale.Scripts.PatchUI.PatchForUI;


namespace GalacticScale.Scripts.PatchUI
{
    [HarmonyPatch(typeof(UIVersionText))]
    public class PatchOnUIVersionText
    {
        [HarmonyPrefix]
        [HarmonyPatch("Refresh")]
        public static bool Refresh(ref Text ___textComp, string ___prefix, ref AccountData ___displayAccount, ref bool ___firstFrame)
        {
            if (___textComp != null)
            {
                bool flag = false;
                if (GameMain.data != null && !GameMain.instance.isMenuDemo && GameMain.isRunning)
                {
                    if (___displayAccount != GameMain.data.account)
                    {
                        ___displayAccount = GameMain.data.account;
                        flag = true;
                    }
                }
                else if (___displayAccount.userId != 0UL)
                {
                    ___displayAccount = AccountData.NULL;
                    flag = true;
                }
                if (___firstFrame || flag)
                {
                    string userName = ___displayAccount.detail.userName;
                    if (string.IsNullOrEmpty(userName))
                    {
                        ___textComp.fontSize = 18;
                        ___textComp.text = ___prefix.Translate() + " " + GameConfig.gameVersion.ToFullString() + "\nGalactic Scale v" + Patch.Version;
                    }
                    else
                    {
                        ___textComp.fontSize = 24;
                        ___textComp.text = ___prefix.Translate() + " " + GameConfig.gameVersion.ToFullString() + "\r\n" + userName + " - Galactic Scale v" + Patch.Version;
                    }

                }
            }
            ___firstFrame = false;
            return false;
        }
    }
}