using HarmonyLib;

namespace GalacticScale
{
    public class PatchOnFactoryModel
    {
        /// <summary>
        /// The mining effect that appears on veins when actively mined by an Advanced Miner are hardcoded for 200 radius planets in the shader.
        /// To compensate for the this, adjust the Y values on the model vertices themselves by the difference in planet radius.
        /// Adjust each time a planet is loaded since the mesh is shared globally, but each planet can be a different radius.
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FactoryModel), "InitCollectorMaterial")]
        public static void AdjustMk2MiningEffect(ref FactoryModel __instance)
        {
            const float standardRadius = 200f;
            const float standardPlanetCurveOffset = 0.3f;

            var realRadius = __instance.planet.realRadius;
            if (realRadius == standardRadius) return;

            var adjustVertexY = realRadius - standardRadius;

            // veins at the far end of the miner are lower due to the curve of the planet. Adjust this offset a bit so the effect appears closer to where it should.
            // Between 0.1 and 0.9. Anything above 0.9 clips above the the top glass.
            var planetCurveOffset = (uint)(90.0f / realRadius * 10.0f) / 10.0f;
            planetCurveOffset = planetCurveOffset > 0.9 ? 0.9f : planetCurveOffset;
            adjustVertexY += planetCurveOffset - standardPlanetCurveOffset;

            if (!Utils.AdjustMk2MinerEffectVertices(adjustVertexY))
                GS2.Error("Failed to adjust mining effect for planet.");

        }
    }
}