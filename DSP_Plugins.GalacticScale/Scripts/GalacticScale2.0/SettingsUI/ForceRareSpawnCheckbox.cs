namespace GalacticScale
{
    public static partial class GS2
    {
        public static GSUI Force1RareChanceOption;
        public static bool Force1RareChance;

        public static void Force1RareOptionCallback(Val o)
        {
            Force1RareChance = o;
        }

        public static void Force1RareOptionPostfix()
        {
            Force1RareChanceOption.Set(Force1RareChance);
        }
    }
}