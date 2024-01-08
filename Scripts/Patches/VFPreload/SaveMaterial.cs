using HarmonyLib;
using UnityEngine;

namespace GalacticScale
{
    public partial class PatchOnVFPreload
    {
        [HarmonyPatch(typeof(VFPreload), nameof(VFPreload.SaveMaterial))]
        [HarmonyPrefix]
        public static bool VFPreload_SaveMaterial_Prefix(Material mat)
        {
            if (mat == null) return false;
            Utils.ReplaceShaderIfAvailable(mat);
        
            return true;
        }

        [HarmonyPatch(typeof(VFPreload), nameof(VFPreload.SaveMaterials), typeof(Material[]))]
        [HarmonyPrefix]
        public static bool VFPreload_SaveMaterials_Prefix(Material[] mats)
        {
            if (mats == null) return false;

            foreach (var mat in mats)
            {
                if (mat == null) continue;
                Utils.ReplaceShaderIfAvailable(mat);
            }

            return true;
        }

        [HarmonyPatch(typeof(VFPreload), nameof(VFPreload.SaveMaterials), typeof(Material[][]))]
        [HarmonyPrefix]
        public static bool VFPreload_SaveMaterials_Prefix(Material[][] mats)
        {
            if (mats == null) return false;

            foreach (var matarray in mats)
            {
                if (matarray == null) continue;
                foreach (var mat in matarray)
                {
                    if (mat == null) continue;
                    Utils.ReplaceShaderIfAvailable(mat);
                }
            }

            return true;
        }
    }
}