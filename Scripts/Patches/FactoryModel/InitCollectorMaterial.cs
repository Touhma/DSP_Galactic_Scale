using HarmonyLib;

namespace GalacticScale
{
    public class PatchOnFactoryModel
    {
        /// <summary>
        /// The mining effect that appears on veins when actively mined by an Advanced Miner are hardcoded for 200 radius planets in the shader.
        /// If using a custom shader without a hardcoded radius, set planet radius property.
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(FactoryModel), "InitCollectorMaterial")]
        public static void InitCollectorMaterial(ref FactoryModel __instance)
        {
            if (GS2.Vanilla) return;
            
            var realRadius = __instance.planet.realRadius;
            CustomShaderManager.SetPropByShortName("_Radius", realRadius, "mk2effect");
        }
    }
}