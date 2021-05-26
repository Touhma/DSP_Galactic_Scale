namespace GalacticScale
{
    public static partial class GS2
    {
        public static GSUI GS2RareChanceCheckbox;
        public static bool Force1RareChance = false;
        public static void Force1RareOptionCallback(object o)
        {
            Force1RareChance = (bool)o;
        }
        public static void Force1RareOptionPostfix()
        {
            GS2RareChanceCheckbox.Set(Force1RareChance);
        }
    }
}