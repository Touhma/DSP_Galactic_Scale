using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using GSSerializer;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = System.Random;

namespace GalacticScale
{
    public static class Utils
    {
        public static string Serialize(object value, bool pretty = true)
        {
            var serializer = new fsSerializer();
            serializer.TrySerialize(value, out var data);
            if (!pretty)
                // GS2.Warn(fsJsonPrinter.CompressedJson(data));
                return fsJsonPrinter.CompressedJson(data);

            return fsJsonPrinter.PrettyJson(data);
        }

        public static T[] ResourcesLoadArray<T>(string path, string format, bool emptyNull) where T : UnityEngine.Object
        {

            List<T> list = new List<T>();

            T t = Resources.Load<T>(path);
            if (t == null)
            {
                //GS2.Log("Resource returned null, exiting");
                return null;
            }
            //GS2.Log("Resource loaded");
            int num = 0;
            if (t != null)
            {
                list.Add(t);
                num = 1;
            }
            do
            {
                t = Resources.Load<T>(string.Format(format, path, num));
                if (t == null || ((num == 1 || num == 2) && list.Contains(t)))
                {
                    break;
                }
                list.Add(t);
                num++;
            }
            while (num < 1024);
            if (emptyNull && list.Count == 0)
            {
                return null;
            }
            return list.ToArray();
        }

        public static VectorLF3 PolarToCartesian(double p, double theta, double phi)
        {
            var x = p * Math.Sin(phi) * Math.Cos(theta);
            var y = p * Math.Sin(phi) * Math.Sin(theta);
            var z = p * Math.Cos(phi);
            return new VectorLF3(z, y, z);
        }

        public static Vector3 PositionAtSurface(Vector3 position, GSPlanet planet)
        {
            return position.normalized * planet.planetData.data.QueryHeight(position);
        }

        public static bool IsUnderWater(Vector3 position, GSPlanet planet)
        {
            if (planet.planetData.waterItemId == 0) return false;
            if (position.magnitude < planet.planetData.realRadius) return true;
            return false;
        }

        public static Vector3 RandomDirection(GS2.Random random)
        {
            //random = new GS2.Random(GSSettings.Seed);
            var randomVector = Vector3.zero;
            randomVector.x = (float)random.NextDouble() * 2f - 1f; //Tiny Vector3 made up of Random numbers between -0.5 and 0.5
            randomVector.y = (float)random.NextDouble() * 2f - 1f;
            randomVector.z = (float)random.NextDouble() * 2f - 1f;
            return randomVector;
        }

        public static bool ContainsLocalStarPlanet(IEnumerable<PlanetData> genPlanetReqList)
        {
            var containsLocalPlanet = false;
            var localPlanets = 0;
            var otherPlanets = 0;
            foreach (var p in genPlanetReqList)
                if (p.star == GameMain.localStar)
                {
                    localPlanets++;
                    containsLocalPlanet = true;
                    //GS2.Warn("Contains Local Planet");
                }
                else
                {
                    otherPlanets++;
                }

            //GS2.Warn($"Checking Planets while in System {GameMain.localStar.name}. {localPlanets} planets in queue are local, {otherPlanets} planets are from other stars.");
            return containsLocalPlanet;
        }

        public static bool PlanetInStar(PlanetData planet, StarData star)
        {
            //GS2.Warn($"Checking if {planet.name} is in star {star.name}");
            var planetIsLocal = false;
            foreach (var p in star.planets)
                if (p == planet)
                {
                    planetIsLocal = true;
                    break;
                }

            //GS2.Warn($"PlanetIsLocal:{planetIsLocal}");
            return planetIsLocal;
        }

        public static float CalculateOrbitPeriod(float orbitRadius, float speed = 0.0005f)
        {
            if (orbitRadius <= 0) return 100000f;
            var d = Mathf.PI * orbitRadius * 2;
            return d / speed;
        }

        public static float CalculateOrbitPeriodFromStarMass(float orbitRadius, float massStar)
        {
            var G = 6.67408 * Math.Pow(10, -11);
            var fourPIsquared = 39.4784176;
            var radiusCubed = Math.Pow(orbitRadius, 3);
            var psquared = radiusCubed * (fourPIsquared / (G / massStar));
            var periodFactor = Math.Sqrt(psquared) / 365 / 24 / 3600 * 40;
            return (float)(36000 * periodFactor);
        }

        public static FloatPair CalculateHabitableZone(float luminosity)
        {
            return new FloatPair((float)Math.Sqrt(luminosity / 1.1), (float)Math.Sqrt(luminosity / 0.53));
        }

        public static Type GetCallingType()
        {
            return new StackTrace().GetFrame(2).GetMethod().ReflectedType;
        }

        public static double DistanceVLF3(VectorLF3 a, VectorLF3 b)
        {
            return new VectorLF3(a.x - b.x, a.y - b.y, a.z - b.z).magnitude;
        }

        public static iConfigurableGenerator GetConfigurableGeneratorInstance(Type t)
        {
            if (GS2.Config.GetType() == t) return GS2.Config;
            foreach (var g in GS2.Generators)
                if (g.GetType() == t)
                {
                    if (g is iConfigurableGenerator)
                        return g as iConfigurableGenerator;
                    GS2.Warn($"Generator {t} is not configurable");
                }

            return null;
        }

        public static iConfigurablePlugin GetConfigurablePluginInstance(Type t)
        {
            foreach (var g in GS2.Plugins)
                if (g.GetType() == t)
                    if (g is iConfigurablePlugin)
                        return g;
            return null;
        }

        public static bool CheckStarCollision(List<VectorLF3> pts, VectorLF3 pt, double min_dist)
        {
            var num1 = min_dist * min_dist;
            foreach (var pt1 in pts)
            {
                var num2 = pt.x - pt1.x;
                var num3 = pt.y - pt1.y;
                var num4 = pt.z - pt1.z;
                if (num2 * num2 + num3 * num3 + num4 * num4 < num1) return true;
            }

            return false;
        }

        public static Sprite GetSpriteAsset(string name)
        {
            return GS2.Bundle.LoadAsset<Sprite>(name);
        }

        public static Sprite GetSplashSprite()
        {
            var r = new Random();
            var i = r.Next(15);
            var spriteName = "s14";
            if (i > 0) spriteName = "s" + i;
            // switch (i)
            // {
            //     case 1: spriteName = "s1"; break;
            //     case 2: spriteName = "s2"; break;
            //     case 3: spriteName = "s3"; break;
            //     case 4: spriteName = "s4"; break;
            //     case 4: spriteName = "s4"; break;
            //     
            // }
            return GS2.Bundle.LoadAsset<Sprite>(spriteName);
        }

        public static Cubemap TintCubeMap(Cubemap input, Color color)
        {
            // return input; //Kills performance too much to use!
            //GS2.Log("Tinting Cubemap");
            var highStopwatch = new HighStopwatch();
            highStopwatch.Begin();

            var output = Object.Instantiate(input);

            var colors = output.GetPixels(CubemapFace.PositiveX);
            var tinted = new Color[colors.Length];
            for (var i = 0; i < colors.Length; i++)
                tinted[i] = Color.Lerp(new Color(colors[i].grayscale, colors[i].grayscale, colors[i].grayscale), new Color(color.r, color.g, color.b), color.a);

            output.SetPixels(tinted, CubemapFace.PositiveX);

            colors = output.GetPixels(CubemapFace.PositiveY);
            tinted = new Color[colors.Length];
            for (var i = 0; i < colors.Length; i++)
                tinted[i] = Color.Lerp(new Color(colors[i].grayscale, colors[i].grayscale, colors[i].grayscale), new Color(color.r, color.g, color.b), color.a);

            output.SetPixels(tinted, CubemapFace.PositiveY);

            colors = output.GetPixels(CubemapFace.PositiveZ);
            tinted = new Color[colors.Length];
            for (var i = 0; i < colors.Length; i++)
                tinted[i] = Color.Lerp(new Color(colors[i].grayscale, colors[i].grayscale, colors[i].grayscale), new Color(color.r, color.g, color.b), color.a);

            output.SetPixels(tinted, CubemapFace.PositiveZ);

            colors = output.GetPixels(CubemapFace.NegativeX);
            tinted = new Color[colors.Length];
            for (var i = 0; i < colors.Length; i++)
                tinted[i] = Color.Lerp(new Color(colors[i].grayscale, colors[i].grayscale, colors[i].grayscale), new Color(color.r, color.g, color.b), color.a);

            output.SetPixels(tinted, CubemapFace.NegativeX);

            colors = output.GetPixels(CubemapFace.NegativeY);
            tinted = new Color[colors.Length];
            for (var i = 0; i < colors.Length; i++)
                tinted[i] = Color.Lerp(new Color(colors[i].grayscale, colors[i].grayscale, colors[i].grayscale), new Color(color.r, color.g, color.b), color.a);

            output.SetPixels(tinted, CubemapFace.NegativeY);

            colors = output.GetPixels(CubemapFace.NegativeZ);
            tinted = new Color[colors.Length];
            for (var i = 0; i < colors.Length; i++)
                tinted[i] = Color.Lerp(new Color(colors[i].grayscale, colors[i].grayscale, colors[i].grayscale), new Color(color.r, color.g, color.b), color.a);

            output.SetPixels(tinted, CubemapFace.NegativeZ);

            //GS2.Log("End");
            // GS2.Log($"TintCubeMap Took {highStopwatch.duration:F5}s");

            return output;
        }

        public static Texture GetTextureFromBundle(string name)
        {
            var bundle = GS2.Bundle;
            return bundle.LoadAsset<Texture>(name);
        }

        public static Texture GetTextureFromFile(string path)
        {
            GS2.Log("Loading texture from file : " + path);
            var data = File.ReadAllBytes(path);
            if (data == null)
            {
                GS2.Warn("Bytes = Null");
                return null;
            }

            var t = new Texture2D(2048, 2048);
            t.filterMode = FilterMode.Point;
            t.LoadImage(data);

            return t;
        }

        public static Texture GetTextureFromResource(string path)
        {
            return null;
        }

        public static Texture GetTextureFromExternalBundle(string path)
        {
            return null;
        }

        public static bool ArrayCompare<T>(T[] a1, T[] a2)
        {
            return a1.SequenceEqual(a2);
        }

        public static T ReverseLookup<T, W>(Dictionary<T, W> dict, W val)
        {
            foreach (var kvp in dict)
                if (kvp.Value.ToString() == val.ToString())
                    return kvp.Key;

            return default;
        }

        public static float diff(float a, float b)
        {
            return !(a > b) ? b - a : a - b;
        }

        public static int ParsePlanetSize(float radius)
        {
            if (radius < 8f) return 5;

            radius = Mathf.Clamp(radius, 10, 510) / 10;
            radius = Mathf.RoundToInt(radius) * 10;
            //GS2.Warn(radius.ToString());
            return (int)radius;
        }

        public static string GetStarDetail(StarData star)
        {
            var gsStar = GS2.GetGSStar(star);
            var output = "";

            foreach (var planet in gsStar.Planets) output += "\r\n" + GetGSPlanetDetail(planet);

            var sa = output.Split(new[] { "\r\n" }, StringSplitOptions.None);
            // GS2.WarnJson(sa);
            // GS2.Warn(sa.Length.ToString());
            if (sa.Length > 50)
            {
                output = "";

                output = "Luminosity: " + Math.Round(Math.Pow(star.luminosity, 0.33), 2);
                if (gsStar.BinaryCompanion != null)
                {
                    var binary = GS2.GetGSStar(gsStar.BinaryCompanion);
                    if (binary != null) output += $"Binary Star ({binary.Type})\r\n";
                }

                var sa1 = new string[50];
                var sa2 = new string[sa.Length - 50];
                for (var i = 0; i < 50; i++)
                {
                    var j = i + 50;
                    // GS2.Warn(i + " " + (j) + " "+sa.Length);
                    if (sa.Length > j)
                    {
                        // GS2.Log(i.ToString() + " " + j.ToString());
                        var a = string.Format("{0,30}", sa[i]);
                        var b = string.Format("{0,-30}", sa[j]);
                        output += $"\r\n{a}  {b}";
                    }
                    else
                    {
                        // GS2.Warn(i.ToString());
                        output += $"\r\n{string.Format("{0,30}", sa[i])}";
                    }
                }

                // GS2.Log(output);
                return output;
            }

            var output2 = "";
            if (gsStar.BinaryCompanion != null)
            {
                var binary = GS2.GetGSStar(gsStar.BinaryCompanion);
                output2 += $"Binary Star:{binary.displayType}\r\n";
            }

            output2 += "Luminosity: " + Math.Round(Math.Pow(star.luminosity, 0.33), 2);
            output2 += output;


            return output2;
        }

        public static string GetGSPlanetDetail(GSPlanet planet, int indentation = 1)
        {
            var ind = "";
            for (var i = 0; i <= indentation; i++) ind += "  ";

            var output = "";
            output += $"{ind}-{planet.Theme} ({planet.Radius * planet.Scale})";
            foreach (var moon in planet.Moons) output += $"\r\n{ind}{GetGSPlanetDetail(moon, indentation + 1)}";

            return output;
        }


        public static List<VectorLF3> RegularPointsOnSphere(float radius, int count)
        {
            var points = new List<VectorLF3>();
            if (count == 0) return points;

            var a = 4.0 * Math.PI * (Math.Pow(radius, 2) / count);
            var d = Math.Sqrt(a);
            var m_theta = (int)Math.Round(Math.PI / d);
            var d_theta = Math.PI / m_theta;
            var d_phi = a / d_theta;
            for (var m = 0; m < m_theta; m++)
            {
                var theta = Math.PI * (m + 0.5) / m_theta;
                var m_phi = (int)Math.Round(2.0 * Math.PI * Math.Sin(theta) / d_phi);
                for (var n = 0; n < m_phi; n++)
                {
                    var phi = 2.0 * Math.PI * n / m_phi;
                    var x = radius * Math.Sin(theta) * Math.Cos(phi);
                    var y = radius * Math.Sin(theta) * Math.Sin(phi);
                    var z = radius * Math.Cos(theta);
                    points.Add(new VectorLF3(x, y, z));
                }
            }

            return points;
        }

        public static int GetStarDataGasCount(StarData sd)
        {
            if (sd == null) return -1;
            var count = 0;
            foreach (var planet in sd.planets)
                if (planet.type == EPlanetType.Gas)
                    count++;

            return count;
        }

        public static int GetStarDataTelluricCount(StarData sd)
        {
            if (sd == null) return -1;
            var count = 0;
            foreach (var planet in sd.planets)
                if (planet.type != EPlanetType.Gas && planet.orbitAroundPlanet == null)
                    count++;

            return count;
        }

        public static int GetStarDataMoonCount(StarData sd)
        {
            if (sd == null) return -1;
            var count = 0;
            foreach (var planet in sd.planets)
                if (planet.orbitAroundPlanet != null)
                    count++;
            return count;
        }

        public static void DumpProtosToCSharp()
        {
            foreach (var t in LDB._themes.dataArray)
            {
                var themeName = string.Concat(t.displayName.Split(' '));
                GS2.Warn($"Dumping {t.name}");
                var lines = new List<string>();
                lines.Add("using System.Collections.Generic;");
                lines.Add("using UnityEngine;");
                lines.Add(" ");
                lines.Add("namespace GalacticScale");
                lines.Add("{");
                lines.Add("  public static partial class Themes");
                lines.Add("  {");
                lines.Add($"      public static GSTheme {themeName} = new GSTheme");
                lines.Add("      {");
                lines.Add($"         Name = \"{t.displayName}\",");
                lines.Add("         Base = true,");
                lines.Add($"         DisplayName = \"{t.displayName}\".Translate(),");
                lines.Add($"         PlanetType = EPlanetType.{t.PlanetType},");
                lines.Add($"         ThemeType = EThemeType.{(t.PlanetType == EPlanetType.Gas ? "Gas" : "Telluric")},");
                lines.Add(" ");
                lines.Add($"         LDBThemeId = {t.ID},");
                lines.Add($"         Algo = {t.Algos[0]},");
                lines.Add("         MinRadius = 5,");
                lines.Add("         MaxRadius = 510,");
                lines.Add($"         MaterialPath = \"{t.MaterialPath}\",");
                lines.Add($"         Temperature = {t.Temperature}f,");
                lines.Add($"         Distribute = EThemeDistribute.{t.Distribute},");
                lines.Add($"         Habitable = {(t.PlanetType == EPlanetType.Ocean ? "true" : "false")},");
                lines.Add($"         ModX = new Vector2({t.ModX.x}f, {t.ModX.y}f),");
                lines.Add($"         ModY = new Vector2({t.ModY.x}f, {t.ModY.y}f),");
                lines.Add("         CustomGeneration = false,");
                lines.Add("          TerrainSettings = new GSTerrainSettings");
                lines.Add("         {");
                lines.Add("             Algorithm = \"Vanilla\"");
                lines.Add("         },");
                lines.Add("         VeinSettings = new GSVeinSettings");
                lines.Add("         {");
                lines.Add("              Algorithm = \"Vanilla\",");
                lines.Add("             VeinTypes = new GSVeinTypes()");
                lines.Add("  },");
                int ambientCount = 1;
                if (t.ambientDesc != null)
                    foreach (var a in t.ambientDesc)
                    {
                        lines.Add($"//AmbientSettings {ambientCount}");
                        if (ambientCount > 1) lines.Add("Please duplicate theme for this variant, and delete the others.");
                        lines.Add("AmbientSettings = new GSAmbientSettings");
                        lines.Add("{");
                        lines.Add($"    Color1 = new Color({a.ambientColor0.r}f, {a.ambientColor0.g}f, {a.ambientColor0.b}f, {a.ambientColor0.a}f),");
                        lines.Add($"    Color2 = new Color({a.ambientColor1.r}f, {a.ambientColor1.g}f, {a.ambientColor1.b}f, {a.ambientColor1.a}f),");
                        lines.Add($"    Color3 = new Color({a.ambientColor2.r}f, {a.ambientColor2.g}f, {a.ambientColor2.b}f, {a.ambientColor2.a}f),");
                        lines.Add($"    WaterColor1 = new Color({a.waterAmbientColor0.r}f, {a.waterAmbientColor0.g}f, {a.waterAmbientColor0.b}f, {a.waterAmbientColor0.a}f),");
                        lines.Add($"    WaterColor2 = new Color({a.waterAmbientColor1.r}f, {a.waterAmbientColor1.g}f, {a.waterAmbientColor1.b}f, {a.waterAmbientColor1.a}f),");
                        lines.Add($"    WaterColor3 = new Color({a.waterAmbientColor2.r}f, {a.waterAmbientColor2.g}f, {a.waterAmbientColor2.b}f, {a.waterAmbientColor2.a}f),");
                        lines.Add($"    BiomeColor1 = new Color({a.biomoColor0.r}f, {a.biomoColor0.g}f, {a.biomoColor0.b}f, {a.biomoColor0.a}f),");
                        lines.Add($"    BiomeColor2 = new Color({a.biomoColor1.r}f, {a.biomoColor1.g}f, {a.biomoColor1.b}f, {a.biomoColor1.a}f),");
                        lines.Add($"    BiomeColor3 = new Color({a.biomoColor2.r}f, {a.biomoColor2.g}f, {a.biomoColor2.b}f, {a.biomoColor2.a}f),");
                        lines.Add($"    DustColor1 = new Color({a.biomoDustColor0.r}f, {a.biomoDustColor0.g}f, {a.biomoDustColor0.b}f, {a.biomoDustColor0.a}f),");
                        lines.Add($"    DustColor2 = new Color({a.biomoDustColor1.r}f, {a.biomoDustColor1.g}f, {a.biomoDustColor1.b}f, {a.biomoDustColor1.a}f),");
                        lines.Add($"    DustColor3 = new Color({a.biomoDustColor2.r}f, {a.biomoDustColor2.g}f, {a.biomoDustColor2.b}f, {a.biomoDustColor2.a}f),");
                        lines.Add($"    DustStrength1 = {a.biomoDustStrength0}f,");
                        lines.Add($"    DustStrength2 = {a.biomoDustStrength1}f,");
                        lines.Add($"    DustStrength3 = {a.biomoDustStrength2}f,");
                        lines.Add($"    BiomeSound1 = {a.biomoSound0},");
                        lines.Add($"    BiomeSound2 = {a.biomoSound1},");
                        lines.Add($"    BiomeSound3 = {a.biomoSound2},");
                        lines.Add("    CubeMap = \"Vanilla\",");
                        lines.Add("    Reflections = new Color(),");
                        lines.Add($"    LutContribution = {a.lutContribution}f,");
                        lines.Add("},");
                        ambientCount++;
                    }

                lines.Add("Vegetables0 = new int[]");
                lines.Add("{");
                foreach (var v0 in t.Vegetables0) lines.Add($"{v0},");
                lines.Add("},");
                lines.Add("Vegetables1 = new int[]");
                lines.Add("{");
                foreach (var v1 in t.Vegetables1) lines.Add($"{v1},");
                lines.Add("},");
                lines.Add("Vegetables2 = new int[]");
                lines.Add("{");
                foreach (var v2 in t.Vegetables2) lines.Add($"{v2},");
                lines.Add("},");
                lines.Add("Vegetables3 = new int[]");
                lines.Add("{");
                foreach (var v3 in t.Vegetables3) lines.Add($"{v3},");
                lines.Add("},");
                lines.Add("Vegetables4 = new int[]");
                lines.Add("{");
                foreach (var v4 in t.Vegetables4) lines.Add($"{v4},");
                lines.Add("},");
                lines.Add("Vegetables5 = new int[]");
                lines.Add("{");
                foreach (var v5 in t.Vegetables5) lines.Add($"{v5},");
                lines.Add("},");
                lines.Add("VeinSpot = new int[]");
                lines.Add("{");
                foreach (var x in t.VeinSpot) lines.Add($"{x},");
                lines.Add("},");
                lines.Add("VeinCount = new float[]");
                lines.Add("{");
                foreach (var x in t.VeinCount) lines.Add($"{x}f,");
                lines.Add("},");
                lines.Add("VeinOpacity = new float[]");

                lines.Add("{");
                foreach (var x in t.VeinOpacity) lines.Add($"{x}f,");
                lines.Add("},");

                lines.Add("RareVeins = new int[]");

                lines.Add("{");
                foreach (var x in t.RareVeins) lines.Add($"{x},");
                lines.Add("},");

                lines.Add("RareSettings = new float[]");
                lines.Add("{");
                foreach (var x in t.RareSettings) lines.Add($"{x}f,");
                lines.Add("},");
                lines.Add("GasItems = new int[]");
                lines.Add("{");
                foreach (var x in t.GasItems) lines.Add($"{x},");
                lines.Add("},");
                lines.Add("GasSpeeds = new float[]");
                lines.Add("{");
                foreach (var x in t.GasSpeeds) lines.Add($"{x}f,");
                lines.Add("},");
                lines.Add($"UseHeightForBuild = {(t.UseHeightForBuild?"true":"false")},");
                lines.Add($"Wind = {t.Wind}f,");
                lines.Add($"IonHeight = {t.IonHeight}f,");
                lines.Add($"WaterHeight = {t.WaterHeight}f,");
                lines.Add($"WaterItemId = {t.WaterItemId},");
                lines.Add("Musics = new int[]");
                lines.Add("{");
                foreach (var x in t.Musics) lines.Add($"{x},");
                lines.Add("},");
                lines.Add($"SFXPath = \"{t.SFXPath}\",");
                lines.Add($"SFXVolume = {t.SFXVolume}f,");
                lines.Add($"CullingRadius = {t.CullingRadius}f,");
                lines.Add("terrainMaterial = new GSMaterialSettings");
                lines.Add("{");
                lines.Add("    Colors = new Dictionary<string, Color>{},");
                lines.Add("    Params = new Dictionary<string, float>{}");
                lines.Add("},");
                lines.Add("oceanMaterial = new GSMaterialSettings");
                lines.Add("{");
                lines.Add("    Colors = new Dictionary<string, Color>{},");
                lines.Add("    Params = new Dictionary<string, float>{}");
                lines.Add("},");
                lines.Add("atmosphereMaterial = new GSMaterialSettings");
                lines.Add("{");
                lines.Add("    Colors = new Dictionary<string, Color>{},");
                lines.Add("    Params = new Dictionary<string, float>{}");
                lines.Add("},");
                lines.Add("minimapMaterial = new GSMaterialSettings");
                lines.Add("{");
                lines.Add("    Colors = new Dictionary<string, Color>{},");
                lines.Add("    Params = new Dictionary<string, float>{}");
                lines.Add("},");
                lines.Add("thumbMaterial = new GSMaterialSettings");
                lines.Add("{");
                lines.Add("    Colors = new Dictionary<string, Color>{},");
                lines.Add("    Params = new Dictionary<string, float>{}");
                lines.Add("},");
                lines.Add("        };");
                lines.Add("    }");
                lines.Add("}");
                File.WriteAllLines(Path.Combine(GS2.DataDir, $"{themeName}.cs"), lines);
            }
        }

        public static class AddressHelper
        {
            private static readonly object mutualObject;
            private static readonly ObjectReinterpreter reinterpreter;

            static AddressHelper()
            {
                mutualObject = new object();
                reinterpreter = new ObjectReinterpreter();
                reinterpreter.AsObject = new ObjectWrapper();
            }

            public static IntPtr GetAddress(object obj)
            {
                lock (mutualObject)
                {
                    reinterpreter.AsObject.Object = obj;
                    var address = reinterpreter.AsIntPtr.Value;
                    reinterpreter.AsObject.Object = null;
                    return address;
                }
            }

            public static T GetInstance<T>(IntPtr address)
            {
                lock (mutualObject)
                {
                    reinterpreter.AsIntPtr.Value = address;
                    return (T)reinterpreter.AsObject.Object;
                }
            }

            // I bet you thought C# was type-safe.
            [StructLayout(LayoutKind.Explicit)]
            private struct ObjectReinterpreter
            {
                [FieldOffset(0)] public ObjectWrapper AsObject;
                [FieldOffset(0)] public readonly IntPtrWrapper AsIntPtr;
            }

            private class ObjectWrapper
            {
                public object Object;
            }

            private class IntPtrWrapper
            {
                public IntPtr Value;
            }
        }
    }
}