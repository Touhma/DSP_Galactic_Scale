using HarmonyLib;
using PowerNetworkStructures;
using UnityEngine;

namespace GalacticScale
{
	public class PatchOnPowerSystem
	{
		[HarmonyPrefix]
		[HarmonyPatch(typeof(PowerSystem), "line_arragement_for_add_node")]
		public static bool line_arrangement_for_add_node(Node node, ref int[] ___tmp_state)
		{
			//GS2.Warn("laan");
			if (___tmp_state == null) ___tmp_state = new int[2048];
			return true;
		}

		[HarmonyPrefix]
		[HarmonyPatch(typeof(PowerSystem), "NewGeneratorComponent")]
		public static bool NewGeneratorComponent(int entityId, PrefabDesc desc, ref int __result, PowerSystem __instance)
		{
			if (GS2.Vanilla) return true;
			//GS2.Warn("New Generator Component!");
			int num2;
			if (__instance.genRecycleCursor > 0)
			{
				int[] array = __instance.genRecycle;
				int num = __instance.genRecycleCursor - 1;
				__instance.genRecycleCursor = num;
				num2 = array[num];
			}
			else
			{
				int num = __instance.genCursor;
				__instance.genCursor = num + 1;
				num2 = num;
				if (num2 == __instance.genCapacity)
				{
					__instance.SetGeneratorCapacity(__instance.genCapacity * 2);
				}
			}
			__instance.genPool[num2].id = num2;
			__instance.genPool[num2].entityId = entityId;
			__instance.genPool[num2].subId = desc.subId;
			__instance.genPool[num2].photovoltaic = desc.photovoltaic;
			__instance.genPool[num2].wind = desc.windForcedPower;
			__instance.genPool[num2].gamma = desc.gammaRayReceiver;
			__instance.genPool[num2].geothermal = desc.geothermal;
			__instance.genPool[num2].genEnergyPerTick = desc.genEnergyPerTick;
			__instance.genPool[num2].useFuelPerTick = desc.useFuelPerTick;
			__instance.genPool[num2].fuelMask = (short)desc.fuelMask;
			__instance.genPool[num2].catalystId = desc.powerCatalystId;
			__instance.genPool[num2].catalystPoint = 0;
			__instance.genPool[num2].productId = 0;
			__instance.genPool[num2].productCount = 0f;
			__instance.genPool[num2].productHeat = desc.powerProductHeat;
			__instance.genPool[num2].warmup = 0f;
			__instance.genPool[num2].warmupSpeed = 0f;
			float num3 = __instance.planet.realRadius + __instance.planet.ionHeight * 0.6f;
			float realRadius = __instance.planet.realRadius;
			__instance.genPool[num2].ionEnhance = Mathf.Sqrt(num3 * num3 - realRadius * realRadius) / num3;
			__instance.genPool[num2].gthStrength = 0f;
			Vector3 pos = __instance.factory.entityPool[entityId].pos;
			Quaternion rot = __instance.factory.entityPool[entityId].rot;
			if (desc.geothermal)
			{
				//GS2.Warn($"GEO RealRadius:{__instance.planet.realRadius } QModHeight:{__instance.planet.data.QueryModifiedHeight(pos)}");
				float num4 = __instance.planet.realRadius - 4f - __instance.planet.data.QueryModifiedHeight(pos);
				float num5 = Mathf.Clamp(num4 * ((num4 > 0f) ? 0.3f : 0.9f), -0.9f, 0.3f) * 0.05f;
				for (int i = 0; i < PowerSystem.gthDetectionPos.Length; i++)
				{
					num4 = __instance.planet.realRadius - 4f - __instance.planet.data.QueryModifiedHeight(pos + rot * PowerSystem.gthDetectionPos[i]);
					//GS2.Warn($"num4: { num4} i:{i} QMODTHING:{__instance.planet.data.QueryModifiedHeight(pos + rot * PowerSystem.gthDetectionPos[i])} PowerSystem.gthDetectionPos:{PowerSystem.gthDetectionPos[i]}");

					if (num4 > 0f && i > 0)
					{
						float num6 = __instance.planet.realRadius - 4f - __instance.planet.data.QueryModifiedHeight(pos + rot * (PowerSystem.gthDetectionPos[i] * 1.2f));
						float num7 = (num6 > 0f) ? 0.4f : 0.6f;
						num4 = (1f - num7) * num4 + num7 * num6;
					}
					num5 += Mathf.Clamp(num4 * ((num4 > 0f) ? 0.3f : 0.9f), -0.9f, 0.3f) * 0.19f;
				}
				__instance.genPool[num2].gthStrength = __instance.CalculateGeothermalStrenth(pos, rot);
				__instance.genPool[num2].warmupSpeed = 0.0027777778f;
			}
			Vector3 normalized = pos.normalized;
			__instance.genPool[num2].x = normalized.x;
			__instance.genPool[num2].y = normalized.y;
			__instance.genPool[num2].z = normalized.z;
			__instance.factory.entityPool[entityId].powerGenId = num2;
			__result = num2;
			return false;
		}
		[HarmonyPrefix]
		[HarmonyPatch(typeof(PowerSystem), "CalculateGeothermalStrenth")]
		public static bool CalculateGeothermalStrenth(Vector3 pos, Quaternion rot, ref float __result, PowerSystem __instance)
		{
			if (GS2.Vanilla) return true;
			float num = 0f;
			float rr = __instance.planet.realRadius;
			if (VFInput.alt) rr -= 4f;
            GS2.Warn("CalcGeoStrenth " + rr);
            if (__instance.planet.waterItemId == -1)
			{
				for (int i = 0; i < PowerSystem.gthDetectionPos.Length; i++)
				{
					float num2 = __instance.planet.realRadius -4f - __instance.planet.data.QueryModifiedHeight(pos);
					if (i < 2)
					{
						num += Mathf.Clamp(num2 * ((num2 > 0f) ? 0.8f : 0.9f), -0.9f, 0.3f) * 0.1f;
					}
					else
					{
						num2 = __instance.planet.realRadius - 4f - __instance.planet.data.QueryModifiedHeight(pos + rot * PowerSystem.gthDetectionPos[i]);
						num += Mathf.Clamp(num2 * ((num2 > 0f) ? 0.8f : 0.9f), -0.9f, 0.3f) * 0.08f;
						
						num2 = __instance.planet.realRadius - 4f - __instance.planet.data.QueryModifiedHeight(pos + rot * PowerSystem.gthDetectionPos[i] * 2f);
						num += Mathf.Clamp(num2 * ((num2 > 0f) ? 0.8f : 0.9f), -0.9f, 0.3f) * 0.02f;
					}
				}
				num += 1f;
			}
            GS2.Warn($"***GeoStrenth:{num}");
            float modifier = 1;
			if (__instance.planet.orbitAroundPlanet == null) {
				modifier -= __instance.planet.orbitRadius;
			}
			else if (__instance.planet.orbitAroundPlanet.orbitAroundPlanet == null) {
				modifier -= __instance.planet.orbitAroundPlanet.orbitRadius;
			}
			else if (__instance.planet.orbitAroundPlanet.orbitAroundPlanet.orbitAroundPlanet == null)
			{
				modifier -= __instance.planet.orbitAroundPlanet.orbitAroundPlanet.orbitRadius;
			}
			if (modifier < 0) modifier = 0;
			if (__instance.planet.realRadius > 200)
			{
				var diff = __instance.planet.realRadius - 200;
				var mod = diff / 300f;
				modifier *= (1 + mod);
			}
			num *= (modifier + 1f);
            __result = num;
			return false;
		}
	}
}