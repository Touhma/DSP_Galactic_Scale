using System;
using System.Collections.Generic;

namespace GalacticScale.Scripts {
    public class GeneratorClusterSettings {

        public int IterationCount = 0;
        public float MinStarDistances = 0f;
        
        public float MinStepLen = 0f;
        public float MaxStepLen = 0f;
        
        public float Flatten = 0f;

        public GeneratorSystemSettings startingSystemSettings = new GeneratorSystemSettings();

        public Dictionary<ESpectrType, GeneratorSystemSettings> SpectralTypeGenSettings = new Dictionary<ESpectrType, GeneratorSystemSettings>();
        public Dictionary<EStarType, GeneratorSystemSettings> StarTypeGenSettings = new Dictionary<EStarType, GeneratorSystemSettings>();
        
        public Dictionary<EStarType, float> StarTypeChances = new Dictionary<EStarType, float>();
        public Dictionary<ESpectrType, float> SpectralTypeChances = new Dictionary<ESpectrType, float>();
        public GeneratorClusterSettings() {
            foreach (ESpectrType spectr in (ESpectrType[]) Enum.GetValues(typeof(ESpectrType)))
            {
                SpectralTypeGenSettings.Add(spectr,new GeneratorSystemSettings());
                SpectralTypeChances.Add(spectr,0f);
            }
            foreach (EStarType starType in (EStarType[]) Enum.GetValues(typeof(EStarType)))
            {
                StarTypeGenSettings.Add(starType,new GeneratorSystemSettings());
                StarTypeChances.Add(starType,0f);
            }
        }
    }
}