namespace GalacticScale
{
    public static partial class GS2
    {
        public static bool tutorialsOff;
        public static GSUI NoTutorialsOption;

        public static void NoTutorialsOptionCallback(Val o)
        {
            tutorialsOff = o;
        }

        public static void NoTutorialsOptionPostfix()
        {
            NoTutorialsOption.Set(tutorialsOff);
        }
    }
}