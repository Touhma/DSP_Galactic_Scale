using System.IO;
using GSSerializer;
using UnityEngine;

namespace GalacticScale
{
    public class InputComponent : MonoBehaviour
    {
        public void Awake()
        {
            GS2.Warn("Input component Awake");
        }

        public void FixedUpdate()
        {
            if (VFInput.alt && VFInput._copyKey)
            {
                GS2.Warn($"Local Planet:{GameMain.localPlanet} | {GameMain.localStar}");
                GS2.Warn($"Loaded:{GameMain.localPlanet.loaded} | Loading: {GameMain.localPlanet.loading}");
                GameMain.localPlanet.Load();
            }
            // GS2.Log("FU");
            if (VFInput.alt && VFInput.control && VFInput._openMechLight && GS2.Config.DevMode)
            {
                GS2.debugtool.Label = "Transition Distance";
                GS2.debugtool.Value = HandleLocalStarPlanets.TransitionRadii.ToString(); //alt ctrl L
            }

            // if (VFInput.control && VFInput.alt && VFInput.shift && VFInput._moveRight) GS2.Config.EnableDevMode(); //ctrl alt shift d
            // else if (VFInput.control && VFInput.shift && VFInput._moveRight) Utils.LogDFInfo(GameMain.localStar); //ctrl shift d
            if (VFInput.control && VFInput.shift && GS2.Config.DevMode && VFInput._rotate &&
                GameMain.localPlanet != null) // ctrl shift R
            {
                GS2.Warn("*");
                var filename = Path.Combine(GS2.DataDir, "WorkingTheme.json");
                if (!File.Exists(filename))
                {
                    var oldTheme = GS2.GetGSPlanet(GameMain.localPlanet).GsTheme;
                    var fs = new fsSerializer();
                    fs.TrySerialize(oldTheme, out var data);
                    var json = fsJsonPrinter.PrettyJson(data);
                    File.WriteAllText(filename, json);
                    GS2.ShowMessage("WorkingTheme.json has been exported. Use the same key combination to reload it");
                    return;
                }

                GameMain.mainPlayer.controller.movementStateInFrame = EMovementState.Sail;
                GameMain.mainPlayer.controller.actionSail.ResetSailState();
                GameCamera.instance.SyncForSailMode();

                var p = GameMain.localStar.planets[GameMain.localPlanet.index];
                var gsPlanet = GS2.GetGSPlanet(p);
                GS2.GetGSStar(p.star).counter = p.index;

                var newTheme = GS2.LoadJsonTheme(filename);
                GS2.Warn(
                    $"LOADED THEME {newTheme.Name} CustomGen:{newTheme.CustomGeneration} TA:{newTheme.TerrainSettings.Algorithm}");
                newTheme.Process();
                gsPlanet.Theme = newTheme.Name;

                p.Free();
                p.data = null;
                p.factory = null;
                p.terrainMaterial = null;
                p.oceanMaterial = null;
                p.atmosMaterial = null;
                p.atmosMaterialLate = null;
                p.minimapMaterial = null;
                GS2.SetPlanetTheme(p, gsPlanet);
                GameMain.localStar.planets[p.index] = p;
                gsPlanet.planetData = p;
                PlanetModelingManager.RequestLoadPlanet(p);
                GameMain.data.LeavePlanet();
            }
        }
    }
}