using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnPlanetRawData
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetRawData), "QueryModifiedHeight")]
        public static bool QueryModifiedHeight(ref PlanetRawData __instance, ref float __result, Vector3 vpos)
        {
            //GS2.Warn((__instance == null).ToString());
            vpos.Normalize();
            var index1 = __instance.indexMap[__instance.PositionHash(vpos)];
            var radiusPrecision = 3.1415927f / (__instance.precision * 2) * 1.2f;
            var radiusPrecisionSq = radiusPrecision * radiusPrecision;
            var magnitudeOnPrecisionDummy = 0f;
            var HeightTimePrecision = 0f;
            var stride = __instance.stride;
            for (var index2 = -1; index2 <= 3; index2++)
            for (var index3 = -1; index3 <= 3; index3++)
            {
                var index4 = index1 + index2 + index3 * stride;
                if ((ulong)index4 < (ulong)__instance.dataLength)
                {
                    var sqrMagnitude = (__instance.vertices[index4] - vpos).sqrMagnitude;
                    if (sqrMagnitude <= radiusPrecisionSq)
                    {
                        var magnitudeOnPrecision = 1f - Mathf.Sqrt(sqrMagnitude) / radiusPrecision;
                        magnitudeOnPrecision *= __instance.GetFactoredScale();
                        var modLevel = __instance.GetModLevel(index4);
                        float HeightDataFinal = __instance.heightData[index4];
                        if (modLevel > 0)
                        {
                            float modPlane = __instance.GetModPlaneInt(index4);
                            if (modLevel == 3)
                            {
                                HeightDataFinal = modPlane;
                            }
                            else
                            {
                                var num11 = modLevel * 0.3333333f;
                                HeightDataFinal = __instance.heightData[index4] * (1f - num11) + modPlane * num11;
                            }
                        }

                        magnitudeOnPrecisionDummy += magnitudeOnPrecision;
                        HeightTimePrecision += HeightDataFinal * magnitudeOnPrecision;
                    }
                }
            }

            if (magnitudeOnPrecisionDummy == 0f)
            {
                GS2.Log($"Bad Query. LocalPlanet:{GameMain.localPlanet.name} Vector:{vpos}");
                __result = __instance.heightData[0] * 0.01f;
                return false;
            }

            __result = HeightTimePrecision / magnitudeOnPrecisionDummy * 0.01f;
            return false;
        }
    }
}

/* ChatGPT Generated Rewrite

public static bool QueryModifiedHeight(ref PlanetRawData planetData, ref float height, Vector3 position)
{
    // Normalize input position vector
    position.Normalize();
    
    // Get index of vertex in the planet data
    var vertexIndex = planetData.indexMap[planetData.PositionHash(position)];
    
    // Calculate precision-related values
    var radiusPrecision = 3.1415927f / (planetData.precision * 2) * 1.2f;
    var radiusPrecisionSq = radiusPrecision * radiusPrecision;
    
    // Initialize variables for magnitude and height calculations
    var magnitudeOnPrecisionSum = 0f;
    var heightTimePrecisionSum = 0f;
    var stride = planetData.stride;
    
    // Iterate over neighboring vertices to calculate final height
    for (var x = -1; x <= 3; x++)
    {
        for (var y = -1; y <= 3; y++)
        {
            var neighborIndex = vertexIndex + x + y * stride;
            if ((ulong)neighborIndex < (ulong)planetData.dataLength)
            {
                var sqrMagnitude = (planetData.vertices[neighborIndex] - position).sqrMagnitude;
                if (sqrMagnitude <= radiusPrecisionSq)
                {
                    // Calculate magnitude on precision and multiply by factored scale
                    var magnitudeOnPrecision = 1f - Mathf.Sqrt(sqrMagnitude) / radiusPrecision;
                    magnitudeOnPrecision *= planetData.GetFactoredScale();
                    
                    // Get modification level of vertex
                    var modLevel = planetData.GetModLevel(neighborIndex);
                    
                    // If vertex has modifications, adjust height calculation accordingly
                    var heightData = planetData.heightData[neighborIndex];
                    if (modLevel > 0)
                    {
                        var modPlane = planetData.GetModPlaneInt(neighborIndex);
                        if (modLevel == 3)
                        {
                            heightData = modPlane;
                        }
                        else
                        {
                            var blendFactor = modLevel * 0.3333333f;
                            heightData = planetData.heightData[neighborIndex] * (1f - blendFactor) + modPlane * blendFactor;
                        }
                    }
                    
                    // Add to magnitude and height sums
                    magnitudeOnPrecisionSum += magnitudeOnPrecision;
                    heightTimePrecisionSum += heightData * magnitudeOnPrecision;
                }
            }
        }
    }

    // Handle cases where no valid vertices were found
    if (magnitudeOnPrecisionSum == 0f)
    {
        GS2.Log($"Bad Query. LocalPlanet:{GameMain.localPlanet.name} Vector:{position}");
        height = planetData.heightData[0] * 0.01f;
        return false;
    }

    // Calculate final height using magnitude and height sums
    height = heightTimePrecisionSum / magnitudeOnPrecisionSum * 0.01f;
    return false;
}
*/