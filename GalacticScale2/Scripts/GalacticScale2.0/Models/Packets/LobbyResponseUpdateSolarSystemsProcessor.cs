using NebulaAPI;
using System.IO;

namespace GalacticScale
{
    [RegisterPacketProcessor]
    public class LobbyResponseUpdateSolarSystemsProcessor: BasePacketProcessor<LobbyResponseUpdateSolarSystems>
    {
        public override void ProcessPacket(LobbyResponseUpdateSolarSystems packet, INebulaConnection conn)
        {
            if (IsHost)
            {
                return;
            }

            GameDesc gameDesc = UIRoot.instance.galaxySelect.gameDesc;
            GalaxyData galaxyData = UIRoot.instance.galaxySelect.starmap.galaxyData;

            if(galaxyData == null)
            {
                if (GS2.Vanilla)
                    galaxyData = UniverseGen.CreateGalaxy(gameDesc);
                else
                    galaxyData = GS2.ProcessGalaxy(gameDesc, true);

                UIRoot.instance.galaxySelect.starmap.galaxyData = galaxyData;
            }

            using(MemoryStream ms = new MemoryStream(packet.GSSettings)){
                using (BinaryReader r = new BinaryReader(ms))
                {
                    GSSettings.FromString(r.ReadString());
                }
            }

            GSSettings.lobbyReceivedUpdateValues = true;

            UIRoot.instance.galaxySelect.SetStarmapGalaxy();
        }
    }
}
