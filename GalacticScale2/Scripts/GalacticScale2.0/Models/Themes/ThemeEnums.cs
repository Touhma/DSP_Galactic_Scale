namespace GalacticScale
{
    public enum EThemeType
    {
        Null,
        Gas,
        Planet, //Not Moon
        Moon, //Not Planet
        Private, //Dont select with query
        Telluric //Either Planet or Moon
    }

    public enum EThemeHeat
    {
        Hot,
        Warm,
        Temperate,
        Cold,
        Frozen
    }
}