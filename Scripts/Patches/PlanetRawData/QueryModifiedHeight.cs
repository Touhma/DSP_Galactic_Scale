using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnPlanetRawData
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(PlanetRawData), nameof(PlanetRawData.QueryHeight))]
        [HarmonyPatch(typeof(PlanetRawData), nameof(PlanetRawData.QueryModifiedHeight))]
        public static IEnumerable<CodeInstruction> PatchHeightQueries(
            IEnumerable<CodeInstruction> instructions, MethodBase __originalMethod)
        {
            var modified = __originalMethod.Name == nameof(PlanetRawData.QueryModifiedHeight);
            var matcher = new CodeMatcher(instructions);
            if (modified)
            {
                var getModPlane = AccessTools.Method(typeof(PlanetRawData), nameof(PlanetRawData.GetModPlane));
                matcher.MatchForward(false, new CodeMatch(instruction => instruction.Calls(getModPlane)));
                if (matcher.IsValid)
                    matcher.Set(OpCodes.Call, AccessTools.Method(typeof(PlanetRawDataExtension), nameof(PlanetRawDataExtension.GetModPlaneInt)));
                else
                    GS2.Error("QueryModifiedHeight: GetModPlane call not found.");
            }

            matcher.Start().MatchForward(false,
                new CodeMatch(OpCodes.Ldstr, "bad query"),
                new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Debug), nameof(Debug.LogWarning), new[] { typeof(object) })),
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(PlanetRawData), nameof(PlanetRawData.heightData))),
                new CodeMatch(OpCodes.Ldc_I4_0),
                new CodeMatch(OpCodes.Ldelem_U2),
                new CodeMatch(OpCodes.Conv_R4),
                new CodeMatch(OpCodes.Ldc_R4, 0.01f),
                new CodeMatch(OpCodes.Mul),
                new CodeMatch(OpCodes.Ret));
            if (matcher.IsInvalid)
            {
                GS2.Error($"{__originalMethod.Name}: terrain query fallback not found.");
                return matcher.InstructionEnumeration();
            }

            return matcher
                .SetAndAdvance(OpCodes.Ldarg_0, null)
                .SetAndAdvance(OpCodes.Ldarg_1, null)
                .SetAndAdvance(modified ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0, null)
                .SetAndAdvance(OpCodes.Call, AccessTools.Method(typeof(PatchOnPlanetRawData), nameof(RecoverTerrainHeight)))
                .RemoveInstructions(5)
                .InstructionEnumeration();
        }

        private static float RecoverTerrainHeight(PlanetRawData data, Vector3 position, bool modified)
        {
            if (!(position.sqrMagnitude > 0f)) return data.heightData[0] * 0.01f;

            var index = GetTerrainVertexNear(data, position);
            var radius = Mathf.PI / (data.precision * 2) * 1.2f;
            var radiusSquared = radius * radius;
            var totalWeight = 0f;
            var weightedHeight = 0f;
            var stride = data.stride;
            var length = data.vertices.Length;
            for (var x = -1; x <= 3; x++)
            for (var y = -1; y <= 3; y++)
            {
                var neighbor = index + x + y * stride;
                if ((uint)neighbor >= (uint)length) continue;

                var distanceSquared = (data.vertices[neighbor] - position).sqrMagnitude;
                if (distanceSquared >= radiusSquared) continue;

                var weight = 1f - Mathf.Sqrt(distanceSquared) / radius;
                totalWeight += weight;
                weightedHeight += GetVertexHeight(data, neighbor, modified) * weight;
            }

            return (totalWeight > 0f ? weightedHeight / totalWeight : GetVertexHeight(data, index, modified)) * 0.01f;
        }

        private static float GetVertexHeight(PlanetRawData data, int index, bool modified)
        {
            var height = data.heightData[index];
            if (!modified || data.modData == null) return height;

            var level = data.GetModLevel(index);
            if (level == 0) return height;

            var plane = data.GetModPlaneInt(index);
            if (level == 3) return plane;

            var blend = level * 0.3333333f;
            return height * (1f - blend) + plane * blend;
        }

        private static int GetTerrainVertexNear(PlanetRawData data, Vector3 position)
        {
            var north = position.y >= 0f;
            var front = position.z >= 0f;
            var left = (position.x >= 0f) == front;
            var absX = Mathf.Abs(position.x);
            var absY = Mathf.Abs(position.y);
            var absZ = Mathf.Abs(position.z);

            var secondAlongX = left == north;
            var alongA2 = secondAlongX ? absX : absZ;
            var alongB = secondAlongX ? absZ : absX;
            var rowAngle = Mathf.Atan2(alongB, absY + alongA2);
            var row = rowAngle * (2f / Mathf.PI);
            var sinRow = Mathf.Sin(rowAngle);
            var arc = Mathf.Acos(sinRow * sinRow);
            var column = arc > 0f
                ? Mathf.Atan2(alongA2 * Mathf.Sin(arc), absY + alongA2 * Mathf.Cos(arc)) / arc
                : 0f;

            var precision = data.precision;
            var columnIndex = Mathf.RoundToInt((north ? 1f - column * (1f - row) : column * (1f - row)) * precision);
            var rowIndex = Mathf.RoundToInt((north ? row : 1f - row) * precision);
            if (!left) columnIndex += data.substride;
            if (!front) rowIndex += data.substride;
            return rowIndex * data.stride + columnIndex;
        }
    }
}
