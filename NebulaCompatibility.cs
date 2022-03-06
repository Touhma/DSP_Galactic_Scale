using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using GalacticScale;
using NebulaAPI;

namespace NebulaCompatibility
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
            if (NebulaModAPI.NebulaIsInstalled)
            {
                NebulaCompat.NebulaIsInstalled = true;
                NebulaModAPI.OnMultiplayerGameStarted += NebulaCompat.NebulaStart;
                NebulaModAPI.OnMultiplayerGameEnded += NebulaCompat.NebulaEnd;
            }
            Logger = new ManualLogSource("GS2DepCheck");
            BepInEx.Logging.Logger.Sources.Add(Logger);
            Logger.Log(LogLevel.Message, "Loaded");
        }

        public string Version => GS2.Version;

        bool IMultiplayerMod.CheckVersion(string hostVersion, string clientVersion)
        {
            if (GS2.ActiveGenerator.GUID == "space.customizing.generators.vanilla")
            {
                GS2.ShowMessage("Cannot Play Multiplayer using the Vanilla Generator", "Warning", "OK".Translate());
                GS2.ShowMessage(GS2.ActiveGenerator.GUID, "Warning", "OK".Translate());
                return false;
            }

            return hostVersion.Equals(clientVersion);
        }

        public void Export(BinaryWriter w)
        {
            var settings = GSSettings.Serialize();
            w.Write(settings);
        }

        public void Import(BinaryReader r)
        {
            var settings = r.ReadString();
            GSSettings.DeSerialize(settings);
            GS2.ActiveGenerator = GS2.GetGeneratorByID(GSSettings.Instance.generatorGUID);
        }
    }

    public static class NebulaCompat
    {
        public static bool NebulaIsInstalled = false;
        public static bool IsMultiplayerActive = false;
        public static bool IsClient = false;

        public static void NebulaStart()
        {
            IsMultiplayerActive = NebulaModAPI.IsMultiplayerActive;
            IsClient = NebulaModAPI.IsMultiplayerActive && NebulaModAPI.MultiplayerSession.LocalPlayer.IsClient;
            GS2.Log($"IsMultiplayerActive:{IsMultiplayerActive} IsClient:{IsClient}");
        }

        public static void NebulaEnd()
        {
            IsMultiplayerActive = false;
            IsClient = false;
        }

        public static void SendPacket(LobbyRequestUpdateSolarSystems packet)
        {
            NebulaModAPI.MultiplayerSession.Network.SendPacket(packet);
        }
    }
}
