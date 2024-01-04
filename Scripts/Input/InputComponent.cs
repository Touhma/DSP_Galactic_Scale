using System.IO;
using GSSerializer;
using UnityEngine;

namespace GalacticScale
{
    public class InputComponent : MonoBehaviour
    {
        // public void Awake()
        // {
        //     GS3.Warn("Input component Awake");
        // }

        public void FixedUpdate()
        {
            // GS3.Log("FU");
            if (VFInput.alt && VFInput.control && VFInput._openMechLight) GS3.WarnJson(HandleLocalStarPlanets.TransitionRadii); //alt ctrl L
            if (VFInput.control && VFInput.alt && VFInput.shift && VFInput._moveRight) GS3.Config.EnableDevMode(); //ctrl alt shift d
            else if (VFInput.control && VFInput.shift && VFInput._moveRight) Utils.LogDFInfo(GameMain.localStar); //ctrl shift d
            if (VFInput.control && VFInput.shift && VFInput._rotate && GameMain.localPlanet != null) // ctrl shift R
            {
                GS3.Warn("*");
                var filename = Path.Combine(GS3.DataDir, "WorkingTheme.json");
                if (!File.Exists(filename))
                {
                    var oldTheme = GS3.GetGSPlanet(GameMain.localPlanet).GsTheme;
                    var fs = new fsSerializer();
                    fs.TrySerialize(oldTheme, out var data);
                    var json = fsJsonPrinter.PrettyJson(data);
                    File.WriteAllText(filename, json);
                    GS3.ShowMessage("WorkingTheme.json has been exported. Use the same key combination to reload it");
                    return;
                }

                GameMain.mainPlayer.controller.movementStateInFrame = EMovementState.Sail;
                GameMain.mainPlayer.controller.actionSail.ResetSailState();
                GameCamera.instance.SyncForSailMode();

                var p = GameMain.localStar.planets[GameMain.localPlanet.index];
                var gsPlanet = GS3.GetGSPlanet(p);
                GS3.GetGSStar(p.star).counter = p.index;

                var newTheme = GS3.LoadJsonTheme(filename);
                GS3.Warn($"LOADED THEME {newTheme.Name} CustomGen:{newTheme.CustomGeneration} TA:{newTheme.TerrainSettings.Algorithm}");
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
                GS3.SetPlanetTheme(p, gsPlanet);
                GameMain.localStar.planets[p.index] = p;
                gsPlanet.planetData = p;
                PlanetModelingManager.RequestLoadPlanet(p);
                GameMain.data.LeavePlanet();
            }
        }
    }
}