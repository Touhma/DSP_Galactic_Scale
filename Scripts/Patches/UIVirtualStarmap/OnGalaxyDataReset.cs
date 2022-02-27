using HarmonyLib;
using NebulaCompatibility;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnUIVirtualStarmap
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(UIVirtualStarmap), "OnGalaxyDataReset")]
        public static bool OnGalaxyDataReset(ref UIVirtualStarmap __instance)
        {
            //GS2.Error("............................................");
            if (GS2.Vanilla) return true;
            // if (NebulaCompat.NebulaIsInstalled) return true;
            foreach (var starNode in __instance.starPool)
            {
                starNode.active = false;
                starNode.starData = null;
                starNode.pointRenderer.gameObject.SetActive(false);
                starNode.nameText.gameObject.SetActive(false);
            }

            foreach (var connNode in __instance.connPool)
            {
                connNode.active = false;
                connNode.starA = null;
                connNode.starB = null;
                connNode.lineRenderer.gameObject.SetActive(false);
            }

            if (__instance._galaxyData == null) return false;

            var index1 = 0;
            for (var index2 = 0; index2 < __instance._galaxyData.stars.Length; ++index2)
            {
                while (__instance.starPool.Count <= index2)
                    __instance.starPool.Add(new UIVirtualStarmap.StarNode
                    {
                        active = false,
                        starData = null,
                        pointRenderer = Object.Instantiate(__instance.starPointPrefab, __instance.starPointPrefab.transform.parent),
                        nameText = Object.Instantiate(__instance.nameTextPrefab, __instance.nameTextPrefab.transform.parent)
                    });

                var star1 = __instance._galaxyData.stars[index2];
                var a1 = __instance.starColors.Evaluate(star1.color);
                if (star1.type == EStarType.NeutronStar)
                    a1 = __instance.neutronStarColor;
                else if (star1.type == EStarType.WhiteDwarf)
                    a1 = __instance.whiteDwarfColor;
                else if (star1.type == EStarType.BlackHole) a1 = __instance.blackholeColor;

                var num1 = 1.2f;
                if (star1.type == EStarType.GiantStar)
                    num1 = 3f;
                else if (star1.type == EStarType.WhiteDwarf)
                    num1 = 0.6f;
                else if (star1.type == EStarType.NeutronStar)
                    num1 = 0.6f;
                else if (star1.type == EStarType.BlackHole) num1 = 0.8f;

                var str = star1.displayName + "  ";
                if (star1.type == EStarType.GiantStar)
                    str = star1.spectr > ESpectrType.K ? star1.spectr > ESpectrType.F ? star1.spectr != ESpectrType.A ? str + "蓝巨星".Translate() : str + "白巨星".Translate() : str + "黄巨星".Translate() : str + "红巨星".Translate();
                else if (star1.type == EStarType.WhiteDwarf)
                    str += "白矮星".Translate();
                else if (star1.type == EStarType.NeutronStar)
                    str += "中子星".Translate();
                else if (star1.type == EStarType.BlackHole)
                    str += "黑洞".Translate();
                else if (star1.type == EStarType.MainSeqStar) str = str + star1.spectr + "型恒星".Translate();

                if (star1.index == GSSettings.BirthPlanet.planetData.star.index)
                    str = "即将登陆".Translate() + "\r\n" + str;

                __instance.starPool[index2].active = true;
                __instance.starPool[index2].starData = star1;
                __instance.starPool[index2].pointRenderer.material.SetColor("_TintColor", a1);
                __instance.starPool[index2].pointRenderer.transform.localPosition = star1.position;
                __instance.starPool[index2].pointRenderer.transform.localScale = Vector3.one * num1;
                __instance.starPool[index2].pointRenderer.gameObject.SetActive(true);
                __instance.starPool[index2].nameText.text = str;
                __instance.starPool[index2].nameText.color = Color.Lerp(a1, Color.white, 0.5f);
                __instance.starPool[index2].nameText.rectTransform.sizeDelta = new Vector2(__instance.starPool[index2].nameText.preferredWidth, __instance.starPool[index2].nameText.preferredHeight);
                __instance.starPool[index2].nameText.rectTransform.anchoredPosition = new Vector2(-2000f, -2000f);
                __instance.starPool[index2].textContent = str;
                if (star1.index == GSSettings.BirthPlanet.planetData.star.index)
                    __instance.starPool[index2].nameText.gameObject.SetActive(true);
                else
                    __instance.starPool[index2].nameText.gameObject.SetActive(false);

                var graphNode = __instance._galaxyData.graphNodes[index2];
                for (var index3 = 0; index3 < graphNode.lines.Count; ++index3)
                {
                    var line = graphNode.lines[index3];
                    if (line.index > graphNode.index)
                    {
                        while (__instance.connPool.Count <= index1)
                            __instance.connPool.Add(new UIVirtualStarmap.ConnNode
                            {
                                active = false,
                                starA = null,
                                starB = null,
                                lineRenderer = Object.Instantiate(__instance.connLinePrefab, __instance.connLinePrefab.transform.parent)
                            });

                        var star2 = __instance._galaxyData.stars[line.index];
                        var a2 = __instance.starColors.Evaluate(star2.color);
                        if (star2.type == EStarType.NeutronStar)
                            a2 = __instance.neutronStarColor;
                        else if (star2.type == EStarType.WhiteDwarf)
                            a2 = __instance.whiteDwarfColor;
                        else if (star2.type == EStarType.BlackHole) a2 = __instance.blackholeColor;

                        var num2 = 1.2f;
                        if (star2.type == EStarType.GiantStar)
                            num2 = 3f;
                        else if (star2.type == EStarType.WhiteDwarf)
                            num2 = 0.6f;
                        else if (star2.type == EStarType.NeutronStar)
                            num2 = 0.6f;
                        else if (star2.type == EStarType.BlackHole) num2 = 0.8f;

                        __instance.connPool[index1].active = true;
                        __instance.connPool[index1].starA = star1;
                        __instance.connPool[index1].starB = star2;
                        __instance.connPool[index1].lineRenderer.material.SetColor("_LineColorA", Color.Lerp(a1, Color.white, 0.65f));
                        __instance.connPool[index1].lineRenderer.material.SetColor("_LineColorB", Color.Lerp(a2, Color.white, 0.65f));
                        var num3 = Vector3.Distance(star1.position, star2.position);
                        var t1 = (float)(num1 * 0.25 + 0.349999994039536) / num3;
                        var t2 = (float)(1.0 - (num2 * 0.25 + 0.349999994039536) / num3);
                        __instance.connPool[index1].lineRenderer.SetPosition(0, Vector3.Lerp(star1.position, star2.position, t1));
                        __instance.connPool[index1].lineRenderer.SetPosition(1, Vector3.Lerp(star1.position, star2.position, t2));
                        __instance.connPool[index1].lineRenderer.gameObject.SetActive(true);
                        ++index1;
                    }
                }
            }

            return false;
        }
    }
}