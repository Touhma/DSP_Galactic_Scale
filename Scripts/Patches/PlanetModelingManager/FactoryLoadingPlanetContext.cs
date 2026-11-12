using HarmonyLib;

namespace GalacticScale
{
    public partial class PatchOnPlanetModelingManager
    {
        public sealed class Context
        {
            public GPUInstancingManager GpuManager;
            public PlanetData PreviousGpuPlanet;
            public BPGPUInstancingManager BuildPreviewManager;
            public PlanetData PreviousBuildPreviewPlanet;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlanetModelingManager), "LoadingPlanetFactoryMain")]
        public static bool LoadingPlanetFactoryMain_Prefix(PlanetData planet, out Context __state)
        {
            var gpuManager = GameMain.gpuiManager;
            var buildPreviewManager = GameMain.bpgpuiManager;
            __state = new Context
            {
                GpuManager = gpuManager,
                PreviousGpuPlanet = gpuManager?.specifyPlanet,
                BuildPreviewManager = buildPreviewManager,
                PreviousBuildPreviewPlanet = buildPreviewManager?.specifyPlanet
            };

            if (planet == null || !planet.loaded || planet.gameObject == null)
            {
                GS2.Warn($"Dropping stale factory load request for {planet?.name}: loaded:{planet?.loaded} " +
                         $"model:{planet?.gameObject != null} stage:{PlanetModelingManager.currentFactingStage}");
                planet?.UnloadFactory();
                PlanetModelingManager.currentFactingPlanet = null;
                PlanetModelingManager.currentFactingStage = 0;
                return false;
            }

            if (gpuManager != null) gpuManager.specifyPlanet = planet;
            if (buildPreviewManager != null) buildPreviewManager.specifyPlanet = planet;
            return true;
        }

        [HarmonyFinalizer]
        [HarmonyPatch(typeof(PlanetModelingManager), "LoadingPlanetFactoryMain")]
        public static System.Exception LoadingPlanetFactoryMain_Finalizer(System.Exception __exception, Context __state)
        {
            if (__state?.GpuManager != null) __state.GpuManager.specifyPlanet = __state.PreviousGpuPlanet;
            if (__state?.BuildPreviewManager != null)
                __state.BuildPreviewManager.specifyPlanet = __state.PreviousBuildPreviewPlanet;

            return __exception;
        }
    }
}
