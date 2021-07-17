namespace GalacticScale
{
    public static partial class GS2
    {
        public static bool SkipPrologue;
        public static GSUI SkipPrologueOption;

        public static void SkipPrologueOptionCallback(Val o)
        {
            SkipPrologue = o;
        }

        public static void SkipPrologueOptionPostfix()
        {
            SkipPrologueOption.Set(debugOn);
        }
    }
}