namespace GalacticScale
{
    public interface iConfigurablePlugin
    {
        string Name { get; }
        string Author { get; }
        string Description { get; }
        string Version { get; }
        string GUID { get; }
        GSOptions Options { get; }
        bool Enabled { get; set; }
        void Init();
        void Import(GSGenPreferences preferences);


        GSGenPreferences Export();
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