using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using HarmonyLib;

namespace GalacticScale
{
    internal static class MoreMegaStructuresCompatibility
    {
        private const string AssemblyName = "MoreMegaStructure";
        private const string TypeName = "MoreMegaStructure.WarpArray";
        private const string MethodName = "StationShipUpdatesPrePatch";
        private static readonly object InstallLock = new object();
        private static volatile bool installed;
        private static Harmony harmonyInstance;
        private static int retryCounter;

        public static void TryInstall(Harmony harmony)
        {
            lock (InstallLock)
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
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(StationComponent), nameof(StationComponent.InternalTickRemote))]
        public static void RetryInstallIfNeeded()
        {
            if (installed || harmonyInstance == null || Interlocked.Increment(ref retryCounter) < 600)
                return;

            Interlocked.Exchange(ref retryCounter, 0);
            TryInstall(null);
        }

        public static IEnumerable<CodeInstruction> ApplyPathingFixes(IEnumerable<CodeInstruction> instructions, MethodBase __originalMethod)
        {
            return PatchOnStationComponent.ApplyPathingFixes(instructions, __originalMethod, "MoreMegaStructures compatibility");
        }
    }
}
