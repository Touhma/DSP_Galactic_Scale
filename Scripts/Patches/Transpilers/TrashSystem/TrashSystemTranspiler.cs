using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using static System.Reflection.Emit.OpCodes;

namespace GalacticScale
{
    internal class PatchOnTrashSystem
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(TrashSystem), nameof(TrashSystem.Gravity))]
        public static IEnumerable<CodeInstruction> TrashSystem_Gravity_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                // Fix planet loop for outer planets
                // Change: for (int i = nearStarId + 1; i <= nearStarId + 8; i++)
                // To:     for (int i = nearStarId + 1; i <= nearStarId + GameMain.galaxy.StarById(nearStarId / 100).planetCount; i++)
                var matcher = new CodeMatcher(instructions)
                    .MatchForward(true,
                        new CodeMatch(Ldloc_S),
                        new CodeMatch(Ldc_I4_1),
                        new CodeMatch(Add),
                        new CodeMatch(Stloc_S),
                        new CodeMatch(Ldloc_S),
                        new CodeMatch(Ldloc_1),
                        new CodeMatch(Ldc_I4_8), // replace target 
                        new CodeMatch(Add),
                        new CodeMatch(Ble));
                if (matcher.IsValid)
                {
                    matcher
                        .Advance(-2)
                        .RemoveInstructions(2)
                        .Insert(Transpilers.EmitDelegate<Func<int, int>>(nearStarId =>
                        {
                            return nearStarId + GameMain.galaxy.StarById(nearStarId / 100)?.planetCount ?? nearStarId + 8;
                        }));
                }
                else
                {
                    GS2.Warn("can't find planet loop target!");
                }

                // Let BAB AutoPickTrash instantly by setting lPos to solve that some trash moving too fast to land on the ground
                // if (vectorLF5.magnitude > 1.0)
                // {
                //    double num7 = -vectorLF6.x * vectorLF5.x - vectorLF6.y * vectorLF5.y - vectorLF6.z * vectorLF5.z;
                //    VectorLF3 vectorLF7 = vectorLF6 * num7;
                //    VectorLF3 vectorLF8 = vectorLF5 + vectorLF7;
                //    trash.uPos = this.astroPoses[i].uPos + normalized * (num6 + 0.005);
                //    trash.uVel = vectorLF7 * 0.3499999940395355 + vectorLF8 * 0.925000011920929 + vectorLF4;
                //    trash.lPos = vectorLF3; // Insert this line
                // }
                // else // Trash stop rolling on the ground
                // {
                //    trash.landPlanetId = i;
                //    trash.uPos = astroPoses[i].uPos + normalized * (num6 + 0.005);
                //    trash.uVel = VectorLF3.zero;
                //    trash.lPos = vectorLF3; // Copy target
                // }

                var oprand_lPos = AccessTools.Field(typeof(TrashData), nameof(TrashData.lPos));
                matcher.MatchBack(false,
                        new CodeMatch(Ldarg_1),
                        new CodeMatch(Ldloc_S),
                        new CodeMatch(Call),
                        new CodeMatch(Stfld, oprand_lPos));
                var oprand_vectorLF3 = matcher.InstructionAt(1).operand;
                var oprand_convert = matcher.InstructionAt(2).operand;

                matcher.Advance(-2)
                    .MatchBack(false,
                        new CodeMatch(Stfld, AccessTools.Field(typeof(TrashData), nameof(TrashData.uVel))))
                    .Advance(1)
                    .Insert(
                        new CodeInstruction(Ldarg_1),
                        new CodeInstruction(Ldloc_S, oprand_vectorLF3),
                        new CodeInstruction(Call, oprand_convert),
                        new CodeInstruction(Stfld, oprand_lPos)
                    );

                return matcher.InstructionEnumeration();
            }
            catch (Exception e)
            {
                GS2.Warn("TrashSystem_Gravity_Transpiler failed!");
                GS2.Warn(e.ToString());
                return instructions;
            }
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(TrashSystem), nameof(TrashSystem.AddTrash))]
        [HarmonyPatch(typeof(TrashSystem), nameof(TrashSystem.AddTrashFromGroundEnemy))]
        [HarmonyPatch(typeof(TrashSystem), nameof(TrashSystem.AddTrashOnPlanet))]
        public static IEnumerable<CodeInstruction> AddTrashTranspiler(IEnumerable<CodeInstruction> instructions, MethodBase __originalMethod)
        {
            try
            {
                // Change: if (vectorLF.magnitude < 170.0f)
                // To:     if (vectorLF.magnitude < @planet.realRadius - 30.0f)
                var matcher = new CodeMatcher(instructions)
                    .MatchForward(
                        false,
                        new CodeMatch(Ldloca_S),
                        new CodeMatch(op => op.opcode == Call && ((MethodInfo)op.operand).Name == "get_magnitude"),
                        new CodeMatch(op => op.opcode == Ldc_R8 && op.OperandIs(170f))
                    )
                    .Advance(2)
                    .RemoveInstruction();

                switch (__originalMethod.Name)
                {
                    case "AddTrash": // GameMain.localPlanet.realRadius - 30.0f
                        matcher.Insert(
                            new CodeInstruction(Call, AccessTools.PropertyGetter(typeof(GameMain), nameof(GameMain.localPlanet))),
                            new CodeInstruction(Callvirt, AccessTools.PropertyGetter(typeof(PlanetData), nameof(PlanetData.realRadius))),
                            new CodeInstruction(Ldc_R8, 30f),
                            new CodeInstruction(Sub));
                        break;
                    case "AddTrashFromGroundEnemy": // factory.planet - 30.0f
                        matcher.Insert(
                            new CodeInstruction(Ldarg_S, 5), // factory
                            new CodeInstruction(Callvirt, AccessTools.PropertyGetter(typeof(PlanetFactory), nameof(PlanetFactory.planet))),
                            new CodeInstruction(Callvirt, AccessTools.PropertyGetter(typeof(PlanetData), nameof(PlanetData.realRadius))),
                            new CodeInstruction(Ldc_R8, 30f),
                            new CodeInstruction(Sub));
                        break;
                    case "AddTrashOnPlanet": // planet - 30.0f
                        matcher.Insert(
                            new CodeInstruction(Ldarg_S, 5), // planet
                            new CodeInstruction(Callvirt, AccessTools.PropertyGetter(typeof(PlanetData), nameof(PlanetData.realRadius))),
                            new CodeInstruction(Ldc_R8, 30f),
                            new CodeInstruction(Sub));
                        break;
                }

                return instructions;
            }
            catch (Exception e)
            {
                GS2.Warn("PatchOnTrashSystem.AddTrashTranspiler failed: " + __originalMethod.Name.ToString());
                GS2.Warn(e.ToString());
                return instructions;
            }
        }
    }
}
