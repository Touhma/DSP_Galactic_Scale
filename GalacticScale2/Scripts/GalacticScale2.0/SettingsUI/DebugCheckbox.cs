namespace GalacticScale
{
    public static partial class GS2
    {
        public static bool debugOn;
        public static GSUI DebugLogOption;

        public static void DebugLogOptionCallback(Val o)
        {
            debugOn = o;
        }

        public static void DebugLogOptionPostfix()
        {
            DebugLogOption.Set(debugOn);
        }
    }
}