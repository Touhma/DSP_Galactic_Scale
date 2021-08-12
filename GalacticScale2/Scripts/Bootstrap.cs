using System.Collections;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    [BepInPlugin("dsp.galactic-scale.2", "Galactic Scale 2 Plug-In", "2.0.85.1")]
    [BepInDependency("space.customizing.console", BepInDependency.DependencyFlags.SoftDependency)]
    public class Bootstrap : BaseUnityPlugin
    {
        public new static ManualLogSource Logger;

        // Internal Variables
        public static Queue buffer = new Queue();
        public static PlanetData TeleportPlanet;
        public static StarData TeleportStar;
        public static bool TeleportEnabled;
        public static int timer;

        internal void Awake()
        {
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            GS2.Version = $"2.0b{v.Build}.{v.Revision}";
            BCE.Console.Init();
            var _ = new Harmony("dsp.galactic-scale.2");
            Logger = new ManualLogSource("GS2");
            BepInEx.Logging.Logger.Sources.Add(Logger);
            GS2.ConsoleSplash();
            Harmony.CreateAndPatchAll(typeof(PatchOnWhatever));
            Harmony.CreateAndPatchAll(typeof(PatchOnBlueprintUtils));
            Harmony.CreateAndPatchAll(typeof(PatchOnBuildingGizmo));
            Harmony.CreateAndPatchAll(typeof(PatchOnBuildTool_BlueprintCopy));
            Harmony.CreateAndPatchAll(typeof(PatchOnBuildTool_BlueprintPaste));
            Harmony.CreateAndPatchAll(typeof(PatchOnBuildTool_Click));
            Harmony.CreateAndPatchAll(typeof(PatchOnBuildTool_Path));
            Harmony.CreateAndPatchAll(typeof(PatchOnGameData));
            Harmony.CreateAndPatchAll(typeof(PatchOnGameDesc));
            Harmony.CreateAndPatchAll(typeof(PatchOnGameHistoryData));
            Harmony.CreateAndPatchAll(typeof(PatchOnGameLoader));
            Harmony.CreateAndPatchAll(typeof(PatchOnGameMain));
            Harmony.CreateAndPatchAll(typeof(PatchOnGameOption));
            Harmony.CreateAndPatchAll(typeof(PatchOnGraticulePoser));
            Harmony.CreateAndPatchAll(typeof(PatchOnGuideMissionStandardMode));
            Harmony.CreateAndPatchAll(typeof(PatchOnNearColliderLogic));
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
            Harmony.CreateAndPatchAll(typeof(PatchOnUIVersionText));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIVirtualStarmap));
            Harmony.CreateAndPatchAll(typeof(PatchOnUniverseGen));
        }

        private void FixedUpdate()
        {
            timer++;
            if (timer >= 1000)
                timer = 0;
            //GS2.Warn("FixedUpdate");
            if (!GS2.Config.CheatMode) return;
            if (DSPGame.IsMenuDemo) return;
            if (TeleportStar == null && TeleportPlanet == null || TeleportEnabled == false ||
                !(GameMain.localStar != null && GameMain.localStar.loaded)) return;
            if (TeleportPlanet != null)
            {
                GS2.Warn($"TP to Planet {TeleportPlanet?.name} of star {TeleportPlanet?.star?.name}");

                GameMain.data.ArriveStar(TeleportPlanet?.star);
                StartCoroutine(Teleport(TeleportPlanet));
            }
            else if (TeleportStar != null)
            {
                GS2.Warn($"TP to Star {TeleportStar?.name}");
                GameMain.data.ArriveStar(TeleportStar);
                StartCoroutine(Teleport(TeleportStar));
            }
        }

        public static void Debug(object data, LogLevel logLevel, bool isActive)
        {
            if (isActive)
            {
                if (Logger != null)
                {
                    while (buffer.Count > 0)
                    {
                        var o = buffer.Dequeue();
                        var l = ((object data, LogLevel loglevel, bool isActive)) o;
                        if (l.isActive) Logger.Log(l.loglevel, "Q:" + l.data);
                    }

                    Logger.Log(logLevel, data);
                }
                else
                {
                    buffer.Enqueue((data, logLevel, true));
                }
            }
        }

        public static void Debug(object data)
        {
            Debug(data, LogLevel.Message, true);
        }

        private IEnumerator Teleport(PlanetData planet)
        {
            yield return new WaitForEndOfFrame();
            TeleportPlanet = null;
            TeleportStar = null;
            GameMain.mainPlayer.uPosition = planet.uPosition + VectorLF3.unit_z * planet.realRadius;
            GameMain.data.mainPlayer.movementState = EMovementState.Sail;
            TeleportEnabled = false;
            GameMain.mainPlayer.transform.localScale = Vector3.one;
        }

        private IEnumerator Teleport(StarData star)
        {
            yield return new WaitForEndOfFrame();
            TeleportPlanet = null;
            TeleportStar = null;
            GameMain.mainPlayer.uPosition = star.uPosition + VectorLF3.unit_z * 1;
            GameMain.data.mainPlayer.movementState = EMovementState.Sail;
            TeleportEnabled = false;
            GameMain.mainPlayer.transform.localScale = Vector3.one;
            GameCamera.instance.FrameLogic();
        }
    }
}