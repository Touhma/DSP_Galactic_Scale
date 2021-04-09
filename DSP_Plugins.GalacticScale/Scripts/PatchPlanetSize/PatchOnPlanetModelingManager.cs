using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering;
using Patch = GalacticScale.Scripts.PatchPlanetSize.PatchForPlanetSize;

namespace GalacticScale.Scripts.PatchPlanetSize {
    [HarmonyPatch(typeof(PlanetModelingManager))]
    public class PatchOnPlanetModelingManager : MonoBehaviour {

        [HarmonyPrefix]
        [HarmonyPatch("ModelingPlanetMain")]
        public static bool ModelingPlanetMain(PlanetData planet)
        {
            planet.data.AddFactoredRadius(planet);
            return true;
        }
        [HarmonyPostfix]
        [HarmonyPatch("ModelingPlanetMain")]
        public static void ModellingPlanetMainPostFix(PlanetData planet)
        {
            if (!planet.gameObject) return;
            if (planet.type != EPlanetType.Gas) return;
            //MeshRenderer[] meshrenderers = planet.gameObject.GetComponentsInChildren(typeof(MeshRenderer)) as MeshRenderer[];
            //if (meshrenderers == null) return;
            //Patch.Debug("MeshRenderers = " + meshrenderers.Length, LogLevel.Message, true);
            //foreach (MeshRenderer m in meshrenderers)
            //{
            //    Patch.Debug("MESH " + (m), LogLevel.Message, true);
            //    m.material.color = Color.red;
            //}
            var planetbody = planet.gameObject.transform.GetChild(1);
            var pbt = planetbody.transform;
                Patch.Debug("Childcount =" +pbt.childCount, LogLevel.Message, true);
            for (var i = 0; i < pbt.childCount; i++)
            {
                var child = pbt.GetChild(i);
                if (child.name != "Surface") return;
                Patch.Debug("-" + i + "-" + pbt.GetChild(i).name, LogLevel.Message, true);
                MeshRenderer mr = child.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
                if (mr == null)
                {
                    Patch.Debug("didnt get meshrenderer", LogLevel.Message, true);
                    break;
                }
                var m = mr.materials[0];
                m.color = Color.red; // SetColor("_Color", Color.red);
            }

        }
        
        [HarmonyTranspiler]
        [HarmonyPatch("ModelingPlanetMain")]
        public static IEnumerable<CodeInstruction> ModelingPlanetMainTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> instructionList = new List<CodeInstruction>(instructions);

            Patch.Debug("ModelingPlanetMain Transpiler.", LogLevel.Debug, Patch.DebugPlanetModelingManagerDeep);
            for (int instructionCounter = 0; instructionCounter < instructionList.Count; instructionCounter++)
            {
                if (instructionList[instructionCounter].Calls(typeof(PlanetData).GetProperty("realRadius").GetGetMethod()))
                {
                    Patch.Debug("Found realRadius Property getter call.", LogLevel.Debug, Patch.DebugPlanetModelingManagerDeep);
                    if (instructionCounter + 4 < instructionList.Count &&
                        instructionList[instructionCounter + 1].opcode == OpCodes.Ldc_R4 && instructionList[instructionCounter + 1].OperandIs(0.2f) &&
                        instructionList[instructionCounter + 2].opcode == OpCodes.Add &&
                        instructionList[instructionCounter + 3].opcode == OpCodes.Ldc_R4 && instructionList[instructionCounter + 3].OperandIs(0.025f))
                    {
                        Patch.Debug("Found THE CORRECT realRadius Property getter call.", LogLevel.Debug, Patch.DebugPlanetModelingManagerDeep);
                        //+1 = ldc.r4 0.2
                        //+2 = add
                        //+3 = ldc.r4 0.025 <-- replace
                        instructionList.RemoveAt(instructionCounter + 3);
                        List<CodeInstruction> toInsert = new List<CodeInstruction>()
                        {
                            new CodeInstruction(OpCodes.Ldarg_0),
                            new CodeInstruction(instructionList[instructionCounter]),
                            new CodeInstruction(OpCodes.Ldc_R4, 8000f),
                            new CodeInstruction(OpCodes.Div)
                        };
                        instructionList.InsertRange(instructionCounter + 3, toInsert);
                    }
                }
                else if (instructionList[instructionCounter].Calls(typeof(PlanetRawData).GetMethod("GetModPlane")))
                {
                    Patch.Debug("Found GetModPlane callvirt. Replacing with GetModPlaneInt call.", LogLevel.Debug, Patch.DebugPlanetModelingManagerDeep);
                    instructionList[instructionCounter] = new CodeInstruction(OpCodes.Call, typeof(PlanetRawDataExtension).GetMethod("GetModPlaneInt"));
                }
            }

            return instructionList.AsEnumerable();
        }
    }
}