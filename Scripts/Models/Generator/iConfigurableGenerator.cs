namespace GalacticScale
{
    public interface iConfigurableGenerator : iGenerator
    {
        public GSOptions Options { get; }
        public void Import(GSGenPreferences preferences);
        public GSGenPreferences Export();
        public void OnUpdate(string key, Val val);
        
        // Usual Implementation:
        //-----------------------
        // public void OnUpdate(string key, Val val)
        // {
        //     Preferences.Set(key, val);
        // }
    }
    
}