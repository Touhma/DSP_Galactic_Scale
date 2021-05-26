namespace GalacticScale
{
    public static partial class GS2
    {        
        public static bool debugOn = false;
        public static GSUI DebugLogOption;
        public static void DebugLogOptionCallback(object o)
        {
            debugOn = (bool)o;
        }
        public static void DebugLogOptionPostfix()
        {
            DebugLogOption.Set(debugOn);
        }
    }
}