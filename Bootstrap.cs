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


    [BepInPlugin("dsp.galactic-scale.2", "Galactic Scale 2 Plug-In", "2.3.13")]
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
            GS2.Version = $"{v.Major}.{v.Minor}.{v.Build}";
            BCE.Console.Init();
            Logger = new ManualLogSource("GS2");
            BepInEx.Logging.Logger.Sources.Add(Logger);
            GS2.ConsoleSplash();
        }
        private void InitializeComponents()
        {
            if (GS2.TP == null) GS2.TP = gameObject.AddComponent<TeleportComponent>();
            GS2.Warn(GS2.TP.name);
            GS2.Warn("Adding InputComponent");
            if (GS2.InputComponent == null) GS2.InputComponent = gameObject.AddComponent<InputComponent>();
            GS2.Warn("Added");
            GS2.Warn(GS2.InputComponent.name);
        }
        private void ApplyHarmonyPatches()
        {
            var _ = new Harmony("dsp.galactic-scale.2");
            Harmony.CreateAndPatchAll(typeof(PatchOnWhatever));
            Harmony.CreateAndPatchAll(typeof(PatchOnBlueprintUtils));
            Harmony.CreateAndPatchAll(typeof(PatchOnBuildingGizmo));
            Harmony.CreateAndPatchAll(typeof(PatchOnBuildTool_BlueprintCopy));
            Harmony.CreateAndPatchAll(typeof(PatchOnBuildTool_BlueprintPaste));
            Harmony.CreateAndPatchAll(typeof(PatchOnBuildTool_Click));
            Harmony.CreateAndPatchAll(typeof(PatchOnBuildTool_Path));
            Harmony.CreateAndPatchAll(typeof(PatchOnGameAbnormalityData));
            Harmony.CreateAndPatchAll(typeof(PatchOnGameData));
            Harmony.CreateAndPatchAll(typeof(PatchOnGameDesc));
            Harmony.CreateAndPatchAll(typeof(PatchOnGameHistoryData));
            Harmony.CreateAndPatchAll(typeof(PatchOnGameLoader));
            Harmony.CreateAndPatchAll(typeof(PatchOnGameMain));
            Harmony.CreateAndPatchAll(typeof(PatchOnGameOption));
            Harmony.CreateAndPatchAll(typeof(PatchOnGameSave));
            Harmony.CreateAndPatchAll(typeof(PatchOnGraticulePoser));
            Harmony.CreateAndPatchAll(typeof(PatchOnGuideMissionStandardMode));
            Harmony.CreateAndPatchAll(typeof(PatchOnNearColliderLogic));
            Harmony.CreateAndPatchAll(typeof(PatchOnPlanetAlgorithm));            
            Harmony.CreateAndPatchAll(typeof(PatchOnPlanetAtmoBlur));
            Harmony.CreateAndPatchAll(typeof(PatchOnPlanetData));
            Harmony.CreateAndPatchAll(typeof(PatchOnPlanetFactory));
            Harmony.CreateAndPatchAll(typeof(PatchOnPlanetGrid));
            Harmony.CreateAndPatchAll(typeof(PatchOnPlanetModelingManager));
            Harmony.CreateAndPatchAll(typeof(PatchOnPlanetRawData));
            Harmony.CreateAndPatchAll(typeof(PatchOnPlanetSimulator));
            Harmony.CreateAndPatchAll(typeof(PatchOnPlatformSystem));
            Harmony.CreateAndPatchAll(typeof(PatchOnPlayerFootsteps));
            Harmony.CreateAndPatchAll(typeof(PatchOnPlayerMove_Fly));
            Harmony.CreateAndPatchAll(typeof(PatchOnPlayerMove_Sail));
            Harmony.CreateAndPatchAll(typeof(PatchOnPowerSystem));
            Harmony.CreateAndPatchAll(typeof(PatchOnStarGen));
            Harmony.CreateAndPatchAll(typeof(PatchOnStationComponent));
            Harmony.CreateAndPatchAll(typeof(PatchOnTrashSystem));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIAdvisorTip));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIBuildingGrid));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIEscMenu));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIGalaxySelect));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIGame));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIGameLoadingSplash));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIMainMenu));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIOptionWindow));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIPlanetDetail));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIResearchResultsWindow));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIRoot));
            Harmony.CreateAndPatchAll(typeof(PatchOnUISpaceGuide));
            Harmony.CreateAndPatchAll(typeof(PatchOnUISpaceGuideEntry));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIStarDetail));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIStarmap));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIStarmapPlanet));
            Harmony.CreateAndPatchAll(typeof(PatchOnUITutorialTip));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIVeinDetail));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIVersionText));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIVirtualStarmap));
            Harmony.CreateAndPatchAll(typeof(PatchOnUniverseGen));
            Harmony.CreateAndPatchAll(typeof(PatchOnUniverseSimulator));
            Harmony.CreateAndPatchAll(typeof(PatchOnVFPreload));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIAchievementPanel));
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