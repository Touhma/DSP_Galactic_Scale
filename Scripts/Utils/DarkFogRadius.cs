/*
 * Change Log:
 * - 2026-02-22: Add vanilla-capped star radius helper for Dark Fog pathing.
 */
namespace GalacticScale
{
    public static class DarkFogRadius
    {
        // Vanilla practical max derived from StarGen + StarData.physicsRadius (radius ~= 23.6 * 1200).
        public const float VanillaMaxStarPhysicsRadius = 28320f;

        public static float CapStarRadiusToVanillaMax(float uRadius)
        {
            if (uRadius <= 0f)
            {
                return uRadius;
            }

            return uRadius > VanillaMaxStarPhysicsRadius ? VanillaMaxStarPhysicsRadius : uRadius;
        }
    }
}
