using System;
using HarmonyLib;
using UnityEngine;
using static GalacticScale.GS3;

namespace GalacticScale.Patches
{
    public static partial class PatchOnPlayerController
    {
    [HarmonyPrefix]
	[HarmonyPatch(typeof(PlayerController), nameof(PlayerController.ApplyGravity))]
	private static bool ApplyGravity(ref PlayerController __instance)
	{
		if (Config.NewGravity) return true;
		VectorLF3 v = VectorLF3.zero;
		if (GameMain.localStar != null)
		{
			StarData localStar = GameMain.localStar;
			double num = 0.0;
			VectorLF3 lhs = VectorLF3.zero;
			for (int i = 0; i < localStar.planetCount; i++)
			{
				PlanetData planetData = localStar.planets[i];
				VectorLF3 lhs2 = planetData.uPosition - __instance.player.uPosition;
				double magnitude = lhs2.magnitude;
				if (magnitude > 1.0)
				{
					double y = (double)Math.Max((800f - (float)magnitude) / 150f, 0f);
					double num2 = Math.Pow(10.0, y);
					VectorLF3 lhs3 = lhs2 / magnitude;
					double num3 = magnitude / (double)planetData.realRadius;
					double num4 = num3 * 0.800000011920929;
					double num5 = ((num3 < 1.0) ? num3 : (1.0 / (num4 * num4)));
					if (num5 > 1.0)
					{
						num5 = 1.0;
					}
					double num6 = Math.Sqrt((double)planetData.realRadius) * 3.5;
					lhs += lhs3 * (num6 * num5 * num2);
					num += num2;
				}
			}
			VectorLF3 lhs4 = localStar.uPosition - __instance.player.uPosition;
			double magnitude2 = lhs4.magnitude;
			if (magnitude2 > 1.0)
			{
				double num7 = 1.0;
				VectorLF3 lhs5 = lhs4 / magnitude2;
				double num8 = magnitude2 / (double)(localStar.orbitScaler * 800f);
				double num9 = num8 * 0.10000000149011612;
				double num10 = ((num8 < 1.0) ? num8 : (1.0 / (num9 * num9)));
				if (num10 > 1.0)
				{
					num10 = 1.0;
				}
				double num11 = 26.7;
				lhs += lhs5 * (num11 * num10 * num7);
				num += num7;
			}
			v = lhs / num;
		}
		if (v.sqrMagnitude > 1E-06)
		{
			__instance.universalGravity = v;
			__instance.localGravity = Maths.QInvRotateLF(__instance.gameData.relativeRot, v);
		}
		else
		{
			__instance.universalGravity = VectorLF3.zero;
			__instance.localGravity = Vector3.zero;
		}
		if (!__instance.player.sailing && !__instance.gameData.disableController && __instance.player.isAlive)
		{
			__instance.AddLocalForce(__instance.localGravity);
		}
		Debug.DrawRay(__instance.transform.localPosition, __instance.localGravity * 0.1f, Color.white);
		if (__instance.gameData.localPlanet != null)
		{
			Vector3 forward = __instance.transform.forward;
			Vector3 normalized = __instance.transform.localPosition.normalized;
			Vector3 normalized2 = Vector3.Cross(Vector3.Cross(normalized, forward).normalized, normalized).normalized;
			__instance.transform.localRotation = Quaternion.LookRotation(normalized2, normalized);
			return false;
		}
		__instance.transform.localRotation = Quaternion.identity;
		return false;
	}
    }
}