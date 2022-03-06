namespace GalacticScale
{
    public class LobbyResponseUpdateSolarSystems
    {
        public LobbyResponseUpdateSolarSystems()
        {
        }

        public LobbyResponseUpdateSolarSystems(byte[] gssettings)
        {
            GSSettings = gssettings;
        }

        public byte[] GSSettings { get; set; }
    }
}