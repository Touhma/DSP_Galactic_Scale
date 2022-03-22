using System.IO;
using NebulaAPI;
using GalacticScale;

namespace NebulaCompatibility
{
    [RegisterPacketProcessor]
    public class LobbyResponseUpdateSolarSystemsProcessor : BasePacketProcessor<LobbyResponseUpdateSolarSystems>
    {
        public override void ProcessPacket(LobbyResponseUpdateSolarSystems packet, INebulaConnection conn)
        {
            if (IsHost) return;

            var gameDesc = UIRoot.instance.galaxySelect.gameDesc;
            var galaxyData = UIRoot.instance.galaxySelect.starmap.galaxyData;

            if (galaxyData == null)
            {
                galaxyData = GS2.Vanilla ? UniverseGen.CreateGalaxy(gameDesc) : GS2.ProcessGalaxy(gameDesc, true);

                UIRoot.instance.galaxySelect.starmap.galaxyData = galaxyData;
            }

            using (var ms = new MemoryStream(packet.GSSettings))
            {
                using (var r = new BinaryReader(ms))
                {
                    GSSettings.FromString(r.ReadString());
                }
            }

            GSSettings.lobbyReceivedUpdateValues = true;

            UIRoot.instance.galaxySelect.SetStarmapGalaxy();

            // Old galaxyData is free in SetStarmapGalaxy(), so we need to reassign again
            galaxyData = UIRoot.instance.galaxySelect.starmap.galaxyData;
            for (int i = 0; i < packet.Names.Length; i++)
            {
                GS2.Warn($"{packet.Names[i]} {packet.StarIds[i]} {packet.PlanetIds[i]}");
                if (packet.StarIds[i] != NebulaModAPI.STAR_NONE)
                {
                    StarData star = galaxyData.StarById(packet.StarIds[i]);
                    star.overrideName = packet.Names[i];
                    star.NotifyOnDisplayNameChange();
                }
                else
                {
                    PlanetData planet = galaxyData.PlanetById(packet.PlanetIds[i]);
                    planet.overrideName = packet.Names[i];
                    planet.NotifyOnDisplayNameChange();
                }
            }
            // Update display names
            UIRoot.instance.galaxySelect.starmap.OnGalaxyDataReset();
        }
    }
}