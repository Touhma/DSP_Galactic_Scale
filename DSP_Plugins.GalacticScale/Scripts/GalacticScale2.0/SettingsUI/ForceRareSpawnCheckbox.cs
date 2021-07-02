namespace GalacticScale
{
    public static partial class GS2
    {
        public static GSUI Force1RareChanceOption;
        public static bool Force1RareChance = false;
        public static void Force1RareOptionCallback(object o) => Force1RareChance = (bool)o;
        public static void Force1RareOptionPostfix() => Force1RareChanceOption.Set(Force1RareChance);
    }
}