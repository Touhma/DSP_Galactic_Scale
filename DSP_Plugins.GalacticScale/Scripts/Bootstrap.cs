using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace GalacticScale {

    [BepInPlugin("dsp.galactic-scale.GSStar-system-generation", "Galactic Scale Plug-In - Star System Generation", "2.0.0.0")]
    public class Bootstrap : BaseUnityPlugin {
        public new static ManualLogSource Logger;

        // Internal Variables
        public static bool DebugReworkPlanetGen = false;
        public static bool DebugReworkPlanetGenDeep = false;
        public static bool DebugStarGen = false;
        public static bool DebugStarGenDeep = false;
        public static bool DebugStarNamingGen = false;

        internal void Awake() {
            System.Version v = Assembly.GetExecutingAssembly().GetName().Version;
            GS2.Version = $"2.0a{v.Build}.{v.Revision}";
            BCE.console.init();
            var harmony = new Harmony("dsp.galactic-scale.star-system-generation");
            Logger = new ManualLogSource("GS2");
            BepInEx.Logging.Logger.Sources.Add(Logger);
            GS2.ConsoleSplash();
            Harmony.CreateAndPatchAll(typeof(PatchOnBuildingGizmo));
            Harmony.CreateAndPatchAll(typeof(PatchUIBuildingGrid));
            Harmony.CreateAndPatchAll(typeof(PatchOnGameData));
            Harmony.CreateAndPatchAll(typeof(PatchOnGameDesc));
            Harmony.CreateAndPatchAll(typeof(PatchOnGameLoader));
            Harmony.CreateAndPatchAll(typeof(PatchOnGameMain));
            Harmony.CreateAndPatchAll(typeof(PatchOnGameOption));
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
            //PatchOnPlayerAction_Build // new dsp patch completely changed this, removed for now
            Harmony.CreateAndPatchAll(typeof(PatchPlayerFootsteps));
            Harmony.CreateAndPatchAll(typeof(PatchPlayerMove_Fly));
            Harmony.CreateAndPatchAll(typeof(PatchOnPlayerMove_Sail));
            Harmony.CreateAndPatchAll(typeof(PatchOnStarGen)); // Only used when using vanilla generator, to allow 1024 stars
            Harmony.CreateAndPatchAll(typeof(PatchOnStationComponent));
            Harmony.CreateAndPatchAll(typeof(PatchUIAdvisorTip));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIEscMenu));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIGalaxySelect));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIGameLoadingSplash));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIOptionWindow));  
            Harmony.CreateAndPatchAll(typeof(PatchOnUIPlanetDetail));           
            Harmony.CreateAndPatchAll(typeof(PatchOnUIResearchResultsWindow));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIRoot)); 
            Harmony.CreateAndPatchAll(typeof(PatchOnUISpaceGuide));
            Harmony.CreateAndPatchAll(typeof(PatchOnUISpaceGuideEntry));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIStarDetail));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIStarmap));
            Harmony.CreateAndPatchAll(typeof(PatchOnUIStarmapPlanet));         
            Harmony.CreateAndPatchAll(typeof(PatchUITutorialTip)); 
            Harmony.CreateAndPatchAll(typeof(PatchOnUIVersionText));                              
            Harmony.CreateAndPatchAll(typeof(PatchOnUIVirtualStarmap)); 
            Harmony.CreateAndPatchAll(typeof(PatchOnUniverseGen));
        }

        public static void Debug(object data, LogLevel logLevel, bool isActive) {
            if (isActive) Logger.Log(logLevel, data);
        }
        public static void Debug(object data)
        {
            Logger.LogMessage(data);
        }
        public static PlanetData TeleportPlanet = null;
        public static StarData TeleportStar = null;
        public static bool TeleportEnabled = false;
        private void Update()
        {
            //GS2.Warn($"Cheatmode:{GS2.CheatMode} ResearchUnlocked:{GS2.ResearchUnlocked}");
            if (!GS2.CheatMode) return;
            if (DSPGame.IsMenuDemo) return;

            //GS2.Warn($"Teleport Status {TeleportStar} {TeleportPlanet} {TeleportEnabled}");
            if ((TeleportStar == null && TeleportPlanet == null) || TeleportEnabled == false || !GameMain.localStar.loaded) return;
            if (TeleportPlanet != null)
            {
                GS2.Warn($"TP to Planet {TeleportPlanet.name} of star {TeleportPlanet.star?.name}");
                GameMain.data.ArriveStar(TeleportPlanet.star);
                StartCoroutine(Teleport(TeleportPlanet));
            } else if (TeleportStar != null)
            {
                GameMain.data.ArriveStar(TeleportStar);
                StartCoroutine(Teleport(TeleportStar));
            }

        }
        private IEnumerator Teleport(PlanetData planet)
        {

            yield return new WaitForEndOfFrame();
            TeleportPlanet = null;
            TeleportStar = null;
            //GS2.Warn($"Teleporting to {planet.name} @ {planet.uPosition}");
            GameMain.mainPlayer.uPosition = planet.uPosition + VectorLF3.unit_z * (planet.realRadius);
            
            GameMain.data.mainPlayer.movementState = EMovementState.Sail;
            TeleportEnabled = false;
            GameMain.mainPlayer.transform.localScale = Vector3.one;
            //GS2.Warn($"End of TP, LocalPlanet:{GameMain.localPlanet?.name} LocalStar:{GameMain.localStar?.name}");
        }
        private IEnumerator Teleport(StarData star)
        {
            yield return new WaitForEndOfFrame();
            TeleportPlanet = null;
            TeleportStar = null;
            //GS2.Warn($"Teleporting to {star.name}" + star.uPosition.ToString());
            GameMain.mainPlayer.uPosition = star.uPosition + VectorLF3.unit_z * 1;
            GameMain.data.mainPlayer.movementState = EMovementState.Sail;
            TeleportEnabled = false;
            GameMain.mainPlayer.transform.localScale = Vector3.one;
            //GS2.Warn($"End of TP, LocalPlanet:{GameMain.localPlanet?.name} LocalStar:{GameMain.localStar?.name}");
        }
    }
}