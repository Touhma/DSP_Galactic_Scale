namespace GalacticScale
{
    public class LobbyResponseUpdateSolarSystems
    {
        public byte[] GSSettings { get; set; }
        public LobbyResponseUpdateSolarSystems() { }
        public LobbyResponseUpdateSolarSystems(byte[] gssettings)
        {
            GSSettings = gssettings;
        }
    }
}
