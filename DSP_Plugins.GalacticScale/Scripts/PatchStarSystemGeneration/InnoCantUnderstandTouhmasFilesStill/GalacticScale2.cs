
using BepInEx;
using FullSerializer;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static List<VectorLF3> tmp_poses;
        public static List<VectorLF3> tmp_drunk;
        public static int[] tmp_state;
        public static GalaxyData galaxy;
        public static System.Random random;
        public static GameDesc gameDesc;
        public static void CreateDummySettings(int starCount)
        {
            settings.Stars.Clear();
            List<planet> p = new List<planet>
                {
                    new planet("Urf")
                };
            Patch.Debug("Setting BirthStar");
            settings.Stars.Add(new star(1,"BeetleJuice", ESpectrType.O, EStarType.MainSeqStar, p));
            Patch.Debug("Creating Dummy Stars");
            for (var i = 1; i < starCount; i++)
            {
                Patch.Debug("Adding new Star = " + i);
                settings.Stars.Add(new star(1,"Star" + i.ToString(), ESpectrType.X, EStarType.BlackHole, new List<planet>()));
            }
            Patch.Debug("Creating Dummy Params");
            galaxyParams g = new galaxyParams();
            g.iterations = 4;
            g.flatten = 0.18;
            g.minDistance = 2;
            g.minStepLength = 2.3; 
            g.maxStepLength = 3.5;
            settings.GalaxyParams = g;
        }
        public static bool LoadSettingsFromJson(string path)
        {
            if (!CheckJsonFileExists(path)) return false;
            Patch.Debug("Loading Settings from " + path);
            fsSerializer serializer = new fsSerializer(); 
            settings.Stars.Clear();
            string json = File.ReadAllText(path);
            settings result = settings.Instance;
            fsData data2 = fsJsonParser.Parse(json);
            serializer.TryDeserialize<settings>(data2, ref result).AssertSuccessWithoutWarnings();
            settings.Instance = result;
            return true;

        }
        private static bool CheckJsonFileExists(string path)
        {
            Patch.Debug("Checking if Json File Exists");
            if (File.Exists(path)) return true;
            Debug.Log("Json file does not exist at " + path);
            return false;
        }
        public static void SaveSettingsToJson(string path)
        {
            Patch.Debug("Saving Settings to " + path);
            fsSerializer serializer = new fsSerializer();

            serializer.TrySerialize<settings>(settings.Instance, out fsData data).AssertSuccessWithoutWarnings();
            string json = fsJsonPrinter.PrettyJson(data);
            //string json = settings.Serialize();
            File.WriteAllText(path, json);

        }
    }
}

