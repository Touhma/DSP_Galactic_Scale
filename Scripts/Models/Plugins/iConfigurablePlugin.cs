namespace GalacticScale
{
    public interface iConfigurablePlugin
    {
        public string Name { get; }
        public string Author { get; }
        public string Description { get; }
        public string Version { get; }
        public string GUID { get; }
        public GSOptions Options { get; }
        public bool Enabled { get; set; }
        public void Init();
        public void Import(GSGenPreferences preferences);
        public void OnOpen();
        public void OnCancel();
        public void OnApply();

        public GSGenPreferences Export();
        public void Update(string key, Val val);
    }

    public static partial class GS2
    {
        public static iConfigurablePlugin GetPluginByID(string guid)
        {
            foreach (var g in Plugins)
                if (g.GUID == guid)
                    return g;

            return null;
        }
    }
}