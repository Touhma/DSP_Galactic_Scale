namespace GalacticScale
{
    public interface iConfigurableGenerator : iGenerator
    {
        public GSOptions Options { get; }
        public void Import(GSGenPreferences preferences);
        public GSGenPreferences Export();

        public void OnOpen();
        public void OnCancel();
        public void OnApply();
    }
    
}