using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using NebulaAPI;

namespace GalacticScale
{
    [BepInPlugin("dsp.galactic-scale.2.nebula", "Galactic Scale 2 Nebula Compatibility Plug-In", "1.0.0.0")]
    [BepInDependency(NebulaModAPI.API_GUID)]
    public class Ignore_This_Warning_If_Not_Using_Nebula_Multiplayer_Mod : BaseUnityPlugin, IMultiplayerModWithSettings
    {
        public new static ManualLogSource Logger;

        private void Awake()
        {
            NebulaModAPI.RegisterPackets(Assembly.GetExecutingAssembly());
            var v = Assembly.GetExecutingAssembly().GetName().Version;
            BCE.Console.Init();
            var _ = new Harmony("dsp.galactic-scale.2.depcheck");
            Logger = new ManualLogSource("GS2DepCheck");
            BepInEx.Logging.Logger.Sources.Add(Logger);
            Logger.Log(LogLevel.Message, "Loaded");
            // Harmony.CreateAndPatchAll(typeof(NebulaCompatPatch));
        }

        public string Verson => Bootstrap.VERSION;
        public bool CheckVersion => true;

        public void Export(BinaryWriter w)
        {
            GS2.Export(w);
        }

        public void Import(BinaryReader r)
        {
            GS2.Import(r);
        }
    }

    public static class NebulaCompatPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(GameData), "SetForNewGame")]
        public static void NebulaCheck()
        {
            if (NebulaModAPI.GetSimulatedWorld().Initialized && !NebulaModAPI.GetLocalPlayer().IsMasterClient) GS2.NebulaClient = true;
            else GS2.NebulaClient = false;
        }
    }
}