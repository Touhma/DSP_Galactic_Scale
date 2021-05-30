namespace GalacticScale
{
    public static partial class GS2
    {        
        public static bool tutorialsOff = false;
        public static GSUI NoTutorialsOption;
        public static void NoTutorialsOptionCallback(object o)
        {
            tutorialsOff = (bool)o;
        }
        public static void NoTutorialsOptionPostfix()
        {
            NoTutorialsOption.Set(tutorialsOff);
        }
    }
}