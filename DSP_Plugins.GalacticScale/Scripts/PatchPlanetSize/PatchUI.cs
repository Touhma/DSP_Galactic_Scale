using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Rendering;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(UIVersionText))]
    public class PatchUI{

        public static int refreshGridRadius = -1;

        //segment count to 512 lut
        public static Dictionary<int, int[]> LUT512 = new Dictionary<int, int[]>();

        [HarmonyPrefix]
        [HarmonyPatch("Refresh")]
        public static bool PatchRefresh (ref Text ___textComp, string ___prefix, ref AccountData ___displayAccount, ref bool ___firstFrame)
        {
            if ((UnityEngine.Object)___textComp != (UnityEngine.Object)null)
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
                    string empty = string.Empty;
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