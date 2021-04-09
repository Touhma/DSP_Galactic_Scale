using HarmonyLib;
using UnityEngine.UI;
using System.Reflection;
using System.IO;
using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using System;

namespace GalacticScale.Scripts.PatchUI
{
    [BepInPlugin("dsp.galactic-scale.debug", "Galactic Scale Plug-In - DEBUG", "1.3.4")]
    public class DebugCommands : BaseUnityPlugin
    {
        public new static ManualLogSource Logger;
        internal void Awake()
        {
            bool enabled = true;

            if (enabled)
            {
                var harmony = new Harmony("dsp.galactic-scale.debug");
                Logger = new ManualLogSource("GS DEBUG");
                BepInEx.Logging.Logger.Sources.Add(Logger);
                Logger.LogMessage("Galactic Scale DebugCommands Loaded");
                try
                {
                    harmony.PatchAll(typeof(DebugCommands));
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message);
                    Logger.LogError(e.StackTrace);
                }
            }
        }
    }
}