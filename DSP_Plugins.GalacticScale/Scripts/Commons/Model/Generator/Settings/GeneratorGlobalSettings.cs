namespace GalacticScale.Scripts {
    public class GeneratorGlobalSettings {
        public GeneratorClusterSettings GeneratorClusterSettings =new GeneratorClusterSettings();

        public bool UseGeneralSettingsForOrbits = false;
        public bool UseSystemSettingsForOrbits = false;
        public GeneratorOrbitsSettings GeneratorOrbitsSettings = new GeneratorOrbitsSettings();

        public bool UseGeneralSettingsForPlanet = false;
        public GeneratorPlanetSettings GeneratorPlanetSettings=new GeneratorPlanetSettings();
        
        public bool UseGeneralSettingsForSystem = false;
        public GeneratorSystemSettings GeneratorSystemSettings=new GeneratorSystemSettings();
        
        public bool UseGeneralSettingsForPlanetTheme = false;
        public GeneratorPlanetThemeSettings GeneratorPlanetThemeSettings = new GeneratorPlanetThemeSettings();
    }
}