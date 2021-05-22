using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
//UNUSED (Left for Debugging)
namespace GalacticScale
{
    [HarmonyPatch(typeof(UIVirtualStarmap))]
    public static class PatchOnUIVirtualStarMap
    {

		[HarmonyPrefix]
		[HarmonyPatch("OnGalaxyDataReset")]
		public static bool OnGalaxyDataReset(ref UIVirtualStarmap __instance)
		{
			//GS2.Log("OnGalaxyDataReset");
			foreach (UIVirtualStarmap.StarNode item in __instance.starPool)
			{
				item.active = false;
				item.starData = null;
				item.pointRenderer.gameObject.SetActive(value: false);
				item.nameText.gameObject.SetActive(value: false);
			}
			foreach (UIVirtualStarmap.ConnNode item2 in __instance.connPool)
			{
				item2.active = false;
				item2.starA = null;
				item2.starB = null;
				item2.lineRenderer.gameObject.SetActive(value: false);
			}
			if (__instance._galaxyData == null)
			{
				return false;
			}
			int num = 0;
			for (int i = 0; i < __instance._galaxyData.stars.Length; i++)
			{
				while (__instance.starPool.Count <= i)
				{
					UIVirtualStarmap.StarNode starNode = new UIVirtualStarmap.StarNode();
					starNode.active = false;
					starNode.starData = null;
					starNode.pointRenderer = Object.Instantiate(__instance.starPointPrefab, __instance.starPointPrefab.transform.parent);
					starNode.nameText = Object.Instantiate(__instance.nameTextPrefab, __instance.nameTextPrefab.transform.parent);
					__instance.starPool.Add(starNode);
				}
				StarData starData = __instance._galaxyData.stars[i];
				Color color = __instance.starColors.Evaluate(starData.color);
				//GS2.Log("Star type " + starData.type + " spectr " + starData.spectr + " color " + starData.color + " classFactor " + starData.classFactor);
				if (starData.type == EStarType.NeutronStar)
				{
					color = __instance.neutronStarColor;
				}
				else if (starData.type == EStarType.WhiteDwarf)
				{
					color = __instance.whiteDwarfColor;
				}
				else if (starData.type == EStarType.BlackHole)
				{
					color = __instance.blackholeColor;
				}
				float num2 = 1.2f;
				if (starData.type == EStarType.GiantStar)
				{
					num2 = 3f;
				}
				else if (starData.type == EStarType.WhiteDwarf)
				{
					num2 = 0.6f;
				}
				else if (starData.type == EStarType.NeutronStar)
				{
					num2 = 0.6f;
				}
				else if (starData.type == EStarType.BlackHole)
				{
					num2 = 0.8f;
				}
				string text = starData.displayName + "  ";
				if (starData.type == EStarType.GiantStar)
				{
					text = ((starData.spectr <= ESpectrType.K) ? (text + "红巨星".Translate()) : ((starData.spectr <= ESpectrType.F) ? (text + "黄巨星".Translate()) : ((starData.spectr != ESpectrType.A) ? (text + "蓝巨星".Translate()) : (text + "白巨星".Translate()))));
				}
				else if (starData.type == EStarType.WhiteDwarf)
				{
					text += "白矮星".Translate();
				}
				else if (starData.type == EStarType.NeutronStar)
				{
					text += "中子星".Translate();
				}
				else if (starData.type == EStarType.BlackHole)
				{
					text += "黑洞".Translate();
				}
				else if (starData.type == EStarType.MainSeqStar)
				{
					text = text + starData.spectr.ToString() + "型恒星".Translate();
				}
				if (starData.index == 0)
				{
					text = "即将登陆".Translate() + "\r\n" + text;
				}
				__instance.starPool[i].active = true;
				__instance.starPool[i].starData = starData;
				__instance.starPool[i].pointRenderer.material.SetColor("_TintColor", color);
				__instance.starPool[i].pointRenderer.transform.localPosition = starData.position;
				__instance.starPool[i].pointRenderer.transform.localScale = Vector3.one * num2;
				__instance.starPool[i].pointRenderer.gameObject.SetActive(value: true);
				__instance.starPool[i].nameText.text = text;
				__instance.starPool[i].nameText.color = Color.Lerp(color, Color.white, 0.5f);
				__instance.starPool[i].nameText.rectTransform.sizeDelta = new Vector2(__instance.starPool[i].nameText.preferredWidth, __instance.starPool[i].nameText.preferredHeight);
				__instance.starPool[i].nameText.rectTransform.anchoredPosition = new Vector2(-2000f, -2000f);
				__instance.starPool[i].textContent = text;
				if (starData.index == 0)
				{
					__instance.starPool[i].nameText.gameObject.SetActive(value: true);
				}
				else
				{
					__instance.starPool[i].nameText.gameObject.SetActive(value: false);
				}
				StarGraphNode starGraphNode = __instance._galaxyData.graphNodes[i];
				for (int j = 0; j < starGraphNode.lines.Count; j++)
				{
					StarGraphNode starGraphNode2 = starGraphNode.lines[j];
					if (starGraphNode2.index > starGraphNode.index)
					{
						while (__instance.connPool.Count <= num)
						{
							UIVirtualStarmap.ConnNode connNode = new UIVirtualStarmap.ConnNode();
							connNode.active = false;
							connNode.starA = null;
							connNode.starB = null;
							connNode.lineRenderer = Object.Instantiate(__instance.connLinePrefab, __instance.connLinePrefab.transform.parent);
							__instance.connPool.Add(connNode);
						}
						StarData starData2 = __instance._galaxyData.stars[starGraphNode2.index];
						Color a = __instance.starColors.Evaluate(starData2.color);
						if (starData2.type == EStarType.NeutronStar)
						{
							a = __instance.neutronStarColor;
						}
						else if (starData2.type == EStarType.WhiteDwarf)
						{
							a = __instance.whiteDwarfColor;
						}
						else if (starData2.type == EStarType.BlackHole)
						{
							a = __instance.blackholeColor;
						}
						float num3 = 1.2f;
						if (starData2.type == EStarType.GiantStar)
						{
							num3 = 3f;
						}
						else if (starData2.type == EStarType.WhiteDwarf)
						{
							num3 = 0.6f;
						}
						else if (starData2.type == EStarType.NeutronStar)
						{
							num3 = 0.6f;
						}
						else if (starData2.type == EStarType.BlackHole)
						{
							num3 = 0.8f;
						}
						__instance.connPool[num].active = true;
						__instance.connPool[num].starA = starData;
						__instance.connPool[num].starB = starData2;
						__instance.connPool[num].lineRenderer.material.SetColor("_LineColorA", Color.Lerp(color, Color.white, 0.65f));
						__instance.connPool[num].lineRenderer.material.SetColor("_LineColorB", Color.Lerp(a, Color.white, 0.65f));
						float num4 = Vector3.Distance(starData.position, starData2.position);
						float t = (num2 * 0.25f + 0.35f) / num4;
						float t2 = 1f - (num3 * 0.25f + 0.35f) / num4;
						__instance.connPool[num].lineRenderer.SetPosition(0, Vector3.Lerp(starData.position, starData2.position, t));
						__instance.connPool[num].lineRenderer.SetPosition(1, Vector3.Lerp(starData.position, starData2.position, t2));
						__instance.connPool[num].lineRenderer.gameObject.SetActive(value: true);
						num++;
					}
				}
			}
			return false;
		}
	}
}