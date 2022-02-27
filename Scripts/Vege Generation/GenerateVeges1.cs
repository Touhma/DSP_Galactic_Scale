using System;
using UnityEngine;

namespace GalacticScale
{
    public static class VegeAlgorithms
    {
        private static GS2.Random random = new(GSSettings.Seed);

        public static void GenerateVeges1(GSPlanet gsPlanet)
        {
            //GS2.Log("GenerateVeges1|" + gsPlanet.Name);
            //GS2.Warn($"USING GS2 Vege FOR {gsPlanet.Name} with seed {GSSettings.Seed}");
            var planet = gsPlanet.planetData;
            var themeProto = LDB.themes.Select(planet.theme);
            if (themeProto == null) return;
            random = new GS2.Random(GSSettings.Seed);
            //TODO convert veges in GSTheme
            //GS2.Log("GenerateVeges1|" + gsPlanet.Name+"1");
            var vegetables = themeProto.Vegetables0;
            var vegetables2 = themeProto.Vegetables1;
            var vegetables3 = themeProto.Vegetables2;
            var vegetables4 = themeProto.Vegetables3;
            var vegetables5 = themeProto.Vegetables4;
            var vegetables6 = themeProto.Vegetables5;
            //GS2.Log("GenerateVeges1|" + gsPlanet.Name + "2");
            var num = 1.3f;
            var num2 = -0.5f;
            var num3 = 2.5f;
            var num4 = 4f;
            var num5 = 0.5f;
            var num6 = 1f;
            var num7 = 2f;
            var num8 = -0.2f;
            var num9 = 1.4f;
            //System.Random random = new System.Random(planet.seed);
            random.Next();
            random.Next();
            random.Next();
            var num10 = random.Next();
            //System.Random random2 = new System.Random(num10);
            var simplexNoise = new SimplexNoise(random.Next());
            var simplexNoise2 = new SimplexNoise(random.Next());
            var data = planet.data;
            var stride = data.stride;
            var num11 = stride / 2;
            var num12 = planet.radius * 3.14159f * 2f / (data.precision * 4f);
            //GS2.Log("GenerateVeges1|" + gsPlanet.Name + "3");
            var vege = default(VegeData);
            var vegeProtos = PlanetModelingManager.vegeProtos;
            var vegeScaleRanges = PlanetModelingManager.vegeScaleRanges;
            var vegeHps = PlanetModelingManager.vegeHps;
            //GS2.Log("GenerateVeges1|" + gsPlanet.Name + "4");
            for (var i = 0; i < data.dataLength; i++)
            {
                var num13 = i % stride;
                var num14 = i / stride;
                if (num13 > num11) num13--;
                if (num14 > num11) num14--;
                if (num13 % 2 != 1 || num14 % 2 != 1) continue;
                var vector = data.vertices[i];
                double num15 = data.vertices[i].x * planet.radius;
                double num16 = data.vertices[i].y * planet.radius;
                double num17 = data.vertices[i].z * planet.radius;
                var num18 = data.heightData[i] * 0.01f;
                var num19 = data.heightData[i + 1 + stride] * 0.01f;
                var num20 = data.heightData[i - 1 + stride] * 0.01f;
                var num21 = data.heightData[i + 1 - stride] * 0.01f;
                var num22 = data.heightData[i - 1 - stride] * 0.01f;
                var num23 = data.heightData[i + 1] * 0.01f;
                var num24 = data.heightData[i - 1] * 0.01f;
                var num25 = data.heightData[i + stride] * 0.01f;
                var num26 = data.heightData[i - stride] * 0.01f;
                var num27 = data.biomoData[i] * 0.01f;
                var num28 = planet.radius + 0.15f;
                var flag = false;
                if (num18 < num28)
                    flag = true;
                else if (num19 < num28)
                    flag = true;
                else if (num20 < num28)
                    flag = true;
                else if (num21 < num28)
                    flag = true;
                else if (num22 < num28)
                    flag = true;
                else if (num23 < num28)
                    flag = true;
                else if (num24 < num28)
                    flag = true;
                else if (num25 < num28)
                    flag = true;
                else if (num26 < num28) flag = true;
                if (flag && (vegetables6 == null || vegetables6.Length == 0)) continue;
                var flag2 = true;
                if (Utils.diff(num18, num19) > 0.2f) flag2 = false;
                if (Utils.diff(num18, num20) > 0.2f) flag2 = false;
                if (Utils.diff(num18, num21) > 0.2f) flag2 = false;
                if (Utils.diff(num18, num22) > 0.2f) flag2 = false;
                var num29 = random.NextDouble();
                num29 *= num29;
                var num30 = random.NextDouble();
                var num31 = (float)random.NextDouble() - 0.5f;
                var num32 = (float)random.NextDouble() - 0.5f;
                var num33 = (float)Math.Sqrt(random.NextDouble());
                var angle = (float)random.NextDouble() * 360f;
                var num34 = (float)random.NextDouble();
                var num35 = (float)random.NextDouble();
                var num36 = 1f;
                var num37 = 0.5f;
                var num38 = 1f;
                int[] array;
                if (!flag)
                {
                    if (num27 < 0.8f)
                    {
                        array = vegetables;
                        num36 = num;
                        num37 = num2;
                        num38 = num3;
                    }
                    else
                    {
                        array = vegetables2;
                        num36 = num4;
                        num37 = num5;
                        num38 = num6;
                    }
                }
                else
                {
                    array = null;
                }

                var num39 = simplexNoise.Noise(num15 * 0.07, num16 * 0.07, num17 * 0.07) * num36 + num37 + 0.5;
                var num40 = simplexNoise2.Noise(num15 * 0.4, num16 * 0.4, num17 * 0.4) * num7 + num8 + 0.5;
                var num41 = num40 - 0.55;
                var num42 = num40 - 1.1;
                int[] array2;
                double num43;
                int num44;
                if (!flag)
                {
                    if (num27 > 1f)
                    {
                        array2 = vegetables3;
                        num43 = num40;
                        num44 = vegetables6 != null && vegetables6.Length != 0 ? 2 : 4;
                    }
                    else if (num27 > 0.5f)
                    {
                        array2 = vegetables4;
                        num43 = num41;
                        num44 = 1;
                    }
                    else if (num27 > 0f)
                    {
                        array2 = vegetables5;
                        num43 = num41;
                        num44 = 1;
                    }
                    else
                    {
                        array2 = null;
                        num43 = num40;
                        num44 = 1;
                    }
                }
                else
                {
                    if (!(num18 < num28 - 1f) || !(num18 > num28 - 2.2f)) continue;
                    array2 = vegetables6;
                    num43 = num42;
                    num44 = 1;
                }

                if (flag2 && num30 < num39 && array != null && array.Length > 0)
                {
                    vege.protoId = (short)array[(int)(num29 * array.Length)];
                    var quaternion = Quaternion.FromToRotation(Vector3.up, vector);
                    var vector2 = quaternion * Vector3.forward;
                    var vector3 = quaternion * Vector3.right;
                    var vector4 = vegeScaleRanges[vege.protoId];
                    var vector5 = vector * num18;
                    var normalized = (vector3 * num31 + vector2 * num32).normalized;
                    var num45 = num33 * num38;
                    var vector6 = normalized * (num45 * num12);
                    var num46 = num35 * (vector4.x + vector4.y) + (1f - vector4.x);
                    var num47 = (num34 * (vector4.z + vector4.w) + (1f - vector4.z)) * num46;
                    vege.pos = (vector5 + vector6).normalized;
                    num18 = data.QueryHeight(vege.pos);
                    vege.pos *= num18;
                    vege.rot = Quaternion.FromToRotation(Vector3.up, vege.pos.normalized) * Quaternion.AngleAxis(angle, Vector3.up);
                    vege.scl = new Vector3(num47, num46, num47);
                    vege.modelIndex = (short)vegeProtos[vege.protoId].ModelIndex;
                    vege.hp = vegeHps[vege.protoId];
                    var num48 = data.AddVegeData(vege);
                    data.vegeIds[i] = (ushort)num48;
                }

                if (num30 < num43 && array2 != null && array2.Length > 0)
                {
                    vege.protoId = (short)array2[(int)(num29 * array2.Length)];
                    var quaternion2 = Quaternion.FromToRotation(Vector3.up, vector);
                    var vector7 = quaternion2 * Vector3.forward;
                    var vector8 = quaternion2 * Vector3.right;
                    var vector9 = vegeScaleRanges[vege.protoId];
                    for (var j = 0; j < num44; j++)
                    {
                        var num49 = (float)random.NextDouble() - 0.5f;
                        var num50 = (float)random.NextDouble() - 0.5f;
                        var num51 = (float)Math.Sqrt(random.NextDouble());
                        var angle2 = (float)random.NextDouble() * 360f;
                        var num52 = (float)random.NextDouble();
                        var num53 = (float)random.NextDouble();
                        var vector10 = vector * num18;
                        var normalized2 = (vector8 * num49 + vector7 * num50).normalized;
                        var num54 = num51 * num9;
                        var vector11 = normalized2 * (num54 * num12);
                        var num55 = num53 * (vector9.x + vector9.y) + (1f - vector9.x);
                        var num56 = (num52 * (vector9.z + vector9.w) + (1f - vector9.z)) * num55;
                        vege.pos = (vector10 + vector11).normalized;
                        num18 = !flag ? data.QueryHeight(vege.pos) : num28;
                        vege.pos *= num18;
                        vege.rot = Quaternion.FromToRotation(Vector3.up, vege.pos.normalized) * Quaternion.AngleAxis(angle2, Vector3.up);
                        vege.scl = new Vector3(num56, num55, num56);
                        vege.modelIndex = (short)vegeProtos[vege.protoId].ModelIndex;
                        vege.hp = 1;
                        var num57 = data.AddVegeData(vege);
                        data.vegeIds[i] = (ushort)num57;
                    }
                }
            }
        }
    }
}