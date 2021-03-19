namespace GalacticScale.Scripts {
    public class GeneratorGlobalSettings {
        public float MoonOrbitInclinationFactor = 0f;
        public float NeutronStarOrbitInclinationFactor = 0f;
        public float ChancePlanetLaySide = 0f;
        public float LaySideBaseAngle = 0f;
        public float LaySideAddingAngle = 0f;
        public float ChanceBigObliquity = 0f;
        public float BigObliquityBaseAngle = 0f;
        public float BigObliquityAddingAngle = 0f;
        public float StandardObliquityAngle = 0f;
        public float RotationPeriodBaseTime = 0f;
        public float RotationPeriodVariabilityFactor = 0f;
        public float ChanceTidalLock = 0f;
        public float ChanceTidalLock11 = 0f;
        public float ChanceTidalLock12 = 0f;
        public float ChanceTidalLock14 = 0f;
        public float ChanceRetrogradeOrbit = 0f;

        public GeneratorClusterSettings GeneratorClusterSettings =new GeneratorClusterSettings();
        public GeneratorPlanetSettings GeneratorPlanetSettings=new GeneratorPlanetSettings();
        public GeneratorSystemSettings GeneratorSystemSettings=new GeneratorSystemSettings();
        public GeneratorStarSettings GeneratorStarSettings=new GeneratorStarSettings();
        public GeneratorPlanetThemeSettings GeneratorPlanetThemeSettings = new GeneratorPlanetThemeSettings();
    }
}