using HarmonyLib;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;
using FullSerializer;
using System;
using System.IO;

namespace GalacticScale.Scripts.PatchPlanetSize
{
    [HarmonyPatch(typeof(GameMain))]
    public class PatchOnGameMain
    {
        static int delay = 0;
        [HarmonyPrefix]
        [HarmonyPatch("OnMainCameraPostRender")]
        static bool PatchOnMainCameraPostRender(Camera cam)
        {
            if (GameMain.data != null) GameMain.data.OnPostDraw();
            return false;
        }

        [HarmonyPostfix, HarmonyPatch("FixedUpdate")]
        static void DebugCommand()
        {
            if (VFInput.alt && Input.GetKeyDown(KeyCode.Y) && delay == 0)
            {
                PlanetData planet = GameMain.localPlanet;
                Vector3 position = GameMain.mainPlayer.position;
                Patch.Debug("DebugCommand", BepInEx.Logging.LogLevel.Message, true);
                Patch.Debug("---------------", BepInEx.Logging.LogLevel.Message, true);
                Patch.Debug("Planet Segments : " + ((int)(planet.radius / 4f + 0.1f) * 4), BepInEx.Logging.LogLevel.Message, true);
                Patch.Debug("Planet Segment Division: " + planet.segment, BepInEx.Logging.LogLevel.Message, true);
                Patch.Debug("Position: " + position, BepInEx.Logging.LogLevel.Message, true);
                Patch.Debug("Height: " + planet.data.QueryHeight(position), BepInEx.Logging.LogLevel.Message, true);
                Patch.Debug("ModifiedHeight: " + planet.data.QueryModifiedHeight(position), BepInEx.Logging.LogLevel.Message, true);
                Patch.Debug("Position Hash: " + planet.data.PositionHash(position), BepInEx.Logging.LogLevel.Message, true);
                Patch.Debug("Precision: " + planet.data.precision, BepInEx.Logging.LogLevel.Message, true);
                Patch.Debug("Algorithm: " + planet.algoId, BepInEx.Logging.LogLevel.Message, true);
                Patch.Debug("---------------", BepInEx.Logging.LogLevel.Message, true);
                delay = 5;
            }
            else if (VFInput.alt && Input.GetKeyDown(KeyCode.U) && delay == 0)
            {
                Dumper.json = StringSerializationAPI.Serialize(typeof(byte[]), GameMain.localPlanet.modData);
            }
            else if (VFInput.alt && Input.GetKeyDown(KeyCode.I) && delay == 0)
            {
                Dumper.Dump("modData");
                Patch.Debug("Dumped", BepInEx.Logging.LogLevel.Message, true);
            }
            else if (VFInput.control && Input.GetKeyDown(KeyCode.U) && delay == 0)
            {
                for (var i = 0; i < GameMain.localPlanet.dirtyFlags.Length; i++)
                {
                    GameMain.localPlanet.dirtyFlags[i] = true;
                }
                GameMain.localPlanet.UpdateDirtyMeshes();
            }
            else if (delay > 0) delay--;
        }
    }
    public static class Dumper
    {
        public static string json = "";
        public static void Dump(string prefix)
        {
            using (StreamWriter writer = File.CreateText(BepInEx.Paths.PluginPath + "\\dump"+prefix+".json"))
            {
                writer.Write(json);
            }
        }
    }
    public static class StringSerializationAPI
    {
        private static readonly fsSerializer _serializer = new fsSerializer();

        public static string Serialize(Type type, object value)
        {
            // serialize the data
            fsData data;
            _serializer.TrySerialize(type, value, out data).AssertSuccessWithoutWarnings();

            // emit the data via JSON
            return fsJsonPrinter.CompressedJson(data);
        }

        public static object Deserialize(Type type, string serializedState)
        {
            // step 1: parse the JSON data
            fsData data = fsJsonParser.Parse(serializedState);

            // step 2: deserialize the data
            object deserialized = null;
            _serializer.TryDeserialize(data, type, ref deserialized).AssertSuccessWithoutWarnings();

            return deserialized;
        }
    }
}