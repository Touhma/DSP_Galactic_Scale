namespace GalacticScale
{
    public interface iConfigurableGenerator : iGenerator
    {
        GSOptions Options { get; }
        void Import(GSGenPreferences preferences);
        GSGenPreferences Export();
    }
}