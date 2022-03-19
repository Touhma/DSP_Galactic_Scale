namespace GalacticScale
{
    public class LobbyResponseUpdateSolarSystems
    {
        public LobbyResponseUpdateSolarSystems()
        {
        }

        public LobbyResponseUpdateSolarSystems(byte[] gssettings, string[] names, int[] starIds, int[] planetIds)
        {
            GSSettings = gssettings;
            Names = names;
            StarIds = starIds;
            PlanetIds = planetIds;
        }

        public byte[] GSSettings { get; set; }
        public string[] Names { get; set; }
        public int[] StarIds { get; set; }
        public int[] PlanetIds { get; set; }
    }
}