using System.Collections.Generic;

namespace GalacticScale.Generators
{
    public class Dummy : iConfigurableGenerator
    {
        public string Name => "Dummy";

        public string Author => "innominata";

        public string Description => "The most basic generator. Simply to test";

        public string Version => "0.0";

        public string GUID => "space.customizing.generators.dummy";
        public GSGeneratorConfig Config => config;
        public List<GSOption> Options => options;

        public bool DisableStarCountSlider => false;
        private GSGeneratorConfig config = new GSGeneratorConfig();
        public void Init()
        {
            GS2.Log("Dummy:Initializing");
            config.DisableSeedInput = true;
            config.DisableStarCountSlider = false;
            config.MaxStarCount = 2048;
            config.MinStarCount = 1;
            List<string> testList = new List<string>() { "Densest", "Denser", "Default", "Sparse", "Sparsest" };
            options.Add(new GSOption("Density", "IComboBox", testList, SetDensity, SetComboBox));
        }
        public void Import(GSGenPreferences prefs)
        {
            GS2.Log("Dummy:Import");
            preferences = prefs;
            SetDensity(prefs["Density"]);
        }
        public GSGenPreferences Export() => preferences;
        public void Generate(int starCount)
        {
            generate(starCount);
        }
        ////////////////////////////////////////////////////////////////////
        private float minStepLength = 2.3f;
        private float maxStepLength = 3.5f;
        private float minDistance = 2f;
        public List<GSOption> options = new List<GSOption>();
        private GSGenPreferences preferences = new GSGenPreferences();



        public void SetComboBox()
        {
            GS2.Log("Dummy:Postfix:Setting Combobox " + preferences["Density"]);
            options[0].rectTransform.GetComponentInChildren<UIComboBox>().itemIndex = (int)preferences["Density"];
        }

        public void SetDensity(object index)
        {
            if (!(index is int))
            {
                GS2.Log("Density index missing, using default of 2");
                index = 2;
            }
            preferences["Density"] = (int)index;
            GS2.Log("Dummy:Callback:SetDensity:" + index);
            int i = (int)index;
            switch (i)
            {
                case 0: minStepLength = 1.2f; maxStepLength = 1.5f; minDistance = 1.2f; break;
                case 1: minStepLength = 1.6f; maxStepLength = 2.5f; minDistance = 1.7f; break;
                case 3: minStepLength = 2.2f; maxStepLength = 5.0f; minDistance = 2.2f; break;
                case 4: minStepLength = 3.0f; maxStepLength = 7.0f; minDistance = 3.0f; break;
                default: minStepLength = 2f; maxStepLength = 3.5f; minDistance = 2.3f; break;
            }

        }

        public void generate(int starCount)
        {
            GS2.Log("Dummy:Creating New Settings");
            GSSettings.Reset();
            SetDensity(preferences["Density"]);
            List<GSplanet> p = new List<GSplanet>
            {
                new GSplanet("Urf")
            };
            GSSettings.Stars.Add(new GSStar(1, "BeatleJooce", ESpectrType.O, EStarType.MainSeqStar, p));
            for (var i = 1; i < starCount; i++)
            {
                GSSettings.Stars.Add(new GSStar(1, "Star" + i.ToString(), ESpectrType.F, EStarType.GiantStar, new List<GSplanet>()));
            }
            GSSettings.GalaxyParams = new galaxyParams();
            GSSettings.GalaxyParams.iterations = 4;
            GSSettings.GalaxyParams.flatten = 0.18;
            GSSettings.GalaxyParams.minDistance = minDistance;
            GSSettings.GalaxyParams.minStepLength = minStepLength;
            GSSettings.GalaxyParams.maxStepLength = maxStepLength;
        }


    }
}