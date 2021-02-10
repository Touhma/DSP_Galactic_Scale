using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;


namespace DSP_Plugins.QOLFeatures {
    [BepInPlugin("touhma.dsp.plugins.qol-features", "QOL Features Plug-In", "1.0.0.0")]
    public class QOLFeatures : BaseUnityPlugin {
        public static ConfigEntry<int> ConfigDisassemblingRadiusMax;
        public static ConfigEntry<int> MAXArrayOfBuildingSize;
        
        void Awake() {
            
            ConfigDisassemblingRadiusMax = Config.Bind("Disassembling",  
                "DisassemblingRadiusMax",  
                20, 
                "The Maximum Size of an Internal Array For Mass Disassembling");
            MAXArrayOfBuildingSize = Config.Bind("Disassembling",  
                "ArrayOfBuildingSize",  
                300, 
                "Increase this if you crash when deleting a shit ton of building at once :)");
            
            var harmony = new Harmony("touhma.dsp.plugins.qol-features");
            
            Harmony.CreateAndPatchAll(typeof(PatchOnPlayerAction_Build));
        }
    }
}