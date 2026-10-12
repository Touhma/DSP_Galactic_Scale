using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;

namespace GalacticScale
{
    internal static class MoreMegaStructuresCompatibility
    {
        private const string AssemblyName = "MoreMegaStructure";
        private const string TypeName = "MoreMegaStructure.WarpArray";
        private const string MethodName = "StationShipUpdatesPrePatch";
        private static bool installed;
        private static Harmony harmonyInstance;
        private static int retryCounter;

        public static void TryInstall(Harmony harmony)
        {
            if (harmony != null)
                harmonyInstance = harmony;
            if (installed || harmonyInstance == null)
                return;

            Assembly assembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(candidate => string.Equals(candidate.GetName().Name, AssemblyName, StringComparison.OrdinalIgnoreCase));
            Type patchType = assembly?.GetType(TypeName, false);
            MethodInfo target = patchType?.GetMethod(MethodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
            if (target == null)
                return;

            MethodInfo transpiler = typeof(MoreMegaStructuresCompatibility).GetMethod(
                nameof(ApplyPathingFixes), BindingFlags.Static | BindingFlags.Public);
            harmonyInstance.Patch(target, transpiler: new HarmonyMethod(transpiler));
            installed = true;
            GS2.Log("Applied optional MoreMegaStructures logistics-vessel pathing compatibility.");
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StationComponent), nameof(StationComponent.InternalTickRemote))]
        public static void RetryInstallIfNeeded()
        {
            if (installed || harmonyInstance == null || ++retryCounter < 600)
                return;

            retryCounter = 0;
            TryInstall(null);
        }

        public static IEnumerable<CodeInstruction> ApplyPathingFixes(IEnumerable<CodeInstruction> instructions, MethodBase __originalMethod)
        {
            List<CodeInstruction> source = instructions.ToList();
            var output = new List<CodeInstruction>(source.Count + 48);
            FieldInfo radiusField = AccessTools.Field(typeof(AstroData), nameof(AstroData.uRadius));
            MethodInfo capRadius = AccessTools.Method(typeof(DarkFogRadius), nameof(DarkFogRadius.CapStarRadiusToVanillaMax));
            MethodInfo destinationRadius = AccessTools.Method(typeof(PatchOnStationComponent), nameof(PatchOnStationComponent.GetRadiusFromShipDestination));
            MethodInfo departureWarpDistance = AccessTools.Method(typeof(PatchOnStationComponent), nameof(PatchOnStationComponent.GetWarpStartDistanceSquaredFromShipDeparture));
            MethodInfo reentryDistance = AccessTools.Method(typeof(PatchOnStationComponent), nameof(PatchOnStationComponent.GetWarpReentryDistance));
            FieldInfo warpEnableDistField = AccessTools.Field(typeof(StationComponent), nameof(StationComponent.warpEnableDist));
            int shipLocal = FindShipReferenceLocal(__originalMethod);
            ParameterInfo[] parameters = __originalMethod.GetParameters();
            int sailSpeedArgument = FindArgument(parameters, "shipSailSpeed");
            int warpSpeedArgument = FindArgument(parameters, "shipWarpSpeed");
            int reentryThresholdStore = FindReentryThresholdStore(source, warpEnableDistField);
            bool reentryThresholdPatched = false;
            int cappedReads = 0;
            int scaledThresholds = 0;
            int softenedStarMultipliers = 0;

            for (int i = 0; i < source.Count; i++)
            {
                CodeInstruction instruction = source[i];

                if (instruction.opcode == OpCodes.Ldc_R4 && instruction.OperandIs(2.5f) &&
                    i > 0 && IsLoadLocal(source[i - 1]) && i + 1 < source.Count && source[i + 1].opcode == OpCodes.Mul)
                {
                    CodeInstruction softened = new CodeInstruction(instruction) { operand = 1.0f };
                    output.Add(softened);
                    softenedStarMultipliers++;
                    continue;
                }

                output.Add(instruction);
                if (i == reentryThresholdStore && shipLocal >= 0 && sailSpeedArgument >= 0 && warpSpeedArgument >= 0)
                {
                    int thresholdLocal = GetStoredLocalIndex(instruction);
                    output.Add(CreateLoadLocal(thresholdLocal));
                    output.Add(CreateLoadLocal(shipLocal));
                    output.Add(CreateLoadArgument(sailSpeedArgument));
                    output.Add(CreateLoadArgument(warpSpeedArgument));
                    output.Add(new CodeInstruction(OpCodes.Call, reentryDistance));
                    output.Add(CreateStoreLocal(thresholdLocal));
                    reentryThresholdPatched = true;
                }

                if (instruction.LoadsField(radiusField))
                {
                    output.Add(new CodeInstruction(OpCodes.Call, capRadius));
                    cappedReads++;
                }

                if (shipLocal >= 0 && instruction.opcode == OpCodes.Ldc_R8 && instruction.OperandIs(5000.0))
                {
                    output.Add(CreateLoadLocal(shipLocal));
                    output.Add(new CodeInstruction(OpCodes.Call, destinationRadius));
                    scaledThresholds++;
                }
                else if (shipLocal >= 0 && instruction.opcode == OpCodes.Ldc_R8 && instruction.OperandIs(25000000.0))
                {
                    output.Add(CreateLoadLocal(shipLocal));
                    output.Add(new CodeInstruction(OpCodes.Call, departureWarpDistance));
                    scaledThresholds++;
                }
            }

            if (cappedReads == 0)
                GS2.Warn("MoreMegaStructures compatibility: no AstroData.uRadius reads found in its logistics prefix.");
            if (softenedStarMultipliers == 0)
                GS2.Warn("MoreMegaStructures compatibility: star steering multiplier pattern was not found.");
            if (shipLocal < 0)
                GS2.Warn("MoreMegaStructures compatibility: ShipData reference local was not found; destination-scaled thresholds were skipped.");
            else if (scaledThresholds == 0)
                GS2.Warn("MoreMegaStructures compatibility: no clearance thresholds were found.");
            if (reentryThresholdStore < 0)
                GS2.Warn("MoreMegaStructures compatibility: warp re-entry threshold pattern was not found.");
            else if (sailSpeedArgument < 0 || warpSpeedArgument < 0)
                GS2.Warn("MoreMegaStructures compatibility: ship sail/warp speed arguments were not found; warp re-entry threshold was not adjusted.");
            else if (!reentryThresholdPatched)
                GS2.Warn("MoreMegaStructures compatibility: could not apply the warp re-entry threshold adjustment.");

            return output;
        }

        private static int FindShipReferenceLocal(MethodBase method)
        {
            try
            {
                return method.GetMethodBody()?.LocalVariables
                    .FirstOrDefault(local => local.LocalType.IsByRef && local.LocalType.GetElementType() == typeof(ShipData))
                    ?.LocalIndex ?? -1;
            }
            catch
            {
                return -1;
            }
        }

        private static bool IsLoadLocal(CodeInstruction instruction)
        {
            return instruction.opcode == OpCodes.Ldloc || instruction.opcode == OpCodes.Ldloc_S ||
                   instruction.opcode == OpCodes.Ldloc_0 || instruction.opcode == OpCodes.Ldloc_1 ||
                   instruction.opcode == OpCodes.Ldloc_2 || instruction.opcode == OpCodes.Ldloc_3;
        }

        private static CodeInstruction CreateLoadLocal(int index)
        {
            return index <= byte.MaxValue
                ? new CodeInstruction(OpCodes.Ldloc_S, (byte)index)
                : new CodeInstruction(OpCodes.Ldloc, index);
        }

        private static int FindArgument(ParameterInfo[] parameters, string name)
        {
            return parameters.FirstOrDefault(parameter => string.Equals(parameter.Name, name, StringComparison.Ordinal))?.Position ?? -1;
        }

        private static int FindReentryThresholdStore(List<CodeInstruction> instructions, FieldInfo warpEnableDistField)
        {
            for (int i = 0; i + 3 < instructions.Count; i++)
            {
                if (instructions[i].LoadsField(warpEnableDistField) &&
                    instructions[i + 1].opcode == OpCodes.Ldc_R8 && instructions[i + 1].OperandIs(0.5) &&
                    instructions[i + 2].opcode == OpCodes.Mul && IsStoreLocal(instructions[i + 3]))
                    return i + 3;
            }

            return -1;
        }

        private static bool IsStoreLocal(CodeInstruction instruction)
        {
            return instruction.opcode == OpCodes.Stloc || instruction.opcode == OpCodes.Stloc_S ||
                   instruction.opcode == OpCodes.Stloc_0 || instruction.opcode == OpCodes.Stloc_1 ||
                   instruction.opcode == OpCodes.Stloc_2 || instruction.opcode == OpCodes.Stloc_3;
        }

        private static int GetStoredLocalIndex(CodeInstruction instruction)
        {
            if (instruction.opcode == OpCodes.Stloc_0) return 0;
            if (instruction.opcode == OpCodes.Stloc_1) return 1;
            if (instruction.opcode == OpCodes.Stloc_2) return 2;
            if (instruction.opcode == OpCodes.Stloc_3) return 3;
            if (instruction.operand is LocalBuilder localBuilder) return localBuilder.LocalIndex;
            return Convert.ToInt32(instruction.operand);
        }

        private static CodeInstruction CreateStoreLocal(int index)
        {
            return index <= byte.MaxValue
                ? new CodeInstruction(OpCodes.Stloc_S, (byte)index)
                : new CodeInstruction(OpCodes.Stloc, index);
        }

        private static CodeInstruction CreateLoadArgument(int index)
        {
            switch (index)
            {
                case 0: return new CodeInstruction(OpCodes.Ldarg_0);
                case 1: return new CodeInstruction(OpCodes.Ldarg_1);
                case 2: return new CodeInstruction(OpCodes.Ldarg_2);
                case 3: return new CodeInstruction(OpCodes.Ldarg_3);
                default:
                    return index <= byte.MaxValue
                        ? new CodeInstruction(OpCodes.Ldarg_S, (byte)index)
                        : new CodeInstruction(OpCodes.Ldarg, index);
            }
        }
    }
}
