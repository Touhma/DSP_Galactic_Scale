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
            int shipLocal = FindShipReferenceLocal(__originalMethod);
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
                if (instruction.LoadsField(radiusField))
                {
                    output.Add(new CodeInstruction(OpCodes.Call, capRadius));
                    cappedReads++;
                }

                if (shipLocal >= 0 && instruction.opcode == OpCodes.Ldc_R8 &&
                    (instruction.OperandIs(5000.0) || instruction.OperandIs(25000000.0)))
                {
                    output.Add(CreateLoadLocal(shipLocal));
                    output.Add(new CodeInstruction(OpCodes.Call, destinationRadius));
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
    }
}
