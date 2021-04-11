using HarmonyLib;
using System.Reflection;
using System.IO;
using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using System;

namespace GalacticScale.Scripts.PatchUI {
    [BepInPlugin("dsp.galactic-scale.ui", "Galactic Scale Plug-In - UI", "1.4.0")]
    public class PatchForUI : BaseUnityPlugin {
        public new static ManualLogSource Logger;
        public static string Version = "1.4";
        public static AssetBundle bundle;
        internal void Awake()
        {
            var harmony = new Harmony("dsp.galactic-scale.ui");
            Logger = new ManualLogSource("PatchUI");
            BepInEx.Logging.Logger.Sources.Add(Logger);
            Logger.LogMessage("Galactic Scale Version " + Version + " loading");
            try
            {
                harmony.PatchAll(typeof(PatchForUI));
                Harmony.CreateAndPatchAll(typeof(PatchOnUIEscMenu));
                Harmony.CreateAndPatchAll(typeof(PatchOnUIGameLoadingSplash));
                Harmony.CreateAndPatchAll(typeof(PatchOnUIVersionText)); 
                Harmony.CreateAndPatchAll(typeof(PatchOnUIPlanetDetail));
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                Logger.LogError(e.StackTrace);
            }
        }

        public static Sprite GetSpriteAsset(string name)
        {
            if (bundle == null) bundle = AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetAssembly(typeof(PatchForUI)).Location), "galacticbundle"));
            if (bundle == null)
            {
                Debug.Log("Failed to load AssetBundle!");
                return null;
            }
            return bundle.LoadAsset<Sprite>(name);
        }
    }
}
