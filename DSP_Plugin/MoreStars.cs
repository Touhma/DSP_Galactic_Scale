using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using BepInEx;
using HarmonyLib;
using UnityEngine.UI;

namespace DSP_Plugin {
    // All credits to : https://github.com/ragzilla/DSP_Mods/blob/main/DSP_MoreStars/MoreStars.cs
    [BepInPlugin("org.bepinex.plugins.moreStars", "More Stars Plug-In", "1.0.0.0")]
    public class DSP_MoreStars : BaseUnityPlugin {
        internal void Awake() {
            var harmony = new Harmony("org.bepinex.plugins.moreStars");
            Harmony.CreateAndPatchAll(typeof(Patch));
        }

        [HarmonyPatch(typeof(UIGalaxySelect))]
        private class Patch {
            [HarmonyPostfix]
            [HarmonyPatch("_OnInit")]
            public static void Postfix(UIGalaxySelect __instance, ref Slider ___starCountSlider) {
                ___starCountSlider.maxValue = 255;
            }
        }
        
        [HarmonyPatch(typeof(UIGalaxySelect), "OnStarCountSliderValueChange")]
        public static class UIGalaxySelect_OnStarCountSliderValueChange
        {
            //// [196 10 - 196 23]
            //IL_0023: ldloc.0      // num
            //IL_0024: ldc.i4.s     80 // 0x50
            //IL_0026: ble IL_002e

            //// [197 7 - 197 15]
            //IL_002b: ldc.i4.s     80 // 0x50
            //IL_002d: stloc.0      // num

            static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                var codes = new List<CodeInstruction>(instructions);
                for (var i = 0; i < codes.Count; i++)
                {
                    if (codes[i].opcode == OpCodes.Ldloc_0 && codes[i + 1].opcode == OpCodes.Ldc_I4_S && codes[i + 2].opcode == OpCodes.Ble && codes[i + 3].opcode == OpCodes.Ldc_I4_S)
                    {
                        UnityEngine.Debug.Log("DSP_MoreStars: Found signature. Applying patch.");
                        codes[i].opcode     = OpCodes.Nop;
                        codes[i + 1].opcode = OpCodes.Nop;
                        codes[i + 2].opcode = OpCodes.Br;
                        return codes.AsEnumerable();
                    }
                }
                UnityEngine.Debug.Log("DSP_MoreStars: Failed to apply all patches.");
                return codes.AsEnumerable();
            }
        }
    }
}