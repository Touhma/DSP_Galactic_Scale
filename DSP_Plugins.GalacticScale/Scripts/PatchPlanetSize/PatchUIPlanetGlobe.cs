using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;
using System.Reflection;


namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(UIPlanetGlobe))]
    public class PatchUIPlanetGlobe{
        
        [HarmonyPostfix]
        [HarmonyPatch("_OnUpdate")]
        public static void PatchOnUpdate(ref Text ___geoInfoText) {
        if (GameMain.localPlanet != null && VFInput.alt) ___geoInfoText.text = "\r\nRadius " + GameMain.localPlanet.radius;
        }
    }
}