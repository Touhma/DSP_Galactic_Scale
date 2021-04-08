using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(PlanetData))]
    public class PatchOnPlanetData {

        //Strategy: 1) Remove checks for PlanetData.scale; 2) Convert GetModPlane to use our Int version
        // 1) find all calls to ldArg.0
        // if the following instruction is ldfld float32 PlanetData::scale
        //    change ldarg.0 to OpCodes.Nop
        //    change the following instruction to ldc.r4 1
        // 2) find all calls to GetModPlane
        //[HarmonyPatch("UpdateDirtyMesh")]
        //public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
        //    var codes = new List<CodeInstruction>(instructions);
        //    for (var i = 0; i < codes.Count; i++)
        //        if (codes[i].opcode == OpCodes.Ldarg_0 && i < codes.Count - 1)
        //            // This condition removes references to this.scale. First we just check for the IL equivalent of "this."
        //            // We stop checking this 1 early because we operate on both the current AND following line when we do anything
        //        {
        //            // Check if the field we're reading from "this." is scale
        //            if (codes[i + 1].LoadsField(typeof(PlanetData).GetField("scale"))) {
        //                // Prevent "this." from being added to the stack (ordinarily, the field reference would remove it from the stack)
        //                codes[i] = new CodeInstruction(OpCodes.Nop);
        //                // Instead load the fixed value 1, as a float32
        //                codes[i + 1] = new CodeInstruction(OpCodes.Ldc_R4, 1f);
        //            }

        //        }
        //        else if (codes[i].Calls(typeof(PlanetRawData).GetMethod("GetModPlane")))
        //            // This condition finds calls to PlanetRawData.GetModPlane (which returns a short)
        //        {
        //            // We instead call PlanetRawDataExtension.GetModPlaneInt (which returns an int)
        //            // All existing calls to GetModPlane cast the result to a float, anyway...
        //            codes[i] = new CodeInstruction(OpCodes.Call, typeof(PlanetRawDataExtension).GetMethod("GetModPlaneInt"));
        //        }
        //    return codes.AsEnumerable();
        //}

        [HarmonyPrefix]
        [HarmonyPatch("realRadius", MethodType.Getter)]
        public static bool RealRadiusGetter(ref PlanetData __instance, ref float __result) {
            if (__instance.type != EPlanetType.Gas) {
                __result = __instance.radius;
                return false;
            }

            return true;
        }
        [HarmonyPrefix, HarmonyPatch("UpdateDirtyMesh")]
        public static bool UpdateDirtyMesh(int dirtyIdx, ref PlanetData __instance)
        {
            PatchForPlanetSize.Debug("Dirty Mesh", BepInEx.Logging.LogLevel.Message, true);
                if (!__instance.dirtyFlags[dirtyIdx])
                    return false;
                __instance.dirtyFlags[dirtyIdx] = false;
                int num1 = __instance.precision / __instance.segment;
                int num2 = __instance.segment * __instance.segment;
                int num3 = dirtyIdx / num2;
                int num4 = num3 % 2;
                int num5 = num3 / 2;
                int num6 = dirtyIdx % num2;
                int num7 = num6 % __instance.segment * num1 + num4 * __instance.data.substride;
                int num8 = num6 / __instance.segment * num1 + num5 * __instance.data.substride;
                int stride = __instance.data.stride;
                float num9 = (float)((double)__instance.radius * (double)__instance.scale + 0.200000002980232);
                Mesh mesh = __instance.meshes[dirtyIdx];
                Vector3[] vertices = mesh.vertices;
                Vector3[] normals = mesh.normals;
                int index1 = 0;
                for (int index2 = num8; index2 <= num8 + num1; ++index2)
                {
                    for (int index3 = num7; index3 <= num7 + num1; ++index3)
                    {
                        int index4 = index3 + index2 * stride;
                        float num10 = (float)__instance.data.heightData[index4] * 0.01f * __instance.scale;
                        float num11 = (float)__instance.data.GetModLevel(index4) * 0.3333333f;
                        float num12 = num9;
                        if ((double)num11 > 0.0)
                            num12 = (float)__instance.data.GetModPlaneInt(index4) * 0.01f * __instance.scale;
                        float num13 = (float)((double)num10 * (1.0 - (double)num11) + (double)num12 * (double)num11);
                        vertices[index1].x = __instance.data.vertices[index4].x * num13;
                        vertices[index1].y = __instance.data.vertices[index4].y * num13;
                        vertices[index1].z = __instance.data.vertices[index4].z * num13;
                        normals[index1].x = (float)((double)__instance.data.normals[index4].x * (1.0 - (double)num11) + (double)__instance.data.vertices[index4].x * (double)num11);
                        normals[index1].y = (float)((double)__instance.data.normals[index4].y * (1.0 - (double)num11) + (double)__instance.data.vertices[index4].y * (double)num11);
                        normals[index1].z = (float)((double)__instance.data.normals[index4].z * (1.0 - (double)num11) + (double)__instance.data.vertices[index4].z * (double)num11);
                        normals[index1].Normalize();
                        ++index1;
                    }
                }
                mesh.vertices = vertices;
                mesh.normals = normals;
                __instance.meshColliders[dirtyIdx].sharedMesh = (Mesh)null;
                __instance.meshColliders[dirtyIdx].sharedMesh = mesh;
                return true;
       
        }
    }
}