using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace GalacticScale
{
    public class PatchOnDysonStatistics
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(UIStatisticsDysonDetailPanel), nameof(UIStatisticsDysonDetailPanel.RefreshShellGroup))]
        public static IEnumerable<CodeInstruction> RefreshShellGroup_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return UseSphereBounds(instructions);
        }

        [HarmonyTranspiler]
        [HarmonyPatch(typeof(DysonSphereStatPlan), nameof(DysonSphereStatPlan.RefreshDetailData))]
        public static IEnumerable<CodeInstruction> RefreshDetailData_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            return UseSphereBounds(instructions);
        }

        private static IEnumerable<CodeInstruction> UseSphereBounds(IEnumerable<CodeInstruction> instructions)
        {
            var physicsGetter = AccessTools.PropertyGetter(typeof(StarData), nameof(StarData.physicsRadius));
            var dysonField = AccessTools.Field(typeof(StarData), nameof(StarData.dysonRadius));
            var physicsHelper = AccessTools.Method(typeof(PatchOnDysonStatistics), nameof(MinBoundPhysicsRadius));
            var dysonHelper = AccessTools.Method(typeof(PatchOnDysonStatistics), nameof(MaxBoundDysonRadius));
            foreach (var instruction in instructions)
            {
                if (instruction.Calls(physicsGetter))
                {
                    instruction.opcode = OpCodes.Call;
                    instruction.operand = physicsHelper;
                }
                else if (instruction.LoadsField(dysonField))
                {
                    instruction.opcode = OpCodes.Call;
                    instruction.operand = dysonHelper;
                }

                yield return instruction;
            }
        }

        private static DysonSphere SphereOf(StarData star)
        {
            var spheres = GameMain.data?.dysonSpheres;
            if (star == null || spheres == null || star.index < 0 || star.index >= spheres.Length) return null;
            return spheres[star.index];
        }

        public static float MinBoundPhysicsRadius(StarData star)
        {
            var sphere = SphereOf(star);
            if (sphere == null || sphere.minOrbitRadius <= 0f) return star.physicsRadius;
            var radius = sphere.minOrbitRadius / 1.5f;
            if (star.type == EStarType.GiantStar) radius /= 0.6f;
            return radius;
        }

        public static float MaxBoundDysonRadius(StarData star)
        {
            var sphere = SphereOf(star);
            if (sphere == null || sphere.maxOrbitRadius <= 0f) return star.dysonRadius;
            return sphere.maxOrbitRadius / 80000f;
        }
    }
}