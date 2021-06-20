namespace GalacticScale
{
    public static partial class GS2
    {
        public static bool SkipPrologue = false;
        public static GSUI SkipPrologueOption;
        public static void SkipPrologueOptionCallback(object o) => SkipPrologue = (bool)o;
        public static void SkipPrologueOptionPostfix() => SkipPrologueOption.Set(debugOn);
    }
}