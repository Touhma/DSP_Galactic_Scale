using System;
using UnityEngine;

namespace GalacticScale
{
	public static partial class VegeAlgorithms
	{
		public static void GenerateVeges1(GSPlanet gsPlanet)
		{
			GS2.Log("GenerateVeges1|" + gsPlanet.Name);
			PlanetData planet = gsPlanet.planetData;
			ThemeProto themeProto = LDB.themes.Select(planet.theme);
			if (themeProto == null)
			{
				return;
			}
			//TODO convert veges in GSTheme
			GS2.Log("GenerateVeges1|" + gsPlanet.Name+"1");
			int[] vegetables = themeProto.Vegetables0;
			int[] vegetables2 = themeProto.Vegetables1;
			int[] vegetables3 = themeProto.Vegetables2;
			int[] vegetables4 = themeProto.Vegetables3;
			int[] vegetables5 = themeProto.Vegetables4;
			int[] vegetables6 = themeProto.Vegetables5;
			GS2.Log("GenerateVeges1|" + gsPlanet.Name + "2");
			float num = 1.3f;
			float num2 = -0.5f;
			float num3 = 2.5f;
			float num4 = 4f;
			float num5 = 0.5f;
			float num6 = 1f;
			float num7 = 2f;
			float num8 = -0.2f;
			float num9 = 1.4f;
            System.Random random = new System.Random(planet.seed);
            random.Next();
			random.Next();
			random.Next();
			int num10 = random.Next();
			System.Random random2 = new System.Random(num10);
			SimplexNoise simplexNoise = new SimplexNoise(random2.Next());
			SimplexNoise simplexNoise2 = new SimplexNoise(random2.Next());
			PlanetRawData data = planet.data;
			int stride = data.stride;
			int num11 = stride / 2;
			float num12 = planet.radius * 3.14159f * 2f / ((float)data.precision * 4f);
			GS2.Log("GenerateVeges1|" + gsPlanet.Name + "3");
			VegeData vege = default(VegeData);
			VegeProto[] vegeProtos = PlanetModelingManager.vegeProtos;
			Vector4[] vegeScaleRanges = PlanetModelingManager.vegeScaleRanges;
			short[] vegeHps = PlanetModelingManager.vegeHps;
			GS2.Log("GenerateVeges1|" + gsPlanet.Name + "4");
			for (int i = 0; i < data.dataLength; i++)
			{
				int num13 = i % stride;
				int num14 = i / stride;
				if (num13 > num11)
				{
					num13--;
				}
				if (num14 > num11)
				{
					num14--;
				}
				if (num13 % 2 != 1 || num14 % 2 != 1)
				{
					continue;
				}
				Vector3 vector = data.vertices[i];
				double num15 = data.vertices[i].x * planet.radius;
				double num16 = data.vertices[i].y * planet.radius;
				double num17 = data.vertices[i].z * planet.radius;
				float num18 = (float)(int)data.heightData[i] * 0.01f;
				float num19 = (float)(int)data.heightData[i + 1 + stride] * 0.01f;
				float num20 = (float)(int)data.heightData[i - 1 + stride] * 0.01f;
				float num21 = (float)(int)data.heightData[i + 1 - stride] * 0.01f;
				float num22 = (float)(int)data.heightData[i - 1 - stride] * 0.01f;
				float num23 = (float)(int)data.heightData[i + 1] * 0.01f;
				float num24 = (float)(int)data.heightData[i - 1] * 0.01f;
				float num25 = (float)(int)data.heightData[i + stride] * 0.01f;
				float num26 = (float)(int)data.heightData[i - stride] * 0.01f;
				float num27 = (float)(int)data.biomoData[i] * 0.01f;
				float num28 = planet.radius + 0.15f;
				bool flag = false;
				if (num18 < num28)
				{
					flag = true;
				}
				else if (num19 < num28)
				{
					flag = true;
				}
				else if (num20 < num28)
				{
					flag = true;
				}
				else if (num21 < num28)
				{
					flag = true;
				}
				else if (num22 < num28)
				{
					flag = true;
				}
				else if (num23 < num28)
				{
					flag = true;
				}
				else if (num24 < num28)
				{
					flag = true;
				}
				else if (num25 < num28)
				{
					flag = true;
				}
				else if (num26 < num28)
				{
					flag = true;
				}
				if (flag && (vegetables6 == null || vegetables6.Length == 0))
				{
					continue;
				}
				bool flag2 = true;
				if (GS2.Utils.diff(num18, num19) > 0.2f)
				{
					flag2 = false;
				}
				if (GS2.Utils.diff(num18, num20) > 0.2f)
				{
					flag2 = false;
				}
				if (GS2.Utils.diff(num18, num21) > 0.2f)
				{
					flag2 = false;
				}
				if (GS2.Utils.diff(num18, num22) > 0.2f)
				{
					flag2 = false;
				}
				double num29 = random2.NextDouble();
				num29 *= num29;
				double num30 = random2.NextDouble();
				float num31 = (float)random2.NextDouble() - 0.5f;
				float num32 = (float)random2.NextDouble() - 0.5f;
				float num33 = (float)Math.Sqrt(random2.NextDouble());
				float angle = (float)random2.NextDouble() * 360f;
				float num34 = (float)random2.NextDouble();
				float num35 = (float)random2.NextDouble();
				float num36 = 1f;
				float num37 = 0.5f;
				float num38 = 1f;
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
				double num39 = simplexNoise.Noise(num15 * 0.07, num16 * 0.07, num17 * 0.07) * (double)num36 + (double)num37 + 0.5;
				double num40 = simplexNoise2.Noise(num15 * 0.4, num16 * 0.4, num17 * 0.4) * (double)num7 + (double)num8 + 0.5;
				double num41 = num40 - 0.55;
				double num42 = num40 - 1.1;
				int[] array2;
				double num43;
				int num44;
				if (!flag)
				{
					if (num27 > 1f)
					{
						array2 = vegetables3;
						num43 = num40;
						num44 = ((vegetables6 != null && vegetables6.Length != 0) ? 2 : 4);
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
					if (!(num18 < num28 - 1f) || !(num18 > num28 - 2.2f))
					{
						continue;
					}
					array2 = vegetables6;
					num43 = num42;
					num44 = 1;
				}
				if (flag2 && num30 < num39 && array != null && array.Length > 0)
				{
					vege.protoId = (short)array[(int)(num29 * (double)array.Length)];
					Quaternion quaternion = Quaternion.FromToRotation(Vector3.up, vector);
					Vector3 vector2 = quaternion * Vector3.forward;
					Vector3 vector3 = quaternion * Vector3.right;
					Vector4 vector4 = vegeScaleRanges[vege.protoId];
					Vector3 vector5 = vector * num18;
					Vector3 normalized = (vector3 * num31 + vector2 * num32).normalized;
					float num45 = num33 * num38;
					Vector3 vector6 = normalized * (num45 * num12);
					float num46 = num35 * (vector4.x + vector4.y) + (1f - vector4.x);
					float num47 = (num34 * (vector4.z + vector4.w) + (1f - vector4.z)) * num46;
					vege.pos = (vector5 + vector6).normalized;
					num18 = data.QueryHeight(vege.pos);
					vege.pos *= num18;
					vege.rot = Quaternion.FromToRotation(Vector3.up, vege.pos.normalized) * Quaternion.AngleAxis(angle, Vector3.up);
					vege.scl = new Vector3(num47, num46, num47);
					vege.modelIndex = (short)vegeProtos[vege.protoId].ModelIndex;
					vege.hp = vegeHps[vege.protoId];
					int num48 = data.AddVegeData(vege);
					data.vegeIds[i] = (ushort)num48;
				}
				if (num30 < num43 && array2 != null && array2.Length > 0)
				{
					vege.protoId = (short)array2[(int)(num29 * (double)array2.Length)];
					Quaternion quaternion2 = Quaternion.FromToRotation(Vector3.up, vector);
					Vector3 vector7 = quaternion2 * Vector3.forward;
					Vector3 vector8 = quaternion2 * Vector3.right;
					Vector4 vector9 = vegeScaleRanges[vege.protoId];
					for (int j = 0; j < num44; j++)
					{
						float num49 = (float)random2.NextDouble() - 0.5f;
						float num50 = (float)random2.NextDouble() - 0.5f;
						float num51 = (float)Math.Sqrt(random2.NextDouble());
						float angle2 = (float)random2.NextDouble() * 360f;
						float num52 = (float)random2.NextDouble();
						float num53 = (float)random2.NextDouble();
						Vector3 vector10 = vector * num18;
						Vector3 normalized2 = (vector8 * num49 + vector7 * num50).normalized;
						float num54 = num51 * num9;
						Vector3 vector11 = normalized2 * (num54 * num12);
						float num55 = num53 * (vector9.x + vector9.y) + (1f - vector9.x);
						float num56 = (num52 * (vector9.z + vector9.w) + (1f - vector9.z)) * num55;
						vege.pos = (vector10 + vector11).normalized;
						num18 = ((!flag) ? data.QueryHeight(vege.pos) : num28);
						vege.pos *= num18;
						vege.rot = Quaternion.FromToRotation(Vector3.up, vege.pos.normalized) * Quaternion.AngleAxis(angle2, Vector3.up);
						vege.scl = new Vector3(num56, num55, num56);
						vege.modelIndex = (short)vegeProtos[vege.protoId].ModelIndex;
						vege.hp = 1;
						int num57 = data.AddVegeData(vege);
						data.vegeIds[i] = (ushort)num57;
					}
				}
			}
		}
	}
}