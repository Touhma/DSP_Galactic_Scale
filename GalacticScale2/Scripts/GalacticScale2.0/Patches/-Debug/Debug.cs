using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace GalacticScale

{
    public class PatchOnWhatever
    {

        // [HarmonyPostfix, HarmonyPatch(typeof(UIDysonBrush_Shell), "_OnInit")]
        // public static void _OnInit(UIDysonBrush_Shell __instance)
        // {
        //     __instance.previewRenderer2.startWidth = 0.5f;
        //     __instance.previewRenderer2.endWidth = 0.5f;
        // }
        // [HarmonyPrefix, HarmonyPatch(typeof(UIDysonDrawingGrid), "InitLineSegs")]
        // public static bool InitLineSegs(ref UIDysonDrawingGrid __instance)
        // {
        //     if (__instance.mcl != null)
        //     {
        //         __instance._tmp_hash = new HashSet<int>();
        //         Mesh sharedMesh = __instance.mcl.sharedMesh;
        //         Vector3[] array = sharedMesh.vertices;
        //         int[] triangles = sharedMesh.triangles;
        //         Color[] colors = sharedMesh.colors;
        //         __instance.vertices = array;
        //         __instance.indices = triangles;
        //         __instance.linesegs = new Vector3[triangles.Length * 2];
        //         __instance.lineWeights = new float[triangles.Length * 2];
        //         __instance.lineseg_cnt = 0;
        //         for (int i = 0; i < triangles.Length; i++)
        //         {
        //             int num = i / 3 * 3 + (i + 1) % 3;
        //             int num2 = triangles[i];
        //             int num3 = triangles[num];
        //             if (num3 < num2)
        //             {
        //                 int num4 = num2;
        //                 num2 = num3;
        //                 num3 = num4;
        //             }
        //             int item = num2 << 16 | num3;
        //             if (!__instance._tmp_hash.Contains(item))
        //             {
        //                 __instance._tmp_hash.Add(item);
        //                 Vector3 vector = array[num2];
        //                 Vector3 vector2 = array[num3];
        //                 float num5 = Mathf.Min(colors[num2].r, colors[num3].r);
        //                 if (__instance.isGraticule && Mathf.Abs((vector2 - vector).y) > 0.001f)
        //                 {
        //                     Vector3 from = vector;
        //                     Vector3 to = vector2;
        //                     from.y = 0f;
        //                     to.y = 0f;
        //                     if (Mathf.Abs(Vector3.Angle(from, to)) > 0.5f)
        //                     {
        //                         goto IL_1B1;
        //                     }
        //                 }
        //                 __instance.linesegs[__instance.lineseg_cnt * 2] = vector;
        //                 __instance.linesegs[__instance.lineseg_cnt * 2 + 1] = vector2;
        //                 __instance.lineWeights[__instance.lineseg_cnt * 2] = num5 * 10f;
        //                 __instance.lineWeights[__instance.lineseg_cnt * 2 + 1] = num5 * 10f;
        //                 __instance.lineseg_cnt++;
        //             }
        //             IL_1B1:;
        //         }
        //         __instance._tmp_hash.Clear();
        //         __instance._tmp_hash = null;
        //         GS2.LogJson(__instance.lineWeights);
        //         return false;
        //     }
        //     __instance.linesegs = null;
        //     __instance.lineseg_cnt = 0;
        //     return false;
        // }
        // [HarmonyPrefix, HarmonyPatch(typeof(Logger), "LogException")]
        [HarmonyPostfix, HarmonyPatch(typeof(UIRoot), "ExitProgramSplash")]
        public static void ExitProgramSplash(UIRoot __instance)
        {
            var lg = GameObject.Find("UI Root/Overlay Canvas/Splash/");
            // while (lg.transform.childCount > 0)
            //     Object.DestroyImmediate(lg.transform.GetChild(0).gameObject);
            var images = lg.GetComponentsInChildren<Image>();
            var rimages = lg.GetComponentsInChildren<RawImage>();
            foreach (var image in images)
                if (image.name == "black" || image.name == "black-bg")
                {
                    var splash = Utils.GetSplashSprite();
                    if (splash != null) image.sprite = splash;
                    GS2.splashImage = image;
                    image.color = Color.white;
                }
                else if (image.name == "bg" || image.name == "logo" || image.name == "dsp" || image.name =="dots" || image.name == "health-advice")
                {
                    image.enabled = false;
                }

            foreach (var rimage in rimages)
                if (rimage.name == "vignette" || rimage.name == "logo")
                    rimage.enabled = false;
        }
        [HarmonyPostfix, HarmonyPatch(typeof(VFPreload), "Update")]
        public static void Update(VFPreload __instance)
        {
            __instance.splashes[0].gameObject.SetActive(true);
            __instance.splashes[1].gameObject.SetActive(false);
        }
        [HarmonyPostfix, HarmonyPatch(typeof(VFPreload), "Restart")]
        public static void Restart(VFPreload __instance)
        {
            GS2.splashImage.sprite = Utils.GetSplashSprite() ?? null;
        }
        [HarmonyPostfix, HarmonyPatch(typeof(VFPreload), "Start")]
        public static void Start()
        {
            var lg = GameObject.Find("UI Root/Overlay Canvas/Splash/");
            // while (lg.transform.childCount > 0)
            //     Object.DestroyImmediate(lg.transform.GetChild(0).gameObject);
            var images = lg.GetComponentsInChildren<Image>();
            var rimages = lg.GetComponentsInChildren<RawImage>();
            foreach (var image in images)
                if (image.name == "black" || image.name == "black-bg")
                {
                    var splash = Utils.GetSplashSprite();
                    if (splash != null) image.sprite = splash;
                    GS2.splashImage = image;
                    image.color = Color.white;
                }
                else if (image.name == "bg" || image.name == "logo" || image.name == "dsp" || image.name =="dots" || image.name == "health-advice")
                {
                    image.enabled = false;
                }

            foreach (var rimage in rimages)
                if (rimage.name == "vignette" || rimage.name == "logo")
                    rimage.enabled = false;
            
        }
    }
}