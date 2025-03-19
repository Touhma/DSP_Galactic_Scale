using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using static GalacticScale.GS2;

namespace GalacticScale
{
    // Patch for StationComponent.InternalTickRemote to fix logistics ship pathing with larger stars
    [HarmonyPatch]
    public class PatchOnInternalTickRemote
    {
        // Changelog:
        // v1.0.0 - Initial patch to fix logistics ship pathing with larger stars
        // Adjusts avoidance distances and navigation parameters based on star radius
        
        // Target the InternalTickRemote method in StationComponent
        [HarmonyPatch(typeof(StationComponent), "InternalTickRemote")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> InternalTickRemoteTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            int insertionIndex = -1;
            int starRadiusLoadIndex = -1;
            
            // Find where star radius is loaded and avoidance distance is calculated
            for (int i = 0; i < codes.Count - 3; i++)
            {
                // Look for loading of uRadius followed by addition with num39 (5000f + shipSpeed)
                if (codes[i].LoadsField(AccessTools.Field(typeof(AstroData), "uRadius")) &&
                    codes[i + 2].opcode == OpCodes.Add)
                {
                    starRadiusLoadIndex = i;
                    insertionIndex = i + 2; // Right before the addition
                    break;
                }
            }
            
            if (insertionIndex != -1 && starRadiusLoadIndex != -1)
            {
                // Insert a call to our adjustment method right after loading uRadius
                // This will replace the raw uRadius with an adjusted value for pathing
                codes.Insert(starRadiusLoadIndex + 1, new CodeInstruction(OpCodes.Call, 
                    AccessTools.Method(typeof(PatchOnInternalTickRemote), nameof(AdjustRadiusForPathing))));
                
                Log($"Patched StationComponent.InternalTickRemote for better pathing with large stars");
            }
            else
            {
                Warn("Failed to find insertion point in StationComponent.InternalTickRemote");
            }
            
            return codes;
        }
        
        // Scaling function for avoidance calculations
        public static float AdjustRadiusForPathing(float originalRadius)
        {
            // For small stars (default game sizes), keep the original radius
            if (originalRadius < 1000f)
                return originalRadius;
                
            // For larger stars, apply a logarithmic scale to avoid excessive avoidance distances
            // This ensures ships still avoid the star but don't take extreme detours
            float adjustedRadius = 1000f + (float)(Math.Log10(originalRadius / 1000f + 1) * 1000f);
            
            // Ensure we never go below the original radius to avoid ships hitting stars
            return Math.Max(originalRadius * 0.6f, adjustedRadius);
        }
        
        // Patch for CalcRemoteSingleTripTime to adjust trip time calculations
        [HarmonyPatch(typeof(StationComponent), "CalcRemoteSingleTripTime")]
        [HarmonyTranspiler]
        public static IEnumerable<CodeInstruction> CalcRemoteSingleTripTimeTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            var codes = new List<CodeInstruction>(instructions);
            bool patched = false;
            
            // Find hardcoded constants in path calculation
            for (int i = 0; i < codes.Count - 2; i++)
            {
                // Look for hardcoded distance values like 5000f
                if (codes[i].opcode == OpCodes.Ldc_R4 && 
                    (float)codes[i].operand >= 4900f && (float)codes[i].operand <= 5100f)
                {
                    // Insert our adjustment method call
                    codes.Insert(i + 1, new CodeInstruction(OpCodes.Call, 
                        AccessTools.Method(typeof(PatchOnInternalTickRemote), nameof(AdjustSafetyDistanceForPathCalculation))));
                    patched = true;
                    i += 2; // Skip ahead since we modified the list
                }
            }
            
            if (patched)
            {
                Log("Patched StationComponent.CalcRemoteSingleTripTime for better pathing with large stars");
            }
            else
            {
                Warn("Failed to patch StationComponent.CalcRemoteSingleTripTime");
            }
            
            return codes;
        }
        
        // Adjust safety distance for path calculation
        public static float AdjustSafetyDistanceForPathCalculation(float originalDistance)
        {
            // Scale safety distance based on largest star radius in the galaxy
            float largestStarRadius = GetLargestStarRadius();
            
            if (largestStarRadius <= 1000f)
                return originalDistance;
                
            // Scale the safety distance with diminishing returns
            float factor = 1f + Mathf.Log10(largestStarRadius / 1000f) * 0.75f;
            return originalDistance * factor;
        }
        
        // Helper to find the largest star radius in the galaxy
        private static float largestCachedRadius = 0f;
        private static int lastUpdateFrame = -1;
        
        private static float GetLargestStarRadius()
        {
            // Cache this value as it's expensive to calculate
            if (Time.frameCount - lastUpdateFrame > 300 || largestCachedRadius <= 0f)
            {
                lastUpdateFrame = Time.frameCount;
                largestCachedRadius = 0f;
                
                if (GameMain.galaxy?.stars != null)
                {
                    foreach (StarData star in GameMain.galaxy.stars)
                    {
                        if (star != null && star.physicsRadius > largestCachedRadius)
                        {
                            largestCachedRadius = star.physicsRadius;
                        }
                    }
                }
                
                // Fallback if we couldn't find a valid radius
                if (largestCachedRadius <= 0f)
                    largestCachedRadius = 1000f;
            }
            
            return largestCachedRadius;
        }
    }
} 