using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public class PatchOnDysonSphere
    {
        /*************************************************************************************************************
        * This patch replaces the original DysonSphere.Init method to fix array initialization issues with larger star counts.
        * 
        * Changes from original:
        * 1. Added null checks for __instance, _gameData, and _starData parameters
        * 2. Explicitly initializes all arrays with proper sizes using the correct constants:
        *    - layersSorted[kMaxLayerCount] - Fixed size array for sorted layer references
        *    - layersIdBased[kMaxLayerCount + 1] - Fixed size array for layer lookup by ID
        *    - rocketPool and rocketRecycle - Initialized with default capacity
        *    - autoNodes[kAutoNodeMax] - Fixed size array for auto-selected nodes
        *    - nrdPool and nrdRecycle - Initialized with default capacity
        * 3. Ensures all arrays are initialized before use to prevent null reference exceptions
        * 4. Properly initializes all cursors and counters to their starting values
        * 5. Sets up renderers and render masks with proper initialization
        * 6. Added try-catch block with proper error handling
        * 
        * The patch returns false to completely replace the original method, ensuring our initialization logic
        * is used instead of the original to prevent array bounds issues with larger star systems.
        *************************************************************************************************************/
        [HarmonyPrefix]
        [HarmonyPatch(typeof(DysonSphere), "Init")]
        public static bool Init(DysonSphere __instance, GameData _gameData, StarData _starData)
        {
            try
            {
                if (__instance == null || _gameData == null || _starData == null) return false;

                // Initialize basic properties first
                __instance.gameData = _gameData;
                __instance.starData = _starData;
                __instance.sunColor = Color.white;
                __instance.energyGenPerSail = Configs.freeMode.solarSailEnergyPerTick;
                __instance.energyGenPerNode = Configs.freeMode.dysonNodeEnergyPerTick;
                __instance.energyGenPerFrame = Configs.freeMode.dysonFrameEnergyPerTick;
                __instance.energyGenPerShell = Configs.freeMode.dysonShellEnergyPerTick;

                // Initialize arrays with proper sizes using constants
                const int kMaxLayerCount = 10;  // From DysonSphere class
                const int kAutoNodeMax = 8;     // From DysonSphere class
                
                __instance.layerCount = 0;
                __instance.layersSorted = new DysonSphereLayer[kMaxLayerCount];
                __instance.layersIdBased = new DysonSphereLayer[kMaxLayerCount + 1];

                // Initialize rocket-related arrays
                __instance.rocketCapacity = 256;
                __instance.rocketPool = new DysonRocket[__instance.rocketCapacity];
                __instance.rocketRecycle = new int[__instance.rocketCapacity];
                __instance.rocketCursor = 1;
                __instance.rocketRecycleCursor = 0;

                // Initialize auto nodes array
                __instance.autoNodes = new DysonNode[kAutoNodeMax];
                __instance.autoNodeCount = 0;

                // Initialize node render data arrays
                __instance.nrdCapacity = 128;
                __instance.nrdPool = new DysonNodeRData[__instance.nrdCapacity];
                __instance.nrdRecycle = new int[__instance.nrdCapacity];
                __instance.nrdCursor = 1;
                __instance.nrdRecycleCursor = 0;

                if (_starData != null)
                {
                    float num = 4f;
                    __instance.gravity = (float)(86646732.73933044 * (double)_starData.mass) * num;
                    double luminoMult = _starData.dysonLumino;
                    __instance.energyGenPerSail = (long)((double)__instance.energyGenPerSail * luminoMult);
                    __instance.energyGenPerNode = (long)((double)__instance.energyGenPerNode * luminoMult);
                    __instance.energyGenPerFrame = (long)((double)__instance.energyGenPerFrame * luminoMult);
                    __instance.energyGenPerShell = (long)((double)__instance.energyGenPerShell * luminoMult);

                    __instance.sunColor = Configs.builtin.dysonSphereSunColors.Evaluate(_starData.color);
                    __instance.emissionColor = Configs.builtin.dysonSphereEmissionColors.Evaluate(_starData.color);
                    if (_starData.type == EStarType.NeutronStar)
                    {
                        __instance.sunColor = Configs.builtin.dysonSphereNeutronSunColor;
                        __instance.emissionColor = Configs.builtin.dysonSphereNeutronEmissionColor;
                    }

                    __instance.defOrbitRadius = (float)((double)_starData.dysonRadius * 40000.0);
                    __instance.minOrbitRadius = _starData.physicsRadius * 1.5f;
                    if (__instance.minOrbitRadius < 4000f)
                    {
                        __instance.minOrbitRadius = 4000f;
                    }
                    __instance.maxOrbitRadius = __instance.defOrbitRadius * 2f;
                    
                    // Safely handle planets array
                    if (_starData.planets != null && _starData.planets.Length > 0)
                    {
                        __instance.avoidOrbitRadius = (float)((double)_starData.planets[0].orbitRadius * 40000.0);
                    }
                    else
                    {
                        __instance.avoidOrbitRadius = __instance.minOrbitRadius * 1.5f;
                    }
                    
                    if (_starData.type == EStarType.GiantStar)
                    {
                        __instance.minOrbitRadius *= 0.6f;
                    }
                    __instance.defOrbitRadius = Mathf.Round(__instance.defOrbitRadius / 100f) * 100f;
                    __instance.minOrbitRadius = Mathf.Ceil(__instance.minOrbitRadius / 100f) * 100f;
                    __instance.maxOrbitRadius = Mathf.Round(__instance.maxOrbitRadius / 100f) * 100f;
                    __instance.randSeed = _starData.seed;
                }

                // Initialize swarm after arrays are set up
                __instance.swarm = new DysonSwarm(__instance);
                __instance.swarm.Init();

                // Initialize renderers last
                __instance.modelRenderer = new DysonSphereSegmentRenderer(__instance);
                __instance.modelRenderer.Init();
                __instance.rocketRenderer = new DysonRocketRenderer(__instance);
                __instance.inEditorRenderMaskL = -1;
                __instance.inEditorRenderMaskS = -1;
                __instance.inGameRenderMaskL = -1;
                __instance.inGameRenderMaskS = -1;

                return false; // Skip original method
            }
            catch (System.Exception e)
            {
                GS2.Warn($"Error in DysonSphere.Init patch: {e.Message}\n{e.StackTrace}");
                return true; // Let the original method handle it if we fail
            }
        }
    }
} 