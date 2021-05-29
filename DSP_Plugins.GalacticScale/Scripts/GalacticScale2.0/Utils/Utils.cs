using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GalacticScale
{
    public static class Utils
    {

        public static UnityEngine.Cubemap TintCubeMap(UnityEngine.Cubemap input, UnityEngine.Color color)
        {
            GS2.Log("Tinting Cubemap");
            UnityEngine.Cubemap output = UnityEngine.Object.Instantiate(input);

            var colors = output.GetPixels(UnityEngine.CubemapFace.PositiveX);
            var tinted = new UnityEngine.Color[colors.Length];
            for (var i = 0; i < colors.Length; i++) tinted[i] = UnityEngine.Color.Lerp(new UnityEngine.Color(colors[i].grayscale, colors[i].grayscale, colors[i].grayscale), new UnityEngine.Color(color.r, color.g, color.b), color.a);
            output.SetPixels(tinted, UnityEngine.CubemapFace.PositiveX);

            colors = output.GetPixels(UnityEngine.CubemapFace.PositiveY);
            tinted = new UnityEngine.Color[colors.Length];
            for (var i = 0; i < colors.Length; i++) tinted[i] = UnityEngine.Color.Lerp(new UnityEngine.Color(colors[i].grayscale, colors[i].grayscale, colors[i].grayscale), new UnityEngine.Color(color.r, color.g, color.b), color.a);
            output.SetPixels(tinted, UnityEngine.CubemapFace.PositiveY);

            colors = output.GetPixels(UnityEngine.CubemapFace.PositiveZ);
            tinted = new UnityEngine.Color[colors.Length];
            for (var i = 0; i < colors.Length; i++) tinted[i] = UnityEngine.Color.Lerp(new UnityEngine.Color(colors[i].grayscale, colors[i].grayscale, colors[i].grayscale), new UnityEngine.Color(color.r, color.g, color.b), color.a);
            output.SetPixels(tinted, UnityEngine.CubemapFace.PositiveZ);

            colors = output.GetPixels(UnityEngine.CubemapFace.NegativeX);
            tinted = new UnityEngine.Color[colors.Length];
            for (var i = 0; i < colors.Length; i++) tinted[i] = UnityEngine.Color.Lerp(new UnityEngine.Color(colors[i].grayscale, colors[i].grayscale, colors[i].grayscale), new UnityEngine.Color(color.r, color.g, color.b), color.a);
            output.SetPixels(tinted, UnityEngine.CubemapFace.NegativeX);

            colors = output.GetPixels(UnityEngine.CubemapFace.NegativeY);
            tinted = new UnityEngine.Color[colors.Length];
            for (var i = 0; i < colors.Length; i++) tinted[i] = UnityEngine.Color.Lerp(new UnityEngine.Color(colors[i].grayscale, colors[i].grayscale, colors[i].grayscale), new UnityEngine.Color(color.r, color.g, color.b), color.a);
            output.SetPixels(tinted, UnityEngine.CubemapFace.NegativeY);

            colors = output.GetPixels(UnityEngine.CubemapFace.NegativeZ);
            tinted = new UnityEngine.Color[colors.Length];
            for (var i = 0; i < colors.Length; i++) tinted[i] = UnityEngine.Color.Lerp(new UnityEngine.Color(colors[i].grayscale, colors[i].grayscale, colors[i].grayscale), new UnityEngine.Color(color.r, color.g, color.b), color.a);
            output.SetPixels(tinted, UnityEngine.CubemapFace.NegativeZ);

            GS2.Log("End");
            return output;
        }
        public static Texture GetTextureFromBundle(string name)
        {
            AssetBundle bundle = GS2.bundle;
            return bundle.LoadAsset<Texture>(name);
        }
        public static Texture GetTextureFromFile(string path)
        {
            GS2.Log("Loading texture from file : " + path);
            var data = System.IO.File.ReadAllBytes(path);
            if (data == null)
            {
                GS2.Warn("Bytes = Null");
                return null;
            }
            Texture2D t = new Texture2D(2048,2048);
            t.LoadImage(data);
            return t;
            
        }
        public static Texture GetTextureFromResource(string path) { return null; }
        public static Texture GetTextureFromExternalBundle(string path) { return null; }

        public static bool ArrayCompare<T>(T[] a1, T[] a2)
        {
            return a1.SequenceEqual(a2);
        }
        public static T ReverseLookup<T, W>(Dictionary<T, W> dict, W val)
        {
            foreach (KeyValuePair<T, W> kvp in dict) if (kvp.Value.ToString() == val.ToString()) return kvp.Key;
            return default(T);
        }
        public static float diff(float a, float b)
        {
            return (!(a > b)) ? (b - a) : (a - b);
        }
        public static float ParsePlanetSize(float radius)
        {
            if (radius < 8f) return 5f;
            return UnityEngine.Mathf.RoundToInt(UnityEngine.Mathf.Clamp(radius, 10, 510) / 10) * 10;
        }
        public static List<VectorLF3> RegularPointsOnSphere(float radius, int count)
        {
            List<VectorLF3> points = new List<VectorLF3>();
            if (count == 0) return points;
            double a = 4.0 * Math.PI * (Math.Pow(radius, 2) / count);
            double d = Math.Sqrt(a);
            int m_theta = (int)Math.Round(Math.PI / d);
            double d_theta = Math.PI / m_theta;
            double d_phi = a / d_theta;
            for (int m = 0; m < m_theta; m++)
            {
                double theta = Math.PI * (m + 0.5) / m_theta;
                int m_phi = (int)Math.Round(2.0 * Math.PI * Math.Sin(theta) / d_phi);
                for (int n = 0; n < m_phi; n++)
                {
                    double phi = 2.0 * Math.PI * n / m_phi;
                    double x = radius * Math.Sin(theta) * Math.Cos(phi);
                    double y = radius * Math.Sin(theta) * Math.Sin(phi);
                    double z = radius * Math.Cos(theta);
                    points.Add(new VectorLF3(x, y, z));
                }
            }
            return points;
        }
    }

}