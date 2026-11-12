using System;
using HarmonyLib;

namespace GalacticScale
{
    public static partial class PatchOnPlanetFactory
    {
        [ThreadStatic]
        private static FlattenBatchScope _constructionFlattenScope;

        private struct FlattenBatchScope
        {
            public PlanetFactory Factory;
            public bool Started;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(ConstructionSystem), "ExecuteBuildTasks")]
        private static void BeginConstructionFlattenBatch(ref PlanetFactory ___factory, out FlattenBatchScope __state)
        {
            __state = _constructionFlattenScope;
            _constructionFlattenScope = new FlattenBatchScope { Factory = ___factory };
        }

        [HarmonyFinalizer]
        [HarmonyPatch(typeof(ConstructionSystem), "ExecuteBuildTasks")]
        private static Exception EndConstructionFlattenBatch(Exception __exception, FlattenBatchScope __state)
        {
            var scope = _constructionFlattenScope;
            try
            {
                if (scope.Started && scope.Factory != null) scope.Factory.EndFlattenTerrain();
            }
            catch (Exception ex)
            {
                GS2.Error($"Failed to finish batched construction terrain flattening: {ex}");
                if (__exception == null) __exception = ex;
            }
            finally
            {
                _constructionFlattenScope = __state;
            }

            return __exception;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetFactory), nameof(PlanetFactory.BuildFinally))]
        private static void BatchBotFlattening(PlanetFactory __instance, bool flattenTerrain, ref bool autoRefresh)
        {
            if (!autoRefresh || !flattenTerrain || !ReferenceEquals(_constructionFlattenScope.Factory, __instance)) return;

            if (!_constructionFlattenScope.Started)
            {
                __instance.BeginFlattenTerrain();
                _constructionFlattenScope.Started = true;
            }

            autoRefresh = false;
        }
    }
}
