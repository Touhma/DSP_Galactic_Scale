using System;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnUILoadGameWindow
    {
                [HarmonyPrefix]
        [HarmonyPatch(typeof(UILoadGameWindow), "OnSelectedChange")]
public static bool OnSelectedChange(UILoadGameWindow __instance)
{
	if (__instance.selected == null || __instance.selected.saveName == null)
	{
		__instance.ReadScreenShotTexture(null);
		__instance.detailContent.gameObject.SetActive(false);
		__instance.prop1Text.text = "<color=#FFFFFF19>—</color>";
		__instance.prop2Text.text = "<color=#FFFFFF19>—</color>";
		__instance.prop3Text.text = "<color=#FFFFFF19>—</color>";
		__instance.prop4Text.text = "<color=#FFFFFF19>—</color>";
		__instance.prop5Text.text = "<color=#FFFFFF19>—</color>";
		__instance.prop6Text.text = "<color=#FFFFFF19>—</color>";
		__instance.prop7Text.text = "<color=#FFFFFF19>—</color>";
		__instance.prop8Text.text = "<color=#FFFFFF19>—</color>";
		__instance.loadButton.button.interactable = false;
        __instance.gameDescIsSandBox = false;
        __instance.loadWithSandboxTools = false;
        __instance.loadSandboxGroup.SetActive(false);
        __instance.loadSandboxToggle.isOn = false;
        __instance.loadSandboxTipText.gameObject.SetActive(false);
		return false;
	}
	__instance.detailContent.gameObject.SetActive(true);
	GameSaveHeader gameSaveHeader;
	GameDesc gameDesc;
	ClusterPropertyData clusterPropertyData;
	GameSave.ReadHeaderAndDescAndProperty(__instance.selected.saveName, true, out gameSaveHeader, out gameDesc, out clusterPropertyData);
    GS2.Warn($"Header Version:{gameSaveHeader?.headerVersion}");
	if (gameSaveHeader != null)
	{
		__instance.ReadScreenShotTexture(gameSaveHeader.themeImage);
		__instance.corruptText.gameObject.SetActive(false);
		__instance.prop1Text.text = string.Format("{0:yyyy-MM-dd  HH:mm:ss}", gameSaveHeader.saveTime.ToLocalTime());
		int num = (int)(gameSaveHeader.gameTick % 60L);
		int num2 = (int)(gameSaveHeader.gameTick / 60L);
		int num3 = num2 / 60;
		int num4 = num3 / 60;
		num2 %= 60;
		num3 %= 60;
		__instance.prop2Text.text = string.Format("{0:00}:{1:00}:{2:00} <size=11>{3:00}</size>", new object[]
		{
			num4,
			num3,
			num2,
			num
		});
		__instance.prop3Text.text = gameSaveHeader.fileSize.ToString("#,##0");
		__instance.prop4Text.text = __instance.VersionToRichString(gameDesc.creationVersion) + " / " + __instance.VersionToRichString(gameSaveHeader.lastSaveVersion);
		if (string.IsNullOrEmpty(gameSaveHeader.accountData.userName))
		{
			__instance.prop5Text.text = "匿名".Translate();
			__instance.prop5Text.color = new Color(1f, 1f, 1f, 0.1f);
		}
		else
		{
			__instance.prop5Text.text = gameSaveHeader.accountData.userName;
			__instance.prop5Text.color = new Color(1f, 1f, 1f, 1f);
		}
		if (gameSaveHeader.clusterGeneration == 0UL)
		{
			__instance.prop6Text.text = "<color=#FFFFFF19>—</color>";
		}
		else
		{
			__instance.prop6Text.text = (gameSaveHeader.clusterGeneration * 60UL).ToString("#,##0") + " <size=14>W</size>";
		}
		__instance.prop7Text.text = gameDesc.clusterString;
		if (gameDesc.creationTime > new DateTime(2021, 6, 20))
		{
			__instance.prop8Text.text = string.Format("{0:yyyy-MM-dd  HH:mm:ss}", gameDesc.creationTime.ToLocalTime());
		}
		else
		{
			__instance.prop8Text.text = "<color=#FFFFFF19>—</color>";
		}
		__instance.loadButton.button.interactable = true;
		if (clusterPropertyData != null)
		{
			int[] productIds = PropertySystem.productIds;
			int num5 = 0;
			for (int i = 0; i < productIds.Length; i++)
			{
				int itemProduction = clusterPropertyData.GetItemProduction(productIds[i]);
				num5 += itemProduction;
				__instance.propertyItems[i].SetCountTextN0(itemProduction);
			}
			__instance.zeroPropertyProduction = (num5 == 0);
            __instance.gameDescIsSandBox = gameDesc.isSandboxMode;
            __instance.loadWithSandboxTools = __instance.gameDescIsSandBox;
            __instance.loadSandboxGroup.SetActive(true);
            __instance.loadSandboxToggle.isOn = __instance.loadWithSandboxTools;
            __instance.loadSandboxTipText.gameObject.SetActive(false);
			return false;
		}
	}
	else
	{
		__instance.ReadScreenShotTexture(null);
		__instance.corruptText.gameObject.SetActive(true);
		__instance.prop1Text.text = "<color=#FFFFFF19>—</color>";
		__instance.prop2Text.text = "<color=#FFFFFF19>—</color>";
		__instance.prop3Text.text = "<color=#FFFFFF19>—</color>";
		__instance.prop4Text.text = "<color=#FFFFFF19>—</color>";
		__instance.prop5Text.text = "<color=#FFFFFF19>—</color>";
		__instance.prop6Text.text = "<color=#FFFFFF19>—</color>";
		__instance.prop7Text.text = "<color=#FFFFFF19>—</color>";
		__instance.prop8Text.text = "<color=#FFFFFF19>—</color>";
		__instance.loadButton.button.interactable = false;
        __instance.gameDescIsSandBox = false;
        __instance.loadWithSandboxTools = false;
        __instance.loadSandboxGroup.SetActive(false);
        __instance.loadSandboxToggle.isOn = false;
        __instance.loadSandboxTipText.gameObject.SetActive(false);
	}

    return false;
}
    }
}