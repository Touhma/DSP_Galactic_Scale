using System.Collections;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using GSSerializer;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class GS2
    {
        public static int PreferencesVersion = 2104;
    }
    

    [BepInPlugin("dsp.galactic-scale.2", "Galactic Scale 2 Plug-In", "2.4.25")]
    [BepInDependency("space.customizing.console", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("dsp.nebula-multiplayer-api", BepInDependency.DependencyFlags.SoftDependency)]
    public class Bootstrap : BaseUnityPlugin
    {
        public new static ManualLogSource Logger;
        public static Queue buffer = new();

        internal void Awake()
        {
            
            InitializeLogger();
            InitializeComponents();
            ApplyHarmonyPatches();
        }

        private void InitializeLogger()
        {
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            Assembly a = Assembly.GetAssembly(typeof(GSStar));
            GS2.Version = $"{v.Major}.{v.Minor}.{v.Build}";
            BCE.Console.Init();
            Logger = new ManualLogSource("GS2");
            BepInEx.Logging.Logger.Sources.Add(Logger);
            GS2.ConsoleSplash();
        }
        private void InitializeComponents()
        {
            if (GS2.TP == null) GS2.TP = gameObject.AddComponent<TeleportComponent>();
            if (GS2.InputComponent == null) GS2.InputComponent = gameObject.AddComponent<InputComponent>();
        }
        private void ApplyHarmonyPatches()
        {
            var harmony = new Harmony("dsp.galactic-scale.2");
            harmony.PatchAll(typeof(PatchOnWhatever));
            harmony.PatchAll(typeof(PatchOnBlueprintUtils));
            harmony.PatchAll(typeof(PatchOnBuildingGizmo));
            harmony.PatchAll(typeof(PatchOnBuildTool_BlueprintCopy));
            harmony.PatchAll(typeof(PatchOnBuildTool_BlueprintPaste));
            harmony.PatchAll(typeof(PatchOnBuildTool_Click));
            harmony.PatchAll(typeof(PatchOnBuildTool_Path));
            harmony.PatchAll(typeof(PatchOnBuildTool_PathAddon));
            harmony.PatchAll(typeof(PatchOnGameAbnormalityData));
            harmony.PatchAll(typeof(PatchOnGameData));
            harmony.PatchAll(typeof(PatchOnGameDesc));
            harmony.PatchAll(typeof(PatchOnGameHistoryData));
            harmony.PatchAll(typeof(PatchOnGameLoader));
            harmony.PatchAll(typeof(PatchOnGameMain));
            harmony.PatchAll(typeof(PatchOnGameOption));
            harmony.PatchAll(typeof(PatchOnGameSave));
            harmony.PatchAll(typeof(PatchOnGraticulePoser));
            harmony.PatchAll(typeof(PatchOnGuideMissionStandardMode));
            harmony.PatchAll(typeof(PatchOnNearColliderLogic));
            harmony.PatchAll(typeof(PatchOnPlanetAlgorithm));
            harmony.PatchAll(typeof(PatchOnPlanetAtmoBlur));
            harmony.PatchAll(typeof(PatchOnPlanetData));
            harmony.PatchAll(typeof(PatchOnPlanetFactory));
            harmony.PatchAll(typeof(PatchOnPlanetGrid));
            harmony.PatchAll(typeof(PatchOnPlanetModelingManager));
            harmony.PatchAll(typeof(PatchOnPlanetRawData));
            harmony.PatchAll(typeof(PatchOnPlanetSimulator));
            harmony.PatchAll(typeof(PatchOnPlatformSystem));
            harmony.PatchAll(typeof(PatchOnPlayerFootsteps));
            harmony.PatchAll(typeof(PatchOnPlayerMove_Fly));
            harmony.PatchAll(typeof(PatchOnPlayerMove_Sail));
            harmony.PatchAll(typeof(PatchOnPowerSystem));
            harmony.PatchAll(typeof(PatchOnSprayCoaterComponent));
            harmony.PatchAll(typeof(PatchOnStarGen));
            harmony.PatchAll(typeof(PatchOnStationComponent));
            harmony.PatchAll(typeof(PatchOnTrashSystem));
            harmony.PatchAll(typeof(PatchOnUIAdvisorTip));
            harmony.PatchAll(typeof(PatchOnUIBuildingGrid));
            harmony.PatchAll(typeof(PatchOnUIEscMenu));
            harmony.PatchAll(typeof(PatchOnUIGalaxySelect));
            harmony.PatchAll(typeof(PatchOnUIGame));
            harmony.PatchAll(typeof(PatchOnUIGameLoadingSplash));
            harmony.PatchAll(typeof(PatchOnUIMainMenu));
            harmony.PatchAll(typeof(PatchOnUIOptionWindow));
            harmony.PatchAll(typeof(PatchOnUIPlanetDetail));
            harmony.PatchAll(typeof(PatchOnUIResearchResultsWindow));
            harmony.PatchAll(typeof(PatchOnUIRoot));
            harmony.PatchAll(typeof(PatchOnUISpaceGuide));
            harmony.PatchAll(typeof(PatchOnUISpaceGuideEntry));
            harmony.PatchAll(typeof(PatchOnUIStarDetail));
            harmony.PatchAll(typeof(PatchOnUIStarmap));
            harmony.PatchAll(typeof(PatchOnUIStarmapPlanet));
            harmony.PatchAll(typeof(PatchOnUITutorialTip));
            harmony.PatchAll(typeof(PatchOnUIVeinDetail));
            harmony.PatchAll(typeof(PatchOnUIVersionText));
            harmony.PatchAll(typeof(PatchOnUIVirtualStarmap));
            harmony.PatchAll(typeof(PatchOnUniverseGen));
            harmony.PatchAll(typeof(PatchOnUniverseSimulator));
            harmony.PatchAll(typeof(PatchOnVFPreload));
            harmony.PatchAll(typeof(PatchOnUIAchievementPanel));
        }

        public static void Debug(object data, LogLevel logLevel, bool isActive)
        {
            if (isActive && Logger != null)
                {
                    while (buffer.Count > 0)
                    {
                        var o = buffer.Dequeue();
                        var l = ((object data, LogLevel loglevel, bool isActive))o;
                        if (l.isActive) Logger.Log(l.loglevel, "Q:" + l.data);
                    }

                    Logger.Log(logLevel, data);
                }
                else
                {
                    buffer.Enqueue((data, logLevel, true));
                }
            
        }

        public static void Debug(object data)
        {
            Debug(data, LogLevel.Message, true);
        }


    }
}